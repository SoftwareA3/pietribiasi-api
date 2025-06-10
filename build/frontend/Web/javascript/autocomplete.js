// Funzione per l'autocompletamento
// InputElement: input dove l'utente scrive
// ListElement: div dove mostrare i risultati
// List: array di oggetti da cui filtrare i risultati. list dev'essere un array di oggetti. 
// Ogni oggetto deve avere una proprietà "display" che contiene la stringa da mostrare nell'autocompletamento.
export function setupAutocomplete(inputElement, listElement, list) {
    let currentFocus = -1;

    
    list = list.filter((item, index, self) =>
        index === self.findIndex((t) => t.display === item.display)
    );

    // Gestisce il focus sull'input
    inputElement.addEventListener("focus", function() {
        showAllItems();
    });

    // Event listener per l'input
    inputElement.addEventListener("input", function() {
        const value = this.value.toString().toUpperCase();
        
        // Chiude la lista esistente
        listElement.innerHTML = "";
        
        if (!value) {
            showAllItems();
            return;
        }
        
        // Filtra gli elementi che corrispondono all'input
        const matchingItems = list.filter(item => 
            item && item.display && item.display.toString().toUpperCase().includes(value)
        );

        // Resetta l'indice del focus
        currentFocus = -1;
        listElement.classList.remove("hidden");

        // Aggiunge gli elementi alla lista
        matchingItems.forEach((item) => {
            const itemDiv = document.createElement("div");
            const itemStr = item.display.toString();

            // Evidenzia la parte corrispondente
            const index = itemStr.toUpperCase().indexOf(value);
            if (index !== -1) {
                itemDiv.innerHTML = itemStr.substring(0, index);
                itemDiv.innerHTML += "<strong>" + itemStr.substring(index, index + value.length) + "</strong>";
                itemDiv.innerHTML += itemStr.substring(index + value.length);
            } else {
                itemDiv.textContent = itemStr;
            }

            // Event listener per il click sull'elemento
            itemDiv.addEventListener("click", function() {
                inputElement.value = itemStr;
                closeAutocompleteList();
                
                setTimeout(() => {
                    // Simula la pressione del tasto Enter
                    const event = new KeyboardEvent("change", { bubbles: true });
                    inputElement.dispatchEvent(event);
                }, 100);
            });

            listElement.appendChild(itemDiv);
        });
        
        if (listElement.children.length > 3) {
            listElement.classList.add("scrollable");
        }
    });

    // Funzione per mostrare tutti gli elementi
    function showAllItems() {
        listElement.innerHTML = "";
        listElement.classList.remove("hidden");
        
        list.forEach((item) => {
            const itemDiv = document.createElement("div");
            itemDiv.textContent = item.display;
            
            itemDiv.addEventListener("click", function() {
                inputElement.value = item.display;
                closeAutocompleteList();
                
                setTimeout(() => {
                    // Simula la pressione del tasto Enter
                    const event = new KeyboardEvent("change", { bubbles: true });
                    inputElement.dispatchEvent(event);
                }, 100);
            });
            
            listElement.appendChild(itemDiv);
        });
        
        if (listElement.children.length > 3) {
            listElement.classList.add("scrollable");
        }
    }

    // Event listener per la navigazione con tastiera
    inputElement.addEventListener("keydown", function(e) {
        const items = listElement.getElementsByTagName("div");

        // Backspace
        if (e.key === "Backspace") {
            const event = new Event('input', { bubbles: true });
            inputElement.dispatchEvent(event);
        }

        // Freccia giù
        if (e.key === "ArrowDown") {
            currentFocus++;
            addActive(items);
            e.preventDefault();
        } 
        // Freccia su
        else if (e.key === "ArrowUp") {
            currentFocus--;
            addActive(items);
            e.preventDefault();
        } 
        // Invio
        else if (e.key === "Enter") {
            e.preventDefault();
            if (currentFocus > -1 && items.length > 0) {
                items[currentFocus].click();
            }
        } 
        // Esc
        else if (e.key === "Escape") {
            closeAutocompleteList();
        }
    });

    // Focus out: chiude la lista quando l'utente clicca altrove
    document.addEventListener("click", function(e) {
        if (e.target !== inputElement) {
            closeAutocompleteList();
        }
    });

    // Funzione per evidenziare l'elemento attivo
    function addActive(items) {
        if (!items || items.length === 0) return;

        // Rimuovi prima la classe active da tutti gli elementi
        removeActive(items);

        // Corregge l'indice se necessario
        if (currentFocus >= items.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = items.length - 1;

        // Aggiunge la classe active all'elemento corrente
        items[currentFocus].classList.add("autocomplete-active");

        // Assicura che l'elemento sia visibile (scroll)
        items[currentFocus].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }

    // Rimuove la classe active da tutti gli elementi
    function removeActive(items) {
        for (let i = 0; i < items.length; i++) {
            items[i].classList.remove("autocomplete-active");
        }
    }

    // Chiude la lista di autocompletamento
    function closeAutocompleteList() {
        listElement.classList.add("hidden");
    }
}

// Funzione personalizzata per l'autocompletamento che mostra solo 5 elementi
export function setupCustomAutocomplete(inputElement, listElement, list) {
    let currentFocus = -1;
    const MAX_ITEMS = 5;
    
    list = list.filter((item, index, self) =>
        index === self.findIndex((t) => t.display === item.display)
    );

    // Event listener per l'input
    inputElement.addEventListener("input", function() {
        const value = this.value.toString().toUpperCase();
        
        // Chiude la lista esistente
        listElement.innerHTML = "";
        
        if (!value) {
            // Non mostriamo niente se non c'è input
            listElement.classList.add("hidden");
            return;
        }
        
        // Filtra gli elementi che corrispondono all'input
        const matchingItems = list.filter(item => 
            item && item.display && item.display.toString().toUpperCase().includes(value)
        ).slice(0, MAX_ITEMS); // Limita a MAX_ITEMS risultati

        // Resetta l'indice del focus
        currentFocus = -1;
        
        if (matchingItems.length > 0) {
            listElement.classList.remove("hidden");
            
            // Aggiunge gli elementi alla lista
            matchingItems.forEach((item) => {
                const itemDiv = document.createElement("div");
                const itemStr = item.display.toString();
    
                // Evidenzia la parte corrispondente
                const index = itemStr.toUpperCase().indexOf(value);
                if (index !== -1) {
                    itemDiv.innerHTML = itemStr.substring(0, index);
                    itemDiv.innerHTML += "<strong>" + itemStr.substring(index, index + value.length) + "</strong>";
                    itemDiv.innerHTML += itemStr.substring(index + value.length);
                } else {
                    itemDiv.textContent = itemStr;
                }
    
                // Event listener per il click sull'elemento
                itemDiv.addEventListener("click", function() {
                    listElement.classList.add("hidden");

                    inputElement.value = itemStr;
                    
                    setTimeout(() => {
                        // Simula la pressione del tasto Enter
                        const event = new KeyboardEvent("change", { bubbles: true });
                        inputElement.dispatchEvent(event);
                    }, 300);
                });
    
                listElement.appendChild(itemDiv);
            });
            
            if (listElement.children.length > 3) {
                listElement.classList.add("scrollable");
            }
        } else {
            listElement.classList.add("hidden");
        }
    });

    // Event listener per la navigazione con tastiera
    inputElement.addEventListener("keydown", function(e) {
        const items = listElement.getElementsByTagName("div");
        if (items.length === 0) return;

        // Backspace
        if (e.key === "Backspace") {
            const event = new Event('input', { bubbles: true });
            inputElement.dispatchEvent(event);
        }

        // Freccia giù
        if (e.key === "ArrowDown") {
            currentFocus++;
            addActive(items);
            e.preventDefault();
        } 
        // Freccia su
        else if (e.key === "ArrowUp") {
            currentFocus--;
            addActive(items);
            e.preventDefault();
        } 
        // Invio
        else if (e.key === "Enter" && !listElement.classList.contains("hidden")) {
            e.preventDefault();
            if (currentFocus > -1 && items.length > 0) {
                items[currentFocus].click();
            }
        } 
        // Esc
        else if (e.key === "Escape") {
            listElement.classList.add("hidden");
        }
    });

    // Funzione per evidenziare l'elemento attivo
    function addActive(items) {
        if (!items || items.length === 0) return;

        // Rimuovi prima la classe active da tutti gli elementi
        removeActive(items);

        // Corregge l'indice se necessario
        if (currentFocus >= items.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = items.length - 1;

        // Aggiunge la classe active all'elemento corrente
        items[currentFocus].classList.add("autocomplete-active");

        // Assicura che l'elemento sia visibile (scroll)
        items[currentFocus].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }

    // Rimuove la classe active da tutti gli elementi
    function removeActive(items) {
        for (let i = 0; i < items.length; i++) {
            items[i].classList.remove("autocomplete-active");
        }
    }
}