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

class WebServer:
    def __init__(self):
        self.app = None
        self.server_thread = None
        self.host = "localhost"
        self.port = 8080
        
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
        
    def load_config_from_build_json(self, config_path="build.json"):
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
        
    def load_config_from_local_ip(self, config_path="build.json"):
        """Carica la configurazione per la porta dal file build.json e l'IP locale"""
        try:
            # Ottiene l'IP locale
            self.host = self.get_local_ip()
            print(f"IP locale rilevato: {self.host}")

            # Carica la configurazione dal file build.json
            if Path(config_path).exists():
                with open(config_path, 'r', encoding='utf-8') as f:
                    config = json.load(f)
                
                frontend_config = config.get('server', {}).get('frontend', {})
                self.port = frontend_config.get("port", 8080)

                print(f"Configurazione caricata: {self.host}:{self.port}")
            else:
                print("File di configurazione non trovato, uso valori predefiniti")
        except Exception as e:
            print(f"Errore nel caricamento configurazione: {e}")

    def create_flask_app(self):
        """Crea e configura l'applicazione Flask"""
        app = Flask(__name__)
        CORS(app)  # Abilita CORS per tutte le route
        
        # Disabilita il logging di Flask per mantenere l'output pulito
        import logging
        log = logging.getLogger('werkzeug')
        log.setLevel(logging.ERROR)
        
        # Disabilita il caching
        @app.after_request
        def after_request(response):
            response.headers["Cache-Control"] = "no-cache, no-store, must-revalidate"
            response.headers["Pragma"] = "no-cache"
            response.headers["Expires"] = "0"
            return response
        
        # Serve i file statici
        @app.route('/', defaults={'path': ''})
        @app.route('/<path:path>')
        def serve_static(path):
            if path == "":
                path = "index.html"
            try:
                return send_from_directory(DIRECTORY, path)
            except FileNotFoundError:
                # Fallback per SPA (Single Page Application)
                return send_from_directory(DIRECTORY, 'index.html')
        
        # Gestisci le richieste API che non esistono (evita errori 404 nel frontend)
        @app.route('/api/<path:path>', methods=['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'])
        def api_not_found(path):
            return {"error": "API endpoint not found", "path": path}, 404
        
        return app

    def start_server(self, config_path="build.json"):
        """Avvia il server Flask in un thread separato"""
        if not Path(DIRECTORY).exists():
            print(f"Errore: Directory '{DIRECTORY}' non trovata!")
            return False
            
        # Carica configurazione
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                config = json.load(f)
        
        if config['server']['frontend']['local_ip_automatically'] == True or "true":
            self.load_config_from_local_ip(config_path)
        else:
            self.load_config_from_build_json(config_path)
        
        # Crea l'app Flask
        self.app = self.create_flask_app()
        
        def run_flask():
            """Funzione per eseguire Flask nel thread separato"""
            try:
                self.app.run(
                    host=self.host, 
                    port=self.port, 
                    debug=False, 
                    threaded=True,
                    use_reloader=False 
                )
            except Exception as e:
                print(f"Errore nell'esecuzione del server Flask: {e}")
        
        try:
            self.server_thread = threading.Thread(target=run_flask, daemon=True)
            self.server_thread.start()
            print(f"Server Flask avviato su http://{self.host}:{self.port}")
            return True
            
        except Exception as e:
            print(f"Errore avvio server Flask: {e}")
            return False
    
    def stop_server(self):
        """Ferma il server Flask"""
        print("Arresto del server...")
        # Con Flask in modalità threaded, il thread si chiude automaticamente
        # quando l'applicazione principale termina (daemon=True)
        if self.server_thread:
            print("Server arrestato")

class PietribasiApp:
    def __init__(self):
        self.web_server = WebServer()
        
    def on_window_closed(self):
        """Callback chiamato quando la finestra viene chiusa"""
        print("Finestra chiusa, arresto dell'applicazione...")
        self.web_server.stop_server()
    
    def get_icon_path(self, use_absolute=True):
        icon_path = Path("frontend/assets/icon.ico")
        absolute_path = os.path.abspath(icon_path)
        return str(absolute_path) if use_absolute else str(icon_path)
        
    def run(self):
        """Avvia l'applicazione desktop"""
        # Avvia il server
        if not self.web_server.start_server():
            print("Impossibile avviare il server. Uscita.")
            return
        
        # Attende che il server sia pronto
        time.sleep(2)  # Aumentato il tempo di attesa per Flask
        
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
    app = PietribasiApp()
    app.run()
    
    print("Applicazione terminata.")

if __name__ == "__main__":
    main()