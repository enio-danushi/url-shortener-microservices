# Build stage: the full .NET SDK compiles and publishes the service.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, from the files that list dependencies. They change rarely,
# so this layer stays cached while you edit code.
COPY global.json ./
COPY src/Links.Api/Links.Api.csproj src/Links.Api/
RUN dotnet restore src/Links.Api/Links.Api.csproj

# Then the source code, which changes on almost every commit.
COPY src/Links.Api/ src/Links.Api/
RUN dotnet publish src/Links.Api/Links.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Final stage: only the ASP.NET Core runtime plus the published output.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# curl, for the health check Compose runs inside the container. The .NET images
# leave it out; installing it before the COPY keeps this layer cached.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Links.Api.dll"]