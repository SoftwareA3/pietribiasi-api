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

# I PERCORSI SONO RELATIVI ALL'ESEGUIBILE CREATO CON PyInstaller
# VIENE FATTO UN CONROLLO PER VERIFICARE CHE, SE VIENE ESEGUITO L'ESEGUIBILE,
# IL PERCORSO DEL PROGETTO SIA CORRETTO, ALTRIMENTI È RELATIVO AL FILE build.py

class AppBuilder:
    """Classe per la costruzione dell'applicazione PietriBiasi"""
    # Inizializza il percorso del progetto e carica la configurazione
    def __init__(self, config_path="build.json"):
        
        if getattr(sys, 'frozen', False):
            self.project_root = Path(sys.executable).parent
            self.config_path = config_path
            self.app_dir = self.project_root / "PietribiasiApp"
        else:
            self.project_root = Path(__file__).parent.parent 
            self.config_path = self.project_root / "BuildScripts" / config_path
            self.app_dir = self.project_root / "App" / "PietribiasiApp"
        print(f"Root del progetto: {self.project_root}")
        
        # Carica la configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                self.config = json.load(f)
        else:
            self.config = script_utils.default_config()

    async def build(self):
        try:
            print(f"=== Build di {self.config['app']['name']} ===")
            if not Path(self.app_dir).exists():
                self.app_dir.mkdir(parents=True, exist_ok=True)
            
            # Recupera l'IP locale per il Backend se il file di configurazione lo richiede
            await script_utils.update_host_ip(self.config_path)

            # Carica la configurazione da build.json
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
                script_utils.copy_backend_to_build(self)
            
            # Configura il backend con i valori di configurazione
            print("Aggiornamento di appsettings.json...")

            # Percorsi dei file da aggiornare
            await script_utils.update_appsettings(self)
            
            # Copia la documentazione nella directory di build
            script_utils.copy_documentation_to_build(self)

            # Crea lo script di avvio per il target specificato
            script_utils.create_launcher_script(self, target)
        
            # Creazione del file ZIP di distribuzione
            script_utils.create_zip_archive(self)

        except Exception as e:
            print(f"❌ Build fallito: {e}")
            return False
        
        print(f"✅ Build completata con successo in {self.app_dir}")
        input("Premi Invio per continuare...")
        return True

if __name__ == "__main__":
    builder = AppBuilder()
    asyncio.run(builder.build())