import { fetchWithAuth } from "./fetch.js";
import { setCookie, deleteCookie, getCookie } from "./cookies.js";
import { getApiUrl } from "./main.js";

document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("login-form");
    const errorMessage = document.getElementById('error-message');
    const successMessage = document.getElementById('success-message');
    
    // Flag per evitare il doppio invio del form
    let isSubmitting = false;

    // Se siamo nella pagina di login, controlliamo lo stato di autenticazione
    if (window.location.href.includes("login.html")) {
        // Ripulisci qualsiasi messaggio precedente
        hideAllMessages();
        
        // Verifica se ci sono già credenziali valide
        const credentials = getCookie("basicAuthCredentials");
        const userInfo = getCookie("userInfo");
        const loginStatus = sessionStorage.getItem("login");

        const powerUserInfo = getCookie("pu-User");
        if(powerUserInfo) {
            deleteCookie("pu-User");
        }
        
        if (credentials && userInfo && loginStatus === "true") {
            // L'utente è già autenticato, mostra successo e reindirizza
            showMessage(successMessage);
            redirectToHome();
        } else if (loginStatus === "false") {
            // Login fallito in precedenza, mostra messaggio di errore
            showMessage(errorMessage);
            // Pulizia di credenziali parziali che potrebbero causare problemi
            deleteCookie("basicAuthCredentials");
            deleteCookie("userInfo");
            // Reset dello stato dopo la visualizzazione
            setTimeout(() => {
                sessionStorage.removeItem("login");
            }, 1500);
        }
    }

    // Setup del form di login
    if (loginForm) {
        loginForm.addEventListener("submit", async function (event) {
            event.preventDefault();
            event.stopPropagation();
            
            // Previene invii multipli
            if (isSubmitting) return;
            isSubmitting = true;
            
            // Nascondi eventuali messaggi precedenti
            hideAllMessages();
            
            try {
                // Get form information
                const passwordInput = document.getElementById("login-password");
                if (!passwordInput) {
                    throw new Error("Campo password non trovato");
                }
                
                const password = passwordInput.value.trim();
                if (!password) {
                    throw new Error("Password non inserita");
                }

                console.log("Invio richiesta di login...");
                
                // Prima richiesta: ottieni workerId
                const request = await fetch(getApiUrl("api/worker/login"), {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({"password": password}),
                    // Aggiungi un timeout per evitare richieste che non si completano mai
                    signal: AbortSignal.timeout(10000)
                }).catch(error => {
                    console.error("Errore di rete:", error);
                    throw new Error("Errore di connessione al server");
                });

                if (!request || !request.ok) {
                    const status = request ? request.status : "N/A";
                    const text = request ? request.statusText : "Connessione fallita";
                    throw new Error(`Errore nella richiesta: ${status} ${text}`);
                }
                
                // Estrai le informazioni del lavoratore
                const result = await request.json().catch(error => {
                    console.error("Errore nel parsing della risposta:", error);
                    throw new Error("Errore nel formato della risposta");
                });
                
                const workerId = result.workerId;
                
                if (!workerId) {
                    throw new Error("ID lavoratore non trovato nella risposta");
                }
                
                //console.log("WorkerId ottenuto:", workerId);
                
                // Pulizia preventiva dei cookie
                deleteCookie("basicAuthCredentials");
                deleteCookie("userInfo");
                
                // Salva le credenziali nel cookie
                const credentials = btoa(`${workerId}:${password}`);
                setCookie("basicAuthCredentials", credentials, 1);
                
                // Verifica che il cookie sia stato correttamente impostato
                const savedCredentials = getCookie("basicAuthCredentials");
                if (!savedCredentials || savedCredentials !== credentials) {
                    throw new Error("Errore nel salvataggio delle credenziali");
                }
                
                // Salva le informazioni utente nel cookie
                setCookie("userInfo", JSON.stringify(result), 1);
                
                console.log("Validazione credenziali...");
                
                // Seconda richiesta: valida le credenziali
                const validationResponse = await fetchWithAuth(getApiUrl("api/auth/validate"), {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    signal: AbortSignal.timeout(10000)
                }).catch(error => {
                    console.error("Errore durante la validazione:", error);
                    throw new Error("Errore nella validazione delle credenziali");
                });

                if (!validationResponse || !validationResponse.ok) {
                    const status = validationResponse ? validationResponse.status : "N/A";
                    const text = validationResponse ? validationResponse.statusText : "Connessione fallita";
                    throw new Error(`Validazione fallita: ${status} ${text}`);
                }
                
                console.log("Validazione completata con successo");
                
                // Autenticazione riuscita
                sessionStorage.setItem("login", "true");
                showMessage(successMessage);
                redirectToHome();
                
            } catch (error) {
                console.error("Errore di login:", error);
                deleteCookie("basicAuthCredentials");
                deleteCookie("userInfo");
                sessionStorage.setItem("login", "false");
                showMessage(errorMessage);
            } finally {
                // Reset del flag di submitting
                setTimeout(() => {
                    isSubmitting = false;
                }, 2000);
            }
        });
    }
    
    // Funzioni helper
    function hideAllMessages() {
        if (errorMessage && errorMessage.classList) errorMessage.classList.add('hidden');
        if (successMessage && successMessage.classList) successMessage.classList.add('hidden');
    }
    
    function showMessage(messageElement) {
        if (messageElement && messageElement.classList) {
            messageElement.classList.remove('hidden');
        }
    }
    
    function redirectToHome() {
        console.log("Reindirizzamento a home.html...");
        setTimeout(() => {
            window.location.href = "../html/home.html";
        }, 1500);
    }
});