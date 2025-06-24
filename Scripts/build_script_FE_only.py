#!/usr/bin/env python3
import os
import shutil
import subprocess
import json
import zipfile
import socket
#import requests
from pathlib import Path
import asyncio
from datetime import datetime
import script_utils

class FrontendOnlyBuilder:
    def __init__(self, config_path="build.json"):
        # Imposta la root del progetto come la cartella superiore rispetto a questo script
        self.project_root = Path(__file__).parent.parent
        print(f"Root del progetto: {self.project_root}")
        
        # Carica la configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                self.config = json.load(f)
        else:
            self.config = script_utils.default_config()
            
        self.build_dir, self.dist_dir = script_utils.create_build_and_distr_dir(self, "build_FE")
    
    async def copy_and_configure_frontend(self):
        """Copia il frontend e configura l'URL dell'API remoto"""
        print("Copia e configurazione del frontend...")
        
        # La cartella sorgente del frontend
        frontend_src = self.project_root / self.config['build']['frontend_path']
        # La cartella di destinazione nella build
        frontend_dst = self.build_dir / "frontend"
        
        if frontend_dst.exists():
            shutil.rmtree(frontend_dst)
        shutil.copytree(frontend_src, frontend_dst)
        
        # Il file main.js si trova in frontend/Web/javascript/main.js
        main_js_path = frontend_dst / "Web" / "javascript" / "main.js"

        if not main_js_path.exists():
            print(f"ATTENZIONE: {main_js_path} non trovato. Configurazione saltata.")
            return

        print(f"Configurazione dell'URL API remoto in {main_js_path}...")

        # Usa la configurazione del backend remoto
        if self.config.get('remote_backend', {}).get('enabled', False):
            remote_config = self.config['remote_backend']
            api_base_url = f"{remote_config['protocol']}://{remote_config['host']}:{remote_config['port']}"
        else:
            # Fallback alla configurazione normale
            backend_config = self.config['server']['backend']
            api_base_url = f"http://{backend_config['host']}:{backend_config['port']}"
        
        try:
            with open(main_js_path, 'r', encoding='utf-8') as f:
                content = f.read()

            updated_content = content.replace('##API_BASE_URL##', api_base_url)

            if content == updated_content:
                print(f"ATTENZIONE: Il segnaposto '##API_BASE_URL##' non è stato trovato in main.js.")
            else:
                print(f"URL API remoto impostato a: {api_base_url}")

            with open(main_js_path, 'w', encoding='utf-8') as f:
                f.write(updated_content)

        except Exception as e:
            print(f"❌ Errore durante la configurazione di main.js: {e}")
    
    async def build(self):
        """Esegue l'intero processo di build per il solo frontend"""
        try:
            print(f"=== Build Frontend-Only di {self.config['app']['name']} ===")

            await script_utils.clean(self)

            #await script_utils.update_frontend_host_ip("build.json")
            
            # Aggiorna i file di configurazione PRIMA del build
            await script_utils.update_appsettings(self)
            
            # Test connessione backend (opzionale)
            # await self.test_backend_connection()
            
            # Copia e configura il frontend
            await self.copy_and_configure_frontend()

            script_utils.copy_python_server(self)
            script_utils.copy_build_json_to_build(self, True)

            
            # Crea gli script e file di supporto
            # self.create_frontend_launcher()
            # self.create_configuration_file()
            # self.create_package_json()

            # script_utils.create_executable_from_batchscript(self)
            script_utils.create_pyinstaller_executable(self)
            
            # Crea l'archivio
            print(f"\n✅ Build frontend completato con successo!")
            if self.config['packaging'].get('create_portable', True):
                zip_path = script_utils.create_zip_archive(self)
                print(f"📦 Archivio creato in: {zip_path}")
                print(f"📁 Dimensione: {zip_path.stat().st_size / 1024 / 1024:.1f} MB")
            print(f"🌐 Il frontend si connetterà al backend remoto:")
            
            if self.config.get('remote_backend', {}).get('enabled', False):
                remote_config = self.config['remote_backend']
                backend_url = f"{remote_config['protocol']}://{remote_config['host']}:{remote_config['port']}"
            else:
                backend_config = self.config['server']['backend']
                backend_url = f"http://{backend_config['host']}:{backend_config['port']}"
            
            print(f"    {backend_url}")

            print(f"🚀 Script terminato!")
            
        except Exception as e:
            print(f"❌ Build fallito: {e}")
            return False
        
        return True

if __name__ == "__main__":
    builder = FrontendOnlyBuilder()
    asyncio.run(builder.build())