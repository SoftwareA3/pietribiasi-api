"""
Script per avviare solo il server HTTP.
Contiene la classe WebServer.
"""
# FIXME: Alcune fuzinoni sono state create per evitare la duplicazione dei processi.
# Risolto il problema della duplicazione dei processi in PyInstaller: normale per questi eseguibili

import signal
import sys
import time
import os
import atexit
import threading
import socket
import json
from pathlib import Path
from flask import Flask, send_from_directory
from flask_cors import CORS

# Variabili globali per il server
_server_instance = None
_server_thread = None
_shutdown_event = threading.Event()

class WebServer:
    """Classe per gestire il server Flask in modo standalone"""
    
    def __init__(self):
        self.app = None
        self.flask_server = None
        self.server_thread = None
        self.host = "localhost"
        self.port = 8080
        self.shutdown_flag = threading.Event()
        
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
                return send_from_directory("frontend", path)
            except FileNotFoundError:
                # Fallback per SPA (Single Page Application)
                try:
                    return send_from_directory("frontend", 'index.html')
                except FileNotFoundError:
                    return f"Errore: File '{path}' non trovato e index.html non disponibile", 404
        
        # Gestisci le richieste API che non esistono (evita errori 404 nel frontend)
        @app.route('/api/<path:path>', methods=['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'])
        def api_not_found(path):
            return {"error": "API endpoint not found", "path": path}, 404
        
        # Route per controllo stato server
        @app.route('/health')
        def health_check():
            return {"status": "ok", "server": "Pietribiasi App Server"}
        
        # Route per shutdown controllato (per debugging)
        @app.route('/shutdown', methods=['POST'])
        def shutdown():
            if self.shutdown_flag:
                self.shutdown_flag.set()
                return {"message": "Server shutting down..."}
            return {"error": "Shutdown not available"}, 403
        
        return app

    def start_server(self, config_path="build.json"):
        """Avvia il server Flask in un thread separato"""
        if not Path("frontend").exists():
            print(f"Errore: Directory 'frontend' non trovata!")
            return False
            
        self.load_config_from_build_json(config_path)
        
        # Controlla che la porta sia libera
        if self.is_port_in_use(self.host, self.port):
            print(f"Errore: Porta {self.port} già in uso su {self.host}!")
            return False
        
        # Crea l'app Flask
        self.app = self.create_flask_app()
        
        def run_flask():
            """Funzione per eseguire Flask nel thread separato"""
            try:
                # Usa il server Werkzeug per un controllo migliore
                from werkzeug.serving import make_server
                
                self.flask_server = make_server(
                    host=self.host, 
                    port=self.port, 
                    app=self.app,
                    threaded=True
                )
                
                print(f"Server Flask configurato su {self.host}:{self.port}")
                
                # Avvia il server - questo blocca fino allo shutdown
                self.flask_server.serve_forever()
                
            except Exception as e:
                print(f"Errore nell'esecuzione del server Flask: {e}")
                self.shutdown_flag.set()
        
        try:
            self.server_thread = threading.Thread(target=run_flask, daemon=False)
            self.server_thread.start()
            
            # Attendi che il server sia pronto
            max_attempts = 10
            for attempt in range(max_attempts):
                time.sleep(0.5)
                if self.is_port_in_use(self.host, self.port):
                    print(f"Server Flask avviato con successo su http://{self.host}:{self.port}")
                    return True
                    
            print("Timeout: Server Flask non si è avviato nei tempi previsti")
            return False
            
        except Exception as e:
            print(f"Errore avvio server Flask: {e}")
            return False
    
    def is_port_in_use(self, host, port):
        """Controlla se una porta è già in uso"""
        try:
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
                s.settimeout(1)
                result = s.connect_ex((host, port))
                return result == 0
        except Exception:
            return False
    
    def stop_server(self):
        """Ferma il server Flask"""
        print("Arresto del server in corso...")
        
        # Imposta il flag di shutdown
        self.shutdown_flag.set()
        
        # Ferma il server Flask se esiste
        if hasattr(self, 'flask_server') and self.flask_server:
            try:
                print("Fermando server Flask...")
                self.flask_server.shutdown()
                print("Server Flask fermato")
            except Exception as e:
                print(f"Errore fermando server Flask: {e}")
        
        # Attendi che il thread si chiuda
        if self.server_thread and self.server_thread.is_alive():
            try:
                print("Attesa chiusura thread server...")
                self.server_thread.join(timeout=5)
                if self.server_thread.is_alive():
                    print("⚠️ Warning: Server thread non si è chiuso nei tempi previsti")
            except Exception as e:
                print(f"Errore durante join del thread: {e}")
        
        print("Server arrestato completamente")

