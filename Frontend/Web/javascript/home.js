import { getCookie } from "./cookies.js";

document.addEventListener("DOMContentLoaded", async function() {
    const puButton = document.getElementById("goto-poweruser-login")

    const userCookie = JSON.parse(getCookie("userInfo"));

    if(userCookie && userCookie.tipoUtente === "Amministrazione") {
        puButton.style.display = "block";
    }
});