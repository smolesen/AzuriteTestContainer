# Use dotnet runtime deps to gather all dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env

WORKDIR /app


ENV TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal

COPY ["AzuriteTestContainer.sln", "./"]
COPY ["AzuriteTestContainer/AzuriteTestContainer.csproj", "AzuriteTestContainer/"]

RUN dotnet restore "AzuriteTestContainer.sln"

COPY ["AzuriteTestContainer/*", "AzuriteTestContainer/"]


WORKDIR "AzuriteTestContainer"

CMD ["dotnet", "test", "--no-restore"]

