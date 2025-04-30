import { fetchWithAuth } from "./fetch.js";
import { setupAutocomplete } from "./autocomplete.js";
import { createPagination } from "./pagination.js";
import { getCookie } from "./cookies.js";

// Variabili globali per mantenere lo stato
let filteredList = [];
let commessaList = [];
let lavorazioneList = [];
let odpList = [];
let itemList = [];
let barcodeList = [];

document.addEventListener("DOMContentLoaded", async function() {
    // Recupera gli elementi del DOM:
    const filterDataDa = document.getElementById("filter-prel-data-da");
    const filterDataA = document.getElementById("filter-prel-data-a");
    const filterCommessa = document.getElementById("filter-prel-commessa");
    const filterLavorazione = document.getElementById("filter-prel-lavorazione");
    const filterOrdineDiProduzione = document.getElementById("filter-prel-odp");
    const filterItem = document.getElementById("filter-prel-item");
    const filterBarcode = document.getElementById("filter-prel-barcode");
    const filterPrelieviSubmit = document.getElementById("filter-prel-submit");
    const prelieviList = document.getElementById("prelievi-list");
    const noContent = document.getElementById("nocontent");

    prelieviList.classList.add("hidden");
    noContent.classList.remove("hidden");

    // Carica i dati iniziali
    try {
        const user = JSON.parse(getCookie("userInfo"));
        const puUser = JSON.parse(getCookie("pu-User"));
        //console.log("user:", user.tipoUtente);
        if(user && user.tipoUtente === "Amministrazione" && !puUser) 
        {
            filteredList = await fetchAllViewPrelievi();
            populatePrelieviList(filteredList);
            console.log("Dati iniziali caricati:", filteredList);
        }
        else if(user && user.tipoUtente === "Amministrazione" && puUser)
        {
            // Se l'utente è un amministratore e ha effettuato il login come addetto, mostra i dati dell'addetto
            const workerId = {
                workerId: puUser.workerId
            };
            console.log("ID utente:", workerId);
            const response = await fetchViewPrelievi(workerId);
            if (response && response.length > 0) {
                filteredList = response;
                populatePrelieviList(filteredList);
                console.log("Dati iniziali caricati:", filteredList);
            } else {
                console.error("Nessun dato trovato per l'utente:", workerId);
                alert("Nessun dato trovato per l'utente.");
            }
        }
        else {
            // Il tipo è un addetto: filtro in base al codice utente
            const userId = {
                workerId: user.workerId
            };
            console.log("ID utente:", userId);
            const response = await fetchViewPrelievi(userId);
            if (response && response.length > 0) {
                filteredList = response;
                populatePrelieviList(filteredList);
                console.log("Dati iniziali caricati:", filteredList);
            } else {
                console.error("Nessun dato trovato per l'utente:", userId);
                alert("Nessun dato trovato per l'utente.");
            }
        }

        // Inizializza le liste per l'autocomplete
        await refreshAutocompleteData();
    } catch (error) {
        console.error("Errore durante il caricamento iniziale:", error);
        alert("Si è verificato un errore durante il recupero dei dati.");
    }

    // Eventi di modifica dei filtri
    filterDataDa.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });

    filterDataA.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });

    filterCommessa.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    filterLavorazione.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    filterOrdineDiProduzione.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    filterItem.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);

        if(barcodeList.length === 0) {
            filterBarcode.disabled = true;
            filterBarcode.value = "";
            filterBarcode.placeholder = "Nessun barcode trovato";
        }
    });

    filterBarcode.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    // Setup pulsante di filtro
    filterPrelieviSubmit.addEventListener("click", async function() {
        try {
            console.log("Pulsante filtro premuto");
            
            // Crea l'oggetto filtro
            const filteredObject = createFilterObject();
            console.log("Filtro applicato:", filteredObject);
            
            // Esegue la chiamata API con i filtri
            const results = await fetchViewPrelievi(filteredObject);
            console.log("Risultati ricevuti:", results);
            
            // Aggiorna la lista filtrata e popola la vista
            filteredList = results;
            populatePrelieviList(filteredList);
            
            // Aggiorna le liste per l'autocomplete con i nuovi dati
            await refreshAutocompleteData();
        } catch (error) {
            console.error("Errore durante il filtraggio:", error);
            alert("Si è verificato un errore durante il recupero dei dati.");
        }
    });
});

