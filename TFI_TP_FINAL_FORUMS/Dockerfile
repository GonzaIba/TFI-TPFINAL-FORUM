# =========================
#  STAGE 1: BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el código de la solución
COPY . .

# OJO: ruta correcta al csproj dentro de TFI_TP_FINAL_FORUMS
RUN dotnet restore "01.Web/ApiForums/ApiForums.csproj"

RUN dotnet publish "01.Web/ApiForums/ApiForums.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
#  STAGE 2: RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiamos lo publicado desde el stage de build
COPY --from=build /app/publish .

# Puerto interno de la API
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Nombre del DLL de tu API
ENTRYPOINT ["dotnet", "ApiForums.dll"]
