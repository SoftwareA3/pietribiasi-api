#!/usr/bin/env python3
import os
import shutil
import subprocess
import json
import zipfile
from pathlib import Path
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
                "backend": {"host": "localhost", "port": 5001},
                "frontend": {"host": "localhost", "port": 8080}
            }
        }
        
    def clean(self):
        """Pulisce le directory di build e di distribuzione"""
        print("Pulizia delle directory in corso...")
        if self.build_dir.exists():
            shutil.rmtree(self.build_dir)
        if self.dist_dir.exists():
            shutil.rmtree(self.dist_dir)
        
        self.build_dir.mkdir(exist_ok=True)
        self.dist_dir.mkdir(exist_ok=True)
    
    def build_backend_for_target(self, target):
        """Compila il backend per una specifica piattaforma"""
        print(f"Compilazione del backend per {target['name']}...")
        
        backend_path = self.project_root / self.config['build']['backend_project']
        build_output = self.build_dir / "backend"
        
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
    
    def copy_and_configure_frontend(self):
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
            
    def create_launcher_script(self, target):
        """Crea uno script di avvio per una specifica piattaforma"""
        print(f"Creazione dello script di avvio per {target['name']}...")
        
        if target['runtime'].startswith('win'):
            self.create_windows_launcher()
        else:
            self.create_unix_launcher()

    def create_windows_launcher(self):
        """Crea uno script di avvio per Windows (.bat)"""
        backend_conf = self.config['server']['backend']
        frontend_conf = self.config['server']['frontend']
        app_name = self.config['app']['name']
        backend_exe = f"{self.config['build']['backend_project']}.exe"

        # --- CORREZIONE FINALE QUI ---
        # Serve la cartella `./frontend` e apre `index.html` alla radice.
        launcher_content = f'''@echo off
title {app_name} Launcher
cd /d "%~dp0"

echo Avvio di {app_name} in corso...

REM Avvia il Backend
echo Avvio del Backend Server...
start "Backend" /B .\\backend\\{backend_exe} --urls="http://{backend_conf['host']}:{backend_conf['port']}"

REM Avvia il Frontend
echo Avvio del Frontend Server...
start "Frontend" /B powershell -command "npx http-server ./frontend -a {frontend_conf['host']} -p {frontend_conf['port']} --cors -o index.html"

echo.
echo {app_name} e' in esecuzione.
echo Chiudi questa finestra per terminare l'applicazione.
pause >nul

echo Arresto dell'applicazione in corso...
taskkill /F /FI "WINDOWTITLE eq Backend" /T > nul
taskkill /F /FI "WINDOWTITLE eq Frontend" /T > nul
'''
        launcher_path = self.build_dir / f"start_{app_name.replace(' ', '_')}.bat"
        with open(launcher_path, 'w', encoding='utf-8') as f:
            f.write(launcher_content)

    def create_unix_launcher(self):
        """Crea uno script di avvio per Unix (Linux/macOS) (.sh)"""
        backend_conf = self.config['server']['backend']
        frontend_conf = self.config['server']['frontend']
        app_name = self.config['app']['name']
        backend_exec = self.config['build']['backend_project']

        # --- CORREZIONE FINALE QUI ---
        # Serve la cartella `./frontend` e apre `index.html` alla radice.
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
    
    def build(self):
        """Esegue l'intero processo di build"""
        try:
            print(f"=== Build di {self.config['app']['name']} ===")
            
            self.clean()
            
            target = self.config['targets'][0] 
            
            self.build_backend_for_target(target)
            self.copy_and_configure_frontend()
            self.create_launcher_script(target)
            
            zip_path = self.create_zip_archive()
            
            print(f"\n✅ Build completato con successo!")
            print(f"📦 Archivio creato in: {zip_path}")
            print(f"📁 Dimensione: {zip_path.stat().st_size / 1024 / 1024:.1f} MB")
            
        except Exception as e:
            print(f"❌ Build fallito: {e}")
            return False
        
        return True

if __name__ == "__main__":
    builder = AppBuilder()
    builder.build()