import { fetchWithAuth } from "./fetch.js";
import { getCookie } from "./cookies.js";
import { setupAutocomplete } from "./autocomplete.js";
import { extractUniqueValues, getApiUrl } from "./main.js";

let globalAllData = null;

document.addEventListener("DOMContentLoaded", async function () {
    // Recupera elementi DOM

    // Input e liste di autocompletamento
    const commessaInput = document.getElementById("prel-mat-commessa");
    const commessaAutocompleteList = document.getElementById("prel-mat-commessa-autocomplete-list");
    const lavorazioneInput = document.getElementById("prel-mat-lavorazione");
    const lavorazioneAutocompleteList = document.getElementById("prel-mat-lavorazione-autocomplete-list");
    const odlInput = document.getElementById("prel-mat-odp");
    const odlAutocompleteList = document.getElementById("prel-mat-odp-autocomplete-list");
    const barcodeInput = document.getElementById("prel-mat-barcode");
    const barcodeAutocompleteList = document.getElementById("prel-mat-barcode-autocomplete-list");
    const quantitaInput = document.getElementById("prel-mat-quantita");
    
    // Tabella di ricerca commesse
    const cercaButton = document.getElementById("prel-mat-cerca");
    const searchOverlay = document.getElementById("search-overlay");
    const closeSearchButton = document.getElementById("close-search-overlay");
    const cancelSearchButton = document.getElementById("cancel-search");
    const selectSearchResultButton = document.getElementById("select-search-result");
    const searchResultsBody = document.getElementById("search-results-body");
    
    // Tabella di ricerca materiali
    const searchMaterialsOverlay = document.getElementById("search-materials-overlay");
    const closeMaterialSearchButton = document.getElementById("close-search-overlay-materials");
    const cancelMaterialSearchButton = document.getElementById("cancel-search-materials");
    const selectMaterialSearchResultButton = document.getElementById("select-search-result-materials");
    const searchMaterialsInput = document.getElementById("search-materials-input");
    const searchMaterialsButton = document.getElementById("search-materials-button");
    const cancelSearchMaterialsInputButton = document.getElementById("cancel-search-materials-input");
    const searchResultsMaterialsBody = document.getElementById("search-results-materials-body");

    // Overlay per la conferma della quantità di materiale da aggiungere
    const materialQtyOverlay = document.getElementById("material-qty-overlay");
    const addMaterialQtyOverlayButton = document.getElementById("add-material-qty-button");
    const cancelMaterialQtyOverlayButton = document.getElementById("cancel-material-qty-button");
    const materialOverlayInput = document.getElementById("material-qty-input");
    
    // Pulsanti
    const addButton = document.getElementById("inv-add-list");
    const saveButton = document.getElementById("inv-save");
    const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
    const eliminaArticoloButton = document.getElementById("prel-mat-elimina-articolo");
    const aggiungiArticoloButton = document.getElementById("prel-mat-aggiungi-articolo");
    
    // Aggiunto quando viene salvata la lista viene
    const noContent = document.getElementById("nocontent");
    const errorQty = document.getElementById("error-quantity");

    // Liste di dati
    let jobList = [];
    let odpList = [];
    let lavorazioneList = [];
    let barcodeList = [];
    let searchResults = [];
    let selectedSearchRow = null;
    let selectedMaterialSearchRow = null;
    let materialsSearchResults = [];
    let isMaterialResultsFetched = false;
    let dataResultList = [];

    let isFillingFromOverlay = false;
    let isAddingNewItem = false;
    let isDelitingItem = false;

    // Inizializza l'autocompletamento per la commessa e carica i dati iniziali
    try {
        const jobResult = await fetchAllJobs();
        jobList = jobResult
            .filter(job => job && job.job && job.description)
            .map(job => ({
                job: job.job,
                description: job.description,
                display: `${job.job} - ${job.description}`
            }));
        // console.log("Lista di lavori:", jobList);
        setupAutocomplete(commessaInput, commessaAutocompleteList, jobList);
        commessaInput.focus();
        aggiungiArticoloButton.classList.add("disabled-button-look");
        eliminaArticoloButton.classList.add("disabled-button-look");
    } catch (error) {
        console.error("Errore nel caricamento iniziale dei lavori:", error);
        alert("Qualcosa è andato storto durante il caricamento iniziale dei dati.");
    }

    // Event listener per il cambio di commessa
    commessaInput.addEventListener("change", async function() {
        // Resetta i campi dipendenti Ordine di Lavoro e Lavorazione
        
        if (isFillingFromOverlay)
        {
            odlInput.value = "";
            odlInput.disabled = true;
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
        
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        if (selectedCommessa) {
            await loadOdpData(selectedCommessa.job);
        }
        else {
            odlInput.value = "";
            odlInput.disabled = true;
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
    });

    commessaInput.addEventListener("focusout", function() {
        // Resetta i campi successivi se il campo commessa è vuoto
        if (commessaInput.value === "") {
            odlInput.value = "";
            odlInput.disabled = true;
            odlInput.value = "";
            odlAutocompleteList.innerHTML = "";
            odlAutocompleteList.classList.add("hidden");
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            lavorazioneList = [];
            lavorazioneAutocompleteList.innerHTML = ""; 
            lavorazioneAutocompleteList.classList.add("hidden");
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            barcodeList = [];
            barcodeAutocompleteList.innerHTML = "";
            barcodeAutocompleteList.classList.add("hidden");
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look"); 
            quantitaInput.disabled = true; 
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            return;
        }
    });

    odlInput.addEventListener("change", async function() {
    
        if (!isFillingFromOverlay)
        {
            // Resetta il campo lavorazione se i dati non sono validi
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true; 
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
    
        // Carica i dati della lavorazione solo se necessario
        // Serve in caso di selezione dalla tabella in overlay
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        if (selectedCommessa && selectedOdp) {
            await loadLavorazioneData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate);
        }
        else {
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true; 
            lavorazioneList = [];
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            barcodeList = [];
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
    });

    odlInput.addEventListener("focusout", function() {
        // Resetta i campi successivi se il campo ordine di produzione è vuoto
        if (odlInput.value === "") {
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            lavorazioneList = [];
            lavorazioneAutocompleteList.innerHTML = ""; 
            lavorazioneAutocompleteList.classList.add("hidden");
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            barcodeList = [];
            barcodeAutocompleteList.innerHTML = "";
            barcodeAutocompleteList.classList.add("hidden");
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            quantitaInput.disabled = true; 
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            return;
        }
    });

    // Event listener per il cambio di lavorazione
    lavorazioneInput.addEventListener("change", async function() {
    
        if (!isFillingFromOverlay)
        {
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            aggiungiArticoloButton.classList.remove("disabled-button-look");
        }

        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);

        if (selectedCommessa && selectedOdp && selectedLavorazione) {
            await loadBarCodeData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation);
            console.log("Lista barcode:", barcodeList);
        }
    });

    lavorazioneInput.addEventListener("focusout", function() {
        // Resetta il campo barcode se il campo lavorazione è vuoto
        if (lavorazioneInput.value === "") {
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            quantitaInput.value = "";
            barcodeList = [];
            barcodeAutocompleteList.innerHTML = ""; // Svuota la lista di autocompletamento
            barcodeAutocompleteList.classList.add("hidden"); // Nasconde la lista di autocompletamento
            quantitaInput.disabled = true; // Disabilita il campo quantità
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            return;
        }
    });

    // Event listener per il cambio di barcode
    barcodeInput.addEventListener("change", async function() {
    //Pulisco il campo quantità quando si cambia il barcode
        quantitaInput.value = "";

        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
        const selectedBarcode = findSelectedItem(barcodeInput.value, barcodeList);
        if (selectedCommessa && selectedOdp && selectedLavorazione && selectedBarcode) {
            if (isFillingFromOverlay) {barcodeAutocompleteList.classList.add("hidden");}
            
            console.log("Commessa selezionata:", selectedCommessa);
            console.log("ODP selezionato:", selectedOdp);
            console.log("Lavorazione selezionata:", selectedLavorazione);
            console.log("Barcode selezionato:", selectedBarcode);
            await loadAllData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation, selectedBarcode.component, selectedBarcode.barCode);
            quantitaInput.disabled = false;
            quantitaInput.focus();
        }
    });

    barcodeInput.addEventListener("focusout", function() {
        if (barcodeInput.value === "") {
            quantitaInput.disabled = true; 
        }
    });

    barcodeInput.addEventListener("keydown", async function(event) {
        if (event.key === "Enter") { 
            event.preventDefault(); 
    
            const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
            const selectedOdp = findSelectedItem(odlInput.value, odpList);
            const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
            const selectedBarcode = barcodeList.find(item => item.barCode === barcodeInput.value.toUpperCase());
            const selectedComponent = barcodeList.find(item => item.component === barcodeInput.value.toUpperCase());
            barcodeInput.value = selectedBarcode ? selectedBarcode.display : "";
    
            if (selectedCommessa && selectedOdp && selectedLavorazione && selectedComponent && selectedBarcode) {
                console.log("Commessa selezionata:", selectedCommessa);
                console.log("ODP selezionato:", selectedOdp);
                console.log("Lavorazione selezionata:", selectedLavorazione);
                console.log("Barcode selezionato:", selectedBarcode);
                console.log("Componente selezionato:", selectedComponent);
                await loadAllData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation, selectedComponent.component, selectedBarcode.barCode);
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.focus();
            }
        }
    });

    quantitaInput.addEventListener("keydown", function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            addButton.click();
        }
    });

    // Event listener per il pulsante Cerca - Ora apre la tabella overlay
    cercaButton.addEventListener("click", async function() {
        // Filtra i risultati in base al valore nel campo commessa
        let filteredResults = [];
        
        if (commessaInput) {
            // Se c'è un valore nel campo commessa, filtra i risultati
            const jobSearchResults = jobList.filter(job => 
                job.display.toLowerCase().includes(commessaInput.value.toLowerCase())
            );

            for(const job of jobSearchResults) {
                const results = await fetchJobMostep(job.job);
                //console.log("Risultati della ricerca:", results);
                filteredResults.push(...results.map(item => ({
                    job: job.job,
                    mono: item.mono || '',
                    creationDate: item.creationDate || '',
                    prelUoM: item.prelUoM || '',
                    prelResQty: item.prelResQty || '',
                    bom: item.bom || '',
                    itemDesc: item.itemDesc || '',
                    operation: item.operation || '',
                    operDesc: item.operDesc || '',
                    component: item.component || '',
                    position: item.position || '',
                    barCode: item.barCode || ''
                })));
            }
        } else {
            for(const job of jobList) {
                const results = await fetchJobMostep(job.job);
                //console.log("Risultati della ricerca:", results);
                filteredResults.push(...results.map(item => ({
                    job: job.job,
                    mono: item.mono || '',
                    creationDate: item.creationDate || '',
                    prelUoM: item.prelUoM || '',
                    prelResQty: item.prelResQty || '',
                    bom: item.bom || '',
                    itemDesc: item.itemDesc || '',
                    operation: item.operation || '',
                    operDesc: item.operDesc || '',
                    component: item.component || '',
                    position: item.position || '',
                    barCode: item.barCode || ''
                })));
            }
        }
        
        searchResults = filteredResults;
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

    // Event listener per il pulsante Seleziona
    selectSearchResultButton.addEventListener("click", function() {
        if (selectedSearchRow) {
            
            isFillingFromOverlay = true; // Imposta il flag per evitare conflitti con gli eventi di input

            // Cerca il job corrispondente nella lista originale per ottenere la description
            const jobItem = jobList.find(job => job.job === selectedSearchRow.job);
            const description = jobItem ? jobItem.description : selectedSearchRow.itemDesc;
            
            // Compila il campo commessa
            commessaInput.value = `${selectedSearchRow.job} - ${description}`;
            
            const commessaEvent = new Event('change', { bubbles: true });
            commessaInput.dispatchEvent(commessaEvent);

            // Compila il campo ODP/mono
            if (selectedSearchRow.mono) {
                odlInput.value = `${selectedSearchRow.mono} - ${selectedSearchRow.creationDate}`;
            } else {
                odlInput.value = ""; // Resetta il campo se non c'è un mono
            }

            if(odlInput.value) {
                setTimeout(() => {
                    const odpEvent = new Event('change', { bubbles: true });
                    odlInput.dispatchEvent(odpEvent);
                    //console.log("Evento change per ODP dispatched");
                }, 300); // Piccolo ritardo per permettere al primo evento di completarsi
            }
            
            // Compila il campo lavorazione
            if (selectedSearchRow.operation) {
                console.log("Lavorazione trovata in overlay:", selectedSearchRow.operation);
                lavorazioneInput.value = `${selectedSearchRow.operation} - ${selectedSearchRow.operDesc} - ${selectedSearchRow.bom}`;
            } else {
                lavorazioneInput.value = ""; // Resetta il campo se non c'è un'operazione
            }

            if(lavorazioneInput.value) {
                setTimeout(() => {
                    const lavorazioneEvent = new Event('change', { bubbles: true });
                    lavorazioneInput.dispatchEvent(lavorazioneEvent);
                    //console.log("Evento change per lavorazione dispatched");
                }, 600);
            }

            if(selectedSearchRow.component) {
                console.log("Component trovato in overlay:", selectedSearchRow.component);
                barcodeInput.value = `Item: ${selectedSearchRow.component} ${selectedSearchRow.barCode === "" || selectedSearchRow.barCode === null ? "" : "- Code: " + selectedSearchRow.barCode} - ${selectedSearchRow.itemDesc === "" || selectedSearchRow.itemDesc === null ? "Nessuna descrizione disponibile" : selectedSearchRow.itemDesc}`;
            }
            else {
                barcodeInput.value = ""; // Resetta il campo se non c'è un barcode
            }

            if(barcodeInput.value) {
                setTimeout(() => {
                    const barcodeEvent = new Event('change', { bubbles: true });
                    barcodeInput.dispatchEvent(barcodeEvent);
                    //console.log("Evento change per barcode dispatched");
                }, 900);
            }

            setTimeout(() => {
                console.log("isFillingFromOverlay:", isFillingFromOverlay);
                isFillingFromOverlay = false; // Ripristina il flag
            }, 1000);

            // Chiude l'overlay
            searchOverlay.classList.remove("active");
            
            // Azzera la selezione alla fine
            selectedSearchRow = null;
        } else {
            alert("Seleziona una riga prima di procedere");
        }
    });

    // Event listener per il pulsante Cerca - Ora apre la tabella overlay
    aggiungiArticoloButton.addEventListener("click", async function() {
        if(aggiungiArticoloButton.classList.contains("disabled-button-look")) {
            alert("Selezionare una commessa, un odp e una lavorazione prima di procedere.");
            return;
        }
        console.log("Apertura overlay di ricerca materiali, isAddingNewItem impostato a true"); // Per Debug
        isAddingNewItem = true; // Imposta qui per chiarezza

        searchMaterialsInput.value = "";
        if(isMaterialResultsFetched == false) {
            materialsSearchResults = [];
            isMaterialResultsFetched = true;
            await defaultTableContent(searchMaterialsOverlay, materialsSearchResults);
        }
        else {
            document.body.classList.add("loading-cursor");

            populateMaterialSearchResults(materialsSearchResults);

            document.body.classList.remove("loading-cursor");

            searchMaterialsOverlay.classList.add("active");
        }
    });

    searchMaterialsInput.addEventListener("keydown", function(event) {
        if (event.key === "Enter") {
            event.preventDefault(); 
            searchMaterialsButton.click();
        }
    });

    searchMaterialsButton.addEventListener("click", async function() {
        const searchTerm = searchMaterialsInput.value.trim().toLowerCase();
        var searchList = materialsSearchResults;
        const filteredResults = searchList.filter(item =>
            item.display.toLowerCase().includes(searchTerm) 
        );
        populateMaterialSearchResults(filteredResults);
    });

    cancelSearchMaterialsInputButton.addEventListener("click", async function() {
        searchMaterialsInput.value = "";
        addMaterialInformation = null;
        if(isMaterialResultsFetched == false) {
            materialsSearchResults = [];
            isMaterialResultsFetched = true;
            await defaultTableContent(searchMaterialsOverlay, materialsSearchResults);
        }
        else {
            setTimeout(() => {
                document.body.classList.add("loading-cursor");
            }, 200);

            searchMaterialsOverlay.classList.add("active");

            populateMaterialSearchResults(materialsSearchResults);

            document.body.classList.remove("loading-cursor");
        }
    });


    // Event listeners per i pulsanti di chiusura dell'overlay
    closeMaterialSearchButton.addEventListener("click", function() {
        searchMaterialsOverlay.classList.remove("active");
        selectedMaterialSearchRow = null;
    });

    cancelMaterialSearchButton.addEventListener("click", function() {
        searchMaterialsOverlay.classList.remove("active");
        selectedMaterialSearchRow = null;
    });

    // Event listener per il pulsante Seleziona
    // Recupera le informazioni della tabella. Aggiunge la quantità per compilare il form
    selectMaterialSearchResultButton.addEventListener("click", async function() {
        if (selectedMaterialSearchRow) {
            console.log("Riga selezionata:", selectedMaterialSearchRow);

            materialQtyOverlay.classList.add("active");
            
            setTimeout(() => {
                materialOverlayInput.value = 0;
                materialOverlayInput.focus();
            }, 200);
        }
    });


    addMaterialQtyOverlayButton.addEventListener("click", async function() {
        const materialQty = materialOverlayInput.value;
        if(materialQty && materialQty <= 0) {
            alert("Inserire una quantità valida maggiore di zero.");
            return;
        }

        if (materialQty && selectedMaterialSearchRow) {
            selectedMaterialSearchRow.neededQty = parseFloat(materialQty);
            console.log("Informazioni aggiuntive per il materiale:", selectedMaterialSearchRow);
        }

        // Compila i campi del form con le informazioni selezionate prima di chiudere l'overlay

        if(selectedMaterialSearchRow.item && selectedMaterialSearchRow.neededQty)
        {
            console.log("Componente trovato in overlay:", selectedMaterialSearchRow.item);
            barcodeInput.value = `Item: ${selectedMaterialSearchRow.item} ${selectedMaterialSearchRow.barCode === "" || selectedMaterialSearchRow.barCode === null ? "" : `- Code: ${selectedMaterialSearchRow.barCode}`} - ${selectedMaterialSearchRow.description === "" || selectedMaterialSearchRow.description === null ? "Nessuna descrizione disponibile" : selectedMaterialSearchRow.description}`;

            const giacenza = await fetchGiacenzeByItem(selectedMaterialSearchRow.item);
            if (giacenza) {
                quantitaLabel.textContent = `Nuova qta. da prelevare: ${selectedMaterialSearchRow.neededQty} - Qta. prelevabile: ${selectedMaterialSearchRow.neededQty} - UoM: ${selectedMaterialSearchRow.uoM} - Giacenza: ${giacenza.bookInv} (${giacenza.uoM})`; 

                quantitaInput.value = selectedMaterialSearchRow.neededQty;
                quantitaInput.disabled = false; // Abilita il campo quantità
                isAddingNewItem = true;
                console.log("isAddingNewItem impostato a true dopo selezione materiale"); // Per Debug
            }
            else {
                alert("Nessuna giacenza disponibile per questo materiale.");
                quantitaInput.value = "";
                quantitaInput.disabled = true; // Disabilita il campo quantità
                isAddingNewItem = false;
            }
        }
        else {
            barcodeInput.value = ""; // Resetta il campo se non c'è un barcode
            alert("Selezionare un materiale prima di procedere.");
        }

        materialQtyOverlay.classList.remove("active");
        searchMaterialsOverlay.classList.remove("active");

        quantitaInput.focus();
        quantitaInput.disabled = false;
    });

    materialOverlayInput.addEventListener("keydown", function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            addMaterialQtyOverlayButton.click();
        }
    });


    cancelMaterialQtyOverlayButton.addEventListener("click", function() {
        materialQtyOverlay.classList.remove("active");
        selectedMaterialSearchRow = null;
    });

    // Pulsante di conferma per la quantità necessaria con invio dei dati a Mago

    function populateMaterialSearchResults(results) {
        searchResultsMaterialsBody.innerHTML = "";
        selectedMaterialSearchRow = null;

        if (results.length === 0) {
            const row = searchResultsMaterialsBody.insertRow();
            const cell = row.insertCell();
            cell.colSpan = 5;
            cell.textContent = "Nessun risultato trovato";
            return;
        }

        results.forEach((result, index) => {
            const row = searchResultsMaterialsBody.insertRow();
            row.dataset.index = index;

            // Aggiunge celle con i dati
            const cellItem = row.insertCell();
            cellItem.textContent = result.item;

            const cellDescription = row.insertCell();
            cellDescription.textContent = result.description;

            const cellBarcode = row.insertCell();
            cellBarcode.textContent = result.barcode;

            const cellBookInv = row.insertCell();
            cellBookInv.textContent = result.bookInv;

            const cellUoM = row.insertCell();
            cellUoM.textContent = result.uoM;

            // Event listener per la selezione della riga
            row.addEventListener("click", function() {
                // Rimuovi la selezione precedente
                document.querySelectorAll("#search-results-materials-body tr.selected").forEach(tr => {
                    tr.classList.remove("selected");
                });
                // Seleziona questa riga
                row.classList.add("selected");
                selectedMaterialSearchRow = results[index];
            });

            // Doppio click per selezionare e confermare
            row.addEventListener("dblclick", function() {
                selectedMaterialSearchRow = results[index];
                selectMaterialSearchResultButton.click();
            });

        });
    }


    // Funzione per popolare la tabella di risultati
    function populateSearchResults(results) {
        searchResultsBody.innerHTML = "";
        selectedSearchRow = null;
        
        if (results.length === 0) {
            const row = searchResultsBody.insertRow();
            const cell = row.insertCell();
            cell.colSpan = 12;
            cell.textContent = "Nessun risultato trovato";
            return;
        }
        
        results.forEach((result, index) => {
            const row = searchResultsBody.insertRow();
            row.dataset.index = index;
            
            // Aggiunge celle con i dati
            const cellJob = row.insertCell();
            cellJob.textContent = result.job;
            
            const cellMono = row.insertCell();
            cellMono.textContent = result.mono;
            
            const cellCreationDate = row.insertCell();
            cellCreationDate.textContent = result.creationDate;
            
            const cellUM = row.insertCell();
            cellUM.textContent = result.prelUoM;
            
            const cellResQty = row.insertCell();
            cellResQty.textContent = result.prelResQty;
            
            const cellBOM = row.insertCell();
            cellBOM.textContent = result.bom;
            
            const cellItemDesc = row.insertCell();
            cellItemDesc.textContent = result.itemDesc;
            
            const cellOperation = row.insertCell();
            cellOperation.textContent = result.operation;
            
            const cellOperDesc = row.insertCell();
            cellOperDesc.textContent = result.operDesc;

            const cellComponent = row.insertCell();
            cellComponent.textContent = result.component;

            const cellPosition = row.insertCell();
            cellPosition.textContent = result.position;       
            
            const cellBarCode = row.insertCell();
            cellBarCode.textContent = result.barCode;
            
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

    // SaveButton: chiamata all'API passando dataResultList per salvare i dati.
    // Chiama la rimozione di tutti gli elementi dalla lista temporanea
    saveButton.addEventListener("click", async function() {
        if (dataResultList.length > 0) {
            console.log("Dati da salvare:", dataResultList);
            var workerId = "";
            const puCookie = JSON.parse(getCookie("pu-User"));
            if(puCookie) {
                //console.log("cookie pu-User:", puCookie);
                workerId = puCookie.workerId.toString();
                console.log("L'operazione viene salvata con l'utente:", workerId);
            }
            else {
                // Recupera il workerid dai cookies
                const cookie = JSON.parse(getCookie("userInfo"));
                console.log(typeof(cookie));
                //console.log("Cookie:", cookie);
                workerId = cookie.workerId.toString();
                console.log("Worker ID:", workerId);
            }

            if (!workerId || workerId === "") {
                console.error("Worker ID non trovato nei cookie.");
                return;
            }
            // Aggiunge il workerId a ogni oggetto nella lista
            dataResultList.forEach(item => {
                item.workerId = workerId;
            });
            console.log("Lista con Worker ID:", dataResultList);
            console.log("JSON inviato a api/prel_mat/post_prel_mat:", JSON.stringify(dataResultList, null, 2)); // Log dettagliato

            try {
                const response = await fetchWithAuth(getApiUrl("api/prel_mat/post_prel_mat"), {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(dataResultList),
                });
                if (response.ok) {
                    const result = await response.json();
                    console.log("Dati salvati con successo:", result);
                    // Pulisce la lista temporanea
                    const list = document.getElementById("prel-mat-lista-temp");
                    while (list.firstChild) {
                        list.removeChild(list.firstChild);
                    }
                    dataResultList = []; // Resetta la lista dei risultati
                    commessaInput.value =  "";
                    odlInput.value = "";
                    odlInput.disabled = true;
                    lavorazioneInput.value = "";
                    lavorazioneInput.disabled = true;
                    barcodeInput.value = "";
                    barcodeInput.disabled = true;
                    quantitaInput.value = "";
                    quantitaInput.disabled = true;
                    noContent.classList.remove("hidden");
                    alert("Dati salvati con successo");
                    commessaInput.focus();
                } else {
                    console.error("Errore durante il salvataggio dei dati:", response.status, response.statusText);
                }
            } catch (error) {
                console.error("Errore durante la richiesta di salvataggio:", error);
            }
        } else {
            alert("Nessun dato da salvare. Aggiungi prima un elemento.");
        }
    }); 

    // Event listener per il pulsante Aggiungi
    addButton.addEventListener("click", async function () {
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
        const selectedQta = parseFloat(document.getElementById("prel-mat-quantita").value) || 0;
        console.log("Valore iniziale di selectedQta:", selectedQta, "Raw input:", document.getElementById("prel-mat-quantita").value);
        let selectedBarcode = null;

        if (isAddingNewItem) {
            console.log("Barcode selezionato dalla tabella:", selectedMaterialSearchRow);
        } else {
            selectedBarcode = findSelectedItem(barcodeInput.value, barcodeList);
            console.log("Barcode selezionato dal form:", selectedBarcode);
        }

        console.log("Dati selezionati:", {
            selectedCommessa,
            selectedOdp,
            selectedLavorazione,
            selectedBarcode: selectedBarcode || selectedMaterialSearchRow,
            selectedQta
        });

        if (!selectedCommessa || !selectedOdp || !selectedLavorazione || (!selectedBarcode && !selectedMaterialSearchRow) || !selectedQta) {
            alert("Compilare tutti i campi richiesti");
            return;
        }

        try {
            let result = null;
            if (isAddingNewItem) {
                console.log("Aggiunta di un nuovo articolo in corso...");
                result = await loadDataForNewItem(
                    selectedCommessa.job,
                    selectedOdp.mono,
                    selectedOdp.creationDate,
                    selectedLavorazione.operation,
                    selectedMaterialSearchRow
                );
                console.log("Risultato di loadDataForNewItem:", result);
                console.log("itemDesc da loadDataForNewItem:", result?.itemDesc); // Log per debug
                //selectedMaterialSearchRow = null;
                //isAddingNewItem = false;
            } else {
                result = await loadAllData(
                    selectedCommessa.job,
                    selectedOdp.mono,
                    selectedOdp.creationDate,
                    selectedLavorazione.operation,
                    selectedBarcode.component,
                    selectedBarcode.barCode
                );
                console.log("itemDesc da loadAllData:", result?.itemDesc); // Log per debug
            }

            console.log("Risultato loadAllData/loadDataForNewItem:", result);
            console.log("selectedQta dopo loadAllData/loadDataForNewItem:", selectedQta, "Raw input:", document.getElementById("prel-mat-quantita").value);

            if (!result || !result.component || !result.uoM) {
                alert("Errore: dati di risultato non validi.");
                console.error("Risultato non valido:", result);
                return;
            }

            const data = {
                job: result.job,
                rtgStep: result.rtgStep,
                alternate: result.alternate,
                altRtgStep: result.altRtgStep || null,
                operation: result.operation,
                operDesc: result.operDesc,
                position: result.position,
                component: result.component,
                bom: result.bom,
                variant: result.variant,
                itemDesc: result.itemDesc,
                moid: result.moid,
                mono: result.mono,
                creationDate: result.creationDate,
                uoM: result.uoM,
                productionQty: result.productionQty,
                producedQty: result.producedQty,
                resQty: result.resQty,
                storage: result.storage,
                barCode: result.barCode,
                wc: result.wc,
                prelQty: selectedQta, // Use user-entered quantity
                neededQty: result.neededQty
            };
            
            console.log("Oggetto data prima di aggiungere a dataResultList:", data); // Log dettagliato
            console.log("itemDesc nell'oggetto data:", data.itemDesc); // Log specifico per itemDesc

            console.log("data.prelQty prima del controllo giacenza:", data.prelQty);

            data.deleted = isDelitingItem ? true : false;
            if (isDelitingItem) {
                console.log("Eliminazione dell'articolo in corso...");
                isDelitingItem = false;
            }

            // Fetch inventory data
            let invItem = await fetchGiacenzeByItem(data.component);
            console.log("Dati giacenza per componente:", invItem);

            if (Array.isArray(invItem)) {
                invItem = invItem.length > 0 ? invItem[0] : null;
            }

            if (!invItem || invItem.bookInv === undefined || invItem.uoM === undefined) {
                alert("Errore: dati di giacenza non validi o nessun dato trovato per il componente.");
                console.error("Giacenza non valida:", invItem, "Componente:", data.component);
                return;
            }

            const bookInv = parseFloat(invItem.bookInv) || 0;
            console.log("Confronto quantità - prelQty:", selectedQta, "bookInv:", bookInv, "uoM match:", invItem.uoM === data.uoM);

            // Inventory check
            if (invItem.uoM === data.uoM && selectedQta > bookInv) {
                const controlloUoM = await fetchControlloUoM();
                console.log("Controllo UoM:", controlloUoM);

                if (!controlloUoM || controlloUoM.controlloUoM === undefined) {
                    alert("Errore: controllo UoM non valido.");
                    console.error("Controllo UoM non valido:", controlloUoM);
                    return;
                }

                if (controlloUoM.controlloUoM === true) {
                    console.log(`Errore: Quantità richiesta (${selectedQta}) supera giacenza disponibile (${bookInv}).`);
                    errorQty.style.display = "block";
                    errorQty.innerHTML = `<p><strong>La quantità richiesta (${selectedQta}) supera la giacenza disponibile (${bookInv}).</strong></p>`;
                    alert(`La quantità richiesta (${selectedQta}) supera la giacenza disponibile (${bookInv}).`);
                    quantitaInput.value = bookInv;
                    return;
                }
            }

            // Optional: Check against needed quantity
            if (selectedQta > result.neededQty) {
                console.log(`Attenzione: Quantità richiesta (${selectedQta}) supera quantità necessaria (${result.neededQty}).`);
                if (!confirm(`La quantità richiesta (${selectedQta}) supera la quantità necessaria (${result.neededQty}). Continuare?`)) {
                    return;
                }
            }

            dataResultList.push(data);
            addToTemporaryList(data, dataResultList);

            // Reset form fields

            //commessaInput.value = "";
            //odlInput.value = "";
            //odlInput.disabled = true;
            //lavorazioneInput.value = "";
            //lavorazioneInput.disabled = true;
            barcodeInput.value = "";
            //barcodeInput.disabled = true;
            quantitaInput.value = "";
            quantitaInput.disabled = true;
            //aggiungiArticoloButton.classList.add("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        } catch (error) {
            console.error("Errore durante l'aggiunta dell'articolo:", error);
            alert("Errore durante l'aggiunta dell'articolo. Riprova.");
        }
    });

    eliminaArticoloButton.addEventListener("click", async function() {
        if(eliminaArticoloButton.classList.contains("disabled-button-look")) {
            alert("Selezionare un materiale con quantità da prelevare a zero prima di procedere.");
            return;
        }

        isDelitingItem = true;
        addButton.click();
    });


    // Funzione per trovare l'elemento selezionato in base al valore dell'input
    // Si può esportare
    function findSelectedItem(inputValue, list) {
        if (!inputValue || !list || list.length === 0) return null;
        
        return list.find(item => item.display === inputValue);
    }

    // Funzione per caricare i dati dell'ODP in base alla commessa selezionata
    async function loadOdpData(jobId) {
        if (!jobId) return;
        
        try {
            odlInput.value = ""; // pulisco il campo ODP al cambio della commessa
            odlInput.disabled = false;
            const odpResult = await fetchJobMostep(jobId);
            //console.log("Risultato ODP:", odpResult);
            odpList = odpResult
                .filter(odp => odp && odp.mono && odp.creationDate)
                .map(odp => ({
                    mono: odp.mono,
                    creationDate: odp.creationDate,
                    bom: odp.bom,
                    display: `${odp.mono} - ${odp.creationDate} - ${odp.bom}`
                }));

            //console.log("Lista di ODP:", odpList);
            setupAutocomplete(odlInput, odlAutocompleteList, odpList);
            const odpDistinctList = odpList.filter((item, index, self) =>
                index === self.findIndex((t) => t.mono === item.mono && t.creationDate === item.creationDate));
            //console.log("Lista di ODP Distinti:", odpDistinctList);
            if(odpDistinctList.length === 1) {
                setTimeout(() => {
                    odlInput.value = odpDistinctList[0].display;
                    const event = new Event('change', { bubbles: true });
                    odlInput.dispatchEvent(event);
                    odlInput.disabled = true;
                }, 100);
            }
            else
            {
                odlInput.focus();
            }
        } catch (error) {
            console.error("Errore nel caricamento dei dati ODP:", error);
        }
    }

    // Funzione per caricare i dati della lavorazione
    async function loadLavorazioneData(jobId, mono, creationDate) {
        console.log("Caricamento lavorazione per jobId:", jobId, "e mono:", mono);
        if (!jobId || !mono || !creationDate) return;
        
        try {
            lavorazioneInput.disabled = false;
            const lavorazioneResult = await fetchJobsByOdp(jobId, mono, creationDate);
            //console.log("Risultato lavorazione:", lavorazioneResult);
            lavorazioneList = lavorazioneResult
                .filter(lav => lav && lav.operation && lav.operDesc)
                .map(lav => ({
                    operation: lav.operation,
                    moId: lav.moid,
                    operDesc: lav.operDesc,
                    display: `${lav.operation} - ${lav.operDesc}`
                }));
            console.log("Lista di lavorazioni:", lavorazioneList);  

            //console.log("Lista di lavorazioni:", lavorazioneList);
            setupAutocomplete(lavorazioneInput, lavorazioneAutocompleteList, lavorazioneList);
            
            // Siccome per una lavorazione ci sono più barcode, e quindi le lavoraizoni sono ripetute, crea una lista di lavorazioni distinte 
            const lavorazioneDistinctList = lavorazioneList.filter((item, index, self) =>
                index === self.findIndex((t) => t.operation === item.operation && t.operDesc === item.operDesc));
            if(lavorazioneDistinctList.length === 1) {
                setTimeout(() => {
                    lavorazioneInput.value = lavorazioneDistinctList[0].display;
                    const event = new Event('change', { bubbles: true });
                    lavorazioneInput.dispatchEvent(event);
                    lavorazioneInput.disabled = true;
                }, 100);
            }
            else
            {
                lavorazioneInput.focus();
            }
        } catch (error) {
            console.error("Errore nel caricamento dei dati lavorazione:", error);
        }
    }

    async function loadBarCodeData(jobId, mono, creationDate, operation) {
        console.log("Caricamento barcode per jobId:", jobId, ", mono:", mono, " e operation:", operation);
        if (!jobId || !mono || !creationDate || !operation) return;

        try {
            // Fetch barcode data
            barcodeInput.disabled = false;
            aggiungiArticoloButton.classList.remove("disabled-button-look");
            eliminaArticoloButton.classList.add("disabled-button-look");
            const barCodeResult = await fetchJobsByLavorazione(jobId, mono, creationDate, operation);

            // Fetch giacenze data to get correct descriptions
            const giacenze = await fetchAllGiacenze();

            // Create a map of giacenze for quick lookup (using item as the key)
            const giacenzeMap = new Map();
            giacenze.forEach(g => {
                giacenzeMap.set(g.item, g.description); // Match barCode.component to g.item
            });

            // Process barcode data and replace itemDesc with giacenze description
            barcodeList = barCodeResult
                .filter(barCode => barCode && barCode.component)
                .map(barCode => {
                    // Get the correct description from giacenzeMap, fallback to default
                    const description = giacenzeMap.get(barCode.component) || 
                                    "Nessuna descrizione disponibile";

                    return {
                        component: barCode.component,
                        barCode: barCode.barCode || '',
                        itemDesc: description, // Use the correct description from giacenze
                        display: `Item: ${barCode.component} ${barCode.barCode === "" ? "" : "- Code: " + barCode.barCode} - ${description}`,
                    };
                });

            // Setup autocomplete with the updated barcodeList
            setupAutocomplete(barcodeInput, barcodeAutocompleteList, barcodeList);

            // Deduplicate barcodeList
            const barcodeDistinctList = barcodeList.filter((item, index, self) =>
                index === self.findIndex((t) => t.component === item.component && t.barCode === item.barCode && t.itemDesc === item.itemDesc));

            if (barcodeDistinctList.length === 1) {
                setTimeout(() => {
                    barcodeInput.value = barcodeDistinctList[0].display;
                    const event = new Event('change', { bubbles: true });
                    barcodeInput.dispatchEvent(event);
                    barcodeInput.disabled = true;
                }, 100);
            } else {
                barcodeInput.focus();
            }
        } catch (error) {
            console.error("Errore nel caricamento dei dati barcode:", error);
        }
    }

    async function loadAllData(jobId, mono, creationDate, operation, component, barCode = "") {
        if (!jobId || !mono || !creationDate || !operation || !component) return null;

        try {
            const allDataResult = await fetchJobsByComponent(jobId, mono, creationDate, operation, component, barCode);
            console.log("Lista di tutti i dati:", allDataResult);

            if (allDataResult.length > 0) {
                const dataItem = { ...allDataResult[0] };
                //Imposto itemDesc usando fetchHiacenzaByItem
                const giacenza = await fetchGiacenzeByItem(component);
                dataItem.itemDesc = giacenza && giacenza.description ? giacenza.description : "Nessuna descrizione disponibile";
                console.log("Descrizione impostata per itemDesc in loadAllData:", dataItem.itemDesc);


                var sum = 0;
                // Sum quantities from temporary list
                dataResultList.forEach(element => {
                    if (element.moid === dataItem.moid && element.component === dataItem.component) {
                        sum += parseFloat(element.prelQty || 0);
                    }
                });
                console.log("Somma delle quantità della lista temporanea:", sum);

                const prelMatQtyList = await fetchA3PrelMatQtyList(dataItem.component);
                console.log("Lista di quantità da A3_app_prel_mat:", prelMatQtyList);

                if (prelMatQtyList.length > 0) {
                    prelMatQtyList.forEach(element => {
                        sum += parseFloat(element.prelQty || 0);
                    });
                }

                console.log("Somma totale:", sum);

                const finalSum = parseFloat(dataItem.prelResQty || 0) - sum;
                console.log("prelResQty:", dataItem.prelResQty, "sum:", sum, "finalSum:", finalSum);

                quantitaInput.disabled = false;
                // Only set quantitaInput.value if empty to preserve user input
                if (!quantitaInput.value) {
                    quantitaInput.value = finalSum;
                }
                console.log("quantitaInput.value dopo impostazione:", quantitaInput.value);

                const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
                const invItem = await fetchGiacenzeByItem(dataItem.component);
                console.log("Dati giacenza:", invItem);

                if (quantitaLabel && invItem) {
                    quantitaLabel.textContent = `Qta. da prelevare: ${dataItem.prelResQty} - Qta. prelevabile: ${finalSum} - Qta. già prelevata su ERP: ${dataItem.pickedQuantity} - UoM: ${dataItem.prelUoM} - Giacenza: ${invItem.bookInv} (${invItem.uoM})`;
                }

                if (parseFloat(allDataResult[0].pickedQuantity) === 0) {
                    eliminaArticoloButton.classList.remove("disabled-button-look");
                } else {
                    eliminaArticoloButton.classList.add("disabled-button-look");
                }

                // Remove redundant inventory check to avoid returning null
                // Inventory check will be handled in addButton

                dataItem.finalSum = finalSum;
                dataItem.neededQty = parseFloat(dataItem.prelResQty || 0); // Set neededQty correctly

                return dataItem;
            } else {
                console.error("Nessun dato trovato per i parametri forniti.");
                return null;
            }
        } catch (error) {
            console.error("Errore nel caricamento dei dati:", error);
            return null;
        }
    }

    async function loadDataForNewItem(jobId, mono, creationDate, operation, selectedMaterialSearchRow) {
        // Ricerca per tutti i parametri fino a component. Poi prende quelli interessanti e aggiunge:
        // component, barCode, itemDesc, neededQty ecc..

        try {
            // Recupera le informazioni della lavorazione per aggiungere quelle sul materiale dalla tabella
            const allDataResult = await fetchJobsByLavorazione(jobId, mono, creationDate, operation);
            console.log("Lista di tutti i dati:", allDataResult);
            console.log("Riga selezionata dalla tabella:", selectedMaterialSearchRow);

            /*
            var dataItem = allDataResult[0];
            dataItem.component = selectedMaterialSearchRow.item;
            dataItem.barCode = selectedMaterialSearchRow.barCode || "";
            //dataItem.itemDesc = selectedMaterialSearchRow.itemDesc || "";
            dataItem.prelQty = selectedMaterialSearchRow.neededQty || 0;
            dataItem.prelResQty = selectedMaterialSearchRow.neededQty || 0;
            dataItem.prelNeededQty = selectedMaterialSearchRow.neededQty || 0;
            dataItem.neededQty = selectedMaterialSearchRow.neededQty || 0;
            dataItem.storage = selectedMaterialSearchRow.storage || "";
            dataItem.prelUoM = selectedMaterialSearchRow.uoM || "";
            dataItem.position = 0;
            */

           const dataItem = {
                job: allDataResult[0].job || '',
                rtgStep: allDataResult[0].rtgStep || '',
                alternate: allDataResult[0]?.alternate || '',
                altRtgStep: allDataResult[0]?.altRtgStep || '',
                operation: allDataResult[0]?.operation || '',
                operDesc: allDataResult[0]?.operDesc || '',
                moid: allDataResult[0]?.moid || '',
                mono: allDataResult[0]?.mono || '',
                creationDate: allDataResult[0]?.creationDate || '',
                productionQty: allDataResult[0]?.productionQty || 0,
                producedQty: allDataResult[0]?.producedQty || 0,
                resQty: allDataResult[0]?.resQty || 0,
                wc: allDataResult[0]?.wc || '',
                component: selectedMaterialSearchRow.item,
                barCode: selectedMaterialSearchRow.barCode || '',
                prelQty: selectedMaterialSearchRow.neededQty || 0,
                prelResQty: selectedMaterialSearchRow.neededQty || 0,
                prelNeededQty: selectedMaterialSearchRow.neededQty || 0,
                neededQty: selectedMaterialSearchRow.neededQty || 0,
                storage: selectedMaterialSearchRow.storage || '',
                prelUoM: selectedMaterialSearchRow.uoM || '',
                uoM: selectedMaterialSearchRow.uoM || '',
                position: 0,
                bom: allDataResult[0]?.bom || '',
                variant: allDataResult[0]?.variant || ''
            };



            //===================================================
            // Aggiunta per modifica itemDesc
            const giacenza = await fetchGiacenzeByItem(selectedMaterialSearchRow.item);
            dataItem.itemDesc = giacenza && giacenza.description ? giacenza.description : "Nessuna descrizione disponibile";
            //===================================================
            console.log("Dati per il nuovo articolo:", dataItem);
             console.log("Descrizione impostata per itemDesc:", dataItem.itemDesc); // Log per debug

            if(dataItem) {
                quantitaInput.disabled = false;
                const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
                
                const invItem = fetchGiacenzeByItem(dataItem.component);

                if (quantitaLabel && invItem) {
                    quantitaLabel.innerHTML = `Nuova qta. da prelevare: ${dataItem.prelNeededQty} - Qta. prelevabile: ${dataItem.prelResQty} - UoM: ${dataItem.prelUoM} - Giacenza: ${invItem.bookInv} (${invItem.uoM})`;
                }
                
                // Se la quantità è negativa o maggiore di prelResQty, mostra un messaggio di errore
                if (parseFloat(quantitaInput.value) < 0) {
                    errorQty.style.display = "block";
                }

                if( (invItem.uoM === dataItem.prelUoM) && parseFloat(quantitaInput.value) > parseFloat(invItem.bookInv)) {
                    const controlloUoM = await fetchControlloUoM();
                    if(controlloUoM.controlloUoM === true) {
                        errorQty.style.display = "block";
                        errorQty.innerHTML = "<p><strong>La quantità richiesta supera la giacenza disponibile.</strong></p>";
                        alert("La quantità richiesta supera la giacenza disponibile.");
                        quantitaInput.value = invItem.bookInv;
                        return null;
                    }
                }

                isAddingNewItem = false; // Imposta il flag per indicare che si sta aggiungendo un nuovo articolo

                return dataItem;
            }
            else {
                console.error("Nessun dato trovato per i parametri forniti.");
                return null;
            }

        } catch (error) {
            console.error("Errore nel caricamento dei dati:", error);
            return null;
        }
    }

    async function defaultTableContent(searchMaterialsOverlay, materialsSearchResults) {
        setTimeout(() => {
            document.body.classList.add("loading-cursor");
        }, 200);

        var itemList = await fetchAllGiacenze(); 

        console.log("Lista di articoli:", itemList);

        itemList = itemList.filter(item => {
            return !barcodeList.some(barcode => barcode.component === item.item);
        });
        
        for(const item of itemList) {
            materialsSearchResults.push(({
                item: item.item || '',
                description: item.description || '',
                barCode: item.barCode || '',
                bookInv: item.bookInv || '',
                uoM: item.uoM || '',
                display: `${item.item} - ${item.barCode} - ${item.description}` 
            }));
        }
        
        populateMaterialSearchResults(materialsSearchResults);

        document.body.classList.remove("loading-cursor");

        searchMaterialsOverlay.classList.add("active");
    }
});

// Funzioni di fetch
async function fetchJobMostep(job) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/mostepsmocomponent/job"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"job": job}),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const jobInfo = await request.json();
        return jobInfo;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchJobsByOdp(job, mono, creationDate) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/mostepsmocomponent/mono"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "job": job,
                "mono": mono,
                "creationDate": creationDate
            }),
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        
        const jobInfo = await request.json();
        return jobInfo;
    }
    catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchJobsByComponent(job, mono, creationDate, operation, component, barCode = "") {
    try {
        const request = await fetchWithAuth(getApiUrl("api/mostepsmocomponent/component_barcode"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "job": job,
                "mono": mono,
                "creationDate": creationDate,
                "operation": operation,
                "component": component,
                "barCode": barCode
            }),
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        
        const jobInfo = await request.json();
        return jobInfo;
    }
    catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchAllJobs() {
    if(globalAllData) return globalAllData;
    try {
        const request = await fetchWithAuth(getApiUrl("api/job"), {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        globalAllData = await request.json();
        return globalAllData;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchA3PrelMatQtyList(component) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/prel_mat/get_prel_mat_with_component"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"component": component}),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const qtyList = await request.json();
        return qtyList;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
};

async function fetchJobsByLavorazione(job, mono, creationDate, operation) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/mostepsmocomponent/operation"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "job": job,
                "mono": mono,
                "creationDate": creationDate,
                "operation": operation
            }),
        });
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        const data = await request.json();
        return data;
    }
    catch (error) {
        console.error("Errore durante la fetch:", error);
    }
}

