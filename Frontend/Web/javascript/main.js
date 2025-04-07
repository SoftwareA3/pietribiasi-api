import { deleteCookie, getCookie } from "./cookies.js";

document.addEventListener("DOMContentLoaded", async function() {
    // Check if user is not authenticated
    if (sessionStorage.getItem("login") !== "true") {
        // Only redirect if we're not already on the login page
        if (!window.location.href.includes("login.html")) {
            window.location.href = "../html/login.html";
            return;
        }
    }
    
    // Continue with normal page loading if authenticated
    await fetch("../html/header.html")
        .then(response => response.text())
        .then(data => {
            const headerElement = document.getElementsByClassName("header")[0];
            if (headerElement) {
                headerElement.innerHTML = data;
            }
        })
        .catch(error => console.error("Error loading header:", error));
    
    setupLogoutButton();
    await loadWorkerInfo();
});

function setupLogoutButton() {
    const logoutButton = document.getElementById("logout");
    if (logoutButton) {
        logoutButton.addEventListener("click", function() {
            deleteCookie("basicAuthCredentials");
            deleteCookie("userInfo");
            sessionStorage.removeItem("login");
            window.location.href = "../html/login.html";
        });
    }
}

async function loadWorkerInfo() {
    const workerInformations = document.getElementById("current-user");
    if (!workerInformations) {
        console.error("Elemento worker-informations non trovato nel DOM");
        return;
    }
    
    // Verifica se ci sono credenziali salvate
    const encodedCredentials = getCookie("basicAuthCredentials");
    console.log("Credenziali salvate:", encodedCredentials);
    if (!encodedCredentials) {
        displayError(workerInformations, "Credenziali non trovate");
        return;
    }
    
    try {
        // Decodifica delle credenziali
        const stringCredentials = atob(encodedCredentials);
        const [workerId, password] = stringCredentials.split(":");
        
        if (!workerId || !password) {
            throw new Error("Formato credenziali non valido");
        }
        
        // Recupero informazioni worker dall'API
        const workerInfo = getCookie("userInfo");
        
        // Visualizzazione delle informazioni
        displayWorkerInfo(workerInformations, workerInfo);
    } catch (error) {
        console.error("Errore nel caricamento delle informazioni:", error);
        displayError(workerInformations, "Errore nel caricamento delle informazioni");
    }
}

function displayWorkerInfo(container, cookie) {
    try {
        cookie = JSON.parse(cookie);
        if (!cookie) {
            displayError(container, "Cookie non valido o vuoto");
            return;
        }
        container.innerHTML = `
            ${cookie.tipoUtente || 'N/A'}: ${cookie.password || 'N/A'} - ${cookie.name || 'N/A'} ${cookie.lastName || 'N/A'} ${displayCurrentDate()}`;
    } catch (error) {
        console.error("Errore nel parsing del cookie:", error);
        displayError(container, "Errore nel formato del cookie");
    }
}

function displayError(container, message) {
    container.innerHTML = `<p class="error">${message}</p>`;
}

function displayCurrentDate() {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();

    today = mm + '/' + dd + '/' + yyyy;
    return today;
}