def signal_handler(sig, frame):
    """Gestisce il segnale di interruzione (Ctrl+C)"""
    print(f"\nRicevuto segnale di interruzione ({sig})")
    
    print("Pulizia completata. Arrivederci!")
    
    # Forza l'uscita dopo un breve delay
    def force_exit():
        time.sleep(1)
        os._exit(0)
    
    threading.Thread(target=force_exit, daemon=True).start()

def check_single_instance():
    """Controlla che ci sia una sola istanza in esecuzione"""
    try:
        # Prova a creare un socket su una porta specifica per verificare l'unicità
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        sock.bind(('localhost', 58901))  # Porta per controllo unicità
        sock.listen(1)
        return sock
    except socket.error:
        print("❌ Un'altra istanza dell'applicazione è già in esecuzione!")
        print("   Chiudi l'altra istanza prima di avviarne una nuova.")
        time.sleep(3)
        sys.exit(1)

def check_dependencies():
    """Controlla che tutte le dipendenze siano presenti (solo in modalità script Python)"""
    # Se viene eseguito come eseguibile PyInstaller, non controllare le dipendenze
    # Le dipendenze vengono già controllate durante il processo di build
    if getattr(sys, 'frozen', False):
        return

    missing_deps = []
    
    try:
        import flask
    except ImportError:
        missing_deps.append("flask")
    
    try:
        import flask_cors
    except ImportError:
        missing_deps.append("flask-cors")
    
    if missing_deps:
        print("❌ Dipendenze mancanti:")
        for dep in missing_deps:
            print(f"   - {dep}")
        print("\nInstalla le dipendenze con:")
        print("   pip install " + " ".join(missing_deps))
        input("\nPremi INVIO per uscire...")
        sys.exit(1)

def main():
    """Funzione principale per avviare solo il server Flask"""
    global _server_instance
    
    print("=" * 50)
    print("    PIETRIBIASI APP - SERVER STANDALONE")
    print("=" * 50)
    print()
    
    # Controlla che ci sia una sola istanza
    print("Controllo istanza unica...")
    instance_lock = check_single_instance()
    print("✅ Controllo superato")
    
    # Controlla dipendenze
    print("Controllo dipendenze...")
    check_dependencies()
    print("✅ Dipendenze OK")
    
    # Controlla directory frontend
    print("Controllo struttura progetto...")
    if not Path("frontend").exists():
        print("❌ Errore: Directory 'frontend' non trovata!")
        print("   Assicurati di eseguire lo script dalla directory principale del progetto.")
        input("\nPremi INVIO per uscire...")
        return
    print("✅ Struttura progetto OK")
    
    # Configura i gestori dei segnali
    signal.signal(signal.SIGINT, signal_handler)
    if hasattr(signal, 'SIGTERM'):
        signal.signal(signal.SIGTERM, signal_handler)
    
    # Crea e avvia il server
    print("\nAvvio del server HTTP...")
    _server_instance = WebServer()
    
    if _server_instance.start_server():
        print(f"✅ Server avviato con successo!")
        print(f"   Indirizzo locale: http://{_server_instance.host}:{_server_instance.port}")
        
        # Mostra anche l'IP locale se diverso da localhost in caso non sia disponibile l'ip in `build.json`
        if _server_instance.host == "localhost":
            local_ip = _server_instance.get_local_ip()
            if local_ip != "127.0.0.1":
                print(f"   Indirizzo di rete: http://{local_ip}:{_server_instance.port}")
        
        print()
        print("🚀 Server pronto per le connessioni!")
        print("   Premi Ctrl+C per fermare il server")
        print("=" * 50)
        
        try:
            # Mantiene il server in esecuzione fino al shutdown
            while not _shutdown_event.is_set():
                time.sleep(1)
        except KeyboardInterrupt:
            # Gestito dal signal_handler
            pass
        except Exception as e:
            print(f"❌ Errore durante l'esecuzione del server: {e}")
    else:
        print("❌ Impossibile avviare il server. Verifica la configurazione.")
        input("\nPremi INVIO per uscire...")

# Protezione contro l'esecuzione multipla in PyInstaller
if __name__ == "__main__":
    main()