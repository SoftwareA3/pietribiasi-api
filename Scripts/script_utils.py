import shutil
from pathlib import Path
import socket
import json
import asyncio
import zipfile
import os
import subprocess

async def clean(obj):
    """Pulisce le directory di build e di distribuzione"""
    print("Pulizia delle directory in corso...")
    if obj.build_dir.exists():
        shutil.rmtree(obj.build_dir)
    if obj.dist_dir.exists():
        shutil.rmtree(obj.dist_dir)
    
    obj.build_dir.mkdir(exist_ok=True)
    obj.dist_dir.mkdir(exist_ok=True)

async def copy_and_configure_frontend(obj, build_name):
    """Copia il frontend e configura l'URL dell'API in main.js"""
    print("Copia e configurazione del frontend...")
    # La cartella sorgente del frontend
    frontend_src = obj.project_root / obj.config[build_name]['frontend_path']
    # La cartella di destinazione nella build
    frontend_dst = obj.build_dir / "frontend"
    
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
    if obj.config.get('remote_backend', {}).get('enabled', False):
        remote_config = obj.config['remote_backend']
        api_base_url = f"{remote_config['protocol']}://{remote_config['host']}:{remote_config['port']}"
    else:
        # Fallback alla configurazione normale
        backend_config = obj.config['server']['backend']
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

def get_local_ip():
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

async def update_frontend_host_ip(build_json_path):
    """Aggiorna server.frontend.host con l'IP locale."""
    with open(build_json_path, 'r', encoding='utf-8') as f:
        config = json.load(f)
    automatic_ip = config['server']['frontend']['local_ip_automatically']
    if automatic_ip:
        local_ip = get_local_ip()
        config['server']['frontend']['host'] = local_ip
        with open(build_json_path, 'w', encoding='utf-8') as f:
            json.dump(config, f, indent=4, ensure_ascii=False)
        print(f"✅ server.frontend.host aggiornato a {local_ip}")

def create_zip_archive(obj):
    """Crea l'archivio ZIP finale"""
    print("Creazione dell'archivio ZIP...")
    zip_path = obj.dist_dir / f"{obj.config['app']['name']}_v{obj.config['app']['version']}.zip"
    
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zf:
        for root, _, files in os.walk(obj.build_dir):
            for file in files:
                file_path = Path(root) / file
                arc_path = file_path.relative_to(obj.build_dir)
                zf.write(file_path, arc_path)

    return zip_path

def copy_build_json_to_build(obj, filter_frontend=True):
    """Copia build.json nella cartella di build, scegliendo se rimuovere la connection_string"""
    print("Copia di build.json nella cartella di build...")
    
    build_json_src = obj.project_root / "Scripts/build.json"
    build_json_dst = obj.build_dir / "build.json"
    
    # Legge il file build.json originale
    with open(build_json_src, 'r', encoding='utf-8') as f:
        config = json.load(f)
    
    # Rimuove la connection_string se presente
    if filter_frontend and ('server' in config and 'backend' in config['server']):
        if 'connection_string' in config['server']['backend']:
            del config['server']['backend']

    if filter_frontend and ('build' in config):
        del config['build']
    
    # Scrive il file senza connection_string
    with open(build_json_dst, 'w', encoding='utf-8') as f:
        json.dump(config, f, indent=4, ensure_ascii=False)
    
    print("✅ build.json copiato nella cartella di build")
    return True
        
def default_config():
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

async def update_appsettings(obj):
    """Aggiorna appsettings.json in apiPB/ con i valori da build.json nella root"""
    print("Aggiornamento di appsettings.json...")

    # Carica la configurazione da build.json nella root
    build_json_path = obj.project_root / "Scripts/build.json"
    if not build_json_path.exists():
        print(f"ATTENZIONE: {build_json_path} non trovato.")
        return

    with open(build_json_path, 'r', encoding='utf-8') as f:
        config = json.load(f)

    # Percorsi dei file da aggiornare
    appsettings_path = obj.project_root / "apiPB" / "appsettings.json"

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

def create_build_and_distr_dir(obj, build_name):
    build_and_distr = obj.project_root / "BuildAndDistr"
    build_and_distr.mkdir(exist_ok=True)
    build_dir = build_and_distr / obj.config[build_name]['temp_dir']
    dist_dir = build_and_distr / obj.config[build_name]['output_dir']
    return build_dir, dist_dir

