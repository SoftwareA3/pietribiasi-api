import { getCookie } from "./cookies.js";

document.addEventListener("DOMContentLoaded", async function() {
    const puButton = document.getElementById("goto-poweruser-login")
    const settingsButton = document.getElementById("goto-settings");

    const userCookie = JSON.parse(getCookie("userInfo"));

    if(userCookie && userCookie.tipoUtente === "Amministrazione") {
        puButton.style.display = "flex";
        settingsButton.style.display = "flex";
    }
});