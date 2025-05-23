import { getCookie } from "./cookies.js";
import { fetchWithAuth } from "./fetch.js";
import { setupCustomAutocomplete } from "./autocomplete.js";
import { getIPString } from "./main.js";

let globalAllData = null;

document.addEventListener("DOMContentLoaded", async function () {
    const barCodeInput = document.getElementById("inv-barcode");
    const quantitaInput = document.getElementById("inv-quantita");
    const addButton = document.getElementById("inv-add-list");
    const saveButton = document.getElementById("inv-save");
    const barcodeAutocompleteList = document.getElementById("inv-barcode-autocomplete-list");
    const noContent = document.getElementById("nocontent");
    const quantitaLabel = document.querySelector('label[for="inv-quantita"]');
    
    var barcodeList = [];
    var dataResultList = [];
    var prevBookInv = 0;

    // Inizializza con tutti gli articoli
    try {
        const items = await getAllItems();
        if (items) {
            barcodeList = items.filter(item => item && item.item).map(item => ({
                item: item.item,
                barCode: item.barCode ? item.barCode : null,
                description: item.description,
                bookInv: item.bookInv,
                storage: item.storage,
                fiscalYear: item.fiscalYear,
                uoM: item.uoM,
                display: item.barCode ? `Item: ${item.item} - Code: ${item.barCode} - Descr: ${item.description}` : `Item: ${item.item} - Descr: ${item.description}`
            }));
            
            // Autocompletamento personalizzato per evitare di visualizzare troppi elementi
            setupCustomAutocomplete(barCodeInput, barcodeAutocompleteList, barcodeList);
        }
    } catch (error) {
        console.error("Error fetching items:", error);
        alert("Si è verificato un errore durante il caricamento iniziale dei dati.");
    }

    barCodeInput.addEventListener("change", async function() {
        // Controlli maggiori a causa di input di grandi dimensioni.
        // La descrizione causava problemi di formattazione del testo e per tanto la ricerca
        // non dava i risultati attesi.
        prevBookInv = 0;
        setTimeout(async () => {
            const inputValue = barCodeInput.value.trim().toUpperCase();
            
            // Cerca una corrispondenza esatta con il display
            let selectedItem = barcodeList.find(item => 
                item.display.toUpperCase().trim() === inputValue);
            
            // Cerca una compatibilità con item o barcode
            if (!selectedItem) {
                selectedItem = barcodeList.find(item => 
                    (item.item && item.item.toUpperCase() === inputValue) || 
                    (item.barCode && item.barCode.toUpperCase() === inputValue));
            }
            
            if (!selectedItem) {
                selectedItem = barcodeList.find(item => 
                    inputValue.includes(item.item) || 
                    (item.barCode && inputValue.includes(item.barCode)));
            }
            
            if (!selectedItem && inputValue.length > 5) {
                selectedItem = barcodeList.find(item => 
                    item.display.toUpperCase().includes(`ITEM: ${inputValue}`) || 
                    (item.barCode && item.display.toUpperCase().includes(`CODE: ${inputValue}`)));
            }
    
            if (selectedItem) {
                // Aggiorna il valore dell'input con il display completo
                quantitaLabel.textContent = "Quantità Rilevata: " + selectedItem.uoM;
                barCodeInput.value = selectedItem.display;
                
                // Validazione e correzione del valore di bookInv
                let bookInvValue = selectedItem.bookInv;
                prevBookInv = bookInvValue;

                try {
                    const existingItem = await getAllAppInventario();
                    const existingItemIds = existingItem.filter(item => item && item.item === selectedItem.item && item.barCode === selectedItem.barCode);
                    if(existingItemIds.length > 0) {
                        console.log("Elemento già esistente:", existingItemIds[0]);
                        bookInvValue = existingItemIds[0].bookInv;
                    }
                }
                catch (error) {
                    console.error("Errore durante la ricerca dell'elemento:", error);
                }
                
                
                // Verifica se bookInv è un numero valido
                // Effettua un controllo per evitare NaN o valori non numerici
                // Lo converte in una stringa per rimpiazzare la virgola e evitare problemi di formattazione
                if (bookInvValue !== undefined && bookInvValue !== null) {
                    // Converte in numero se è una stringa
                    if (typeof bookInvValue === 'string') {
                        bookInvValue = parseFloat(bookInvValue.replace(',', '.'));
                    }
                    
                    // Verifica se è un numero finito e non NaN
                    if (!isNaN(bookInvValue) && isFinite(bookInvValue)) {
                        // Arrotonda a due decimali per evitare problemi di precisione
                        bookInvValue = Math.round((bookInvValue + Number.EPSILON) * 100) / 100;
                    } else {
                        console.warn("bookInv non è un numero valido:", selectedItem.bookInv);
                        bookInvValue = 0;
                    }
                } else {
                    bookInvValue = 0;
                }
                
                // Assicura che l'output sia una stringa formattata correttamente
                quantitaInput.value = bookInvValue.toString();
                quantitaInput.disabled = false;
                quantitaInput.focus();
                barcodeAutocompleteList.classList.add("hidden");
                
                console.log("Elemento selezionato:", selectedItem);
                console.log("bookInv originale:", selectedItem.bookInv);
                console.log("bookInv corretto:", bookInvValue);
            } else {
                quantitaInput.value = "";
                quantitaLabel.textContent = "Quantità Rilevata: ";
            }
        }, 300);

    });

    barCodeInput.addEventListener("focusout", function() {
        // Utilizza un timeout per permettere il click sugli elementi della lista
        setTimeout(() => {
            if(barCodeInput.value === "") {
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.value = "";
                quantitaInput.disabled = true;
                quantitaLabel.textContent = "Quantità Rilevata: ";
            }
        }, 200);
    });

    barCodeInput.addEventListener("keydown", async function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            
            // Impedisce l'autocompletamento se l'input è vuoto
            if(barCodeInput.value === "") {
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.value = "";
                quantitaInput.disabled = true;
                quantitaLabel.textContent = "Quantità Rilevata: ";
                return;
            }

            // Controlla prima la lista completa
            const selectedBarcode = barcodeList.find(item => 
                item.item === barCodeInput.value.toUpperCase() || 
                item.barCode === barCodeInput.value.toUpperCase());
                
            if (selectedBarcode) {
                barCodeInput.value = selectedBarcode.display;
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.value = selectedBarcode.bookInv;
                quantitaInput.disabled = false;
                quantitaInput.focus();
                quantitaLabel.textContent = "Quantità Rilevata: " + selectedBarcode.uoM;
            } else {
                // Verifica se c'è una corrispondenza parziale
                const filteredItems = barcodeList.filter(item => 
                    (item.item && item.item.includes(barCodeInput.value.toUpperCase())) || 
                    (item.barCode && item.barCode.includes(barCodeInput.value.toUpperCase())) ||
                    (item.display && item.display.toUpperCase().includes(barCodeInput.value.toUpperCase()))
                );
                
                if (filteredItems.length > 0) {
                    // Prende il primo risultato
                    barCodeInput.value = filteredItems[0].display;
                    barcodeAutocompleteList.classList.add("hidden");
                    quantitaInput.value = filteredItems[0].bookInv;
                    quantitaInput.disabled = false;
                    quantitaInput.focus();
                    quantitaLabel.textContent = "Quantità Rilevata: " + filteredItems[0].uoM;
                } else {
                    barCodeInput.value = "";
                    quantitaInput.value = "";
                    quantitaInput.disabled = true;
                    quantitaLabel.textContent = "Quantità Rilevata: ";
                }
            }
        }
    });

    quantitaInput.addEventListener("keydown", function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            if (quantitaInput.value) {
                addButton.click();
            }
        }
    });

    addButton.addEventListener("click", function() {
        // Ottiene il valore corrente dell'input barcode
        const barCodeValue = barCodeInput.value.trim();
        let quantitaStr = quantitaInput.value.trim().replace(',', '.');
        let quantita;
        
        // Validazione della quantità
        if (quantitaStr !== "") {
            quantita = parseFloat(quantitaStr);
            if (isNaN(quantita) || !isFinite(quantita)) {
                console.error("Quantità non valida:", quantitaInput.value);
                alert("Inserisci una quantità valida.");
                return;
            }
            
            // Arrotonda a due decimali per evitare problemi di precisione
            quantita = Math.round((quantita + Number.EPSILON) * 100) / 100;
        } else {
            alert("Inserisci una quantità.");
            return;
        }
        
        // Ricerca dell'elemento selezionato - come nel codice precedente
        // Controlli per evitare problemi con la lunghezza dell'input che causa problemi di formattazione
        // e per tanto la ricerca non dava i risultati attesi.
        let selectedBarcode = barcodeList.find(item => item.display === barCodeValue);
        
        if (!selectedBarcode) {
            const itemMatch = barCodeValue.match(/Item:\s*(\S+)/i);
            const item = itemMatch ? itemMatch[1].trim() : null;
            
            const codeMatch = barCodeValue.match(/Code:\s*(\S+)/i);
            const barCode = codeMatch ? codeMatch[1].trim() : null;
            
            selectedBarcode = barcodeList.find(b => 
                (item && b.item === item) || 
                (barCode && b.barCode === barCode) ||
                (b.display && b.display.includes(barCodeValue))
            );
        }

        if (!selectedBarcode && barCodeValue.length > 0) {
            const searchTerms = barCodeValue.toLowerCase().split(/\s+/).filter(term => term.length > 2);
            
            if (searchTerms.length > 0) {
                let bestMatch = null;
                let highestScore = 0;
                
                barcodeList.forEach(item => {
                    if (!item.display) return;
                    
                    const displayLower = item.display.toLowerCase();
                    let score = 0;
                    
                    searchTerms.forEach(term => {
                        if (displayLower.includes(term)) score++;
                    });
                    
                    if (score > highestScore) {
                        highestScore = score;
                        bestMatch = item;
                    }
                });
                
                if (bestMatch) {
                    selectedBarcode = bestMatch;
                }
            }
        }

        if(selectedBarcode) {
            console.log("Elemento trovato:", selectedBarcode);
            
            var data = {
                item: selectedBarcode.item,
                barCode: selectedBarcode.barCode,
                description: selectedBarcode.description,
                fiscalYear: selectedBarcode.fiscalYear,
                storage: selectedBarcode.storage,
                uoM: selectedBarcode.uoM,
                prevBookInv: prevBookInv,
                bookInv: quantita.toString() // Usa il valore arrotondato e convertito a stringa
            };

            dataResultList.push(data);
            addToTemporaryList(data, dataResultList);
            barCodeInput.value = "";
            quantitaInput.value = "";
            quantitaInput.disabled = true;
            barcodeAutocompleteList.classList.add("hidden");
            quantitaLabel.textContent = "Quantità Rilevata: ";
            barCodeInput.focus();
        } else {
            console.error("Selezione non valida:", {
                barCodeValue: barCodeValue,
                quantita: quantita
            });
            alert("Seleziona un elemento valido dall'elenco.");
        }
    });

    // SaveButton: chiamata all'API passando dataResultList per salvare i dati.
    // Chiama la rimozione di tutti gli elementi dalla lista temporanea
    saveButton.addEventListener("click", async function() {
        if (dataResultList.length > 0) {
            console.log("Dati da salvare:", dataResultList);
            var workerId = "";
            const puCookie = JSON.parse(getCookie("pu-User"));
            if(puCookie) {
                console.log("cookie pu-User:", puCookie);
                workerId = puCookie.workerId.toString();
                console.log("L'operazione viene salvata con l'utente:", workerId);
            }
            else {
                // Recupera il workerId dai cookies
                const cookie = JSON.parse(getCookie("userInfo"));
                workerId = cookie.workerId.toString();
                //console.log("Worker ID:", workerId);
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
            try {
                const response = await fetchWithAuth(`http://${getIPString()}:5245/api/inventario/post_inventario`, {
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
                    const list = document.getElementById("inv-lista-temp");
                    while (list.firstChild) {
                        list.removeChild(list.firstChild);
                    }
                    dataResultList = []; // Resetta la lista dei risultati
                    barCodeInput.value = "";
                    quantitaInput.value = "";
                    quantitaInput.disabled = true;
                    barcodeAutocompleteList.classList.add("hidden");
                    barCodeInput.focus();
                    noContent.classList.remove("hidden");
                    alert("Dati salvati con successo!");
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
});

function addToTemporaryList(data, dataResultList) {
    const list = document.getElementById("inv-lista-temp");
    const noContent = document.getElementById("nocontent");
    const newItem = document.createElement("li");
    newItem.classList.add("just-added"); // Aggiungi classe per l'animazione

    newItem.innerHTML = `
        <div class="item-content"><div><spam class="item-content-heading">Item:</spam> ${data.item} ${data.barCode === null ? "" : "- <spam class='item-content-heading'>Code:</spam>" + data.barCode}</div>
        <div><spam class="item-content-heading">Desc:</spam> ${data.description}</div>
        <div><strong>Qta: ${data.bookInv}</strong></div></div>
        <div class="item-actions">
            <button class="button-icon delete option-button" title="Rimuovi">
                <i class="fa-solid fa-trash"></i>
            </button>
        </div>
    `;

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

async function getAllAppInventario()
{
    const response = await fetchWithAuth(`http://${getIPString()}:5245/api/inventario/get_inventario_not_imported`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json"
        }
    });

    if (!response.ok) {
        throw new Error("Failed to fetch items");
    }

    return await response.json();
}

async function getAllItems(){
    if(globalAllData) return globalAllData;
    const response = await fetchWithAuth(`http://${getIPString()}:5245/api/giacenze/get_all`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json"
        }
    });

    if (!response.ok) {
        throw new Error("Failed to fetch items");
    }

    globalAllData = await response.json();
    return globalAllData;
}