// Funzione per creare l'oggetto filtro dai valori degli input
function createFilterObject() {
    const filterDataDa = document.getElementById("filter-prel-data-da");
    const filterDataA = document.getElementById("filter-prel-data-a");
    const filterCommessa = document.getElementById("filter-prel-commessa");
    const filterLavorazione = document.getElementById("filter-prel-lavorazione");
    const filterOrdineDiProduzione = document.getElementById("filter-prel-odp");
    const filterItem = document.getElementById("filter-prel-item");
    const filterBarcode = document.getElementById("filter-prel-barcode");

    
    const filteredObject = {};

    const user = JSON.parse(getCookie("userInfo"));
    if(user && user.tipoUtente === "Addetto")
    {
        filteredObject.workerId = user.workerId;
    }

    const puUser = JSON.parse(getCookie("pu-User"));
    if(puUser && puUser.workerId)
    {
        filteredObject.workerId = puUser.workerId;
    }
    
    if (filterDataDa.value) {
        const fromDate = new Date(filterDataDa.value);
        fromDate.setHours(0, 0, 0, 0); // Imposta l'ora a mezzanotte
        filteredObject.fromDateTime = fromDate.toISOString().slice(0, -1); // Rimuove la "Z" che crea problemi con le chiamate all'API
    }
    
    if (filterDataA.value) {
        const toDate = new Date(filterDataA.value);
        toDate.setHours(23, 59, 59, 999); // Imposta l'ora alle 23:59:59.999
        filteredObject.toDateTime = toDate.toISOString().slice(0, -1);
    }
    if(filterCommessa.value) filteredObject.job = filterCommessa.value;
    if(filterLavorazione.value) filteredObject.operation = filterLavorazione.value;
    if(filterOrdineDiProduzione.value) filteredObject.mono = filterOrdineDiProduzione.value;
    if(filterItem.value) filteredObject.component = filterItem.value;
    if(filterBarcode.value) filteredObject.barCode = filterBarcode.value;
    
    return filteredObject;
}

// Funzione per aggiornare le liste per l'autocomplete
async function refreshAutocompleteData() {
    const filterCommessa = document.getElementById("filter-prel-commessa");
    const filterLavorazione = document.getElementById("filter-prel-lavorazione");
    const filterOrdineDiProduzione = document.getElementById("filter-prel-odp");
    const filterBarcode = document.getElementById("filter-prel-barcode");
    const filterItem = document.getElementById("filter-prel-item");
    const commessaAutocompleteList = document.getElementById("filter-prel-commessa-autocomplete-list");
    const lavorazioneAutocompleteList = document.getElementById("filter-prel-lavorazione-autocomplete-list");
    const odpAutocompleteList = document.getElementById("filter-prel-odp-autocomplete-list");
    const itemAutocompleteList = document.getElementById("filter-prel-item-autocomplete-list");
    const barcodeAutocompleteList = document.getElementById("filter-prel-barcode-autocomplete-list");
    
    // Ottiene i dati per l'autocomplete
    const filteredObject = createFilterObject();
    const tempData = await fetchViewPrelievi(filteredObject);
    
    // Estrae i valori unici per ciascuna lista se necessario
    commessaList = extractUniqueValues(tempData, 'job');
    lavorazioneList = extractUniqueValues(tempData, 'operation');
    odpList = extractUniqueValues(tempData, 'mono');
    barcodeList = extractUniqueValues(tempData, 'barCode');
    itemList = extractUniqueValues(tempData, 'component');
    
    console.log("Liste per autocomplete aggiornate:", {
        commesse: commessaList,
        lavorazioni: lavorazioneList,
        odp: odpList,
        barCodes: barcodeList,
        items: itemList
    });
    
    // Configura l'autocomplete per ciascun campo
    setupAutocomplete(filterCommessa, commessaAutocompleteList, commessaList);
    setupAutocomplete(filterLavorazione, lavorazioneAutocompleteList, lavorazioneList);
    setupAutocomplete(filterOrdineDiProduzione, odpAutocompleteList, odpList);
    setupAutocomplete(filterItem, itemAutocompleteList, itemList);
    setupAutocomplete(filterBarcode, barcodeAutocompleteList, barcodeList);
}

