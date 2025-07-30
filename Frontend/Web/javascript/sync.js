import { fetchWithAuth } from "./fetch.js";
import { setupAutocomplete } from "./autocomplete.js";
import { createPagination } from "./pagination.js";
import { getCookie } from "./cookies.js";
import { getApiUrl, parseDateTime } from "./main.js";

// Variabili globali per mantenere lo stato
let globalInventarioData = null;
let globalOreData = null;
let globalPrelieviData = null;

// Liste filtrate per ciascuna tab
let filteredInventarioList = [];
let filteredOreList = [];
let filteredPrelieviList = [];

// Liste per autocomplete
let inventarioLists = {
    itemList: [],
    barcodeList: []
};

let oreLists = {
    commessaList: [],
    lavorazioneList: [],
    odpList: []
};

let prelieviLists = {
    commessaList: [],
    lavorazioneList: [],
    odpList: [],
    itemList: [],
    barcodeList: []
};

// Tab attiva corrente
let activeTab = 'global';

document.addEventListener("DOMContentLoaded", async function() {
    // Inizializza la gestione delle tab
    await initializeTabs();
    
    // Carica i dati iniziali
    await loadInitialData();
    
    // Setup event listeners per ogni tab
    setupInventarioListeners();
    setupOreListeners();
    setupPrelieviListeners();

    // Setup listener per il pulsante di sincronizzazione
    await setupSyncButtonListener();
});

// =====================================================
// GESTIONE TAB
// =====================================================

async function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabContents = document.querySelectorAll('.tab-content');
    
    tabButtons.forEach(button => {
        button.addEventListener('click', async function() {
            const tabId = this.getAttribute('data-tab');
            
            // Rimuovi active da tutti i bottoni e contenuti
            tabButtons.forEach(btn => btn.classList.remove('active'));
            tabContents.forEach(content => content.classList.remove('active'));
            
            // Aggiungi active al bottone e contenuto corrente
            this.classList.add('active');
            document.getElementById(`${tabId}-tab`).classList.add('active');
            
            activeTab = tabId;
            console.log("Tab attiva:", activeTab);
            await loadInitialData();
        });
    });
}

// =====================================================
// CARICAMENTO DATI INIZIALI
// =====================================================

async function loadInitialData() {
    try {
        const user = JSON.parse(getCookie("userInfo"));
        const puUser = JSON.parse(getCookie("pu-User"));
        
        switch (activeTab) {
            case 'inventario':
                // Carica dati inventario
                if (user && user.tipoUtente === "Amministrazione" && !puUser) {
                    filteredInventarioList = await fetchAllInventarioNotImported();
                } else {
                    const userId = puUser?.workerId || user?.workerId;
                    if (userId) {
                        const response = await fetchInventario({ workerId: userId });
                        filteredInventarioList = response || [];
                    }
                }
                populateInventarioList(filteredInventarioList);
                break;
            case 'ore':
                // Carica dati ore
                if (user && user.tipoUtente === "Amministrazione" && !puUser) {
                    filteredOreList = await fetchAllOreNotImported();
                } else {
                    const userId = puUser?.workerId || user?.workerId;
                    if (userId) {
                        const response = await fetchOre({ workerId: userId });
                        filteredOreList = response || [];
                    }
                }
                populateOreList(filteredOreList);
                break;
            case 'prelievi':
                // Carica dati prelievi
                if (user && user.tipoUtente === "Amministrazione" && !puUser) {
                    filteredPrelieviList = await fetchAllPrelieviNotImported();
                } else {
                    const userId = puUser?.workerId || user?.workerId;
                    if (userId) {
                        const response = await fetchPrelievi({ workerId: userId });
                        filteredPrelieviList = response || [];
                    }
                }
                populatePrelieviList(filteredPrelieviList);
                break;
            case 'global':
                // Carica dati globali (se necessario)
                break; // Non fare nulla per la tab globale
            default:
                console.error("Tab non riconosciuta:", activeTab);
                return;
        }

        // Inizializza autocomplete per tutte le tab
        await refreshAllAutocompleteData();
        
    } catch (error) {
        console.error("Errore durante il caricamento iniziale:", error);
        alert("Si è verificato un errore durante il recupero dei dati.");
    }
}

// =====================================================
// SETUP EVENT LISTENERS - INVENTARIO
// =====================================================

function setupInventarioListeners() {
    const filterDataDa = document.getElementById("filter-inv-data-da");
    const filterDataA = document.getElementById("filter-inv-data-a");
    const filterItem = document.getElementById("filter-inv-item");
    const filterBarcode = document.getElementById("filter-inv-barcode");
    const filterSubmit = document.getElementById("filter-inv-submit");
    const syncInvButton = document.getElementById("sync-inv-data");
    
    // Event listeners per refresh autocomplete
    [filterDataDa, filterDataA].forEach(element => {
        if (element) {
            element.addEventListener("change", async () => {
                await refreshInventarioAutocomplete();
            });
        }
    });
    
    [filterItem, filterBarcode].forEach(element => {
        if (element) {
            element.addEventListener("change", async () => {
                setTimeout(async () => {
                    await refreshInventarioAutocomplete();
                }, 200);
            });
        }
    });
    
    // Setup pulsante filtro
    if (filterSubmit) {
        filterSubmit.addEventListener("click", async () => {
            try {
                const filteredObject = createInventarioFilterObject();
                const results = await fetchInventario(filteredObject);
                filteredInventarioList = results;
                populateInventarioList(filteredInventarioList);
                await refreshInventarioAutocomplete();
            } catch (error) {
                console.error("Errore durante il filtraggio inventario:", error);
                alert("Si è verificato un errore durante il filtraggio.");
            }
        });
    }

    if(syncInvButton) {
        syncInvButton.addEventListener("click", async () => {
            syncInvButton.disabled = true;
            await syncInventarioFiltered();
            setTimeout(() => {
                syncInvButton.disabled = false;
            }, 2500);
        });
    }
}

