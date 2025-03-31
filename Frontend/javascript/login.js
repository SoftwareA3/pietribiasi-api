document.addEventListener("DOMContentLoaded", function() {
    const loginForm = document.getElementById("login-form");

    // Verifica se il form di login esiste sul type="submit"
    loginForm.addEventListener("submit", function(event) {
        event.preventDefault(); 

        // Recupera i valori dal form
        const username = document.querySelector("input[name='username']").value;
        const password = document.querySelector("input[name='password']").value;
        
        // Codifica le credenziali in Base64
        const credentials = btoa(username + ":" + password);
        
        // Salva le credenziali in sessionStorage (si può usare anche localStorage)
        sessionStorage.setItem("basicAuthCredentials", credentials);
        
        // Aggiorna il contenuto della pagina per informare l'utente
        document.getElementById("result").textContent = "Credenziali salvate. Ora sei autenticato.";
    });
});

/**
 * Funzione di esempio per effettuare una chiamata fetch con Basic Authentication.
 * Questa funzione può essere richiamata in altre parti dell'applicazione.
 */
async function fetchWithAuth(url, options = {}) {
    // Recupera le credenziali salvate
    const credentials = sessionStorage.getItem("basicAuthCredentials");
    
    if (!credentials) {
        throw new Error("Credenziali non presenti. Effettua il login prima di chiamare l'API.");
    }
    
    // Imposta l'header Authorization
    const headers = {
        "Authorization": "Basic " + credentials,
        ...options.headers
    };
    
    const response = await fetch(url, {
        ...options,
        headers: headers
    });
    
    return response;
}