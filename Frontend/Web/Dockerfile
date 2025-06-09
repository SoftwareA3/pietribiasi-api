# Usa un'immagine Nginx ufficiale e leggera
FROM nginx:alpine
# Rimuovi la configurazione di default
RUN rm /etc/nginx/conf.d/default.conf
# Copia la tua configurazione personalizzata (la creiamo al prossimo punto)
COPY nginx.conf /etc/nginx/conf.d
# Copia i file del tuo frontend nella cartella servita da Nginx
COPY . /usr/share/nginx/html
# La porta che il frontend espone INTERNAMENTE al container
EXPOSE 80