// Funzione per estrarre valori unici da un array di oggetti
function extractUniqueValues(data, field) {
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

// Recupera tutti i prelievi dalla tabella A3_app_prel_mat
async function fetchAllViewPrelievi() {
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/prel_mat/get_all", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const info = await request.json();
        return info;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

// Funzione per recuperare i prelievi filtrati
async function fetchViewPrelievi(filteredObject) {
    // console.log("Chiamata API con filtri:", filteredObject);

    try {
        const request = await fetchWithAuth("http://localhost:5245/api/prel_mat/get_view_prel_mat", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(filteredObject),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const info = await request.json();
        return info;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

function populatePrelieviList(data) {
    const prelieviList = document.getElementById("prelievi-list");
    const noContent = document.getElementById("nocontent");
    
    // Pulisce la lista attuale
    prelieviList.innerHTML = "";
    prelieviList.classList.add("hidden");
    
    // Controlla se la lista è vuota
    if (!data || data.length === 0) {
        noContent.classList.remove("hidden");
        return;
    }
    
    // Mostra la lista e nascondi il messaggio "nessun elemento"
    prelieviList.classList.remove("hidden");
    noContent.classList.add("hidden");
    
    // Popola la lista con gli elementi
    data.forEach(item => {
        const li = document.createElement("li");
        li.dataset.id = item.prelMatId; // Aggiunge un data attribute per identificare l'elemento
        
        // Formatta la data in un formato più leggibile
        const savedDate = new Date(item.savedDate);
        const formattedDate = savedDate.toLocaleDateString("it-IT");
        
        // Crea il contenuto dell'elemento
        const itemContent = document.createElement("div");
        itemContent.className = "item-content";

        const isImported = item.imported === 0 || item.imported === "0";
        
        const statusIndicator = document.createElement("div");
        statusIndicator.className = `status-indicator ${isImported ? 'status-closed' : 'status-open'}`;
        statusIndicator.title = isImported ? 'Importato' : 'Modificabile';
        itemContent.appendChild(statusIndicator);
        
        // Aggiunge le informazioni dell'elemento
        itemContent.innerHTML += `
            <div><strong>Comm:</strong> ${item.job} </div>
            <div><strong>Lav:</strong> ${item.operation} </div> 
            <div><strong>ODP:</strong> ${item.mono} </div>
            <div><strong>Barcode:</strong> ${item.barCode} </div>
            <div><strong>Item:</strong> ${item.component} </div>
            <div><strong>Operatore:</strong> ${item.workerId} </div>
            <div><strong>Data:</strong> ${formattedDate} </div>
            <div><strong>Qta: <span class="prel-value" id="prel-value-${item.prelMatId}">${item.prelQty}</strong></span> </div>
        `;
        
        li.appendChild(itemContent);
        
        // Aggiunge i pulsanti di azione solo se imported è 0
        if (!isImported) {
            const itemActions = document.createElement("div");
            itemActions.className = "item-actions";
            
            // Container per il campo di modifica delle quantità prelevate (inizialmente nascosto)
            const editContainer = document.createElement("div");
            editContainer.className = "edit-prel-container hidden";
            editContainer.id = `edit-container-${item.prelMatId}`;
            
            // Campo input per la modifica
            const editInput = document.createElement("input");
            editInput.type = "number";
            editInput.min = "1";
            editInput.step = "1";
            editInput.className = "edit-prel-input";
            editInput.id = `edit-prel-input-${item.prelMatId}`;
            editInput.value = item.prelQty;
            
            // Pulsante di conferma modifica
            const confirmButton = document.createElement("button");
            confirmButton.className = "button-icon confirm option-button";
            confirmButton.title = "Conferma modifica";
            confirmButton.innerHTML = '<i class="fa-solid fa-check"></i>';
            confirmButton.addEventListener("click", () => savePrelieviEdit(item, data));
            
            // Pulsante di annullamento modifica
            const cancelButton = document.createElement("button");
            cancelButton.className = "button-icon cancel option-button";
            cancelButton.title = "Annulla modifica";
            cancelButton.innerHTML = '<i class="fa-solid fa-times"></i>';
            cancelButton.addEventListener("click", () => cancelPrelieviEdit(item));
            
            // Aggiunge gli elementi al container di modifica
            editContainer.appendChild(editInput);
            editContainer.appendChild(confirmButton);
            editContainer.appendChild(cancelButton);
            
            // Pulsante di modifica
            const editButton = document.createElement("button");
            editButton.className = "button-icon edit option-button";
            editButton.title = "Modifica Quantità Prelevate";
            editButton.id = `edit-button-${item.prelMatId}`;
            editButton.innerHTML = '<i class="fa-solid fa-pencil"></i>';
            editButton.addEventListener("click", () => editPrelievi(item));
            
            // Pulsante di eliminazione
            const deleteButton = document.createElement("button");
            deleteButton.className = "button-icon delete option-button";
            deleteButton.title = "Elimina Registrazione";
            deleteButton.innerHTML = '<i class="fa-solid fa-trash"></i>';
            deleteButton.addEventListener("click", () => deletePrelievi(item));
            
            // Aggiunge gli elementi al container delle azioni
            itemActions.appendChild(editContainer);
            itemActions.appendChild(editButton);
            itemActions.appendChild(deleteButton);
            li.appendChild(itemActions);
        }
        
        prelieviList.appendChild(li);
    });

    var paginationControls = document.querySelector('.pagination-controls');
    if(paginationControls)
    {
        paginationControls.remove();
    }
    
    var pagination = createPagination("#prelievi-list");
    if(pagination)
    {
        pagination.updatePaginationControls();
    }

    if(data.length < 6)
    {
        const paginationContainer = document.querySelector('.pagination-controls');
        if (paginationContainer) {
            paginationContainer.classList.add('hidden');
        }
    }
}

// Funzione per gestire la modifica delle quantità
function editPrelievi(item) {
    // Identifica gli elementi DOM necessari
    const prelieviValueSpan = document.getElementById(`prel-value-${item.prelMatId}`);
    const editContainer = document.getElementById(`edit-container-${item.prelMatId}`);
    const editButton = document.getElementById(`edit-button-${item.prelMatId}`);
    const editInput = document.getElementById(`edit-prel-input-${item.prelMatId}`);
    
    // Nasconde il pulsante di modifica
    if (editButton) editButton.classList.add("hidden");
    
    // Mostra il container di modifica
    if (editContainer) editContainer.classList.remove("hidden");
    
    if (editInput) {
        editInput.value = item.prelQty;
        editInput.focus();
        editInput.select();
    }
}

// Funzione per salvare le modifiche alle quantità prelevate
async function savePrelieviEdit(item, data) {
    // Ottiene il nuovo valore dall'input
    const editInput = document.getElementById(`edit-prel-input-${item.prelMatId}`);
    const newPrelQty = parseInt(editInput.value);
    
    // Verifica che l'input sia valido
    if (isNaN(newPrelQty) || newPrelQty <= 0) {
        alert("Inserisci un valore numerico positivo.");
        return;
    }

    // Prepara i dati per l'aggiornamento
    const updateData = {
        prelMatId: item.prelMatId,
        prelQty: newPrelQty,
    };
    
    try {
        const response = await fetchWithAuth("http://localhost:5245/api/prel_mat/view_prel_mat/edit_prel_qty", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updateData)
        });
        
        if (!response.ok) {
            throw new Error("Errore durante l'aggiornamento delle ore");
        }

        item.prelQty = newPrelQty;

        const dataItem = data.find(d => d.prelMatId === item.prelMatId);
        if (dataItem) {
            dataItem.prelQty = item.prelQty;
        }
        
        // Aggiorna la visualizzazione
        const prelieviValueSpan = document.getElementById(`prel-value-${item.prelMatId}`);
        if (prelieviValueSpan) prelieviValueSpan.textContent = newPrelQty;
        
        // Ripristina la visualizzazione normale
        cancelPrelieviEdit(item);
        
        // Feedback all'utente
        prelieviValueSpan.classList.add("just-updated");
        setTimeout(() => {
            prelieviValueSpan.classList.remove("just-updated");
        }, 2000);

        populatePrelieviList(data);
        
    } catch (error) {
        console.error("Errore durante l'aggiornamento:", error);
        alert("Si è verificato un errore durante l'aggiornamento delle quantità.");
    }
}

// Funzione per annullare la modifica
function cancelPrelieviEdit(item) {
    // Identifica gli elementi DOM necessari
    const editContainer = document.getElementById(`edit-container-${item.prelMatId}`);
    const editButton = document.getElementById(`edit-button-${item.prelMatId}`);
    
    // Mostra il pulsante di modifica
    if (editButton) editButton.classList.remove("hidden");
    
    // Nasconde il container di modifica
    if (editContainer) editContainer.classList.add("hidden");
}

// Funzione per gestire l'eliminazione di un prelievo
async function deletePrelievi(item) {
    const deleteData = {
        prelMatId: item.prelMatId
    }
    // Chiede conferma all'utente
    if (!confirm("Sei sicuro di voler eliminare questa registrazione?")) {
        return;
    }
    
    try {
        const response = await fetchWithAuth(`http://localhost:5245/api/prel_mat/view_prel_mat/delete_prel_mat_id`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(deleteData)
        });
        
        if (!response.ok) {
            throw new Error("Errore durante l'eliminazione della registrazione");
        }
        
        // Rimuove l'elemento dalla lista filtrata
        filteredList = filteredList.filter(prel => prel.prelMatId !== item.prelMatId);
        
        // Aggiorna la vista
        populatePrelieviList(filteredList);
        
        alert("Registrazione eliminata con successo!");
    } catch (error) {
        console.error("Errore durante l'eliminazione:", error);
        alert("Si è verificato un errore durante l'eliminazione della registrazione.");
    }
}   