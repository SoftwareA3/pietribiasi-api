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
    
    async def build(self):
        """Esegue l'intero processo di build per il solo frontend"""
        try:
            print(f"=== Build Frontend-Only di {self.config['app']['name']} ===")

            await script_utils.clean(self)
            
            # Aggiorna i file di configurazione PRIMA del build
            await script_utils.update_appsettings(self)
            
            # Copia e configura il frontend
            await script_utils.copy_and_configure_frontend(self, "build_FE")

            script_utils.copy_python_server(self)
            script_utils.copy_build_json_to_build(self, True)
            script_utils.copy_documentation_to_build(self)

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