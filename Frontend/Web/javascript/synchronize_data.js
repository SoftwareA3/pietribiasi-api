import { fetchWithAuth } from "./fetch.js";
import {getCookie} from "./cookies.js";

document.addEventListener("DOMContentLoaded", async function() {
    const syncButton = document.getElementById("sync-data");
    const iconElement = syncButton.querySelector(".menu-icon");
    iconElement.classList.remove("sync-success", "sync-error");
    iconElement.classList.remove("sync-loading");

    syncButton.addEventListener("click", async function() {
        // Disabilita il pulsante durante la sincronizzazione
        syncButton.disabled = true;
        await synchronizeData();
        // Riabilita il pulsante dopo il completamento
        setTimeout(() => {
            syncButton.disabled = false;
        }, 2500);
    });
});

async function synchronizeData() {
    const syncButton = document.getElementById("sync-data");
    const iconElement = syncButton.querySelector(".menu-icon");
    const originalIcon = iconElement.className;
    const userCookie = JSON.parse(getCookie("userInfo"));
    console.log("User cookie:", userCookie);
    
    // Inizia l'animazione di caricamento
    startLoadingAnimation(iconElement);
    
    // Garantisce un minimo di tempo di caricamento (1.5 secondi)
    const minLoadingTime = new Promise(resolve => setTimeout(resolve, 2000));
    
    try {
        console.log("Starting data synchronization...");
        console.log("User ID:", userCookie.workerId);
        // Esegui simultaneamente la richiesta e il timer di caricamento minimo
        const [response] = await Promise.all([
            fetchWithAuth("http://localhost:5245/api/mago_api/synchronize", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    userId: userCookie.workerId,
                })
            }),
            minLoadingTime
        ]);

        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));

        if (response.ok) {
            const data = await response.json();
            console.log("Data synchronized successfully:", data);
            // Mostra l'icona di successo
            showSuccessIcon(iconElement);
        } else {
            console.error("Error synchronizing data:", response.statusText);
            // Mostra l'icona di errore
            showErrorIcon(iconElement);
        }
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
        }, 2000);
    } catch (error) {
        console.error("Network error:", error);
        
        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));
        
        // Mostra l'icona di errore
        showErrorIcon(iconElement);
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
        }, 2000);
    }
}

/**
 * Avvia l'animazione di rotazione dell'icona durante il caricamento
 * @param {HTMLElement} iconElement - L'elemento icona da animare
 */
function startLoadingAnimation(iconElement) {
    // Rimuovi tutte le classi esistenti e aggiungi fa-arrows-rotate per sicurezza
    iconElement.className = "fa-solid fa-arrows-rotate menu-icon";
    
    // Aggiungi la classe per l'animazione di rotazione
    iconElement.classList.add("sync-loading");
}

/**
 * Mostra l'icona di successo (check)
 * @param {HTMLElement} iconElement - L'elemento icona da modificare
 */
function showSuccessIcon(iconElement) {
    // Imposta l'icona di spunta e applica l'animazione
    iconElement.className = "fa-solid fa-check menu-icon";
    iconElement.classList.add("sync-success");
}

/**
 * Mostra l'icona di errore (X)
 * @param {HTMLElement} iconElement - L'elemento icona da modificare
 */
function showErrorIcon(iconElement) {
    // Imposta l'icona X e applica l'animazione
    iconElement.className = "fa-solid fa-times menu-icon";
    iconElement.classList.add("sync-error");
}

/**
 * Ripristina l'icona originale
 * @param {HTMLElement} iconElement - L'elemento icona da ripristinare
 * @param {string} originalIcon - La classe originale dell'icona
 */
function resetIcon(iconElement, originalIcon) {
    iconElement.className = originalIcon;
    iconElement.style.color = ""; // Ripristina il colore predefinito
}