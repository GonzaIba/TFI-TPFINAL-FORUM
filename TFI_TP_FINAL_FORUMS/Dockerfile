# =======================
#   STAGE 1: BUILD
# =======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# "TFI_TP_FINAL_FORUMS/01.Web/ApiForums/ApiForums.csproj"
# "01.Web/ApiForums/ApiForums.csproj"
COPY . .

# 🔧 MISMO path que arriba
RUN dotnet restore "TFI_TP_FINAL_FORUMS/01.Web/ApiForums/ApiForums.csproj"

RUN dotnet publish "TFI_TP_FINAL_FORUMS/01.Web/ApiForums/ApiForums.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false


# =======================
#   STAGE 2: RUNTIME
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

# 🔧 Acá va el nombre REAL del dll de tu API
ENTRYPOINT ["dotnet", "ApiForums.dll"]
