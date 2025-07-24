export function createPagination(ulSelector, itemsPerPage = 6, maxPageButtons = 5, labels = {
    prev: 'Precedente',
    next: 'Successiva',
    ellipsis: '...'
}) {
    // Seleziona l'elemento UL
    const ulElement = document.querySelector(ulSelector);
    if (!ulElement) {
        console.error(`Elemento ${ulSelector} non trovato`);
        return;
    }
    
    // Ottiene tutti gli elementi LI
    const items = Array.from(ulElement.querySelectorAll('li'));
    const totalItems = items.length;
    console.log("Elementi totali: " + totalItems);
    const totalPages = Math.ceil(totalItems / itemsPerPage);
    console.log("Pagine totali: " + totalPages);
    
    // Se non ci sono abbastanza elementi per la paginazione, esce
    if (totalItems <= itemsPerPage) {
        return;
    }

    // Se c'è solo una pagina, non mostra la paginazione
    if (totalPages === 1) {
        console.log("Pagine totali: " + totalPages);
        console.log("Nessuna paginazione da mostrare");
        return;
    }
    
    // Variabile per tenere traccia della pagina corrente
    let currentPage = 1;
    
    // Crea il container per i controlli di paginazione
    const paginationContainer = document.createElement('div');
    paginationContainer.className = 'pagination-controls';
    ulElement.parentNode.insertBefore(paginationContainer, ulElement.nextSibling);
    
    // Funzione per mostrare gli elementi della pagina corrente
    function showCurrentPage() {
        const startIndex = (currentPage - 1) * itemsPerPage;
        const endIndex = Math.min(startIndex + itemsPerPage, totalItems);
        
        // Nasconde tutti gli elementi
        items.forEach(item => item.style.display = 'none');
        
        // Mostra solo gli elementi della pagina corrente
        for (let i = startIndex; i < endIndex; i++) {
            items[i].style.display = '';
        }
        
        // Aggiorna i controlli di paginazione
        updatePaginationControls();
    }
    
    // Funzione per aggiornare i controlli di paginazione
    function updatePaginationControls() {
        // Ricostruisce sempre i controlli per gestire correttamente ellipsis e visibilità
        paginationContainer.innerHTML = '';

        // Pulsante pagina precedente
        const prevButton = createButton(labels.prev);
        prevButton.className += ' next-prev-button';
        prevButton.disabled = currentPage === 1;
        prevButton.addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                showCurrentPage();
            }
        });
        paginationContainer.appendChild(prevButton);

        // Logica migliorata per la visualizzazione dei pulsanti pagina
        if (totalPages <= maxPageButtons) {
            // Se il numero totale di pagine è minore o uguale al massimo, mostra tutte
            for (let i = 1; i <= totalPages; i++) {
                paginationContainer.appendChild(createPageButton(i));
            }
        } else {
            // Logica complessa per gestire ellipsis
            const sideButtons = Math.floor((maxPageButtons - 3) / 2); // Pulsanti ai lati (escludendo prima, ultima e corrente)
            
            // Sempre visibile: prima pagina
            paginationContainer.appendChild(createPageButton(1));
            
            let startPage, endPage;
            let showStartEllipsis = false;
            let showEndEllipsis = false;
            
            if (currentPage <= sideButtons + 2) {
                // Siamo nelle prime pagine
                startPage = 2;
                endPage = Math.min(maxPageButtons - 1, totalPages - 1);
                showEndEllipsis = endPage < totalPages - 1;
            } else if (currentPage >= totalPages - sideButtons - 1) {
                // Siamo nelle ultime pagine
                startPage = Math.max(2, totalPages - maxPageButtons + 2);
                endPage = totalPages - 1;
                showStartEllipsis = startPage > 2;
            } else {
                // Siamo nel mezzo
                startPage = Math.max(2, currentPage - sideButtons);
                endPage = Math.min(totalPages - 1, currentPage + sideButtons);
                showStartEllipsis = startPage > 2;
                showEndEllipsis = endPage < totalPages - 1;
                
                // Assicuriamoci che la pagina corrente sia sempre visibile
                if (currentPage < startPage) {
                    startPage = currentPage;
                }
                if (currentPage > endPage) {
                    endPage = currentPage;
                }
            }
            
            // Ellipsis iniziale
            if (showStartEllipsis) {
                const ellipsis = document.createElement('span');
                ellipsis.textContent = labels.ellipsis;
                ellipsis.className = 'pagination-ellipsis';
                paginationContainer.appendChild(ellipsis);
            }
            
            // Pulsanti centrali
            for (let i = startPage; i <= endPage; i++) {
                paginationContainer.appendChild(createPageButton(i));
            }
            
            // Ellipsis finale
            if (showEndEllipsis) {
                const ellipsis = document.createElement('span');
                ellipsis.textContent = labels.ellipsis;
                ellipsis.className = 'pagination-ellipsis';
                paginationContainer.appendChild(ellipsis);
            }
            
            // Sempre visibile: ultima pagina (se non è la prima)
            if (totalPages > 1) {
                paginationContainer.appendChild(createPageButton(totalPages));
            }
        }

        // Pulsante pagina successiva
        const nextButton = createButton(labels.next);
        nextButton.className += ' next-prev-button';
        nextButton.disabled = currentPage === totalPages;
        nextButton.addEventListener('click', () => {
            if (currentPage < totalPages) {
                currentPage++;
                showCurrentPage();
            }
        });
        paginationContainer.appendChild(nextButton);
    }
    
    // Funzione per creare un pulsante generico
    function createButton(text) {
        const button = document.createElement('button');
        button.textContent = text;
        button.className = 'pagination-button';
        return button;
    }
    
    // Funzione per creare un pulsante pagina numerato
    function createPageButton(pageNum) {
        const button = createButton(pageNum);
        if (pageNum === currentPage) {
            button.className += ' active';
        }
        button.addEventListener('click', () => {
            currentPage = pageNum;
            showCurrentPage();
        });
        return button;
    }
    
    // Inizializza la visualizzazione
    showCurrentPage();
    
    // Restituisce un oggetto con metodi pubblici
    return {
        goToPage: (pageNum) => {
            if (pageNum >= 1 && pageNum <= totalPages) {
                currentPage = pageNum;
                showCurrentPage();
            }
        },
        nextPage: () => {
            if (currentPage < totalPages) {
                currentPage++;
                showCurrentPage();
            }
        },
        prevPage: () => {
            if (currentPage > 1) {
                currentPage--;
                showCurrentPage();
            }
        },
        getCurrentPage: () => currentPage,
        getTotalPages: () => totalPages,
        refresh: () => showCurrentPage(),
        updatePaginationControls: () => updatePaginationControls()
    };
}