async function fetchAllGiacenze() {
    try {
        const request = await fetchWithAuth(getApiUrl("api/giacenze/get_all"), {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        });
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        const giacenze = await request.json();
        return giacenze;
    } catch (error) {
        console.error("Errore durante la fetch delle giacenze:", error);
        return [];
    }
}

async function fetchGiacenzeByItem(item) {
    try {
        const request = await fetchWithAuth(getApiUrl("api/giacenze/get_by_item"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "component": item
            }),
        });
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        const giacenze = await request.json();
        return giacenze;
    } catch (error) {
        console.error("Errore durante la fetch delle giacenze:", error);
        return [];
    }
}

async function fetchControlloUoM() {
    try {
        const request = await fetchWithAuth(getApiUrl("api/settings/get_controllo_uom"), {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            },
        });
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        const controlloUoM = await request.json();
        return controlloUoM;
    } catch (error) {
        console.error("Errore durante la fetch del controllo UoM:", error);
        return [];
    }
}

// Si può esportare a patto di mantenere gli id invariati nel file html
function addToTemporaryList(data, dataResultList) {
    const list = document.getElementById("prel-mat-lista-temp");
    const noContent = document.getElementById("nocontent");
    const newItem = document.createElement("li");
    newItem.classList.add("just-added"); // Aggiungi classe per l'animazione


    newItem.innerHTML = `
        <div class="item-content"><div><span class="item-content-heading">Comm:</span> ${data.job} - <span class="item-content-heading">MoId:</span> ${data.moid} - <span class="item-content-heading">MoNo:</span> ${data.mono}</div>
        <div><span class="item-content-heading">Lav:</span> ${data.operation} - <span class="item-content-heading">Desc:</span> ${data.operDesc} </div>
        <div><span class="item-content-heading">BOM:</span> ${data.bom}</div>
        <div><span class="item-content-heading">Item:</span> ${data.component} - <span class="item-content-heading">Desc:</span> ${data.itemDesc || "Nessuna descrizione disponibile"} ${data.barCode === "" || null ? "" : `- <span class="item-content-heading">Code:</span> ${data.barCode}`}</div>        <div class=temp-list-qta-${data.moid}><strong>Qta: ${data.prelQty}</strong></div></div>
        <div class="item-actions">
            <button class="button-icon delete option-button" title="Rimuovi">
                <i class="fa-solid fa-trash"></i>
            </button>
        </div>
    `;

    if(data.position === 0)
    {
        newItem.classList.add("new-prel-item");
    }
    if(data.deleted === true)
    {
        newItem.classList.add("deleted-prel-item");
    }

    list.appendChild(newItem);

    if(list.childElementCount > 0) {
        noContent.classList.add("hidden");
    }

    // Aggiunge event listener per il pulsante di eliminazione
    const deleteButton = newItem.querySelector(".delete");
    deleteButton.addEventListener("click", function() {
        dataResultList.splice(dataResultList.indexOf(data), 1);
        list.removeChild(newItem);
        if(list.childElementCount > 0) {
            noContent.classList.add("hidden");
        }
        else if (list.childElementCount === 0) 
        {
            noContent.classList.remove("hidden");
        }
    });
}