import { getCookie } from "./cookies.js";
import { fetchWithAuth } from "./fetch.js";
import { setupCustomAutocomplete } from "./autocomplete.js";

document.addEventListener("DOMContentLoaded", async function () {
    const barCodeInput = document.getElementById("inv-barcode");
    const quantitaInput = document.getElementById("inv-quantita");
    const addButton = document.getElementById("inv-add-list");
    const saveButton = document.getElementById("inv-save");
    const barcodeAutocompleteList = document.getElementById("inv-barcode-autocomplete-list");
    
    var barcodeList = [];
    var dataResultList = [];

    // Inizializza con tutti gli articoli
    try {
        const items = await getAllItems();
        if (items) {
            barcodeList = items.filter(item => item && item.item).map(item => ({
                item: item.item,
                barCode: item.barCode ? item.barCode : "",
                description: item.description,
                bookInv: item.bookInv,
                display: item.barCode ? `Item: ${item.item} - Code: ${item.barCode} - Descr: ${item.description}` : `Item: ${item.item} - Descr: ${item.description}`
            }));
            
            // Modifica: modifichiamo l'autocomplete per limitare i risultati
            setupCustomAutocomplete(barCodeInput, barcodeAutocompleteList, barcodeList);
        }
    } catch (error) {
        console.error("Error fetching items:", error);
    }

    barCodeInput.addEventListener("change", function() {
        const selectedItem = barcodeList.find(item => item.display === barCodeInput.value);
        if (selectedItem) {
            quantitaInput.value = selectedItem.bookInv;
            quantitaInput.disabled = false;
            quantitaInput.focus();
            barcodeAutocompleteList.classList.add("hidden");
        } else {
            quantitaInput.value = "";
        }
    });

    barCodeInput.addEventListener("focusout", function() {
        // Utilizziamo un timeout per permettere il click sugli elementi della lista
        setTimeout(() => {
            if(barCodeInput.value === "") {
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.value = "";
                quantitaInput.disabled = true;
            }
        }, 200);
    });

    barCodeInput.addEventListener("keydown", function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            
            // Controlliamo prima la lista completa
            const selectedBarcode = barcodeList.find(item => 
                item.item === barCodeInput.value.toUpperCase() || 
                item.barCode === barCodeInput.value.toUpperCase());
                
            if (selectedBarcode) {
                barCodeInput.value = selectedBarcode.display;
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.value = selectedBarcode.bookInv;
                quantitaInput.disabled = false;
                quantitaInput.focus();
            } else {
                // Verifichiamo se c'è una corrispondenza parziale
                const filteredItems = barcodeList.filter(item => 
                    (item.item && item.item.includes(barCodeInput.value.toUpperCase())) || 
                    (item.barCode && item.barCode.includes(barCodeInput.value.toUpperCase())) ||
                    (item.display && item.display.toUpperCase().includes(barCodeInput.value.toUpperCase()))
                );
                
                if (filteredItems.length > 0) {
                    // Prendiamo il primo risultato
                    barCodeInput.value = filteredItems[0].display;
                    barcodeAutocompleteList.classList.add("hidden");
                    quantitaInput.value = filteredItems[0].bookInv;
                    quantitaInput.disabled = false;
                    quantitaInput.focus();
                } else {
                    barCodeInput.value = "";
                    quantitaInput.value = "";
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
        const selectedBarcode = barcodeList.find(item => item.display === barCodeInput.value);
        const quantita = quantitaInput.value;

        if(selectedBarcode && quantita)
        {
            var data = {
                item: selectedBarcode.item,
                barCode: selectedBarcode.barCode,
                description: selectedBarcode.description,
                bookInv: quantita
            };
        }
        dataResultList.push(data);
        addToTemporaryList(data, dataResultList);
        barCodeInput.value = "";
        quantitaInput.value = "";
        quantitaInput.disabled = true;
        barcodeAutocompleteList.classList.add("hidden");
        barCodeInput.focus();
    });
});

function addToTemporaryList(data, dataResultList) {
    const list = document.getElementById("inv-lista-temp");
    const noContent = document.getElementById("nocontent");
    const newItem = document.createElement("li");
    newItem.classList.add("just-added"); // Aggiungi classe per l'animazione

    newItem.innerHTML = `
        <div class="item-content"><div><spam class="item-content-heading">Item:</spam> ${data.item} ${data.barCode === "" ? "" : "- <spam class='item-content-heading'>Code:</spam>" + data.barCode}</div>
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

async function getAllItems(){
    const response = await fetchWithAuth("http://localhost:5245/api/giacenze/get_all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json"
        }
    });

    if (!response.ok) {
        throw new Error("Failed to fetch items");
    }

    const data = await response.json();
    return data;
}