# ETAPA 1: BUILD (Usamos el SDK completo sobre Alpine)
# Especificamos la versi�n exacta de .NET y el SO (3.19 es la versi�n de Alpine)
FROM mcr.microsoft.com/dotnet/sdk:8.0.201-alpine3.19 AS build
WORKDIR /app

# 1. Copiamos solo los archivos de proyecto para aprovechar el "Layer Caching"
# Esto hace que el 'dotnet restore' sea instant�neo si no has agregado paquetes NuGet
COPY ["src/Presentation/Api/Api.csproj", "src/Presentation/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

# Restauramos dependencias espec�ficamente para Linux-musl (el 'apellido' de Alpine)
RUN dotnet restore "src/Presentation/Api/Api.csproj" -r linux-musl-x64

# 2. Copiamos el resto del c�digo y publicamos
COPY . .
WORKDIR "/app/src/Presentation/Api"
# Publicamos como 'Self-Contained: false' porque el runtime ya estar� en la imagen final
RUN dotnet publish "Api.csproj" -c Release -o /app/publish \
    --no-restore \
    -r linux-musl-x64 \
    --self-contained false

# ETAPA 2: RUNTIME (La imagen que realmente viajar� a Azure)
FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-alpine3.19 AS final
WORKDIR /app

# 3. Instalamos 'icu-libs' para que .NET soporte Globalizaci�n (fechas/monedas en espa�ol)
# Alpine no trae esto por defecto para ahorrar espacio
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copiamos solo lo necesario de la etapa anterior
COPY --from=build /app/publish .

# Exponemos el puerto est�ndar de .NET 8 (8080)
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]