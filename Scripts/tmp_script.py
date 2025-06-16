import subprocess
import sys
import os
from pathlib import Path

def main():
    # Ottieni la directory dove si trova l'eseguibile
    if getattr(sys, 'frozen', False):
        # Se è un eseguibile compilato
        exe_dir = Path(sys.executable).parent
        print(f"Eseguibile trovato in: {exe_dir}")
    else:
        # Se è uno script Python
        exe_dir = Path(__file__).parent
        print(f"Script trovato in: {exe_dir}")
    
    # Percorso del file .bat
    bat_file = exe_dir / "Pietribiasi_App_start.bat"
    
    if not bat_file.exists():
        print(f"ERRORE: File {bat_file} non trovato!")
        input("Premi INVIO per chiudere...")
        return
    
    try:
        # Avvia il file .bat
        subprocess.run([str(bat_file)], shell=True, cwd=exe_dir)
    except Exception as e:
        print(f"Errore durante l'avvio: {e}")
        input("Premi INVIO per chiudere...")

if __name__ == "__main__":
    main()