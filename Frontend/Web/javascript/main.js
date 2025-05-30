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

    // Ottieni riferimento al pulsante
    const scrollToTopBtn = document.getElementById('scrollToTopBtn');

    scrollToTopBtn.addEventListener('click', function() {
        // Chiama la funzione per tornare in cima alla pagina
        scrollToTop();
    });
    
    const headerElement = document.getElementsByClassName("app-header")[0]; 

    setupLogoutButton();
    await loadWorkerInfo();
});

// Setup del pulsante di logout con l'eliminazione dei cookie
function setupLogoutButton() {
    const logoutButton = document.getElementById("logout");
    if (logoutButton) {
        logoutButton.addEventListener("click", function() {
            const pu = JSON.parse(getCookie("pu-User"));
            if(pu)
            {
                deleteCookie("pu-User");
            }
            deleteCookie("basicAuthCredentials");
            deleteCookie("userInfo");
            sessionStorage.removeItem("login");
        });
    }
}

export function getIPString() {
    return "192.168.100.113";
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
        
        const puUser = getCookie("pu-User");
        if (puUser) {
            displayWorkerAndPuInfo(workerInformations, workerInfo, puUser);
        }
        else
        {
            displayWorkerInfo(workerInformations, workerInfo);
        }

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

// Popola l'header con le informazioni del lavoratore 
// Se il cookie pu-User è presente, mostra anche le informazioni del lavoratore simulato
function displayWorkerAndPuInfo(container, worker, pu) {
    try {
        worker = JSON.parse(worker);
        pu = JSON.parse(pu);
        if (!worker || !pu) {
            displayError(container, "Cookie non valido o vuoto");
            return;
        }
        container.innerHTML = `
        Stai usanto l'utente: ${pu.workerId || 'N/A'} ${pu.name || 'N/A'} ${pu.lastName || 'N/A'}
        <br> 
        ${worker.tipoUtente || 'N/A'}: ${worker.password || 'N/A'} - ${worker.name || 'N/A'} ${worker.lastName || 'N/A'} ${displayCurrentDate()}`;
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

    today = dd + '/' + mm + '/' + yyyy;
    return today;
}

// Funzione per estrarre valori unici da un array di oggetti
export function extractUniqueValues(data, field) {
    const uniqueValues = [];
    const valueSet = new Set();
    
    if (data && data.length > 0) {
        data.forEach(item => {
            if (item[field] && !valueSet.has(item[field])) {
                valueSet.add(item[field]);
                uniqueValues.push({
                    [field]: item[field],
                    display: item[field].toString()
                });
            }
        });
    }
    
    return uniqueValues;
}

export function parseDateTime(dateString)
{
    let parsedDate = "";
    let parsedTime = "";
    if (dateString) {
        // Gestisce sia formato "YYYY-MM-DD HH:mm:ss.SSS" che ISO "YYYY-MM-DDTHH:mm:ss.SSSZ"
        let dateObj;
        if (dateString.includes("T")) {
        // ISO format
        dateObj = new Date(dateString);
        } else {
        // "YYYY-MM-DD HH:mm:ss.SSS"
        // Sostituisci spazio con "T" per compatibilità con Date
        dateObj = new Date(dateString.replace(" ", "T"));
        }
        if (!isNaN(dateObj)) {
        // Formatta la data come "dd/MM/yyyy"
        parsedDate = dateObj.toLocaleDateString("it-IT");
        // Formatta l'orario come "HH:mm:ss"
        parsedTime = dateObj.toLocaleTimeString("it-IT", { hour12: false });
        }
    }
    return {
        date: parsedDate,
        time: parsedTime
    };
}

// Funzione per mostrare/nascondere il pulsante in base allo scroll
window.onscroll = function() {
    // Mostra il pulsante quando l'utente ha scrollato più di 300px
    if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 300) {
        scrollToTopBtn.classList.add('show');
    } else {
        scrollToTopBtn.classList.remove('show');
    }
};

// Funzione per tornare in cima alla pagina
function scrollToTop() {
    // Scroll fluido verso l'alto
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Versione alternativa per browser più vecchi (opzionale)
function scrollToTopAlternative() {
    const scrollStep = -window.scrollY / (300 / 15);
    const scrollInterval = setInterval(function(){
        if (window.scrollY !== 0) {
            window.scrollBy(0, scrollStep);
        } else {
            clearInterval(scrollInterval);
        }
    }, 15);
}