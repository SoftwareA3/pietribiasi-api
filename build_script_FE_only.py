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
from build_script import AppBuilder

class FrontendOnlyBuilder:
    def __init__(self, config_path="build.json"):
        self.project_root = Path(__file__).parent
        
        # Carica la configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                self.config = json.load(f)
        else:
            self.config = AppBuilder().default_config()
            
        self.build_dir = self.project_root / self.config['build']['temp_dir']
        self.dist_dir = self.project_root / self.config['build']['output_dir']
    
    # async def test_backend_connection(self):
    #     """Testa la connessione al backend remoto"""
    #     if not self.config.get('remote_backend', {}).get('enabled', False):
    #         return True
            
    #     backend_config = self.config['remote_backend']
    #     test_url = f"{backend_config['protocol']}://{backend_config['host']}:{backend_config['port']}"
    #     health_endpoint = backend_config.get('health_endpoint', '/health')
    #     timeout = backend_config.get('timeout_seconds', 10)
        
    #     print(f"Test connessione al backend remoto: {test_url}")
        
    #     try:
    #         response = requests.get(f"{test_url}{health_endpoint}", timeout=timeout)
    #         if response.status_code == 200:
    #             print("✅ Backend remoto raggiungibile")
    #             return True
    #         else:
    #             print(f"⚠️ Backend remoto risponde con status {response.status_code}")
    #             return False
    #     except requests.exceptions.RequestException as e:
    #         print(f"❌ Backend remoto non raggiungibile: {e}")
    #         return False
    
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
    
    def create_frontend_launcher(self):
        """Copia il template Frontend_Pietribiasi_App_start.bat nella directory di build"""
        print("Copia del template script di controllo...")
        
        # Path del template nella root del progetto
        template_path = self.project_root / "Frontend_Pietribiasi_App_start.bat"
        # Path di destinazione nella cartella di build
        destination_path = self.build_dir / "Pietribiasi_App_start.bat"
        
        if not template_path.exists():
            print(f"❌ ERRORE: Template {template_path} non trovato!")
            print("Assicurati che il file Frontend_Pietribiasi_App_start.bat sia presente nella root del progetto.")
            return False
        
        try:
            # Copia il template nella cartella di build
            shutil.copy2(template_path, destination_path)
            print(f"✅ Template copiato da {template_path} a {destination_path}")
            return True
            
        except Exception as e:
            print(f"❌ Errore durante la copia del template: {e}")
            return False

    def create_configuration_file(self):
        """Crea un file di configurazione leggibile per l'utente finale"""
        config_content = f"""# Configurazione {self.config['app']['name']}

## Backend Remoto
- Host: {self.config.get('remote_backend', {}).get('host', 'N/A')}
- Porta: {self.config.get('remote_backend', {}).get('port', 'N/A')}
- URL completo: {self.config.get('remote_backend', {}).get('protocol', 'http')}://{self.config.get('remote_backend', {}).get('host', 'localhost')}:{self.config.get('remote_backend', {}).get('port', '5245')}

## Frontend Locale  
- Host: {self.config['server']['frontend']['host']}
- Porta: {self.config['server']['frontend']['port']}
- URL locale: http://{self.config['server']['frontend']['host']}:{self.config['server']['frontend']['port']}

## Istruzioni
1. Assicurati che il backend remoto sia in esecuzione
2. Avvia il frontend usando lo script .bat
3. Apri il browser all'indirizzo del frontend locale

## Requisiti
- Node.js installato (per http-server)
- Connessione di rete al backend remoto
- Browser web moderno

## Note
- Il frontend comunica direttamente con il backend remoto
- Non è necessario installare il backend localmente
- Assicurati che il firewall permetta le connessioni al backend remoto
"""
        
        config_path = self.build_dir / "CONFIGURAZIONE.md"
        with open(config_path, 'w', encoding='utf-8') as f:
            f.write(config_content)
        
        print("✅ File di configurazione creato: CONFIGURAZIONE.md")

    def create_package_json(self):
        """Crea un package.json per le dipendenze del frontend"""
        package_json = {
            "name": self.config['app']['name'].lower().replace(' ', '-'),
            "version": self.config['app']['version'],
            "description": f"Frontend per {self.config['app']['name']}",
            "scripts": {
                "start": f"http-server ./frontend -a {self.config['server']['frontend']['host']} -p {self.config['server']['frontend']['port']} --cors -o -c-1",
                "install-deps": "npm install -g http-server"
            },
            "dependencies": {},
            "devDependencies": {
                "http-server": "^14.1.1"
            },
            "private": True
        }
        
        package_path = self.build_dir / "package.json"
        with open(package_path, 'w', encoding='utf-8') as f:
            json.dump(package_json, f, indent=2, ensure_ascii=False)
        
        print("✅ package.json creato per le dipendenze")
    
    async def build(self):
        """Esegue l'intero processo di build per il solo frontend"""
        try:
            print(f"=== Build Frontend-Only di {self.config['app']['name']} ===")

            await AppBuilder().clean()
            
            # Test connessione backend (opzionale)
            # await self.test_backend_connection()
            
            # Copia e configura il frontend
            await self.copy_and_configure_frontend()

            AppBuilder().copy_build_json_to_build()

            
            # Crea gli script e file di supporto
            self.create_frontend_launcher()
            self.create_configuration_file()
            self.create_package_json()
            
            # Crea l'archivio
            if self.config['packaging'].get('create_portable', True):
                zip_path = AppBuilder().create_zip_archive()
            
            print(f"\n✅ Build frontend completato con successo!")
            print(f"📦 Archivio creato in: {zip_path}")
            if self.config['packaging'].get('create_portable', True):
                print(f"📁 Dimensione: {zip_path.stat().st_size / 1024 / 1024:.1f} MB")
            print(f"🚀 Usa lo script .bat per avviare il frontend")
            print(f"🌐 Il frontend si connetterà al backend remoto:")
            
            if self.config.get('remote_backend', {}).get('enabled', False):
                remote_config = self.config['remote_backend']
                backend_url = f"{remote_config['protocol']}://{remote_config['host']}:{remote_config['port']}"
            else:
                backend_config = self.config['server']['backend']
                backend_url = f"http://{backend_config['host']}:{backend_config['port']}"
            
            print(f"    {backend_url}")
            
        except Exception as e:
            print(f"❌ Build fallito: {e}")
            return False
        
        return True

if __name__ == "__main__":
    builder = FrontendOnlyBuilder()
    asyncio.run(builder.build())