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
    frontend_dst = obj.app_dir / "wwwroot"
    
    if frontend_dst.exists():
        shutil.rmtree(frontend_dst)
    shutil.copytree(frontend_src, frontend_dst)

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

def copy_build_json_to_build(obj, filter=True):
    """Copia build.json nella cartella di build, scegliendo se rimuovere la connection_string"""
    print("Copia di build.json nella cartella di build...")
    build_json_src = obj.script_dir / "build.json"
    build_json_dst = obj.build_dir / "build.json"
    
    # Legge il file build.json originale
    with open(build_json_src, 'r', encoding='utf-8') as f:
        config = json.load(f)
    
    # Rimuove la connection_string se presente
    if filter and ('server' in config and 'backend' in config['server']):
        if 'connection_string' in config['server']['backend']:
            del config['server']['backend']

    if filter and ('build' in config):
        del config['build']
    
    # Scrive il file senza connection_string
    with open(build_json_dst, 'w', encoding='utf-8') as f:
        json.dump(config, f, indent=4, ensure_ascii=False)
    
    print("✅ build.json copiato nella cartella di build")
    return True

def copy_documentation_to_build(obj):
    """Copia README.md nella cartella di build"""
    print("Copia di README.md nella cartella di build...")
    
    build_doc_src = obj.project_root / "Docs/README.md"
    build_doc_dst = obj.build_dir / "README.md"
    
    # Legge il file README.md originale
    with open(build_doc_src, 'r', encoding='utf-8') as f:
        documentation = f.read()
    
    # Scrive il file README.md nella cartella di build
    with open(build_doc_dst, 'w', encoding='utf-8') as f:
        f.write(documentation)
    
    print("✅ Documentazione copiata nella cartella di build")
    return True
        
def default_config():
    """Configurazione di default"""
    return {
        "app": {
            "name": "Pietribiasi App",
            "version": "1.0.0",
            "description": "Applicazione Pietribiasi",
            "author": "A3 Soluzioni Informatiche"
        },
        "build": {
            "backend_project": "apiPB",
            "frontend_path": "Frontend",
            "output_dir": "PietribiasiApp_distribution",
            "temp_dir": "PietribiasiApp"
        },
        "targets": [
            {
                "name": "Windows",
                "runtime": "win-x64",
                "executable_extension": ".exe"
            },
            {
                "name": "Linux",
                "runtime": "linux-x64",
                "executable_extension": ""
            },
            {
                "name": "macOS",
                "runtime": "osx-x64",
                "executable_extension": ""
            }
        ],
        "packaging": {
            "create_installer": True,
            "create_portable": True,
            "compression_level": 6
        },
        "server": {
            "backend": {
                "host": "localhost",
                "port": 5245,
                "connection_string": "connectionstring_placeholder"
            }
        }
    }

async def build_backend_for_target(self, target):
        """Compila il backend per una specifica piattaforma"""
        print(f"Compilazione del backend per {target['name']}...")
        
        backend_path = self.project_root / self.config['build']['backend_project']
        build_output = self.app_dir

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

# Update appsettings.json in BuildAndDistr
async def update_appsettings(obj):
    f"""Aggiorna appsettings.json in {obj.app_dir} con i valori da build.json nella root"""
    print("Aggiornamento di appsettings.json...")

    # Carica la configurazione da build.json nella root
    build_json_path = obj.script_dir / "build.json"
    if not build_json_path.exists():
        print(f"ATTENZIONE: {build_json_path} non trovato.")
        return

    with open(build_json_path, 'r', encoding='utf-8') as f:
        config = json.load(f)

    # Percorsi dei file da aggiornare
    appsettings_path = obj.app_dir / "appsettings.json"

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

        appsettings['ConnectionStrings'] = {
            "LocalA3Db": config['server']['backend'].get('connection_string', '')
        }

        # Scrivi il file aggiornato
        with open(appsettings_path, 'w', encoding='utf-8-sig') as f:
            json.dump(appsettings, f, indent=2, ensure_ascii=False)

        print(f"✅ appsettings.json aggiornato in {appsettings_path}:")
        print(f"   Backend: {config['server']['backend']['host']}:{config['server']['backend']['port']}")

    except Exception as e:
        print(f"❌ Errore durante l'aggiornamento di {appsettings_path}: {e}")

def create_build_and_distr_dir(obj):
    build_and_distr = obj.project_root / "BuildAndDistr"
    build_and_distr.mkdir(exist_ok=True)
    build_dir = build_and_distr / obj.config['build']['temp_dir']
    dist_dir = build_and_distr / obj.config['build']['output_dir']
    app_dir = build_dir / "App"
    script_dir = obj.project_root / "BuildScripts"
    return build_dir, dist_dir, app_dir, script_dir

async def update_host_ip(build_json_path, automatic_ip=True):
    """Aggiorna server.backend.host con l'IP locale."""
    print("❗❗ Aggiornamento dell'IP locale nel file build.json... ❗❗")
    with open(build_json_path, 'r', encoding='utf-8') as f:
        config = json.load(f)

    if automatic_ip:
        local_ip = get_local_ip()
        
        config['server']['backend']['host'] = local_ip

        
        with open(build_json_path, 'w', encoding='utf-8') as f:
            json.dump(config, f, indent=4, ensure_ascii=False)
        print(f"✅ server.backend.host aggiornato a {local_ip}")
    else:
        print("Aggiornamento automatico dell'IP locale disabilitato. Nessun cambiamento effettuato.")

async def build_backend_for_target(obj, target):
    """Compila il backend per una specifica piattaforma"""
    print(f"Compilazione del backend per {target['name']}...")
    
    backend_path = obj.project_root / obj.config['build']['backend_project']
    build_output = obj.app_dir

    # Leggi la connection string dal file build.json
    connection_string = obj.config['server']['backend']['connection_string']
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

def create_launcher_script(obj):
    """
    Crea uno script batch che esegue apiPB.exe dalla cartella /App
    
    Args:
        obj: Oggetto che contiene l'attributo build_dir (percorso dove salvare lo script)
    """
    # Contenuto dello script batch
    batch_content = '''@echo off
echo Avvio di apiPB.exe...
cd /d "%~dp0"
if exist "App\\apiPB.exe" (
    echo File apiPB.exe trovato in App
    cd App
    echo Avvio di apiPB.exe...
    rem Esegui il file apiPB.exe
    .\\apiPB.exe
    echo apiPB.exe avviato con successo
) else (
    echo ERRORE: File apiPB.exe non trovato nella cartella App
    pause
)
'''
    
    # Percorso completo del file batch
    batch_file_path = os.path.join(obj.build_dir, "start.bat")
    
    try:
        # Crea la directory se non esiste
        os.makedirs(obj.build_dir, exist_ok=True)
        
        # Scrivi il file batch
        with open(batch_file_path, 'w', encoding='utf-8') as f:
            f.write(batch_content)
        
        print(f"Script batch creato con successo in: {batch_file_path}")
        return batch_file_path
        
    except Exception as e:
        print(f"Errore durante la creazione dello script batch: {e}")
        return None