#!/usr/bin/env python3
import os
import shutil
import subprocess
import json
import zipfile
import socket
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
    
    def update_appsettings(self):
        """Aggiorna appsettings.json con i valori da build.json"""
        print("Aggiornamento di appsettings.json...")
        
        appsettings_path = self.project_root / self.config['build']['backend_project'] / "appsettings.json"
        
        if not appsettings_path.exists():
            print(f"ATTENZIONE: {appsettings_path} non trovato.")
            return
        
        try:
            # Leggi il file appsettings.json esistente
            with open(appsettings_path, 'r', encoding='utf-8') as f:
                appsettings = json.load(f)
            
            # Aggiungi/aggiorna la sezione server
            if 'Server' not in appsettings:
                appsettings['Server'] = {}
            
            appsettings['Server']['Backend'] = {
                "Host": self.config['server']['backend']['host'],
                "Port": self.config['server']['backend']['port']
            }
            
            appsettings['Server']['Frontend'] = {
                "Host": self.config['server']['frontend']['host'],
                "Port": self.config['server']['frontend']['port']
            }
            
            # Scrivi il file aggiornato
            with open(appsettings_path, 'w', encoding='utf-8') as f:
                json.dump(appsettings, f, indent=2, ensure_ascii=False)
            
            print(f"✅ appsettings.json aggiornato con:")
            print(f"   Backend: {self.config['server']['backend']['host']}:{self.config['server']['backend']['port']}")
            print(f"   Frontend: {self.config['server']['frontend']['host']}:{self.config['server']['frontend']['port']}")
            
        except Exception as e:
            print(f"❌ Errore durante l'aggiornamento di appsettings.json: {e}")

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
            
    
    def create_advanced_start_bat(self):
        """Crea lo script Pietribiasi_App_start.bat avanzato basato su quello esistente ma con configurazioni da build.json"""
        print("Creazione dello script di controllo avanzato...")
        
        backend_config = self.config['server']['backend']
        frontend_config = self.config['server']['frontend']
        app_name = self.config['app']['name']
        backend_project = self.config['build']['backend_project']
        
        start_bat_content = f'''@echo off
setlocal enabledelayedexpansion
title Controllo Applicazione

REM Variabili per memorizzare i PID dei processi
set BACKEND_PID=
set FRONTEND_PID=

REM Carica la configurazione da build.json
call :LOAD_CONFIG_FROM_BUILD_JSON

:MENU
cls
echo ===============================================
echo    CONTROLLO APPLICAZIONE {app_name.upper()}
echo ===============================================
echo.
echo 0. Avvia/Riavvia applicazione (apertura automatica su index.html)
echo 1. Avvia applicazione 
echo 2. Ferma applicazione  
echo 3. Riavvia applicazione
echo 4. Stato applicazione
echo 5. Mostra indirizzi IP
echo 6. Esci
echo.
set /p choice="Seleziona un'opzione (0-6): "

if "%choice%"=="0" goto FORCE_RESTART_WITH_INDEX
if "%choice%"=="1" goto START_APP
if "%choice%"=="2" goto STOP_APP
if "%choice%"=="3" goto RESTART_APP
if "%choice%"=="4" goto STATUS_APP
if "%choice%"=="5" goto SHOW_IPS
if "%choice%"=="6" goto EXIT
goto MENU

:LOAD_CONFIG_FROM_BUILD_JSON
echo Caricamento configurazione da build.json...

REM Valori di default (caricati da build.json)
set FRONTEND_PORT={frontend_config['port']}
set BACKEND_PORT={backend_config['port']}
set SERVER_IP={backend_config['host']}

REM Verifica se build.json esiste
if not exist "build.json" (
    echo ATTENZIONE: build.json non trovato, uso configurazione embedded
    goto :eof
)

REM Estrae i valori da build.json usando PowerShell
for /f "delims=" %%i in ('powershell -Command "try {{ $$json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $$json.server.backend.host }} catch {{ Write-Output '{backend_config['host']}' }}"') do set SERVER_IP=%%i
for /f "delims=" %%i in ('powershell -Command "try {{ $$json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $$json.server.backend.port }} catch {{ Write-Output '{backend_config['port']}' }}"') do set BACKEND_PORT=%%i
for /f "delims=" %%i in ('powershell -Command "try {{ $$json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $$json.server.frontend.port }} catch {{ Write-Output '{frontend_config['port']}' }}"') do set FRONTEND_PORT=%%i

echo Configurazione caricata:
echo   SERVER_IP: %SERVER_IP%
echo   BACKEND_PORT: %BACKEND_PORT%
echo   FRONTEND_PORT: %FRONTEND_PORT%
echo.

goto :eof

:FORCE_RESTART_WITH_INDEX
echo.
echo Riavvio forzato dell'applicazione con apertura automatica su index.html...
echo.
echo Arresto di eventuali processi in esecuzione...
call :STOP_APP_ENHANCED
timeout /t 2 >nul

echo Avvio del backend...
start "Backend Server" cmd /c "cd backend && {backend_project}.exe --urls=http://%SERVER_IP%:%BACKEND_PORT%"
timeout /t 5 >nul

echo Avvio del frontend...
start "Frontend Server" cmd /c "npx http-server ./frontend -a %SERVER_IP% -p %FRONTEND_PORT% --cors -o index.html -c-1"
timeout /t 3 >nul

echo.
echo Applicazione riavviata! Apertura automatica della pagina...
call :SHOW_ADDRESSES
echo.
pause
goto MENU

:START_APP
echo.
echo Avvio dell'applicazione...
echo.

REM Controlla se i processi sono già in esecuzione
call :CHECK_PROCESSES
if "!BACKEND_RUNNING!"=="1" (
    echo Backend già in esecuzione!
) else (
    echo Avvio del backend...
    start "Backend Server" cmd /c "cd backend && {backend_project}.exe --urls=http://%SERVER_IP%:%BACKEND_PORT%"
    timeout /t 3 >nul
)

if "!FRONTEND_RUNNING!"=="1" (
    echo Frontend già in esecuzione!
) else (
    echo Avvio del frontend...
    timeout /t 2 >nul
    start "Frontend Server" cmd /c "npx http-server ./frontend -a %SERVER_IP% -p %FRONTEND_PORT% --cors index.html -c-1"
)

echo.
echo Applicazione avviata!
call :SHOW_ADDRESSES
echo.
pause
goto MENU

:STOP_APP
echo.
echo Arresto completo dell'applicazione...
echo.
call :STOP_APP_ENHANCED
echo.
echo Applicazione arrestata completamente!
echo.
pause
goto MENU

:RESTART_APP
echo.
echo Riavvio dell'applicazione...
call :STOP_APP_ENHANCED
timeout /t 2 >nul
goto START_APP

:STOP_APP_ENHANCED
REM Funzione migliorata per chiusura completa

echo Chiusura completa di processi e finestre console...

REM 1. Prima terminiamo i processi applicazione
echo Terminazione processi backend...
taskkill /f /im {backend_project}.exe 2>nul

echo Terminazione processi frontend...
taskkill /f /im node.exe 2>nul

REM 2. Attendiamo che i processi terminino completamente
echo Attesa terminazione processi...
timeout /t 3 >nul

REM 3. Ora cerchiamo e chiudiamo le finestre CMD per comando di avvio
echo Chiusura finestre console backend...
wmic process where "name='cmd.exe' and commandline like '%%Backend Server%%'" delete 2>nul

echo Chiusura finestre console frontend...  
wmic process where "name='cmd.exe' and commandline like '%%Frontend Server%%'" delete 2>nul

REM 4. Metodo PowerShell per trovare finestre CMD con i nostri comandi
echo Pulizia finale finestre CMD...
powershell -Command "Get-WmiObject Win32_Process | Where-Object {{ $$_.Name -eq 'cmd.exe' -and ($$_.CommandLine -like '*cd backend*{backend_project}.exe*' -or $$_.CommandLine -like '*npx http-server*frontend*' -or $$_.CommandLine -like '*Backend Server*' -or $$_.CommandLine -like '*Frontend Server*') }} | ForEach-Object {{ try {{ Write-Host \"Chiusura finestra CMD PID: $$($$_.ProcessId)\"; $$_.Terminate() }} catch {{}} }}" 2>nul

REM 5. Metodo finale: cerca finestre che potrebbero essere tornate al titolo originale
timeout /t 1 >nul
taskkill /fi "WindowTitle eq Backend Server*" /f 2>nul
taskkill /fi "WindowTitle eq Frontend Server*" /f 2>nul

echo Pulizia completata.

goto :eof

:STATUS_APP
echo.
echo Stato dell'applicazione:
echo.

call :CHECK_PROCESSES

if "!BACKEND_RUNNING!"=="1" (
    echo [ATTIVO] Backend Server
) else (
    echo [INATTIVO] Backend Server
)

if "!FRONTEND_RUNNING!"=="1" (
    echo [ATTIVO] Frontend Server
) else (
    echo [INATTIVO] Frontend Server
)

echo.
pause
goto MENU

:CHECK_PROCESSES
set BACKEND_RUNNING=0
set FRONTEND_RUNNING=0

REM Controlla se il backend è in esecuzione
tasklist | findstr "{backend_project}.exe" >nul 2>&1
if %errorlevel%==0 set BACKEND_RUNNING=1

REM Controlla se http-server è in esecuzione
tasklist | findstr "node.exe" >nul 2>&1
if %errorlevel%==0 set FRONTEND_RUNNING=1

goto :eof

:SHOW_IPS
call :SHOW_ADDRESSES
pause
goto MENU

:SHOW_ADDRESSES
echo.
echo ----------------------------------------------
echo Per accedere all'applicazione:
echo.
echo Indirizzi IP disponibili:
ipconfig | findstr IPv4
echo.
echo Backend:  http://localhost:%BACKEND_PORT%
echo Frontend: http://localhost:%FRONTEND_PORT%
echo.
echo Dall'esterno della rete:
echo Backend:  http://%SERVER_IP%:%BACKEND_PORT%
echo Frontend: http://%SERVER_IP%:%FRONTEND_PORT%
echo ----------------------------------------------
goto :eof

:EXIT
echo.
set /p confirm="Vuoi chiudere anche l'applicazione in esecuzione? (s/n): "
if /i "%confirm%"=="s" (
    echo.
    echo Chiusura completa dell'applicazione...
    call :STOP_APP_ENHANCED
    timeout /t 2 >nul
    echo Pulizia completata.
)
echo.
echo Arrivederci!
timeout /t 2 >nul
exit'''

        # Scrivi il file Pietribiasi_App_start.bat nella directory di build
        start_bat_path = self.build_dir / "Pietribiasi_App_start.bat"
        with open(start_bat_path, 'w', encoding='utf-8') as f:
            f.write(start_bat_content)

    def copy_build_json_to_build(self):
        """Copia build.json nella cartella di build per permettere la riconfigurazione runtime"""
        print("Copia di build.json nella cartella di build...")
        
        build_json_src = self.project_root / "build.json"
        build_json_dst = self.build_dir / "build.json"
        
        if build_json_src.exists():
            shutil.copy2(build_json_src, build_json_dst)
            print("✅ build.json copiato nella cartella di build")
        else:
            # Crea un build.json con la configurazione corrente
            with open(build_json_dst, 'w', encoding='utf-8') as f:
                json.dump(self.config, f, indent=2, ensure_ascii=False)
            print("✅ build.json generato nella cartella di build")

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

    def update_frontend_host_ip(self, build_json_path):
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

    def update_backend_host_ip(self, build_json_path):
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
    
    def build(self):
        """Esegue l'intero processo di build"""
        try:
            print(f"=== Build di {self.config['app']['name']} ===")

            # Inserisce l'IP locale per il Frontend nel file build.json se configurato per farlo
            self.update_frontend_host_ip("build.json")

            # Inserisce l'IP locale per il Backend nel file build.json se configurato per farlo
            self.update_backend_host_ip("build.json")
            
            # Aggiorna i file di configurazione PRIMA del build
            self.update_appsettings()
            self.update_start_bat()
            
            self.clean()
            
            target = self.config['targets'][0] 
            
            self.build_backend_for_target(target)
            self.copy_and_configure_frontend()
            # self.copy_build_json_to_build()  # Nuovo: copia build.json
            self.create_launcher_script(target)
            
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
    builder.build()