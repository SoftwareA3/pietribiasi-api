import { fetchWithAuth } from "./fetch.js";
import { getCookie, setCookie, deleteCookie } from "./cookies.js";
import { setupAutocomplete } from "./autocomplete.js";

document.addEventListener("DOMContentLoaded", async function () {
    const puInput = document.getElementById("pu-codice-addetto");
    const puAutocompleteList = document.getElementById("pu-codice-addetto-autocomplete-list");
    const searchButton = document.getElementById("pu-cerca");
    const loginButton = document.getElementById("pu-accedi");
    const logoutButton = document.getElementById("pu-disconnetti");
    const searchOverlay = document.getElementById("search-overlay");
    const closeSearchButton = document.getElementById("close-search-overlay");
    const cancelSearchButton = document.getElementById("cancel-search");
    const selectSearchResultButton = document.getElementById("select-search-result");
    const searchResultsBody = document.getElementById("search-results-body");
    const userInfo = document.getElementById("pu-selected-user-name");
    const puActions = document.getElementById("poweruser-actions");

    let userList = [];

    let selectedSearchRow = null;

    if(getCookie("pu-User")) {
        const user = JSON.parse(getCookie("pu-User"));
        puActions.style.display = "block";
        loginButton.style.display = "none";
        logoutButton.style.display = "block";
        puInput.value = user.display;
        puInput.disabled = true;
        userInfo.textContent = `${user.name} ${user.lastName}`;
    } else {
        puActions.style.display = "none";
        loginButton.style.display = "block";
        logoutButton.style.display = "none";
        puInput.value = "";
        puInput.disabled = false;
        userInfo.textContent = "";
    }

    // Recupero degli utenti disponibili
    try {
        const userResult = await getAllWorkers();
        if (userResult) {
            userList = userResult.map(user => ({
                workerId: user.workerId,
                name: user.name,
                lastName: user.lastName,
                tipoUtente: user.tipoUtente,
                password: user.password,
                storageVersamenti: user.storageVersamenti,
                storage: user.storage,
                lastLogin: user.lastLogin,
                display: `${user.workerId} ${user.name} (${user.lastName})`
            }));
            setupAutocomplete(puInput, puAutocompleteList, userList);
        }
    } catch (error) {
        console.error("Error fetching user list:", error);
    }

    // Ricerca tramite tabella
    searchButton.addEventListener("click", function() {
        let searchResults = [];

        for (const user of userList) {
            searchResults.push(user);
        }
        console.log("searchResults", searchResults);

        populateSearchResults(searchResults);
        searchOverlay.classList.add("active");
    });

    // Event listeners per i pulsanti di chiusura dell'overlay
    closeSearchButton.addEventListener("click", function() {
        searchOverlay.classList.remove("active");
        selectedSearchRow = null;
    });

    cancelSearchButton.addEventListener("click", function() {
        searchOverlay.classList.remove("active");
        selectedSearchRow = null;
    });

    selectSearchResultButton.addEventListener("click", function() {
        if (selectedSearchRow) {
            puInput.value = selectedSearchRow.display;

            searchOverlay.classList.remove("active");

            // Azzera la selezione alla fine
            selectedSearchRow = null;
        } else {
            alert("Seleziona una riga prima di procedere");
        }
    });

    loginButton.addEventListener("click", function() {
        const puAddetto = puInput.value;

        if (!puAddetto) {
            alert("Inserisci codice addetto e password");
            return;
        }

        const user = userList.find(user => user.display === puAddetto);
        console.log("user", user);

        if (user) {
            setCookie("pu-User", JSON.stringify(user));
            console.log("User logged in:", user);
            alert("Login effettuato come " + user.tipoUtente + " " + user.display);
            window.location.reload();
        } else {
            alert("Credenziali non valide");
        }
    });

    logoutButton.addEventListener("click", function() {
        deleteCookie("pu-User");
        console.log("User logged out");
        window.location.reload();
    });

    function populateSearchResults(results) {
        searchResultsBody.innerHTML = "";
        selectedSearchRow = null;
        
        if (results.length === 0) {
            const row = searchResultsBody.insertRow();
            const cell = row.insertCell();
            cell.colSpan = 4;
            cell.textContent = "Nessun risultato trovato";
            return;
        }

        results.forEach((result, index) => {
            const row = searchResultsBody.insertRow();
            row.dataset.index = index;

            const workerIdCell = row.insertCell();
            workerIdCell.textContent = result.workerId;
            
            const nameCell = row.insertCell();
            nameCell.textContent = result.name;
            
            const lastNameCell = row.insertCell();
            lastNameCell.textContent = result.lastName;
            
            const tipoUtenteCell = row.insertCell();
            tipoUtenteCell.textContent = result.tipoUtente;

            // Event listener per la selezione della riga
            row.addEventListener("click", function() {
                // Rimuovi la selezione precedente
                document.querySelectorAll("#search-results-body tr.selected").forEach(tr => {
                    tr.classList.remove("selected");
                });
                
                // Seleziona questa riga
                row.classList.add("selected");
                selectedSearchRow = results[index];
            });
            
            // Doppio click per selezionare e confermare
            row.addEventListener("dblclick", function() {
                selectedSearchRow = results[index];
                selectSearchResultButton.click(); 
            });
        });
    }
});

async function getAllWorkers() {
    try {
        const response = await fetchWithAuth("http://localhost:5245/api/worker", "GET");
        if (response.ok) {
            const data = await response.json();
            return data;
        } else {
            console.error("Error fetching workers:", response.statusText);
            return [];
        }
    }
    catch (error) {
        console.error("Error fetching workers:", error);
        return [];
    }
}