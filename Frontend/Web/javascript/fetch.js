/**
 * Funzione per confermare l'autenticazione delle credenziali con Basic Authentication.
 * Questa funzione gestisce la richiesta di autenticazione e il reindirizzamento in caso di errore.
 */
export async function fetchWithAuth(url, options = {}) {
    
    // Recupera le credenziali da sessionStorage
    const credentials = localStorage.getItem("basicAuthCredentials");

    // Se le credenziali non esistono, fa un refresh della pagina
    if (!credentials) {
        window.location.href = "../html/login.html";
        throw new Error("Credenziali non presenti. Effettua il login prima di chiamare l'API.");
    }

    // Aggiungi le credenziali all'intestazione Authorization
    const headers = {
        "Authorization": "Basic " + credentials,
        ...options.headers
    };

    console.log("Authorization Header:", headers["Authorization"]);

    // Esegui la richiesta fetch con le credenziali
    const response = await fetch(url, {
        ...options,
        headers: headers
    });

    // Controlla lo stato della risposta: se non positivo, restituisce caso per caso una risposta
    if (!response.ok) {
        if (response.status === 401) {
            localStorage.removeItem("basicAuthCredentials");
            window.location.href = "../html/login.html";
            throw new Error("Sessione scaduta. Effettua nuovamente il login.");
        }
        throw new Error("Errore nella richiesta: " + response.status + " - " + response.statusText);
    }

    return response;
}