import { fetchWithAuth } from "./fetch.js";
import { setCookie, deleteCookie, getCookie } from "./cookies.js";

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");
    const errorMessage = document.getElementById('error-message');
    const successMessage = document.getElementById('success-message');

    // Check if user is authenticated using cookies instead of sessionStorage
    if(sessionStorage.getItem("login") === "true") {
        successMessage.classList.remove('hidden');
        setTimeout(() => {
            window.location.href = "../html/home.html";
        }, 3000);
    }
    else if (sessionStorage.getItem("login") === "false") {
        errorMessage.classList.remove('hidden');
    }

    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();
            event.stopPropagation();

            // Get form information
            const password = document.getElementById("login-password").value;

            try 
            {
                console.log("Password prima di fetch:", password);
                const request = await fetch("http://localhost:5245/api/worker/login", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({"password": password})
                });

                if (!request.ok) {
                    sessionStorage.setItem("login", "false"); // Set cookie for 1 day
                    console.error("Errore nella richiesta:", request.status, request.statusText);
                    return;
                }
                
                const result = await request.json();
                const workerId = result.workerId;
                
                // Save credentials in a cookie
                if(getCookie("basicAuthCredentials")) {
                    deleteCookie("basicAuthCredentials");
                }
                if(getCookie("userInfo")) {
                    deleteCookie("userInfo");
                }

                const credentials = btoa(`${workerId}:${password}`);
                setCookie("basicAuthCredentials", credentials, 1);
                console.log("basicAuthCredentials cookie: " + getCookie("basicAuthCredentials"));
                
                // Save the entire result in another cookie
                setCookie("userInfo", JSON.stringify(result), 1); // Set cookie for 1 day
                console.log("userInfo cookie: " + getCookie("userInfo"));
                
                console.log("Results:", result);
                console.log("Risultato cript:", credentials);
            }
            catch (error) {
                sessionStorage.setItem("login", "false");
                console.error("Non è stato possibile recuperare l'ID:", error);
                return;
            }
            
            try {
                // Use fetchWithAuth to send the request
                const response = await fetchWithAuth("http://localhost:5245/api/auth/validate", {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json"
                    }
                });

                if (response.ok) {
                    sessionStorage.setItem("login", "true"); // Set cookie for 1 day
                } else {
                    deleteCookie("basicAuthCredentials");
                    sessionStorage.setItem("login", "false");
                    console.error("Errore nella richiesta:", response.status, response.statusText);
                }
            } catch (error) {
                sessionStorage.setItem("login", "false");
                deleteCookie("basicAuthCredentials");
                console.error("Errore nella richiesta:", error);
            }
        });
    }
});