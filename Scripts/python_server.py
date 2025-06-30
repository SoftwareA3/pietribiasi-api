#!/usr/bin/env python3
# desktop_app.py
import webview
import threading
import time
import json
from pathlib import Path
import socket
import sys
import os

# Importazioni Flask
from flask import Flask, send_from_directory, request
from flask_cors import CORS

DIRECTORY = "frontend"

class PietribiasiApp:    
    def get_icon_path(self, use_absolute=True):
        icon_path = Path("frontend/assets/icon.png")
        absolute_path = os.path.abspath(icon_path)
        return str(absolute_path) if use_absolute else str(icon_path)
        
    def run(self):
        """Avvia l'applicazione desktop"""
        
        # Legge l'ip del server dal file di configurazione
        config_path = Path("build.json")
        if config_path.exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                config = json.load(f)
            self.server_ip = config['server']['frontend']['host']
            self.server_port = config['server']['frontend']['port']
        else:
            print("Errore: File di configurazione 'build.json' non trovato!")
            return

        # URL dell'applicazione
        url = f"http://{self.server_ip}:{self.server_port}/index.html"
        
        # Configura la finestra
        window_config = {
            'title': 'Pietribiasi App',
            'width': 1200,
            'height': 800,
            'min_size': (800, 600),
            'resizable': True,
            'fullscreen': False,
            'shadow': True,
            'on_top': False,
        }
        
        # Aggiungi l'icona solo se esiste
        # Usa use_absolute=False per percorso relativo
        icon_path = self.get_icon_path(use_absolute=True)
        
        # Crea e mostra la finestra
        try:
            # Prova prima con l'icona, se fallisce riprova senza
            try:
                webview.create_window(
                    url=url,
                    **window_config,  
                )
            except TypeError as e:
                if 'icon' in str(e) and 'icon' in window_config:
                    print(f"Avviso: Parametro 'icon' non supportato in questa versione di pywebview,{str(e)}")
                    # Rimuovi l'icona e riprova
                    window_config_no_icon = {k: v for k, v in window_config.items() if k != 'icon'}
                    webview.create_window(
                        url=url,
                        **window_config_no_icon
                    )
                else:
                    raise e
            
            # Avvia l'applicazione (blocca fino alla chiusura)
            webview.start(
                debug=False,  # Imposta True per debug
                http_server=False,  # Usiamo il nostro server
            )
            
        except Exception as e:
            print(f"Errore nell'avvio dell'applicazione: {e}")

def main():
    """Funzione principale"""
    print("Avvio Pietribiasi App...")
    
    # Controlla se siamo nella directory corretta
    if not Path("frontend").exists():
        print("Errore: Directory 'frontend' non trovata!")
        print("Assicurati di eseguire lo script dalla directory principale del progetto.")
        input("Premi INVIO per uscire...")
        return
    
    # Controlla se Flask è installato
    try:
        import flask
        import flask_cors
    except ImportError:
        print("Errore: Flask non è installato!")
        print("Installa le dipendenze con: pip install flask flask-cors")
        input("Premi INVIO per uscire...")
        return
    
    # Avvia l'applicazione
    app = PietribiasiApp()
    app.run()
    
    print("Applicazione terminata.")

if __name__ == "__main__":
    main()