// =====================================================
// SETUP EVENT LISTENERS - ORE
// =====================================================

function setupOreListeners() {
    const filterDataDa = document.getElementById("filter-ore-data-da");
    const filterDataA = document.getElementById("filter-ore-data-a");
    const filterCommessa = document.getElementById("filter-ore-commessa");
    const filterLavorazione = document.getElementById("filter-ore-lavorazione");
    const filterOdp = document.getElementById("filter-ore-odp");
    const filterSubmit = document.getElementById("filter-ore-submit");
    const syncOreButton = document.getElementById("sync-ore-data");
    
    // Event listeners per refresh autocomplete
    [filterDataDa, filterDataA].forEach(element => {
        if (element) {
            element.addEventListener("change", async () => {
                await refreshOreAutocomplete();
            });
        }
    });
    
    [filterCommessa, filterLavorazione, filterOdp].forEach(element => {
        if (element) {
            element.addEventListener("change", async () => {
                setTimeout(async () => {
                    await refreshOreAutocomplete();
                }, 200);
            });
        }
    });
    
    // Setup pulsante filtro
    if (filterSubmit) {
        filterSubmit.addEventListener("click", async () => {
            try {
                const filteredObject = createOreFilterObject();
                const results = await fetchOre(filteredObject);
                filteredOreList = results;
                populateOreList(filteredOreList);
                await refreshOreAutocomplete();
            } catch (error) {
                console.error("Errore durante il filtraggio ore:", error);
                alert("Si è verificato un errore durante il filtraggio.");
            }
        });
    }

    if(syncOreButton) {
        syncOreButton.addEventListener("click", async () => {
            syncOreButton.disabled = true;
            await syncRegOreFiltered();
            setTimeout(() => {
                syncOreButton.disabled = false;
            }, 2500);
        });
    }
}

// =====================================================
// SETUP EVENT LISTENERS - PRELIEVI
// =====================================================

async function setupSyncButtonListener() {
    const syncButton = document.getElementById("sync-data");
    const iconElement = syncButton.querySelector(".button-icon");
    iconElement.classList.remove("sync-success", "sync-error");
    iconElement.classList.remove("sync-loading");

    syncButton.addEventListener("click", async function() {
        // Disabilita il pulsante durante la sincronizzazione
        syncButton.disabled = true;
        await synchronizeData();
        // Riabilita il pulsante dopo il completamento
        setTimeout(() => {
            syncButton.disabled = false;
        }, 2500);
    });
}

function setupPrelieviListeners() {
    const filterDataDa = document.getElementById("filter-prel-data-da");
    const filterDataA = document.getElementById("filter-prel-data-a");
    const filterCommessa = document.getElementById("filter-prel-commessa");
    const filterLavorazione = document.getElementById("filter-prel-lavorazione");
    const filterOdp = document.getElementById("filter-prel-odp");
    const filterItem = document.getElementById("filter-prel-item");
    const filterBarcode = document.getElementById("filter-prel-barcode");
    const filterSubmit = document.getElementById("filter-prel-submit");
    const syncPrelButton = document.getElementById("sync-prel-data");
    
    // Event listeners per refresh autocomplete
    [filterDataDa, filterDataA].forEach(element => {
        if (element) {
            element.addEventListener("change", async () => {
                await refreshPrelieviAutocomplete();
            });
        }
    });
    
    [filterCommessa, filterLavorazione, filterOdp, filterItem, filterBarcode].forEach(element => {
        if (element) {
            element.addEventListener("change", async () => {
                setTimeout(async () => {
                    await refreshPrelieviAutocomplete();
                }, 200);
            });
        }
    });
    
    // Setup pulsante filtro
    if (filterSubmit) {
        filterSubmit.addEventListener("click", async () => {
            try {
                const filteredObject = createPrelieviFilterObject();
                const results = await fetchPrelievi(filteredObject);
                filteredPrelieviList = results;
                populatePrelieviList(filteredPrelieviList);
                await refreshPrelieviAutocomplete();
            } catch (error) {
                console.error("Errore durante il filtraggio prelievi:", error);
                alert("Si è verificato un errore durante il filtraggio.");
            }
        });
    }

    if(syncPrelButton) {
        syncPrelButton.addEventListener("click", async () => {
            syncPrelButton.disabled = true;
            await syncPrelieviFiltered();
            setTimeout(() => {
                syncPrelButton.disabled = false;
            }, 2500);
        });
    }
}

// =====================================================
// CREAZIONE OGGETTI FILTRO
// =====================================================

