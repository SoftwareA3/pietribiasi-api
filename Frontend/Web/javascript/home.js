import { getCookie } from "./cookies.js";
import { fetchWithAuth } from "./fetch.js";
import { getIPString } from "./main.js";

document.addEventListener("DOMContentLoaded", async function() {
    const puButton = document.getElementById("goto-poweruser-login")
    const settingsButton = document.getElementById("goto-settings");
    const syncButton = document.getElementById("sync-data");

    const userCookie = JSON.parse(getCookie("userInfo"));

    if(userCookie && userCookie.tipoUtente === "Amministrazione") {
        puButton.style.display = "flex";
        settingsButton.style.display = "flex";
    }

    try {
        const response = await fetchWithAuth(`http://${getIPString()}:5245/api/mago_api/get_sync_global_active`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            }
        });
        if (response.ok) {
            const data = await response.json();
            console.log("Informazioni sulla sincronizzazione globale:", data);
            if(data.syncGlobalActive) {
                syncButton.style.display = "flex";
            }
        } else {
            console.error("Errore durante il recupero delle informazioni sulla sincronizzazione globale:", response.statusText);
        }
    }
    catch (error) {
        console.error("Non è stato possibile recuperare le informazioni sulla sincronizzazione globale:", error);
    }
});