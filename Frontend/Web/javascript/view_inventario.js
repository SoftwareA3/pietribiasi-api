import { fetchWithAuth } from "./fetch.js";
import { setupAutocomplete } from "./autocomplete.js";
import {createPagination} from "./pagination.js";
import { getCookie } from "./cookies.js";

let globalAllData = null;
let showImportedItems = false;
let filteredList = [];
let barcodeList = [];
let itemList = [];  

document.addEventListener("DOMContentLoaded", async () => {
    // Recupera gli elementi del DOM:
    const filterDataDa = document.getElementById("filter-inv-data-da");
    const filterDataA = document.getElementById("filter-inv-data-a");
    const filterItem = document.getElementById("filter-inv-item");
    const filterBarcode = document.getElementById("filter-inv-barcode");
    const filterInvSubmit = document.getElementById("filter-inv-submit");
    const inventarioList = document.getElementById("inventario-list");
    const showImportedToggle = document.getElementById("show-imported");

    const noContent = document.getElementById("nocontent");

    inventarioList.classList.add("hidden");
    noContent.classList.remove("hidden");

    // Carica i dati iniziali
    try {
        const user = JSON.parse(getCookie("userInfo"));
        const puUser = JSON.parse(getCookie("pu-User"));
        //console.log("user:", user.tipoUtente);
        if(user && user.tipoUtente === "Amministrazione" && !puUser) 
        {
            filteredList = await fetchAllViewInventario();
            populateInventarioList(filteredList);
            console.log("Dati iniziali caricati:", filteredList);
        }
        else if(user && user.tipoUtente === "Amministrazione" && puUser)
        {
            // Se l'utente è un amministratore e ha effettuato il login come addetto, mostra i dati dell'addetto
            const workerId = {
                workerId: puUser.workerId
            };
            console.log("ID utente:", workerId);
            const response = await fetchViewInventario(workerId);
            if (response && response.length > 0) {
                filteredList = response;
                populateInventarioList(filteredList);
                console.log("Dati iniziali caricati:", filteredList);
            } else {
                console.error("Nessun dato trovato per l'utente:", workerId);
                alert("Nessun dato trovato per l'utente. Inserire prima un'operazione di inventario.");
            }
        }
        else {
            // Il tipo è un addetto: filtro in base al codice utente
            const userId = {
                workerId: user.workerId
            };
            console.log("ID utente:", userId);
            const response = await fetchViewInventario(userId);
            if (response && response.length > 0) {
                filteredList = response;
                populateInventarioList(filteredList);
                //console.log("Dati iniziali caricati:", filteredList);
            } else {
                console.error("Nessun dato trovato per l'utente:", userId);
                alert("Nessun dato trovato per l'utente. Inserire prima un'operazione di inventario.");
            }
        }

        // Inizializza le liste per l'autocomplete
        await refreshAutocompleteData();
    } catch (error) {
        console.error("Errore durante il caricamento iniziale:", error);
        alert("Si è verificato un errore durante il caricamento iniziale dei dati.");
    }

     // Eventi di modifica dei filtri
    filterDataDa.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });

    filterDataA.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });

    filterItem.addEventListener("change", function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    filterBarcode.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    showImportedToggle.addEventListener("change", async function() {
        showImportedItems = this.checked;
        populateInventarioList(filteredList);
    });

    filterInvSubmit.addEventListener("click", async function() {
        try {
            console.log("Pulsante filtro premuto");
            
            // Crea l'oggetto filtro
            const filteredObject = createFilterObject();
            console.log("Filtro applicato:", filteredObject);
            
            // Esegue la chiamata API con i filtri
            const results = await fetchViewInventario(filteredObject);
            //console.log("Risultati ricevuti:", results);
            
            // Aggiorna la lista filtrata e popola la vista
            filteredList = results;
            populateInventarioList(filteredList);
            // Aggiorna le liste per l'autocomplete con i nuovi dati
            await refreshAutocompleteData();
        } catch (error) {
            console.error("Errore durante il filtraggio:", error);
            alert("Si è verificato un errore durante il recupero dei dati.");
        }
    });


});

