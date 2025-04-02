import { fetchWithAuth } from "./fetch.js";

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");

    // Controlla se l'utente è autenticato e la pagina è già stata caricata
    // In quel caso, mostra il messaggio di successo e reindirizza

    // Debug
    if(localStorage.getItem("basicAuthCredentials") != null) {
        localStorage.removeItem("basicAuthCredentials");
    }
    // Debug

    if(sessionStorage.getItem("apiResults") === "true") {
        let resultDiv = document.getElementById("login-result");
        resultDiv.textContent = "Login effettuato con successo: \nReindirizzamento fra 3 secondi...";
        setTimeout(() => {
            window.location.href = "../html/home.html";
        }, 3000);
    }

    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();
            event.stopPropagation();

            let lock = false;

            let resultDiv = document.getElementById("login-result");
            resultDiv.textContent = "Autenticazione in corso...";

            // Recupera le informazioni dal form
            const password = document.querySelector("#login-password").value;

            // COntrolla che tutte le informazioni siano state inserite
            if (!password) {
                resultDiv.textContent = "Password obbligatoria.";
                return false;
            }

            try 
            {
                const request = await fetch("http://localhost:5245/api/worker/login", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({password})
                });

                if (!request.ok) {
                    resultDiv.textContent = "Errore nella richiesta: " + request.status + " - " + request.statusText;
                    return false;
                }
                const result = await request.json();
                const workerId = result.workerId;
                const credentials = btoa(`${workerId}:${password}`);
                console.log("Risultato cript:", credentials);
                localStorage.setItem("basicAuthCredentials", credentials);
                sessionStorage.setItem("apiResults", true);
            }
            catch (error) {
                resultDiv.textContent = "Errore: " + error.message;
                console.error("Non è stato possibile recuperare l'ID:", error);
            }
            
            try {
                // Chiama la funzione fetchWithAuth per inviare la richiesta
                const response = await fetchWithAuth("http://localhost:5245/api/auth/validate", {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json"
                    }
                });

                if (response.ok) {
                    resultDiv.textContent = "Login effettuato con successo: \nReindirizzamento fra 3 secondi...";
                    
                    return true;
                } else {
                    resultDiv.textContent = "Errore nella richiesta: " + response.status + " - " + response.statusText;
                    console.error("Errore nella richiesta:", response.status, response.statusText);
                    localStorage.removeItem("basicAuthCredentials");
                    return false;
                }
            } catch (error) {
                resultDiv.textContent = "Errore: " + error.message;
                console.error("Errore nella richiesta:", error);
                localStorage.removeItem("basicAuthCredentials");
                return false;
            }
        });
    }
});

