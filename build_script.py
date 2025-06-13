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

class AppBuilder:
    def __init__(self, config_path="build.json"):
        self.project_root = Path(__file__).parent
        
        # Carica la configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                self.config = json.load(f)
        else:
            self.config = self.default_config()
            
        self.build_dir = self.project_root / self.config['build']['temp_dir']
        self.dist_dir = self.project_root / self.config['build']['output_dir']

    def default_config(self):
        """Configurazione di default"""
        return {
            "app": {
                "name": "Pietribiasi App",
                "version": "1.0.0"
            },
            "build": {
                "backend_project": "apiPB",
                "frontend_path": "Frontend",
                "output_dir": "dist",
                "temp_dir": "build"
            },
            "targets": [
                {
                    "name": "Windows",
                    "runtime": "win-x64",
                    "executable_extension": ".exe"
                }
            ],
            "server": {
                "backend": {"host": "localhost", "port": 5001, "local_ip_automatically": True},
                "frontend": {"host": "localhost", "port": 8080, "local_ip_automatically": True}
            },
            "remote_backend": {
                "enabled": True,
                "host": "192.168.100.113",
                "port": 5245,
                "protocol": "http",
                "health_endpoint": "/health",
                "timeout_seconds": 30
            }
        }
    
    async def update_appsettings(self):
        """Aggiorna appsettings.json in apiPB/ con i valori da build.json nella root"""
        print("Aggiornamento di appsettings.json...")

        # Carica la configurazione da build.json nella root
        build_json_path = self.project_root / "build.json"
        if not build_json_path.exists():
            print(f"ATTENZIONE: {build_json_path} non trovato.")
            return

        with open(build_json_path, 'r', encoding='utf-8') as f:
            config = json.load(f)

        # Percorsi dei file da aggiornare
        appsettings_path = self.project_root / "apiPB" / "appsettings.json"

        if not appsettings_path.exists():
            print(f"ATTENZIONE: {appsettings_path} non trovato.")

        try:
            # Leggi il file appsettings.json esistente
            with open(appsettings_path, 'r', encoding='utf-8-sig') as f:
                appsettings = json.load(f)

            # Aggiungi/aggiorna la sezione server
            if 'Server' not in appsettings:
                appsettings['Server'] = {}

            appsettings['Server']['Backend'] = {
                "Host": config['server']['backend']['host'],
                "Port": config['server']['backend']['port']
            }

            appsettings['Server']['Frontend'] = {
                "Host": config['server']['frontend']['host'],
                "Port": config['server']['frontend']['port']
            }

            appsettings['ConnectionStrings'] = {
                "LocalA3Db": config['server']['backend'].get('connection_string', '')
            }

            # Scrivi il file aggiornato
            with open(appsettings_path, 'w', encoding='utf-8-sig') as f:
                json.dump(appsettings, f, indent=2, ensure_ascii=False)

            print(f"✅ appsettings.json aggiornato in {appsettings_path}:")
            print(f"   Backend: {config['server']['backend']['host']}:{config['server']['backend']['port']}")
            print(f"   Frontend: {config['server']['frontend']['host']}:{config['server']['frontend']['port']}")

        except Exception as e:
            print(f"❌ Errore durante l'aggiornamento di {appsettings_path}: {e}")

    def update_start_bat(self):
        """Aggiorna Pietribiasi_App_start.bat con i valori da build.json"""
        print("Aggiornamento di Pietribiasi_App_start.bat...")
        
        start_bat_path = self.project_root / "Pietribiasi_App_start.bat"
        
        if not start_bat_path.exists():
            print(f"ATTENZIONE: {start_bat_path} non trovato.")
            return
        
        try:
            # Leggi il contenuto del file Pietribiasi_App_start.bat
            with open(start_bat_path, 'r', encoding='utf-8') as f:
                content = f.read()
            
            # Sostituisci i valori delle variabili
            backend_config = self.config['server']['backend']
            frontend_config = self.config['server']['frontend']
            
            # Pattern per trovare e sostituire le variabili
            replacements = {
                'set FRONTEND_PORT=8080': f'set FRONTEND_PORT={frontend_config["port"]}',
                'set BACKEND_PORT=5245': f'set BACKEND_PORT={backend_config["port"]}',
                'set SERVER_IP=192.168.100.113': f'set SERVER_IP={backend_config["host"]}'
            }
            
            updated_content = content
            for old_line, new_line in replacements.items():
                if old_line in updated_content:
                    updated_content = updated_content.replace(old_line, new_line)
                    print(f"   Aggiornato: {new_line}")
                else:
                    print(f"   ATTENZIONE: Non trovato pattern '{old_line}' in Pietribiasi_App_start.bat")
            
            # Scrivi il file aggiornato
            with open(start_bat_path, 'w', encoding='utf-8') as f:
                f.write(updated_content)
            
            print("✅ Pietribiasi_App_start.bat aggiornato")
            
        except Exception as e:
            print(f"❌ Errore durante l'aggiornamento di Pietribiasi_App_start.bat: {e}")
        
    async def clean(self):
        """Pulisce le directory di build e di distribuzione"""
        print("Pulizia delle directory in corso...")
        if self.build_dir.exists():
            shutil.rmtree(self.build_dir)
        if self.dist_dir.exists():
            shutil.rmtree(self.dist_dir)
        
        self.build_dir.mkdir(exist_ok=True)
        self.dist_dir.mkdir(exist_ok=True)
    
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
    
    async def copy_and_configure_frontend(self):
        """Copia il frontend e configura l'URL dell'API in main.js"""
        print("Copia e configurazione del frontend...")
        # La cartella sorgente del frontend
        frontend_src = self.project_root / self.config['build']['frontend_path']
        # La cartella di destinazione nella build
        frontend_dst = self.build_dir / "frontend"
        
        if frontend_dst.exists():
            shutil.rmtree(frontend_dst)
        shutil.copytree(frontend_src, frontend_dst)
        
        # Il file main.js si trova ora in frontend/Web/javascript/main.js
        main_js_path = frontend_dst / "Web" / "javascript" / "main.js"

        if not main_js_path.exists():
            print(f"ATTENZIONE: {main_js_path} non trovato. Configurazione saltata.")
            return

        print(f"Configurazione dell'URL API in {main_js_path}...")

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
                print(f"URL API impostato a: {api_base_url}")

            with open(main_js_path, 'w', encoding='utf-8') as f:
                f.write(updated_content)

        except Exception as e:
            print(f"❌ Errore durante la configurazione di main.js: {e}")
            
    
    def create_advanced_start_bat(self):
        """Copia il template Pietribiasi_App_start.bat nella directory di build"""
        print("Copia del template script di controllo...")
        
        # Path del template nella root del progetto
        template_path = self.project_root / "Pietribiasi_App_start.bat"
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

    def copy_build_json_to_build(self):
        """Copia build.json nella cartella di build per permettere la riconfigurazione runtime"""
        print("Copia di build.json nella cartella di build...")
        
        build_json_src = self.project_root / "build.json"
        build_json_dst = self.build_dir / "build.json"
        
        if build_json_src.exists():
            shutil.copy2(build_json_src, build_json_dst)
            print("✅ build.json copiato nella cartella di build")
            return True
        else:
            # Crea un build.json con la configurazione corrente
            with open(build_json_dst, 'w', encoding='utf-8') as f:
                json.dump(self.config, f, indent=2, ensure_ascii=False)
            print("✅ build.json generato nella cartella di build")
            return True

    def create_launcher_script(self, target):
        """Crea lo script di avvio avanzato per una specifica piattaforma"""
        print(f"Creazione dello script di controllo per {target['name']}...")
        
        if target['runtime'].startswith('win'):
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
    
    def get_local_ip(self):
        """Restituisce l'indirizzo IPv4 locale del dispositivo (non localhost)."""
        try:
            # Usa una connessione UDP fittizia per determinare l'IP locale
            with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
                s.connect(('8.8.8.8', 80))
                ip = s.getsockname()[0]
                # Verifica che non sia localhost
                if ip.startswith("127."):
                    raise Exception("Indirizzo locale è localhost")
                return ip
        except Exception:
            # Fallback: cerca tra le interfacce di rete
            try:
                hostname = socket.gethostname()
                ips = socket.gethostbyname_ex(hostname)[2]
                for ip in ips:
                    if not ip.startswith("127."):
                        return ip
            except Exception:
                pass
            return '127.0.0.1'

    async def update_frontend_host_ip(self, build_json_path):
        """Aggiorna server.frontend.host con l'IP locale."""
        with open(build_json_path, 'r', encoding='utf-8') as f:
            config = json.load(f)
        automatic_ip = config['server']['frontend']['local_ip_automatically']
        if automatic_ip:
            local_ip = self.get_local_ip()
            config['server']['frontend']['host'] = local_ip
            with open(build_json_path, 'w', encoding='utf-8') as f:
                json.dump(config, f, indent=4, ensure_ascii=False)
            print(f"✅ server.frontend.host aggiornato a {local_ip}")

    async def update_backend_host_ip(self, build_json_path):
        """Aggiorna server.backend.host con l'IP locale."""
        with open(build_json_path, 'r', encoding='utf-8') as f:
            config = json.load(f)
        automatic_ip = config['server']['backend']['local_ip_automatically']
        if automatic_ip:
            local_ip = self.get_local_ip()
            config['server']['backend']['host'] = local_ip
            with open(build_json_path, 'w', encoding='utf-8') as f:
                json.dump(config, f, indent=4, ensure_ascii=False)
            print(f"✅ server.backend.host aggiornato a {local_ip}")

    def create_zip_archive(self):
        """Crea l'archivio ZIP finale"""
        print("Creazione dell'archivio ZIP...")
        zip_path = self.dist_dir / f"{self.config['app']['name']}_v{self.config['app']['version']}.zip"
        
        with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zf:
            for root, _, files in os.walk(self.build_dir):
                for file in files:
                    file_path = Path(root) / file
                    arc_path = file_path.relative_to(self.build_dir)
                    zf.write(file_path, arc_path)
        
        return zip_path
    
    async def build(self):
        """Esegue l'intero processo di build"""
        try:
            print(f"=== Build di {self.config['app']['name']} ===")

            await self.clean()

            # Inserisce l'IP locale per il Frontend nel file build.json se configurato per farlo
            await self.update_frontend_host_ip("build.json")

            # Inserisce l'IP locale per il Backend nel file build.json se configurato per farlo
            await self.update_backend_host_ip("build.json")
            
            # Aggiorna i file di configurazione PRIMA del build
            await self.update_appsettings()
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
                await self.copy_and_configure_frontend()
            else:
                print("❌ Build interrotta: uno dei valori server:backend:host, server:backend:port, server:frontend:host, server:frontend:port è mancante in build.json")
                return False
            
            self.copy_build_json_to_build()

            self.create_launcher_script(target)

            if self.config['packaging'].get('create_portable', True):
                zip_path = self.create_zip_archive()
            
            print(f"\n✅ Build completato con successo!")
            print(f"📦 Archivio creato in: {zip_path}")
            print(f"📁 Dimensione: {zip_path.stat().st_size / 1024 / 1024:.1f} MB")
            print(f"🚀 Usa 'Pietribiasi_App_start.bat' per controllare l'applicazione")
            
        except Exception as e:
            print(f"❌ Build fallito: {e}")
            return False
        
        return True

if __name__ == "__main__":
    builder = AppBuilder()
    asyncio.run(builder.build())