async function refreshAutocompleteData() {
    const filteredItem = document.getElementById("filter-inv-item");
    const itemAutocompleteList = document.getElementById("filter-inv-item-autocomplete-list");
    const filteredBarcode = document.getElementById("filter-inv-barcode");
    const barcodeAutocompleteList = document.getElementById("filter-inv-barcode-autocomplete-list");

    // Ottiene i dati per l'autocomplete
    const filteredObject = createFilterObject();
    var tempData;
    if(Object.keys(filteredObject).length === 0) {
        tempData = await fetchAllViewInventario();
    }
    else {
        tempData = await fetchViewInventario(filteredObject);
    }
    if (!tempData || tempData.length === 0) {
        tempData = await fetchAllViewInventario();
    }

    itemList = extractUniqueValues(tempData, 'item');
    barcodeList = extractUniqueValues(tempData, 'barCode');

    if(barcodeList.length === 0 && itemList.length > 0) {
        filteredBarcode.disabled = true;
        filteredBarcode.value = "";
        filteredBarcode.placeholder = "Nessun barcode trovato";
    } else {
        filteredBarcode.disabled = false;
        filteredBarcode.placeholder = "";
    }

    //console.log("Dati per l'autocomplete:", itemList, barcodeList);

    setupAutocomplete(filteredItem, itemAutocompleteList, itemList);
    setupAutocomplete(filteredBarcode, barcodeAutocompleteList, barcodeList);
}

