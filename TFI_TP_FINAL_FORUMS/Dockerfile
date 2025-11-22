# =========================
#   STAGE 1: BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el repo
COPY . .

# RUTA REAL del proyecto de la API de foros
# (coincide con lo que mostraste en la captura)
RUN dotnet restore "01.Web/ApiForums/ApiForums.csproj"

RUN dotnet publish "01.Web/ApiForums/ApiForums.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# =========================
#   STAGE 2: RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# La API va a escuchar en el puerto 8080 dentro del contenedor
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Variables sensibles que vas a inyectar desde Dokploy
ARG EmailConfiguration__EmailSMTPConfiguration__Password
ARG AI__ApiKey

ENV EmailConfiguration__EmailSMTPConfiguration__Password=${EmailConfiguration__EmailSMTPConfiguration__Password}
ENV AI__ApiKey=${AI__ApiKey}

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiForums.dll"]
