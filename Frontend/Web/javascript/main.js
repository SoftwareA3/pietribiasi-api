import { deleteCookie, getCookie } from "./cookies.js";

// API configuration - this placeholder will be replaced by the build script
const API_BASE_URL = '##API_BASE_URL##';

document.addEventListener("DOMContentLoaded", async function() {
    // Check if the user is authenticated
    if (sessionStorage.getItem("login") !== "true") {
        if (!window.location.href.includes("login.html")) {
            window.location.href = "../html/login.html";
            return;
        }
    }

    document.addEventListener('keydown', function(e) {
        // Ctrl+R o F5 per refresh
        if ((e.ctrlKey && e.key === 'r') || e.key === 'F5') {
            e.preventDefault();
            window.location.reload();
        }
    });
    // Reference to the scroll-to-top button
    const scrollToTopBtn = document.getElementById('scrollToTopBtn');
    if (scrollToTopBtn) {
        scrollToTopBtn.addEventListener('click', function() {
            scrollToTop();
        });
    }

    setupLogoutButton();
    await loadWorkerInfo();
});

// Setup for the logout button, including cookie deletion
function setupLogoutButton() {
    const logoutButton = document.getElementById("logout");
    if (logoutButton) {
        logoutButton.addEventListener("click", function() {
            const pu = JSON.parse(getCookie("pu-User"));
            if(pu) {
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
    return API_BASE_URL;
}

// Function to get the full API URL with an endpoint
export function getApiUrl(endpoint = '') {
    const baseUrl = getApiBaseUrl();
    const fullUrl = endpoint ? `${baseUrl}/${endpoint}` : baseUrl;
    console.log(`Using API URL: ${fullUrl}`);
    return fullUrl;
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