// Funzione per creare l'oggetto filtro dai valori degli input
function createFilterObject() {
    const filterDataDa = document.getElementById("filter-inv-data-da");
    const filterDataA = document.getElementById("filter-inv-data-a");
    const filterItem = document.getElementById("filter-inv-item");
    const filterBarcode = document.getElementById("filter-inv-barcode");
    
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

    if (filterItem.value) filteredObject.item = filterItem.value;

    if(filterBarcode.value) filteredObject.barCode = filterBarcode.value;
    
    return filteredObject;
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

function populateInventarioList(data) {
    const inventarioList = document.getElementById("inventario-list");
    const noContent = document.getElementById("nocontent");
    var paginationControls = document.querySelector('.pagination-controls');

    inventarioList.innerHTML = ""; // Pulisce la lista esistente
    noContent.classList.add("hidden");

    let displayData = showImportedItems === true ? data : data.filter(item => item.imported === false || item.imported === "0");

    if (!data || displayData.length === 0) {
        inventarioList.classList.add("hidden");
        noContent.classList.remove("hidden");
        if(paginationControls != null)
        {
            paginationControls.classList.add("hidden");
        }
        return;
    }

    // Mostra la lista e nascondi il messaggio "nessun elemento"
    inventarioList.classList.remove("hidden");
    noContent.classList.add("hidden");

    // Popola la lista con gli elementi
    displayData.forEach(item => {
        const li = document.createElement("li");
        li.dataset.id = item.invId // Aggiunge un data attribute per identificare l'elemento
    
        // Formatta la data in un formato leggibile
        const savedDate = new Date(item.savedDate);
        const formattedDate = savedDate.toLocaleDateString("it-IT");

        // Crea il contenuto dell'elemento
        const itemContent = document.createElement("div");
        itemContent.className = "item-content";

        const isImported = item.imported === false || item.imported === "0" ? false : true;
        
        const statusIndicator = document.createElement("div");
        statusIndicator.className = `status-indicator ${isImported === true ? 'status-closed' : 'status-open'}`;
        statusIndicator.title = isImported === true ? 'Importato' : 'Modificabile';
        itemContent.appendChild(statusIndicator);

        // Estrai solo la data e l'orario da dataImp nel formato richiesto
        let parsedDate = "";
        let parsedTime = "";
        if (item.dataImp) {
            // Gestisce sia formato "YYYY-MM-DD HH:mm:ss.SSS" che ISO "YYYY-MM-DDTHH:mm:ss.SSSZ"
            let dateObj;
            if (item.dataImp.includes("T")) {
            // ISO format
            dateObj = new Date(item.dataImp);
            } else {
            // "YYYY-MM-DD HH:mm:ss.SSS"
            // Sostituisci spazio con "T" per compatibilità con Date
            dateObj = new Date(item.dataImp.replace(" ", "T"));
            }
            if (!isNaN(dateObj)) {
            // Formatta la data come "dd/MM/yyyy"
            parsedDate = dateObj.toLocaleDateString("it-IT");
            // Formatta l'orario come "HH:mm:ss"
            parsedTime = dateObj.toLocaleTimeString("it-IT", { hour12: false });
            }
        }

        // Aggiunge le informazioni dell'elemento
        itemContent.innerHTML += `
            <div><strong>Item:</strong> ${item.item} </div>
            ${item.barCode ? "<div><strong>BarCode:</strong>" + item.barCode + "</div>" : ""}
            <div><strong>Desc:</strong> ${item.description} </div>
            <div><strong>Operatore:</strong> ${item.workerId} </div>
            <div><strong>Data:</strong> ${formattedDate} </div>
            <div><strong>Qta: <span class="inv-value" id="inv-value-${item.invId}">${item.bookInv}</strong></span> </div>
            ${isImported === true ? `<div><strong>Importato il:</strong> ${parsedDate} alle ${parsedTime} </div>` : ''}
            ${isImported === true ? `<div><strong>Importato da:</strong> ${item.userImp} </div>` : ''}
        `;
        
        li.appendChild(itemContent);

        if (!isImported) {
            const itemActions = document.createElement("div");
            itemActions.className = "item-actions";
            
            // Container per il campo di modifica delle ore (inizialmente nascosto)
            const editContainer = document.createElement("div");
            editContainer.className = "edit-inv-container hidden";
            editContainer.id = `edit-container-${item.invId}`;
            
            // Campo input per la modifica
            const editInput = document.createElement("input");
            editInput.type = "number";
            editInput.min = "0";
            editInput.step = "0.1";
            editInput.className = "edit-inv-input";
            editInput.id = `edit-inv-input-${item.invId}`;
            editInput.value = item.bookInv;
            
            // Pulsante di conferma modifica
            const confirmButton = document.createElement("button");
            confirmButton.className = "button-icon confirm option-button";
            confirmButton.title = "Conferma modifica";
            confirmButton.innerHTML = '<i class="fa-solid fa-check"></i>';
            confirmButton.addEventListener("click", () => saveInvEdit(item, data));
            
            // Pulsante di annullamento modifica
            const cancelButton = document.createElement("button");
            cancelButton.className = "button-icon cancel option-button";
            cancelButton.title = "Annulla modifica";
            cancelButton.innerHTML = '<i class="fa-solid fa-times"></i>';
            cancelButton.addEventListener("click", () => cancelInvEdit(item));
            
            // Aggiunge gli elementi al container di modifica
            editContainer.appendChild(editInput);
            editContainer.appendChild(confirmButton);
            editContainer.appendChild(cancelButton);
            
            // Pulsante di modifica
            const editButton = document.createElement("button");
            editButton.className = "button-icon edit option-button";
            editButton.title = "Modifica Qta";
            editButton.id = `edit-button-${item.invId}`;
            editButton.innerHTML = '<i class="fa-solid fa-pencil"></i>';
            editButton.addEventListener("click", () => editInv(item));

            // Aggiunge gli elementi al container delle azioni
            itemActions.appendChild(editContainer);
            itemActions.appendChild(editButton);
            li.appendChild(itemActions);
        }

        inventarioList.appendChild(li);
    });

    if(paginationControls)
    {
        paginationControls.remove();
    }
    
    var pagination = createPagination("#inventario-list");
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

async function saveInvEdit(item, data) {
    // Ottiene il nuovo valore dall'input
    const editInput = document.getElementById(`edit-inv-input-${item.invId}`);
    const newBookInvQty = parseFloat(editInput.value);

    // Verifica che l'input sia valido
    if (isNaN(newBookInvQty) || newBookInvQty <= 0) {
        alert("Inserisci un valore numerico positivo.");
        return;
    }

    // Prepara i dati per l'aggiornamento
    const updateData = {
        invId: item.invId,
        bookInv: newBookInvQty,
    };

    try {
        const response = await fetchWithAuth("http://localhost:5245/api/inventario/view_inventario/edit_book_inv", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updateData)
        });
        
        if (!response.ok) {
            throw new Error("Errore durante l'aggiornamento delle ore");
        }

        item.bookInv = newBookInvQty;

        const dataItem = data.find(d => d.invId === item.invId);
        if (dataItem) {
            dataItem.bookInv = item.bookInv;
        }
        
        // Aggiorna la visualizzazione
        const invValueSpan = document.getElementById(`inv-value-${item.invId}`);
        if (invValueSpan) invValueSpan.textContent = newBookInvQty;
        
        // Ripristina la visualizzazione normale
        cancelInvEdit(item);
        
        // Feedback all'utente
        invValueSpan.classList.add("just-updated");
        setTimeout(() => {
            invValueSpan.classList.remove("just-updated");
        }, 2000);

        populateInventarioList(data);
        
    } catch (error) {
        console.error("Errore durante l'aggiornamento:", error);
        alert("Si è verificato un errore durante l'aggiornamento delle quantità.");
    }
}