function createInventarioFilterObject() {
    const filterDataDa = document.getElementById("filter-inv-data-da");
    const filterDataA = document.getElementById("filter-inv-data-a");
    const filterItem = document.getElementById("filter-inv-item");
    const filterBarcode = document.getElementById("filter-inv-barcode");
    
    const filteredObject = {};
    
    const user = JSON.parse(getCookie("userInfo"));
    const puUser = JSON.parse(getCookie("pu-User"));
    
    if (user && user.tipoUtente === "Addetto") {
        filteredObject.workerId = user.workerId;
    }
    
    if (puUser && puUser.workerId) {
        filteredObject.workerId = puUser.workerId;
    }
    
    if (filterDataDa?.value) {
        const fromDate = new Date(filterDataDa.value);
        fromDate.setHours(0, 0, 0, 0);
        filteredObject.fromDateTime = fromDate.toISOString().slice(0, -1);
    }
    
    if (filterDataA?.value) {
        const toDate = new Date(filterDataA.value);
        toDate.setHours(23, 59, 59, 999);
        filteredObject.toDateTime = toDate.toISOString().slice(0, -1);
    }
    
    if (filterItem?.value) filteredObject.item = filterItem.value;
    if (filterBarcode?.value) filteredObject.barCode = filterBarcode.value;
    
    return filteredObject;
}

function createOreFilterObject() {
    const filterDataDa = document.getElementById("filter-ore-data-da");
    const filterDataA = document.getElementById("filter-ore-data-a");
    const filterCommessa = document.getElementById("filter-ore-commessa");
    const filterLavorazione = document.getElementById("filter-ore-lavorazione");
    const filterOdp = document.getElementById("filter-ore-odp");
    
    const filteredObject = {};
    
    const user = JSON.parse(getCookie("userInfo"));
    const puUser = JSON.parse(getCookie("pu-User"));
    
    if (user && user.tipoUtente === "Addetto") {
        filteredObject.workerId = user.workerId;
    }
    
    if (puUser && puUser.workerId) {
        filteredObject.workerId = puUser.workerId;
    }
    
    if (filterDataDa?.value) {
        const fromDate = new Date(filterDataDa.value);
        fromDate.setHours(0, 0, 0, 0);
        filteredObject.fromDateTime = fromDate.toISOString().slice(0, -1);
    }
    
    if (filterDataA?.value) {
        const toDate = new Date(filterDataA.value);
        toDate.setHours(23, 59, 59, 999);
        filteredObject.toDateTime = toDate.toISOString().slice(0, -1);
    }
    
    if (filterCommessa?.value) filteredObject.job = filterCommessa.value;
    if (filterLavorazione?.value) filteredObject.operation = filterLavorazione.value;
    if (filterOdp?.value) filteredObject.mono = filterOdp.value;
    
    return filteredObject;
}

function createPrelieviFilterObject() {
    const filterDataDa = document.getElementById("filter-prel-data-da");
    const filterDataA = document.getElementById("filter-prel-data-a");
    const filterCommessa = document.getElementById("filter-prel-commessa");
    const filterLavorazione = document.getElementById("filter-prel-lavorazione");
    const filterOdp = document.getElementById("filter-prel-odp");
    const filterItem = document.getElementById("filter-prel-item");
    const filterBarcode = document.getElementById("filter-prel-barcode");
    
    const filteredObject = {};
    
    const user = JSON.parse(getCookie("userInfo"));
    const puUser = JSON.parse(getCookie("pu-User"));
    
    if (user && user.tipoUtente === "Addetto") {
        filteredObject.workerId = user.workerId;
    }
    
    if (puUser && puUser.workerId) {
        filteredObject.workerId = puUser.workerId;
    }
    
    if (filterDataDa?.value) {
        const fromDate = new Date(filterDataDa.value);
        fromDate.setHours(0, 0, 0, 0);
        filteredObject.fromDateTime = fromDate.toISOString().slice(0, -1);
    }
    
    if (filterDataA?.value) {
        const toDate = new Date(filterDataA.value);
        toDate.setHours(23, 59, 59, 999);
        filteredObject.toDateTime = toDate.toISOString().slice(0, -1);
    }
    
    if (filterCommessa?.value) filteredObject.job = filterCommessa.value;
    if (filterLavorazione?.value) filteredObject.operation = filterLavorazione.value;
    if (filterOdp?.value) filteredObject.mono = filterOdp.value;
    if (filterItem?.value) filteredObject.component = filterItem.value;
    if (filterBarcode?.value) filteredObject.barCode = filterBarcode.value;
    
    return filteredObject;
}

// =====================================================
// FETCH FUNCTIONS
// =====================================================

// Inventario
async function fetchAllInventarioNotImported() {
    if (globalInventarioData) return globalInventarioData;
    try {
        const request = await fetchWithAuth(getApiUrl("api/inventario/get_inventario_not_imported"), {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta inventario:", request?.status, request?.statusText);
            return [];
        }

        globalInventarioData = await request.json();
        console.log("Dati inventario caricati non importati:", globalInventarioData);
        return globalInventarioData;
    } catch (error) {
        console.error("Errore durante la fetch inventario:", error);
        return [];
    }
}

async function fetchInventario(filteredObject) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/inventario/view_inventario/not_imported/filtered"), {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(filteredObject)
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta inventario:", request?.status, request?.statusText);
            return [];
        }

        return await request.json();
    } catch (error) {
        console.error("Errore durante la fetch inventario:", error);
        return [];
    }
}

// Ore
async function fetchAllOreNotImported() {
    if (globalOreData) return globalOreData;
    try {
        const request = await fetchWithAuth(getApiUrl("api/reg_ore/view_ore/not_imported"), {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta ore:", request?.status, request?.statusText);
            return [];
        }
        globalOreData = await request.json();
        console.log("Dati ore caricati non importati:", globalOreData);
        return globalOreData;
    } catch (error) {
        console.error("Errore durante la fetch ore:", error);
        return [];
    }
}

async function fetchOre(filteredObject) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/reg_ore/view_ore/not_imported/filtered"), {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(filteredObject)
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta ore:", request?.status, request?.statusText);
            return [];
        }
        
        return await request.json();
    } catch (error) {
        console.error("Errore durante la fetch ore:", error);
        return [];
    }
}

