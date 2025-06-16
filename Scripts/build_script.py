#!/usr/bin/env python3
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

class AppBuilder:
    def __init__(self, config_path="Scripts/build.json"):
        self.project_root = Path(__file__).parent.parent
        print(f"Root del progetto: {self.project_root}")
        
        # Carica la configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                self.config = json.load(f)
        else:
            self.config = script_utils.default_config()
            
        self.build_dir, self.dist_dir = script_utils.create_build_and_distr_dir(self, "build")

    async def build_backend_for_target(self, target):
        """Compila il backend per una specifica piattaforma"""
        print(f"Compilazione del backend per {target['name']}...")
        
        backend_path = self.project_root / self.config['build']['backend_project']
        build_output = self.build_dir / "backend"

        # Leggi la connection string dal file build.json
        connection_string = self.config['server']['backend']['connection_string']
        print(f"Connection string backend: {connection_string}")

        cmd = [
            'dotnet', 'publish', str(backend_path),
            '-c', 'Release',
            '-o', str(build_output),
            '--self-contained', 'true',
            '-r', target['runtime']
        ]
        
        result = subprocess.run(cmd, check=True)
        if result.returncode != 0:
            raise Exception(f"La compilazione del backend per {target['name']} è fallita")
        
        return build_output
            
    
    def create_advanced_start_bat(self):
        """Copia il template Pietribiasi_App_start.bat nella directory di build"""
        print("Copia del template script di controllo...")
        
        # Path del template nella root del progetto
        template_path = self.project_root / "Scripts/Pietribiasi_App_start.bat"
        # Path di destinazione nella cartella di build
        destination_path = self.build_dir / "Pietribiasi_App_start.bat"
        
        if not template_path.exists():
            print(f"❌ ERRORE: Template {template_path} non trovato!")
            print("Assicurati che il file Pietribiasi_App_start.bat sia presente nella root del progetto.")
            return False
        
        try:
            # Copia il template nella cartella di build
            shutil.copy2(template_path, destination_path)
            print(f"✅ Template copiato da {template_path} a {destination_path}")
            return True
            
        except Exception as e:
            print(f"❌ Errore durante la copia del template: {e}")
            return False

    def create_launcher_script(self, target):
        """Crea lo script di avvio avanzato per una specifica piattaforma"""
        print(f"Creazione dello script di controllo per {target['name']}...")
        
        if target['runtime'].startswith('win'):
            # Copia il file BAT principale
            self.create_advanced_start_bat()
        else:
            self.create_unix_launcher()  # Mantieni il launcher Unix esistente

    def create_unix_launcher(self):
        """Crea uno script di avvio per Unix (Linux/macOS) (.sh)"""
        backend_conf = self.config['server']['backend']
        frontend_conf = self.config['server']['frontend']
        app_name = self.config['app']['name']
        backend_exec = self.config['build']['backend_project']

        launcher_content = f'''#!/bin/bash
echo "Avvio di {app_name} in corso..."
cd "$(dirname "$0")"

# Avvia il Backend
echo "Avvio del Backend Server..."
./backend/{backend_exec} --urls="http://{backend_conf['host']}:{backend_conf['port']}" &
BACKEND_PID=$!

# Avvia il Frontend
echo "Avvio del Frontend Server..."
npx http-server ./frontend -a {frontend_conf['host']} -p {frontend_conf['port']} --cors -o index.html &
FRONTEND_PID=$!

echo "{app_name} e' in esecuzione."
echo "Premi INVIO per terminare l'applicazione."
read

echo "Arresto dell'applicazione in corso..."
kill $BACKEND_PID
kill $FRONTEND_PID
'''
        launcher_path = self.build_dir / f"start_{app_name.replace(' ', '_')}.sh"
        with open(launcher_path, 'w', encoding='utf-8', newline='\n') as f:
            f.write(launcher_content)
        os.chmod(launcher_path, 0o755)
    
    async def update_backend_host_ip(self, build_json_path):
        """Aggiorna server.backend.host con l'IP locale."""
        with open(build_json_path, 'r', encoding='utf-8') as f:
            config = json.load(f)
        automatic_ip = config['server']['backend']['local_ip_automatically']
        if automatic_ip:
            local_ip = script_utils.get_local_ip()
            config['server']['backend']['host'] = local_ip
            with open(build_json_path, 'w', encoding='utf-8') as f:
                json.dump(config, f, indent=4, ensure_ascii=False)
            print(f"✅ server.backend.host aggiornato a {local_ip}")
    
    async def build(self):
        """Esegue l'intero processo di build"""
        try:
            print(f"=== Build di {self.config['app']['name']} ===")

            await script_utils.clean(self)

            # Inserisce l'IP locale per il Frontend nel file build.json se configurato per farlo
            script_utils.update_frontend_host_ip("build.json")

            # Inserisce l'IP locale per il Backend nel file build.json se configurato per farlo
            await self.update_backend_host_ip("build.json")
            
            # Aggiorna i file di configurazione PRIMA del build
            await script_utils.update_appsettings(self)
            #self.update_start_bat()

            with open("build.json", "r", encoding="utf-8") as f:
                self.config = json.load(f)
            
            target = self.config['targets'][0] 
            
            # Controlla che i valori necessari siano presenti in build.json
            backend_host = self.config['server']['backend'].get('host')
            backend_port = self.config['server']['backend'].get('port')
            frontend_host = self.config['server']['frontend'].get('host')
            frontend_port = self.config['server']['frontend'].get('port')

            # Controlla che host e port non siano stringhe vuote
            if any(x in ("", None) for x in [backend_host, backend_port, frontend_host, frontend_port]):
                print("❌ Build interrotta: uno dei valori server:backend:host, server:backend:port, server:frontend:host, server:frontend:port è mancante o vuoto in build.json")
                return False

            if all([backend_host, backend_port, frontend_host, frontend_port]):
                await self.build_backend_for_target(target)
                await script_utils.copy_and_configure_frontend(self, "build")
            else:
                print("❌ Build interrotta: uno dei valori server:backend:host, server:backend:port, server:frontend:host, server:frontend:port è mancante in build.json")
                return False
            
            script_utils.copy_build_json_to_build(self, True)

            self.create_launcher_script(target)

            script_utils.create_executable_from_batchscript(self)

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