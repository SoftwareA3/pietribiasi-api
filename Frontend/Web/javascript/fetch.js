/**
 * Funzione per confermare l'autenticazione delle credenziali con Basic Authentication.
 * Questa funzione gestisce la richiesta di autenticazione e il reindirizzamento in caso di errore.
 */
import { getCookie } from "./cookies.js";

export async function fetchWithAuth(url, options = {}) {
    const credentials = getCookie("basicAuthCredentials");

    if (!credentials) {
        throw new Error("Credenziali non presenti. Effettua il login prima di chiamare l'API.");
    }

    // Create the headers object
    const headers = {
        "Authorization": "Basic " + credentials,
        ...options.headers
    };

    console.log("Fetching URL:", url);
    console.log("Request Headers:", headers);

    try {
        const response = await fetch(url, {
            ...options,
            headers: headers, // Pass the headers object directly
            mode: "cors"
        });

        if (!response.ok) {
            console.error("Errore nella risposta:", response.status, response.statusText);
        }

        return response;
    } catch (error) {
        console.error("Errore durante la richiesta fetch:", error.message);
        throw error;
    }
}