// Prelievi
async function fetchAllPrelieviNotImported() {
    if (globalPrelieviData) return globalPrelieviData;
    try {
        const request = await fetchWithAuth(getApiUrl("api/prel_mat/view_prel_mat/not_imported"), {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta prelievi:", request?.status, request?.statusText);
            return [];
        }
        
        globalPrelieviData = await request.json();
        console.log("Dati prelievi caricati non importati:", globalPrelieviData);
        return globalPrelieviData;
    } catch (error) {
        console.error("Errore durante la fetch prelievi:", error);
        return [];
    }
}

async function fetchPrelievi(filteredObject) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/prel_mat/view_prel_mat/not_imported/filtered"), {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(filteredObject)
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta prelievi:", request?.status, request?.statusText);
            return [];
        }
        
        return await request.json();
    } catch (error) {
        console.error("Errore durante la fetch prelievi:", error);
        return [];
    }
}

// =====================================================
// REFRESH AUTOCOMPLETE DATA
// =====================================================

async function refreshAllAutocompleteData() {
    switch (activeTab) {
        case 'inventario':
            await refreshInventarioAutocomplete();
            break;
        case 'ore':
            await refreshOreAutocomplete();
            break;
        case 'prelievi':    
            await refreshPrelieviAutocomplete();
            break;
        case 'global':
            break; 
        default:
            console.error("Tab non riconosciuta per il refresh autocomplete:", activeTab);
            return;
    }
}

async function refreshInventarioAutocomplete() {
    const filteredObject = createInventarioFilterObject();
    // const tempData = await fetchInventario(filteredObject);
    
    const itemInput = document.getElementById("filter-inv-item");
    const barcodeInput = document.getElementById("filter-inv-barcode");
    const itemAutocomplete = document.getElementById("filter-inv-item-autocomplete-list");
    const barcodeAutocomplete = document.getElementById("filter-inv-barcode-autocomplete-list");

    var tempData;
    if(Object.keys(filteredObject).length === 0) {
        tempData = await fetchAllInventarioNotImported();
    }
    else {
        tempData = await fetchInventario(filteredObject);
    }
    if (!tempData || tempData.length === 0) {
        tempData = await fetchAllInventarioNotImported();
    }

    inventarioLists.itemList = extractUniqueValues(tempData, 'item');
    console.log("Lista componenti inventario:", inventarioLists.itemList);
    inventarioLists.barcodeList = extractUniqueValues(tempData, 'barCode');
    console.log("Lista barcode inventario:", inventarioLists.barcodeList);
    
    if (itemInput && itemAutocomplete) {
        setupAutocomplete(itemInput, itemAutocomplete, inventarioLists.itemList);
    }
    if (barcodeInput && barcodeAutocomplete) {
        setupAutocomplete(barcodeInput, barcodeAutocomplete, inventarioLists.barcodeList);
    }
}

async function refreshOreAutocomplete() {
    const filteredObject = createOreFilterObject();
    const tempData = await fetchOre(filteredObject);
    
    oreLists.commessaList = extractUniqueValues(tempData, 'job');
    oreLists.lavorazioneList = extractUniqueValues(tempData, 'operation');
    oreLists.odpList = extractUniqueValues(tempData, 'mono');
    
    const commessaInput = document.getElementById("filter-ore-commessa");
    const lavorazioneInput = document.getElementById("filter-ore-lavorazione");
    const odpInput = document.getElementById("filter-ore-odp");
    const commessaAutocomplete = document.getElementById("filter-ore-commessa-autocomplete-list");
    const lavorazioneAutocomplete = document.getElementById("filter-ore-lavorazione-autocomplete-list");
    const odpAutocomplete = document.getElementById("filter-ore-odp-autocomplete-list");
    
    if (commessaInput && commessaAutocomplete) {
        setupAutocomplete(commessaInput, commessaAutocomplete, oreLists.commessaList);
    }
    if (lavorazioneInput && lavorazioneAutocomplete) {
        setupAutocomplete(lavorazioneInput, lavorazioneAutocomplete, oreLists.lavorazioneList);
    }
    if (odpInput && odpAutocomplete) {
        setupAutocomplete(odpInput, odpAutocomplete, oreLists.odpList);
    }
}

async function refreshPrelieviAutocomplete() {
    const filteredObject = createPrelieviFilterObject();
    const tempData = await fetchPrelievi(filteredObject);
    
    prelieviLists.commessaList = extractUniqueValues(tempData, 'job');
    prelieviLists.lavorazioneList = extractUniqueValues(tempData, 'operation');
    prelieviLists.odpList = extractUniqueValues(tempData, 'mono');
    prelieviLists.itemList = extractUniqueValues(tempData, 'component');
    prelieviLists.barcodeList = extractUniqueValues(tempData, 'barCode');
    
    const commessaInput = document.getElementById("filter-prel-commessa");
    const lavorazioneInput = document.getElementById("filter-prel-lavorazione");
    const odpInput = document.getElementById("filter-prel-odp");
    const itemInput = document.getElementById("filter-prel-item");
    const barcodeInput = document.getElementById("filter-prel-barcode");
    
    const commessaAutocomplete = document.getElementById("filter-prel-commessa-autocomplete-list");
    const lavorazioneAutocomplete = document.getElementById("filter-prel-lavorazione-autocomplete-list");
    const odpAutocomplete = document.getElementById("filter-prel-odp-autocomplete-list");
    const itemAutocomplete = document.getElementById("filter-prel-item-autocomplete-list");
    const barcodeAutocomplete = document.getElementById("filter-prel-barcode-autocomplete-list");
    
    if (commessaInput && commessaAutocomplete) {
        setupAutocomplete(commessaInput, commessaAutocomplete, prelieviLists.commessaList);
    }
    if (lavorazioneInput && lavorazioneAutocomplete) {
        setupAutocomplete(lavorazioneInput, lavorazioneAutocomplete, prelieviLists.lavorazioneList);
    }
    if (odpInput && odpAutocomplete) {
        setupAutocomplete(odpInput, odpAutocomplete, prelieviLists.odpList);
    }
    if (itemInput && itemAutocomplete) {
        setupAutocomplete(itemInput, itemAutocomplete, prelieviLists.itemList);
    }
    if (barcodeInput && barcodeAutocomplete) {
        setupAutocomplete(barcodeInput, barcodeAutocomplete, prelieviLists.barcodeList);
    }
}

