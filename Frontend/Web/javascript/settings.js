import { fetchWithAuth } from "./fetch.js";
import { getCookie } from "./cookies.js";
import { getApiUrl } from "./main.js";

document.addEventListener("DOMContentLoaded", async function() {
    const urlInput = document.getElementById("settings-url");
    const usernameInput = document.getElementById("settings-username");
    const passwordInput = document.getElementById("settings-password");
    const companyInput = document.getElementById("settings-company");
    const specificatorInput = document.getElementById("settings-specificator");
    const closedComboBox = document.getElementById("settings-closed");
    const rectificationPosInput = document.getElementById("settings-causale-pos");
    const rectificationNegInput = document.getElementById("settings-causale-neg");
    const storageInput = document.getElementById("settings-deposito");
    const saveButton = document.getElementById("save-settings");
    const syncToggle = document.getElementById("settings-sync-for-all");
    const editToggle = document.getElementById("edit-toggle");

    var settings = {
        magoUrl: "",
        username: "",
        password: "",
        company: "",
        specificatorType: "",
        terminaLavorazioniUtente: false,
        syncGlobalActive: false,
    }

    try {
        const response = await fetchWithAuth(getApiUrl("api/settings/get_settings"), {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (response.ok) {
            settings = await response.json();
            console.log("Settings retrieved successfully:", settings);
        } else {
            console.error("Error retrieving settings:", response.statusText);
        }
    }
    catch (error) {
        console.error("Error fetching settings:", error);
        return;
    }

    // Popola i campi con i dati recuperati
    urlInput.value = settings.magoUrl;
    usernameInput.value = settings.username;
    passwordInput.value = settings.password;
    companyInput.value = settings.company;
    specificatorInput.value = settings.specificatorType;
    rectificationPosInput.value = settings.rectificationReasonPositive;
    rectificationNegInput.value = settings.rectificationReasonNegative;
    storageInput.value = settings.storage;
    closedComboBox.value = settings.terminaLavorazioniUtente ? "True" : "False"
    syncToggle.value = settings.syncGlobalActive ? "True" : "False";
    editToggle.checked = false;

    saveButton.addEventListener("click", async function() {
        settings.magoUrl = urlInput.value;
        settings.username = usernameInput.value;
        settings.password = passwordInput.value;
        settings.company = companyInput.value;
        settings.specificatorType = specificatorInput.value;
        settings.rectificationReasonPositive = rectificationPosInput.value;
        settings.rectificationReasonNegative = rectificationNegInput.value;
        settings.storage = storageInput.value;
        settings.terminaLavorazioniUtente = closedComboBox.value === "True" ? true : false;
        settings.syncGlobalActive = syncToggle.value === "True" ? true : false;
        console.log("Settings to save:", settings);

        try {
            const response = await fetchWithAuth(getApiUrl("api/settings/edit_settings"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(settings)
            });

            if (response.ok) {
                alert("Impostazioni modificate con successo");
                console.log("Settings saved successfully:", response.statusText);
                console.log("Ricaricamento della pagina...");
                location.reload();
            } else {
                console.error("Error saving settings:", response.statusText);
            }
        }
        catch (error) {
            console.error("Error saving settings:", error);
        }
    });

    editToggle.addEventListener("click", function() {
        if (editToggle.checked) {
            urlInput.disabled = false;
            usernameInput.disabled = false;
            passwordInput.disabled = false;
            companyInput.disabled = false;
            specificatorInput.disabled = false;
            closedComboBox.disabled = false;
            rectificationPosInput.disabled = false;
            rectificationNegInput.disabled = false;
            storageInput.disabled = false;
            syncToggle.disabled = false;
            saveButton.disabled = false;
        } else {
            urlInput.disabled = true;
            usernameInput.disabled = true;
            passwordInput.disabled = true;
            companyInput.disabled = true;
            specificatorInput.disabled = true;
            closedComboBox.disabled = true;
            rectificationPosInput.disabled = true;
            rectificationNegInput.disabled = true;
            storageInput.disabled = true;
            syncToggle.disabled = true;
            saveButton.disabled = true;
        }
    });
});