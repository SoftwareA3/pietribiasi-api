// FIXME: Controllare se è possibile utilizzare sessionStorage per il salvataggio delle informazioni 
// invece che sessionStorage

import { fetchWithAuth } from "./fetch.js";

document.addEventListener("DOMContentLoaded", function () {
    const logoutButton = document.getElementById("logout");

    if (logoutButton) {
        logoutButton.addEventListener("click", function () {
            localStorage.removeItem("basicAuthCredentials");
            sessionStorage.clear();

            window.location.href = "../html/login.html";
        });
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