// =====================================================
// POPULATE LISTS
// =====================================================

async function populateInventarioList(data) {
    const inventarioList = document.getElementById("inventario-list");
    const noContent = document.getElementById("inv-nocontent");
    let paginationControls = document.querySelector('.pagination-controls');
    
    inventarioList.innerHTML = "";
    noContent.classList.add("hidden");
    
    if (!data || data.length === 0) {
        inventarioList.classList.add("hidden");
        noContent.classList.remove("hidden");
        if (paginationControls) {
            paginationControls.classList.add("hidden");
        }
        return;
    }
    
    inventarioList.classList.remove("hidden");
    noContent.classList.add("hidden");
    
    await Promise.all(data.map(async (item) => {
        const li = document.createElement("li");
        li.dataset.id = item.invId;
        
        const savedDate = new Date(item.savedDate);
        const formattedDate = savedDate.toLocaleDateString("it-IT");
        
        const itemContent = document.createElement("div");
        itemContent.className = "item-content";
        
        const isImported = item.imported === false || item.imported === "0" ? false : true;
        
        const statusIndicator = document.createElement("div");
        statusIndicator.className = `status-indicator ${isImported ? 'status-closed' : 'status-open'}`;
        statusIndicator.title = isImported ? 'Importato' : 'Modificabile';
        itemContent.appendChild(statusIndicator);
        
        const parsedDateTime = parseDateTime(item.dataImp);
        
        itemContent.innerHTML += `
            <div><strong>Barcode:</strong> ${item.barCode} </div>
            <div><strong>Item:</strong> ${item.item} </div>
            <div><strong>Operatore:</strong> ${item.workerId} </div>
            <div><strong>Data:</strong> ${formattedDate} </div>
            <div><strong>Qta: <span class="inv-value" id="inv-value-${item.invId}">${item.bookInv}</strong></span> </div>
        `;
        
        li.appendChild(itemContent);
        inventarioList.appendChild(li);
    }));
    
    if (paginationControls) {
        paginationControls.remove();
    }
    
    const pagination = createPagination("#inventario-list");
    if (pagination) {
        pagination.updatePaginationControls();
    }
    
    if (data.length < 6) {
        const paginationContainer = document.querySelector('.pagination-controls');
        if (paginationContainer) {
            paginationContainer.classList.add('hidden');
        }
    }
}

async function populateOreList(data) {
    const oreList = document.getElementById("ore-list");
    const noContent = document.getElementById("ore-nocontent");
    let paginationControls = document.querySelector('.pagination-controls');
    
    oreList.innerHTML = "";
    noContent.classList.add("hidden");
    
    if (!data || data.length === 0) {
        oreList.classList.add("hidden");
        noContent.classList.remove("hidden");
        if (paginationControls) {
            paginationControls.classList.add("hidden");
        }
        return;
    }
    
    oreList.classList.remove("hidden");
    noContent.classList.add("hidden");
    
    await Promise.all(data.map(async (item) => {
        const li = document.createElement("li");
        li.dataset.id = item.oreId;
        
        const savedDate = new Date(item.savedDate);
        const formattedDate = savedDate.toLocaleDateString("it-IT");
        
        const itemContent = document.createElement("div");
        itemContent.className = "item-content";
        
        const isImported = item.imported === false || item.imported === "0" ? false : true;
        
        const statusIndicator = document.createElement("div");
        statusIndicator.className = `status-indicator ${isImported ? 'status-closed' : 'status-open'}`;
        statusIndicator.title = isImported ? 'Importato' : 'Modificabile';
        itemContent.appendChild(statusIndicator);
        
        const parsedDateTime = parseDateTime(item.dataImp);
        
        itemContent.innerHTML += `
            <div><strong>Comm:</strong> ${item.job} </div>
            <div><strong>Lav:</strong> ${item.operation} </div> 
            <div><strong>ODP:</strong> ${item.mono} </div>
            <div><strong>Operatore:</strong> ${item.workerId} </div>
            <div><strong>Data:</strong> ${formattedDate} </div>
            ${item.closed === false ? `<div><strong>Ore: <span class="ore-value" id="ore-value-${item.regOreId}">${item.workingTime/3600}</strong></span> </div>` 
                : '<div><span class="closed-indicator">Chiusura Commessa</span></div>'}
        `;

        if(item.closed === true)
        {
            itemContent.classList.add("close-reg-ore-item");
        }
        
        li.appendChild(itemContent);
        oreList.appendChild(li);
    }));
    
    if (paginationControls) {
        paginationControls.remove();
    }
    
    const pagination = createPagination("#ore-list");
    if (pagination) {
        pagination.updatePaginationControls();
    }
    if (data.length < 6) {
        const paginationContainer = document.querySelector('.pagination-controls');
        if (paginationContainer) {
            paginationContainer.classList.add('hidden');
        }
    }
}

