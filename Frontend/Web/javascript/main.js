import { deleteCookie, getCookie } from "./cookies.js";

// API configuration placeholder to be replaced by the build script
const API_CONFIG = {
    baseUrl: '##API_BASE_URL##',
    
    // Helper to get the full API URL
    getApiUrl: function(endpoint = '') {
        return `${this.baseUrl}${endpoint}`;
    }
};

// Async function to initialize API configuration (can be adapted for other needs)
async function initializeApiConfig() {
    // This function can be expanded if configuration needs to be fetched dynamically
    console.log('API base URL configured:', API_CONFIG.baseUrl);
}

document.addEventListener("DOMContentLoaded", async function() {
    // Initialize API configuration
    await initializeApiConfig();
    
    // Check if the user is authenticated
    if (sessionStorage.getItem("login") !== "true") {
        if (!window.location.href.includes("login.html")) {
            window.location.href = "../html/login.html";
            return;
        }
    }

    // Reference to the scroll-to-top button
    const scrollToTopBtn = document.getElementById('scrollToTopBtn');
    if (scrollToTopBtn) {
        scrollToTopBtn.addEventListener('click', function() {
            scrollToTop();
        });
    }
    
    const headerElement = document.getElementsByClassName("app-header")[0]; 

    setupLogoutButton();
    await loadWorkerInfo();
    
    // Setup configuration button if in Electron
    if (window.electronAPI) {
        setupConfigButton();
    }
});

// Setup for the configuration button in Electron
function setupConfigButton() {
    const header = document.querySelector('.app-header');
    if (header && !document.getElementById('config-btn')) {
        const configBtn = document.createElement('button');
        configBtn.id = 'config-btn';
        configBtn.innerHTML = '⚙️ Configurazione';
        configBtn.style.cssText = `
            position: absolute;
            top: 10px;
            right: 100px;
            padding: 8px 16px;
            background: #007bff;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
        `;
        
        configBtn.addEventListener('click', () => {
            if (window.electronAPI && window.electronAPI.openConfig) {
                window.electronAPI.openConfig();
            }
        });
        
        header.appendChild(configBtn);
    }
}

// Setup for the logout button, including cookie deletion
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
            // Redirect to login page after logout
            window.location.href = "../html/login.html";
        });
    }
}

// Function to get the API base URL
export function getApiBaseUrl() {
    return Promise.resolve(API_CONFIG.baseUrl);
}

// Function to get the full API URL with an endpoint
export async function getApiUrl(endpoint = '') {
    const baseUrl = await getApiBaseUrl();
    return `${baseUrl}/${endpoint}`;
}

// Function to load authenticated worker information and populate the header
async function loadWorkerInfo() {
    const workerInformations = document.getElementById("current-user");
    if (!workerInformations) {
        console.error("Element with ID 'current-user' not found in the DOM");
        return;
    }
    
    const encodedCredentials = getCookie("basicAuthCredentials");
    if (!encodedCredentials) {
        displayError(workerInformations, "Credentials not found");
        return;
    }
    
    try {
        const stringCredentials = atob(encodedCredentials);
        const [workerId, password] = stringCredentials.split(":");
        
        if (!workerId || !password) {
            throw new Error("Invalid credentials format");
        }
        
        const workerInfo = getCookie("userInfo");
        const puUser = getCookie("pu-User");

        if (puUser) {
            displayWorkerAndPuInfo(workerInformations, workerInfo, puUser);
        } else {
            displayWorkerInfo(workerInformations, workerInfo);
        }

    } catch (error) {
        console.error("Error loading user information:", error);
        displayError(workerInformations, "Error loading user information");
    }
}

// Populate the header with worker information
function displayWorkerInfo(container, cookie) {
    try {
        const parsedCookie = JSON.parse(cookie);
        if (!parsedCookie) {
            displayError(container, "Invalid or empty cookie");
            return;
        }
        container.innerHTML = `
            ${parsedCookie.tipoUtente || 'N/A'}: ${parsedCookie.password || 'N/A'} - ${parsedCookie.name || 'N/A'} ${parsedCookie.lastName || 'N/A'} ${displayCurrentDate()}`;
    } catch (error) {
        console.error("Error parsing cookie:", error);
        displayError(container, "Error in cookie format");
    }
}

// Populate the header with worker and simulated user information
function displayWorkerAndPuInfo(container, worker, pu) {
    try {
        const parsedWorker = JSON.parse(worker);
        const parsedPu = JSON.parse(pu);
        if (!parsedWorker || !parsedPu) {
            displayError(container, "Invalid or empty cookie");
            return;
        }
        container.innerHTML = `
        Stai usando l'utente: ${parsedPu.workerId || 'N/A'} ${parsedPu.name || 'N/A'} ${parsedPu.lastName || 'N/A'}
        <br> 
        ${parsedWorker.tipoUtente || 'N/A'}: ${parsedWorker.password || 'N/A'} - ${parsedWorker.name || 'N/A'} ${parsedWorker.lastName || 'N/A'} ${displayCurrentDate()}`;
    } catch (error) {
        console.error("Error parsing cookie:", error);
        displayError(container, "Error in cookie format");
    }
}

function displayError(container, message) {
    container.innerHTML = `<p class="error">${message}</p>`;
}

// Format the current date
function displayCurrentDate() {
    const today = new Date();
    const dd = String(today.getDate()).padStart(2, '0');
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const yyyy = today.getFullYear();
    return `${dd}/${mm}/${yyyy}`;
}

// Extract unique values from an array of objects
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

// Parse date and time from a string
export function parseDateTime(dateString) {
    let parsedDate = "";
    let parsedTime = "";
    if (dateString) {
        let dateObj;
        if (dateString.includes("T")) {
            dateObj = new Date(dateString);
        } else {
            dateObj = new Date(dateString.replace(" ", "T"));
        }
        if (!isNaN(dateObj)) {
            parsedDate = dateObj.toLocaleDateString("it-IT");
            parsedTime = dateObj.toLocaleTimeString("it-IT", { hour12: false });
        }
    }
    return {
        date: parsedDate,
        time: parsedTime
    };
}

// Show/hide the scroll-to-top button based on scroll position
window.onscroll = function() {
    const scrollToTopBtn = document.getElementById('scrollToTopBtn');
    if (scrollToTopBtn) {
        if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 300) {
            scrollToTopBtn.classList.add('show');
        } else {
            scrollToTopBtn.classList.remove('show');
        }
    }
};

// Scroll to the top of the page smoothly
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}