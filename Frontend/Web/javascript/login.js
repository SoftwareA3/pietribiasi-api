import { fetchWithAuth } from "./fetch.js";

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");

    // Controlla se l'utente è autenticato e la pagina è già stata caricata
    // In quel caso, mostra il messaggio di successo e reindirizza

    if(sessionStorage.getItem("login") === "true") {
        const successMessage = document.getElementById('success-message');
        successMessage.classList.remove('hidden');
        sessionStorage.removeItem("login");
        setTimeout(() => {
            window.location.href = "../html/home.html";
        }, 3000);
    }
    else if (sessionStorage.getItem("login") === "false") {
        const errorMessage = document.getElementById('error-message');
        errorMessage.classList.remove('hidden');
        sessionStorage.removeItem("login");
    }

    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();
            event.stopPropagation();

            // Recupera le informazioni dal form
            const password = document.querySelector("#login-password").value;

            // COntrolla che tutte le informazioni siano state inserite
            if (!password) {
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
                    sessionStorage.setItem("login", false);
                    console.error("Errore nella richiesta:", request.status, request.statusText);
                }
                const result = await request.json();
                const workerId = result.workerId;
                const credentials = btoa(`${workerId}:${password}`);
                localStorage.setItem("basicAuthCredentials", credentials);
                console.log("Risultato cript:", credentials);
            }
            catch (error) {
                sessionStorage.setItem("login", false);
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
                    sessionStorage.setItem("login", true);
                    return true;
                } else {
                    localStorage.removeItem("basicAuthCredentials");
                    console.error("Errore nella richiesta:", response.status, response.statusText);
                    return false;
                }
            } catch (error) {
                sessionStorage.setItem("login", false);
                localStorage.removeItem("basicAuthCredentials");
                console.error("Errore nella richiesta:", error);
                return false;
            }
        });
    }
});