async function populatePrelieviList(data) {
    const prelieviList = document.getElementById("prelievi-list");
    const noContent = document.getElementById("prelievi-nocontent");
    let paginationControls = document.querySelector('.pagination-controls');
    
    prelieviList.innerHTML = "";
    noContent.classList.add("hidden");
    
    if (!data || data.length === 0) {
        prelieviList.classList.add("hidden");
        noContent.classList.remove("hidden");
        if (paginationControls) {
            paginationControls.classList.add("hidden");
        }
        return;
    }
    
    prelieviList.classList.remove("hidden");
    noContent.classList.add("hidden");
    
    await Promise.all(data.map(async (item) => {
        const li = document.createElement("li");
        li.dataset.id = item.prelId;
        
        const savedDate = new Date(item.savedDate);
        const formattedDate = savedDate.toLocaleDateString("it-IT");
        
        const itemContent = document.createElement("div");
        itemContent.className = "item-content";
        
        const isImported = item.imported === false || item.imported === "0" ? false : true;
        
        const statusIndicator = document.createElement("div");
        statusIndicator.className = `status-indicator ${isImported ? 'status-closed' : 'status-open'}`;
        statusIndicator.title = isImported ? 'Importato' : 'Modificabile';
        itemContent.appendChild(statusIndicator);
        
        const parsedDateTime = parseDateTime(item.dataImp);
        
        itemContent.innerHTML += `
            <div><strong>Barcode:</strong> ${item.barCode} </div>
            <div><strong>Item:</strong> ${item.component} </div>
            <div><strong>Comm:</strong> ${item.job} </div>
            <div><strong>Lav:</strong> ${item.operation} </div>
            <div><strong>ODP:</strong> ${item.mono} </div>
            <div><strong>Operatore:</strong> ${item.workerId} </div>
            <div><strong>Data:</strong> ${formattedDate} </div>
            <div><strong>Qta: <span class="prel-value" id="prel-value-${item.prelMatId}">${item.prelQty}</strong></span> </div>
        `;

        // if(data.position === 0)
        // {
        //     newItem.classList.add("new-prel-item");
        // }
        // if(data.deleted === true)
        // {
        //     newItem.classList.add("deleted-prel-item");
        // }

        if(item.position === 0) {
            itemContent.classList.add("new-prel-item");
        }
        if(item.deleted === true) {
            itemContent.classList.add("deleted-prel-item");
        }
        
        li.appendChild(itemContent);
        prelieviList.appendChild(li);
    }));
    
    if (paginationControls) {
        paginationControls.remove();
    }
    
    const pagination = createPagination("#prelievi-list");
    if (pagination) {
        pagination.updatePaginationControls();
    }
    
    if (data.length < 6) {
        const paginationContainer = document.querySelector('.pagination-controls');
        if (paginationContainer) {
            paginationContainer.classList.add('hidden');
        }
    }
}

// =====================================================
// UTILITY FUNCTIONS
// =====================================================

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

// =====================================================
// SYNC DATA FUNCTION
// =====================================================
async function synchronizeData() {
    const syncButton = document.getElementById("sync-data");
    const iconElement = syncButton.querySelector(".button-icon");
    const goBackButton = document.getElementById("sync-go-back");
    const originalIcon = iconElement.className;
    const userCookie = JSON.parse(getCookie("userInfo"));
    console.log("User cookie:", userCookie);
    
    goBackButton.disabled = true;

    // Inizia l'animazione di caricamento
    startLoadingAnimation(iconElement, "2rem");
    
    // Garantisce un minimo di tempo di caricamento (1.5 secondi)
    //const minLoadingTime = new Promise(resolve => setTimeout(resolve, 2000));
    
    try {
        console.log("Sincronizzazione dei dati...");
        console.log("User ID:", userCookie.workerId);
        // Esegui simultaneamente la richiesta e il timer di caricamento minimo
        const response = await fetchWithAuth(getApiUrl("api/mago_api/synchronize_all"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "workerId": userCookie.workerId,
                })
            });

        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));

        if (response.ok) {
            console.log("Dati sincronizzati correttamente:", response.statusText);
            const data = await response.json();
            console.log("Response DTO:", data);
            // Mostra l'icona di successo
            showSuccessIcon(iconElement);

            setTimeout(() => {
                if (data && data.inventarioRequest.length === 0 && data.regOreRequest.length === 0 && data.prelMatRequest.length === 0) {
                    alert("Nessun dato da sincronizzare.");
                }
            }, 700);
        } else {
            console.error("Errore durante la sincronizzazione dei dati:", response.statusText);
            // Mostra l'icona di errore
            showErrorIcon(iconElement);
        }
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);

            loadInitialData();
        }, 2000);
    } catch (error) {
        console.error("Network error:", error);
        
        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));
        
        // Mostra l'icona di errore
        showErrorIcon(iconElement);
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
        }, 2000);
    }
    finally
    {
        goBackButton.disabled = false;

        // Ricarica i dati iniziali dopo la sincronizzazione
        await loadInitialData();
    }
}

