FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Stage 1: Build sources
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libssl1.1 \
        curl \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY ["TelegramNotifyBot.csproj", "./"]
RUN ls -la
RUN dotnet restore --verbosity detailed

COPY . ./
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS runtime

RUN rm -rf /var/cache/apk/*
RUN apk add --no-cache \  
    curl \
    ca-certificates \
    libssl3 \ 
    && update-ca-certificates
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000

HEALTHCHECK --interval=5s --timeout=5s --start-period=5s --retries=3 \
    CMD curl --fail -m 5 http://127.0.0.1:5000/healthcheck || kill 1

ENTRYPOINT ["dotnet", "TelegramNotifyBot.dll"]