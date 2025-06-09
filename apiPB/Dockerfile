# Fase 1: Build dell'applicazione
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Backend/apiPB.csproj", "Backend/"]
# Potrebbero esserci altri progetti da copiare se apiPB ha dipendenze
RUN dotnet restore "Backend/apiPB.csproj"
COPY . .
WORKDIR "/src/Backend"
RUN dotnet build "apiPB.csproj" -c Release -o /app/build

# Fase 2: Pubblicazione
FROM build AS publish
RUN dotnet publish "apiPB.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase 3: Creazione dell'immagine finale
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
# La porta che il backend espone INTERNAMENTE al container
EXPOSE 8080 
ENTRYPOINT ["dotnet", "apiPB.dll"]