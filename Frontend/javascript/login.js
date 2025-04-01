import { fetchWithAuth } from "./fetch.js";

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");

    // Controlla se l'utente è autenticato e la pagina è già stata caricata
    // In quel caso, mostra il messaggio di successo e reindirizza
    if(localStorage.getItem("apiResults") != null) {
        const resultDiv = document.getElementById("login-result");
        resultDiv.textContent = "Login effettuato con successo: \nReindirizzamento fra 3 secondi...";
        setTimeout(() => {
            window.location.href = "../html/request.html";
        }, 3000);
    }

    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();
            event.stopPropagation();

            const resultDiv = document.getElementById("login-result");
            resultDiv.textContent = "Autenticazione in corso...";

            // Recupera le informazioni dal form
            const username = document.querySelector("#login-username").value;
            const password = document.querySelector("#login-password").value;

            // COntrolla che tutte le informazioni siano state inserite
            if (!username || !password) {
                resultDiv.textContent = "Username e password sono obbligatori.";
                return false;
            }

            const credentials = btoa(username + ":" + password);
            localStorage.setItem("basicAuthCredentials", credentials);

            try {
                // Chiama la funzione fetchWithAuth per inviare la richiesta
                const response = await fetchWithAuth("http://localhost:5245/api/auth/login", {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json"
                    }
                });

                if (response.ok) {
                    const result = await response.json();
                    return true;
                } else {
                    resultDiv.textContent = "Errore nella richiesta: " + response.status + " - " + response.statusText;
                    return false;
                }
            } catch (error) {
                resultDiv.textContent = "Errore: " + error.message;
                console.error("Errore nella richiesta:", error);
            }
            return false;
        });
    }
});

