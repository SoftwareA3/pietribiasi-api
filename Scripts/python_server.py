# python_server.py
import http.server
import socketserver
import json
from pathlib import Path
import os
import webbrowser
import socket

DIRECTORY = "frontend"

def load_config(config_path="build.json"):
    """Carica la configurazione dal file build.json"""
    # Valori di default
    default_host = "localhost"
    default_port = 8080
    
    try:
        if Path(config_path).exists():
            with open(config_path, 'r', encoding='utf-8') as f:
                config = json.load(f)
            
            # Estrae host e porta dalla configurazione
            frontend_config = config.get('server', {}).get('frontend', {})
            host = frontend_config.get("host", default_host)
            port = frontend_config.get("port", default_port)
            
            print(f"Starting up http-server, serving {DIRECTORY}")
            return host, port
        else:
            print(f"Config file {config_path} not found, using defaults")
            print(f"Starting up http-server, serving {DIRECTORY}")
            return default_host, default_port
            
    except Exception as e:
        print(f"Error loading config: {e}")
        print(f"Starting up http-server, serving {DIRECTORY}")
        return default_host, default_port

class HTTPServerHandler(http.server.SimpleHTTPRequestHandler):
    """Custom handler che replica il comportamento di http-server"""
    
    def __init__(self, *args, **kwargs):
        # Cambia directory prima di inizializzare il handler
        super().__init__(*args, directory=DIRECTORY, **kwargs)
    
    def end_headers(self):
        # CORS headers (--cors flag)
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Content-Type, Authorization")
        
        # Cache disabilitato (-c-1 flag)
        self.send_header("Cache-Control", "no-cache, no-store, must-revalidate")
        self.send_header("Pragma", "no-cache")
        self.send_header("Expires", "0")
        
        super().end_headers()
    
    def do_OPTIONS(self):
        """Gestisce le richieste OPTIONS per CORS"""
        self.send_response(200)
        self.end_headers()
    
    def log_message(self, format, *args):
        """Log delle richieste in stile http-server"""
        print(f"[{self.log_date_time_string()}] {format % args}")

def get_local_ip():
    """Ottiene l'IP locale della macchina"""
    try:
        # Crea un socket per determinare l'IP locale
        with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
            s.connect(("8.8.8.8", 80))
            return s.getsockname()[0]
    except:
        return "127.0.0.1"

def main():
    # Verifica che la directory frontend esista
    if not Path(DIRECTORY).exists():
        print(f"Error: Directory '{DIRECTORY}' not found!")
        return
    
    # Carica la configurazione
    HOST, PORT = load_config()
    
    # Ottiene l'IP locale per i messaggi
    local_ip = get_local_ip()
    
    try:
        with socketserver.TCPServer((HOST, PORT), HTTPServerHandler) as httpd:
            print(f"Available on:")
            print(f"  http://{HOST}:{PORT}")
            if HOST != "127.0.0.1" and HOST != "localhost":
                print(f"  http://127.0.0.1:{PORT}")
            if HOST != local_ip:
                print(f"  http://{local_ip}:{PORT}")
            
            print("Hit CTRL-C to stop the server")
            
            # Auto-apertura browser con index.html (-o index.html flag)
            try:
                webbrowser.open(f"http://{HOST}:{PORT}/index.html")
            except Exception as e:
                print(f"Could not open browser: {e}")
            
            httpd.serve_forever()
            
    except OSError as e:
        if "Address already in use" in str(e):
            print(f"Error: Port {PORT} is already in use")
            print("Try changing the port in build.json or stop the application using it")
        else:
            print(f"Error starting server: {e}")
    except KeyboardInterrupt:
        print(f"\nhttp-server stopped.")

if __name__ == "__main__":
    main()