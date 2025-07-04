import os
import shutil
import subprocess
import json
import zipfile
import socket
from pathlib import Path
import asyncio
from datetime import datetime
import script_utils
import sys

class AppBuilder:
    """Classe per la costruzione dell'applicazione PietriBiasi"""
    # Inizializza il percorso del progetto e carica la configurazione
    def __init__(self, config_path="BuildScripts/build.json"):
        
        if getattr(sys, 'frozen', False):
            self.project_root = Path(sys.executable).parent
        else:
            self.project_root = Path(__file__).parent 
        self.config_path = config_path
        print(f"Root del progetto: {self.project_root}")
        
        # Carica la configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                self.config = json.load(f)
        else:
            self.config = self.default_config()
            
        # Crea le directory di build e distribuzione
        self.build_dir, self.dist_dir, self.app_dir, self.script_dir = script_utils.create_build_and_distr_dir(self)


    async def build(self):
        """Esegue l'intero processo di build"""

        try:
            print(f"=== Build di {self.config['app']['name']} ===")

            await script_utils.clean(self)

            # Inserisce l'IP locale per il Backend nel file build.json se configurato per farlo
            await script_utils.update_host_ip(self.config_path)

            # Copia il Frontend nella cartella di build come file wwwroot (da testare)
            with open(self.config_path, "r", encoding="utf-8") as f:
                self.config = json.load(f)
            
            target = self.config['targets'][0] 
            
            # Controlla che i valori necessari siano presenti in build.json
            backend_host = self.config['server']['backend'].get('host')
            backend_port = self.config['server']['backend'].get('port')

            # Controlla che host e port non siano stringhe vuote
            if any(x in ("", None) for x in [backend_host, backend_port]):
                print("❌ Build interrotta: uno dei valori server:backend:host, server:backend:port, server:frontend:host, server:frontend:port è mancante o vuoto in build.json")
                return False

            if all([backend_host, backend_port]):
                await script_utils.build_backend_for_target(self, target)
                await script_utils.copy_and_configure_frontend(self, "build")
            else:
                print("❌ Build interrotta: uno dei valori server:backend:host, server:backend:port, server:frontend:host, server:frontend:port è mancante in build.json")
                return False
            
            # Aggiorna il file di configurazione nella cartella di build
            await script_utils.update_appsettings(self)

            # Copia il file build.json nella cartella (al momento senza filtrarlo)
            script_utils.copy_build_json_to_build(self, True)

            # Copia parte della documentazione nella cartella di build
            script_utils.copy_documentation_to_build(self)

            # Crea uno script che raggiunga ed esegua il file 'apiPB.exe' nella cartella di build
            script_utils.create_launcher_script(self)

            # Crea il file .zip per la distribuzione
            print(f"\n✅ Build completato con successo!")
            if self.config['packaging'].get('create_portable', True):
                zip_path = script_utils.create_zip_archive(self)
                print(f"📦 Archivio creato in: {zip_path}")
                print(f"📁 Dimensione: {zip_path.stat().st_size / 1024 / 1024:.1f} MB")
            
            print(f"🚀 Script terminato!")
            
        except Exception as e:
            print(f"❌ Build fallito: {e}")
            return False
        
        return True

if __name__ == "__main__":
    builder = AppBuilder()
    asyncio.run(builder.build())