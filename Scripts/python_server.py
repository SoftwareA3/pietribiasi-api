#!/usr/bin/env python3
# desktop_app.py
import webview
import threading
import time
import json
from pathlib import Path
import http.server
import socketserver
import socket
import sys
import os

DIRECTORY = "frontend"

class WebServer:
    def __init__(self):
        self.httpd = None
        self.server_thread = None
        self.host = "localhost"
        self.port = 8080
        
    def load_config(self, config_path="build.json"):
        """Carica la configurazione dal file build.json"""
        try:
            if Path(config_path).exists():
                with open(config_path, 'r', encoding='utf-8') as f:
                    config = json.load(f)
                
                frontend_config = config.get('server', {}).get('frontend', {})
                self.host = frontend_config.get("host", "localhost")
                self.port = frontend_config.get("port", 8080)
                
                print(f"Configurazione caricata: {self.host}:{self.port}")
            else:
                print("File di configurazione non trovato, uso valori predefiniti")
                
        except Exception as e:
            print(f"Errore nel caricamento configurazione: {e}")

    def start_server(self):
        """Avvia il server HTTP in un thread separato"""
        if not Path(DIRECTORY).exists():
            print(f"Errore: Directory '{DIRECTORY}' non trovata!")
            return False
            
        self.load_config()
        
        class HTTPServerHandler(http.server.SimpleHTTPRequestHandler):
            def __init__(self, *args, **kwargs):
                super().__init__(*args, directory=DIRECTORY, **kwargs)
            
            def end_headers(self):
                # CORS headers
                self.send_header("Access-Control-Allow-Origin", "*")
                self.send_header("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
                self.send_header("Access-Control-Allow-Headers", "Content-Type, Authorization")
                
                # Cache disabilitato
                self.send_header("Cache-Control", "no-cache, no-store, must-revalidate")
                self.send_header("Pragma", "no-cache")
                self.send_header("Expires", "0")
                
                super().end_headers()
            
            def do_OPTIONS(self):
                self.send_response(200)
                self.end_headers()
            
            def log_message(self, format, *args):
                # Log silenzioso in modalità desktop
                pass
        
        try:
            self.httpd = socketserver.TCPServer((self.host, self.port), HTTPServerHandler)
            self.server_thread = threading.Thread(target=self.httpd.serve_forever, daemon=True)
            self.server_thread.start()
            print(f"Server avviato su http://{self.host}:{self.port}")
            return True
            
        except OSError as e:
            if "Address already in use" in str(e):
                print(f"Errore: Porta {self.port} già in uso")
            else:
                print(f"Errore avvio server: {e}")
            return False
    
    def stop_server(self):
        """Ferma il server HTTP"""
        if self.httpd:
            print("Arresto del server...")
            self.httpd.shutdown()
            self.httpd.server_close()
            if self.server_thread:
                self.server_thread.join(timeout=2)
            print("Server arrestato")

class PietribasiApp:
    def __init__(self):
        self.web_server = WebServer()
        
    def on_window_closed(self):
        """Callback chiamato quando la finestra viene chiusa"""
        print("Finestra chiusa, arresto dell'applicazione...")
        self.web_server.stop_server()
        
    def run(self):
        """Avvia l'applicazione desktop"""
        # Avvia il server
        if not self.web_server.start_server():
            print("Impossibile avviare il server. Uscita.")
            return
        
        # Attende che il server sia pronto
        time.sleep(1)
        
        # URL dell'applicazione
        url = f"http://{self.web_server.host}:{self.web_server.port}/index.html"
        
        # Configura la finestra
        window_config = {
            'title': 'Pietribasi App',
            'width': 1200,
            'height': 800,
            'min_size': (800, 600),
            'resizable': True,
            'fullscreen': False,
            'shadow': True,
            'on_top': False,
        }
        #'icon': 'icon.png'  # Percorso all'icona (PNG, ICO, o file compatibile)
        
        # Crea e mostra la finestra
        try:
            webview.create_window(
                url=url,
                **window_config
            )
            
            # Avvia l'applicazione (blocca fino alla chiusura)
            webview.start(
                debug=False,  # Imposta True per debug
                http_server=False  # Usiamo il nostro server
            )
            
        except Exception as e:
            print(f"Errore nell'avvio dell'applicazione: {e}")
        finally:
            # Assicurati che il server sia fermato
            self.on_window_closed()

def main():
    """Funzione principale"""
    print("Avvio Pietribasi App...")
    
    # Controlla se siamo nella directory corretta
    if not Path("frontend").exists():
        print("Errore: Directory 'frontend' non trovata!")
        print("Assicurati di eseguire lo script dalla directory principale del progetto.")
        input("Premi INVIO per uscire...")
        return
    
    # Avvia l'applicazione
    app = PietribasiApp()
    app.run()
    
    print("Applicazione terminata.")

if __name__ == "__main__":
    main()