async function syncRegOreFiltered() {
    const syncButton = document.getElementById("sync-ore-data");
    const iconElement = syncButton.querySelector(".button-icon");
    const filterDataDa = document.getElementById("filter-ore-data-da");
    const filterDataA = document.getElementById("filter-ore-data-a");
    const filterCommessa = document.getElementById("filter-ore-commessa");
    const filterLavorazione = document.getElementById("filter-ore-lavorazione");
    const filterOdp = document.getElementById("filter-ore-odp");
    const goBackButton = document.getElementById("ore-go-back");
    const filterButton = document.getElementById("filter-ore-submit");
    const originalIcon = iconElement.className;
    const userCookie = JSON.parse(getCookie("userInfo"));
    console.log("elementi filtrati:", filteredOreList);

    filterDataA.disabled = true;
    filterDataDa.disabled = true;
    filterCommessa.disabled = true;
    filterLavorazione.disabled = true;
    filterOdp.disabled = true;
    goBackButton.disabled = true;
    filterButton.disabled = true;

    startLoadingAnimation(iconElement, "1.2rem");

    try {
        console.log("Sincronizzazione delle ore registrate...");
        console.log("User ID:", userCookie.workerId);
        // Esegui simultaneamente la richiesta e il timer di caricamento minimo
        const response = await fetchWithAuth(getApiUrl("api/mago_api/sync_reg_ore_filtered"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "WorkerIdSyncRequestDto" : {"workerId": userCookie.workerId},
                    "RegOreList": filteredOreList
                })
            });

        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));

        if (response.ok) {
            console.log("Ore registrate sincronizzate correttamente:", response.statusText);
            const data = await response.json();
            console.log("Response DTO:", data);
            // Mostra l'icona di successo
            showSuccessIcon(iconElement);
            // Ripristina i campi di filtro
            if (filterDataDa) filterDataDa.value = "";
            if (filterDataA) filterDataA.value = "";
            if (filterCommessa) filterCommessa.value = "";
            if (filterLavorazione) filterLavorazione.value = "";
            if (filterOdp) filterOdp.value = "";
            filteredOreList = []; // Pulisci la lista filtrata
            oreLists = []; // Pulisci la lista visualizzata
            globalOreData = null; // Pulisci i dati globali delle ore
        } else {
            console.error("Errore durante la sincronizzazione dei dati:", response.statusText);
            // Mostra l'icona di errore
            showErrorIcon(iconElement);
        }
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
            loadInitialData();
        }, 2000);
    } catch (error) {
        console.error("Network error:", error);
        
        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));
        
        // Mostra l'icona di errore
        showErrorIcon(iconElement);
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
        }, 2000);
    }
    finally {
        filterDataA.disabled = false;
        filterDataDa.disabled = false;
        filterCommessa.disabled = false;
        filterLavorazione.disabled = false;
        filterOdp.disabled = false;
        goBackButton.disabled = false;
        filterButton.disabled = false;
    }
}

async function syncPrelieviFiltered() { 
    const syncButton = document.getElementById("sync-prel-data");
    const iconElement = syncButton.querySelector(".button-icon");
    const filterDataDa = document.getElementById("filter-prel-data-da");
    const filterDataA = document.getElementById("filter-prel-data-a");
    const itemInput = document.getElementById("filter-prel-item");
    const barcodeInput = document.getElementById("filter-prel-barcode");
    const filterCommessa = document.getElementById("filter-prel-commessa");
    const filterLavorazione = document.getElementById("filter-prel-lavorazione");
    const filterOdp = document.getElementById("filter-prel-odp");
    const goBackButton = document.getElementById("prel-go-back");
    const filterButton = document.getElementById("filter-prel-submit");
    const originalIcon = iconElement.className;
    const userCookie = JSON.parse(getCookie("userInfo"));
    console.log("elementi filtrati:", filteredPrelieviList);

    filterDataA.disabled = true;
    filterDataDa.disabled = true;
    itemInput.disabled = true;
    barcodeInput.disabled = true;
    filterCommessa.disabled = true;
    filterLavorazione.disabled = true;
    filterOdp.disabled = true;
    goBackButton.disabled = true;
    filterButton.disabled = true;

    startLoadingAnimation(iconElement, "1.2rem");

    try {
        console.log("Sincronizzazione dei prelievi...");
        console.log("User ID:", userCookie.workerId);
        // Esegui simultaneamente la richiesta e il timer di caricamento minimo
        const response = await fetchWithAuth(getApiUrl("api/mago_api/sync_prel_mat_filtered"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "WorkerIdSyncRequestDto" : {"workerId": userCookie.workerId},
                    "PrelMatList": filteredPrelieviList
                })
            });

        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));

        if (response.ok) {
            console.log("Prelievi sincronizzati correttamente:", response.statusText);
            const data = await response.json();
            console.log("Response DTO:", data);
            // Mostra l'icona di successo
            showSuccessIcon(iconElement);
            // Ripristina i campi di filtro
            if (filterDataDa) filterDataDa.value = "";
            if (filterDataA) filterDataA.value = "";
            if (itemInput) itemInput.value = "";
            if (barcodeInput) barcodeInput.value = "";
            if (filterCommessa) filterCommessa.value = "";
            if (filterLavorazione) filterLavorazione.value = "";
            if (filterOdp) filterOdp.value = "";
            filteredPrelieviList = []; // Pulisci la lista filtrata
            prelieviLists = []; // Pulisci la lista visualizzata
            globalPrelieviData = null; // Pulisci i dati globali dei prelievi
        } else {
            console.error("Errore durante la sincronizzazione dei dati:", response.statusText);
            // Mostra l'icona di errore
            showErrorIcon(iconElement);
        }
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
            loadInitialData();
        }, 2000);
    } catch (error) {
        console.error("Network error:", error);
        
        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));
        
        // Mostra l'icona di errore
        showErrorIcon(iconElement);
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
        }, 2000);
    }
    finally {
        filterDataA.disabled = false;
        filterDataDa.disabled = false;
        itemInput.disabled = false;
        barcodeInput.disabled = false;
        filterCommessa.disabled = false;
        filterLavorazione.disabled = false;
        filterOdp.disabled = false;
        goBackButton.disabled = false;
        filterButton.disabled = false;
    }
}

