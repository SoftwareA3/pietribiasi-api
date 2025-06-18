#!/usr/bin/env python3
# server_only.py
"""
Script per avviare solo il server HTTP senza la finestra desktop.
Utile per permettere connessioni remote da altre macchine.
"""

import signal
import sys
import time
from pathlib import Path

# Importa la classe WebServer dal modulo principale
try:
    from python_server import WebServer as WS
except ImportError:
    print("Errore: Impossibile importare WebServer da python_server.py")
    print("Assicurati che python_server.py sia nella stessa directory.")
    sys.exit(1)

def signal_handler(sig, frame):
    """Gestisce il segnale di interruzione (Ctrl+C)"""
    print("\nArresto del server in corso...")
    if hasattr(signal_handler, 'server') and signal_handler.server:
        signal_handler.server.stop_server()
    print("Server arrestato. Arrivederci!")
    sys.exit(0)

def main():
    """Funzione principale per avviare solo il server"""
    print("=== Pietribiasi App - Solo Server ===")
    print("Avvio del server HTTP per connessioni remote...")
    
    # Controlla se siamo nella directory corretta
    if not Path("frontend").exists():
        print("Errore: Directory 'frontend' non trovata!")
        print("Assicurati di eseguire lo script dalla directory principale del progetto.")
        input("Premi INVIO per uscire...")
        return
    
    # Configura il gestore del segnale per Ctrl+C
    signal.signal(signal.SIGINT, signal_handler)
    
    # Crea e avvia il server
    server = WS()
    signal_handler.server = server  # Salva il riferimento per la chiusura
    
    if server.start_server():
        print(f"Server avviato con successo!")
        print(f"Indirizzo locale: http://{server.host}:{server.port}")
        print("\nIl server è in esecuzione. Premi Ctrl+C per fermare.")
        
        try:
            # Mantiene il server in esecuzione
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            # Gestito dal signal_handler
            pass
    else:
        print("Impossibile avviare il server. Verifica la configurazione.")
        input("Premi INVIO per uscire...")

if __name__ == "__main__":
    main()