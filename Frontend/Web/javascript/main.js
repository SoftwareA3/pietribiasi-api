import { deleteCookie, getCookie } from "./cookies.js";

document.addEventListener("DOMContentLoaded", async function() {
    // Controlla se l'utente è autenticato
    if (sessionStorage.getItem("login") !== "true") {
        // Reindirizza se l'utente non è autenticato e non è già nella pagina di login
        if (!window.location.href.includes("login.html")) {
            window.location.href = "../html/login.html";
            return;
        }
    }
    
    // Carica l'header
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

// Setup del pulsante di logout con l'eliminazione dei cookie
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

// Funzione che carica le inofrmazioni del lavoratore che si è autenticato e popola l'header
async function loadWorkerInfo() {
    const workerInformations = document.getElementById("current-user");
    if (!workerInformations) {
        console.error("Elemento worker-informations non trovato nel DOM");
        return;
    }
    
    // Verifica se ci sono credenziali salvate
    const encodedCredentials = getCookie("basicAuthCredentials");
    //console.log("Credenziali salvate:", encodedCredentials);
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

// Popola l'header con le informazioni del lavoratore
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

// Funzione per formattare la data corrente
function displayCurrentDate() {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();

    today = mm + '/' + dd + '/' + yyyy;
    return today;
}