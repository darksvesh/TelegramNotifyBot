FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Установите необходимые зависимости
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libssl1.1 \
    && rm -rf /var/lib/apt/lists/*

# Копирование csproj и восстановление зависимостей
COPY ["TelegramNotifyBot.csproj", "./"]
RUN ls -la
RUN dotnet restore --verbosity detailed

# Копирование остального кода и сборка проекта
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS runtime

# Установите необходимые зависимости
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libssl1.1 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "TelegramNotifyBot.dll"]