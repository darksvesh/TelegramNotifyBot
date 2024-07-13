FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libssl1.1 \
    && rm -rf /var/lib/apt/lists/*

COPY ["TelegramNotifyBot.csproj", "./"]
RUN ls -la
RUN dotnet restore --verbosity detailed

COPY . ./
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS runtime

RUN rm -rf /var/cache/apk/*
RUN apk add libssl3 

WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000

HEALTHCHECK --interval=5s --timeout=5s --start-period=5s --retries=3 \
  CMD curl --fail -m 5 http://localhost:5000/healthcheck || exit 1

ENTRYPOINT ["dotnet", "TelegramNotifyBot.dll"]