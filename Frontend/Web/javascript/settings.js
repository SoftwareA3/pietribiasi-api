import { fetchWithAuth } from "./fetch.js";
import { getCookie } from "./cookies.js";

document.addEventListener("DOMContentLoaded", async function() {
    const urlInput = document.getElementById("settings-url");
    const usernameInput = document.getElementById("settings-username");
    const passwordInput = document.getElementById("settings-password");
    const companyInput = document.getElementById("settings-company");
    const specificatorInput = document.getElementById("settings-specificator");
    const closedComboBox = document.getElementById("settings-closed");
    const saveButton = document.getElementById("save-settings");

    var settings = {
        magoUrl: "",
        username: "",
        password: "",
        company: "",
        specificatorType: "",
        closed: false
    }

    try {
        const response = await fetchWithAuth("http://localhost:5245/api/mago_api/get_settings", {
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
    closedComboBox.value = settings.closed ? "True" : "False"

    saveButton.addEventListener("click", async function() {
        settings.magoUrl = urlInput.value;
        settings.username = usernameInput.value;
        settings.password = passwordInput.value;
        settings.company = companyInput.value;
        settings.specificatorType = specificatorInput.value;
        settings.closed = closedComboBox.value === "True" ? true : false;
        console.log("Settings to save:", settings);

        try {
            const response = await fetchWithAuth("http://localhost:5245/api/mago_api/save_settings", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(settings)
            });

            if (response.ok) {
                console.log("Settings saved successfully:", response.statusText);
                console.log("Ricaricamento della pagina...");
                // Ricarica la pagina dopo il salvataggio
                location.reload();
            } else {
                console.error("Error saving settings:", response.statusText);
            }
        }
        catch (error) {
            console.error("Error saving settings:", error);
        }
    });
});