def create_executable_from_batchscript(obj):
    """Crea un eseguibile PowerShell da uno script .ps1 usando ps2exe"""
    print("Creazione eseguibile PowerShell...")
    
    # Percorsi dei file
    bat_file = obj.build_dir / "Pietribiasi_App_start.bat"
    ps1_file = obj.build_dir / "Pietribiasi_App.ps1"
    exe_file = obj.build_dir / "Pietribiasi_App.exe"
    icon_file = obj.project_root / "assets/icon.ico"
    
    # Verifica che il file .bat esista
    if not bat_file.exists():
        print(f"❌ ERRORE: File {bat_file} non trovato!")
        return False
    
    # Creazione del file PowerShell (.ps1) che esegue il .bat
    ps1_content = '''
# Script PowerShell per avviare Pietribiasi App
Write-Host "Avvio Pietribiasi App..."

# Ottieni la directory dello script in modo robusto
$scriptPath = ""
if ($PSScriptRoot) {
    $scriptPath = $PSScriptRoot
} elseif ($MyInvocation.MyCommand.Path) {
    $scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
} else {
    # Fallback per eseguibili ps2exe
    $scriptPath = [System.IO.Path]::GetDirectoryName([System.Reflection.Assembly]::GetExecutingAssembly().Location)
}

Write-Host "Directory script: $scriptPath"

$batPath = [System.IO.Path]::Combine($scriptPath, "Pietribiasi_App_start.bat")
Write-Host "Percorso file bat: $batPath"

if (Test-Path $batPath) {
    Write-Host "File trovato, avvio in corso..."
    try {
        # Avvia il processo e chiudi la console corrente
        Start-Process -FilePath $batPath -WindowStyle Normal
        exit
    } catch {
        Write-Host "ERRORE durante l'avvio: $($_.Exception.Message)" -ForegroundColor Red
        Read-Host "Premi INVIO per chiudere"
    }
} else {
    Write-Host "ERRORE: File $batPath non trovato!" -ForegroundColor Red
    Write-Host "Verifica che il file Pietribiasi_App_start.bat sia nella stessa directory dell'eseguibile." -ForegroundColor Yellow
    Read-Host "Premi INVIO per chiudere"
}
'''
    
    try:
        # Scrivi il file .ps1
        with open(ps1_file, 'w', encoding='utf-8') as f:
            f.write(ps1_content)
        print(f"✅ File PowerShell creato: {ps1_file}")
        
        # Comando PowerShell per convertire .ps1 in .exe con opzioni migliorate
        cmd = f'Invoke-ps2exe -inputFile "{ps1_file}" -outputFile "{exe_file}" -noConsole:$false -title "Pietribiasi App" -version "1.0.0.0" -copyright "Pietribiasi" -iconFile "{icon_file}"'
        
        print(f"Esecuzione comando: {cmd}")
        result = subprocess.run(
            ["powershell", "-ExecutionPolicy", "Bypass", "-Command", cmd], 
            check=True, 
            capture_output=True, 
            text=True,
            cwd=str(obj.build_dir)
        )
        
        if result.returncode == 0:
            print("✅ Comando ps2exe eseguito con successo")
            if result.stdout:
                print(f"Output: {result.stdout}")
        else:
            print(f"❌ Errore nell'esecuzione: {result.stderr}")
            return False
        
        # Verifica che l'eseguibile sia stato creato
        if exe_file.exists():
            print(f"✅ Eseguibile creato con successo: {exe_file}")
            print(f"📁 Dimensione file: {exe_file.stat().st_size / 1024:.1f} KB")
            return True
        else:
            print("❌ L'eseguibile non è stato creato")
            return False
            
    except subprocess.CalledProcessError as e:
        print(f"❌ Errore durante l'esecuzione del comando PowerShell:")
        print(f"   Codice di uscita: {e.returncode}")
        if e.stdout:
            print(f"   Stdout: {e.stdout}")
        if e.stderr:
            print(f"   Stderr: {e.stderr}")
        return False
    except Exception as e:
        print(f"❌ Errore generico: {e}")
        return False

def copy_python_server(obj):
        """Copia python_server.py nella cartella di build per permettere la riconfigurazione runtime"""
        print("Copia di python_server.py nella cartella di build...")
        
        python_server_src = obj.project_root / "Scripts/python_server.py"
        python_server_dst = obj.build_dir / "python_server.py"
        
        if python_server_src.exists():
            shutil.copy2(python_server_src, python_server_dst)
            print("✅ python_server.py copiato nella cartella di build")
            return True
        else:
            # Crea un build.json con la configurazione corrente
            with open(python_server_dst, 'w', encoding='utf-8') as f:
                json.dump(obj.config, f, indent=2, ensure_ascii=False)
            print("✅ python_server.py generato nella cartella di build")
            return True