// Funzione per gestire la modifica delle quantità
function editInv(item) {
    // Identifica gli elementi DOM necessari
    const editContainer = document.getElementById(`edit-container-${item.invId}`);
    const editButton = document.getElementById(`edit-button-${item.invId}`);
    const editInput = document.getElementById(`edit-prel-input-${item.invId}`);
    
    // Nasconde il pulsante di modifica
    if (editButton) editButton.classList.add("hidden");
    
    // Mostra il container di modifica
    if (editContainer) editContainer.classList.remove("hidden");
    
    if (editInput) {
        editInput.value = parseFloat(item.bookInv);
        editInput.focus();
        editInput.select();
    }
}

// Funzione per annullare la modifica
function cancelInvEdit(item) {
    // Identifica gli elementi DOM necessari
    const editContainer = document.getElementById(`edit-container-${item.invId}`);
    const editButton = document.getElementById(`edit-button-${item.invId}`);
    
    // Mostra il pulsante di modifica
    if (editButton) editButton.classList.remove("hidden");
    
    // Nasconde il container di modifica
    if (editContainer) editContainer.classList.add("hidden");
}

async function fetchAllViewInventario() {
    if(globalAllData) {
        //console.log("Dati già caricati, restituisco i dati globali");
        return globalAllData
    };
    try {
        //console.log("CARICAMENTO DI TUTTI I DATI");
        const response = await fetchWithAuth("http://localhost:5245/api/inventario/get_all", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) {
            throw new Error("Errore nella risposta del server");
        }
        globalAllData = await response.json();
        return globalAllData;
    } catch (error) {
        console.error("Errore durante il recupero dei dati:", error);
        alert("Si è verificato un errore durante il recupero dei dati.");
    }
}

async function fetchViewInventario(filteredObject) {
    try {
        console.log("Chiamata API con oggetto filtro:", filteredObject);
        const request = await fetchWithAuth("http://localhost:5245/api/inventario/get_view_inventario", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(filteredObject)
        });
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const info = await request.json();
        console.log("Dati ricevuti:", info);
        return info;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}