// FIXME: Controllare se è possibile utilizzare sessionStorage per il salvataggio delle informazioni 
// invece che sessionStorage

import { fetchWithAuth } from "./fetch.js";

document.addEventListener("DOMContentLoaded", function () {
    const requestForm = document.getElementById("request-form");

    if(sessionStorage.getItem("apiResults") != null) {
        const apiResults = JSON.parse(sessionStorage.getItem("apiResults"));
        displayData(apiResults);
    }

    // Controlla se l'utente è autenticato
    const credentials = localStorage.getItem("basicAuthCredentials");
    if (!credentials) {
        // Reindirizza alla pagina di login se non autenticato
        alert("Sessione scaduta. Effettua nuovamente il login.");
        window.location.href = "login.html";
        return;
    }

    if (requestForm) {
        requestForm.addEventListener("submit", async function (event) {
            // Questo previene il refresh della pagina
            event.preventDefault(); 
            event.stopPropagation();

            // Mostra un indicatore di caricamento
            const resultParagraph = document.getElementById("request-result");
            resultParagraph.textContent = "Elaborazione richiesta in corso...";

            // Retrieve the password from the form
            const password = document.querySelector("input[name='password']").value;

            const data = {
                "password": password
            };

            try {
                // Invia una richiesta all'API con le credenziali
                const response = await fetchWithAuth("http://localhost:5245/api/worker/lastlogin", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(data)
                });

                if (response.ok) {
                    // Risposta positiva: elabora i dati
                    const result = await response.json();
                    resultParagraph.textContent = "Richiesta inviata con successo: " + JSON.stringify(displayData(result));
                    return true;
                } else {
                    resultParagraph.textContent = "Errore nella richiesta: " + response.status + " - " + response.statusText;
                }
            } catch (error) {
                resultParagraph.textContent = "Errore: " + error.message;
                console.error("Errore nella richiesta:", error);
            }
            return false;
        });
        return false;
    }
});

function displayData(data) {
    // Salva i dati in sessionStorage
    sessionStorage.setItem("apiResults", JSON.stringify(data));
    

    const container = document.getElementById("resultContainer");
    // Ad esempio, crea un nuovo elemento per ogni informazione nel JSON
    container.innerHTML = ""; // Pulisce il contenuto precedente, se necessario
    for (const key in data) {
        if (data.hasOwnProperty(key)) {
        const p = document.createElement("p");
        p.textContent = `${key}: ${data[key]}`;
        container.appendChild(p);
        }
    }
}