async function syncInventarioFiltered() {
    const syncButton = document.getElementById("sync-inv-data");
    const iconElement = syncButton.querySelector(".button-icon");
    const filterDataDa = document.getElementById("filter-inv-data-da");
    const filterDataA = document.getElementById("filter-inv-data-a");
    const itemInput = document.getElementById("filter-inv-item");
    const barcodeInput = document.getElementById("filter-inv-barcode");
    const goBackButton = document.getElementById("inv-go-back");
    const filterButton = document.getElementById("filter-inv-submit");
    const originalIcon = iconElement.className;
    const userCookie = JSON.parse(getCookie("userInfo"));
    console.log("elementi filtrati:", filteredInventarioList);
    console.log("Dati da inviare:", globalInventarioData);

    barcodeInput.disabled = true; 
    itemInput.disabled = true;
    filterDataDa.disabled = true;
    filterDataA.disabled = true;
    goBackButton.disabled = true;
    filterButton.disabled = true;
    
    startLoadingAnimation(iconElement, "1.2rem");
    
    

    try {
        console.log("Sincronizzazione delle movimentazioni di inventario...");
        console.log("User ID:", userCookie.workerId);
        console.log("Inventario da sincronizzare:", filteredInventarioList);
        // Esegui simultaneamente la richiesta e il timer di caricamento minimo
        const response = await fetchWithAuth(getApiUrl("api/mago_api/sync_inventario_filtered"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "WorkerIdSyncRequestDto" : {"workerId": userCookie.workerId},
                    "InventarioList": filteredInventarioList
                })
            });

        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));

        if (response.ok) {
            console.log("Inventario sincronizzato correttamente:", response.statusText);
            const data = await response.json();
            console.log("Response DTO:", data);
            // Mostra l'icona di successo
            showSuccessIcon(iconElement);
            // Ripristina i campi di filtro
            if (filterDataDa) filterDataDa.value = "";
            if (filterDataA) filterDataA.value = "";
            if (itemInput) itemInput.value = "";
            if (barcodeInput) barcodeInput.value = "";
            filteredInventarioList = []; // Pulisci la lista filtrata
            inventarioLists = []; // Pulisci la lista visualizzata
            globalInventarioData = null; // Pulisci i dati globali dell'inventario
        } else {
            console.error("Errore durante la sincronizzazione dei dati:", response.statusText);
            // Mostra l'icona di errore
            showErrorIcon(iconElement);
        }
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
            loadInitialData();
        }, 2000);
    }
    catch (error) {
        console.error("Network error:", error);
        
        // Rimuovi l'animazione di caricamento con effetto
        iconElement.classList.remove("sync-loading");
        iconElement.classList.add("sync-shrink-out");
        
        // Attendi la fine dell'animazione di riduzione
        await new Promise(resolve => setTimeout(resolve, 300));
        
        // Mostra l'icona di errore
        showErrorIcon(iconElement);
        
        // Ripristina l'icona originale dopo 2 secondi
        setTimeout(() => {
            resetIcon(iconElement, originalIcon);
        }, 2000);
    }
    finally {
        barcodeInput.disabled = false; 
        itemInput.disabled = false;
        filterDataDa.disabled = false;
        filterDataA.disabled = false;
        goBackButton.disabled = false;
        filterButton.disabled = false;
    }
}

/**
 * Avvia l'animazione di rotazione dell'icona durante il caricamento
 * @param {HTMLElement} iconElement - L'elemento icona da animare
 * @param {string} fontSize - La dimensione del font da applicare all'icona durante l'animazione
 */
function startLoadingAnimation(iconElement, fontSize = "2rem") {
    // Rimuovi tutte le classi esistenti e aggiungi fa-arrows-rotate per sicurezza
    iconElement.className = "fa-solid fa-arrows-rotate menu-icon";
    iconElement.style.fontSize = fontSize; // Imposta la dimensione del font
    // Aggiungi la classe per l'animazione di rotazione
    iconElement.classList.add("sync-loading");
}

/**
 * Mostra l'icona di successo (check)
 * @param {HTMLElement} iconElement - L'elemento icona da modificare
 */
function showSuccessIcon(iconElement) {
    // Imposta l'icona di spunta e applica l'animazione
    iconElement.className = "fa-solid fa-check menu-icon";
    iconElement.classList.add("sync-success");
}

/**
 * Mostra l'icona di errore (X)
 * @param {HTMLElement} iconElement - L'elemento icona da modificare
 */
function showErrorIcon(iconElement) {
    // Imposta l'icona X e applica l'animazione
    iconElement.className = "fa-solid fa-times menu-icon";
    iconElement.classList.add("sync-error");
}

/**
 * Ripristina l'icona originale
 * @param {HTMLElement} iconElement - L'elemento icona da ripristinare
 * @param {string} originalIcon - La classe originale dell'icona
 */
function resetIcon(iconElement, originalIcon) {
    iconElement.className = originalIcon;
    iconElement.style.color = ""; // Ripristina il colore predefinito
}