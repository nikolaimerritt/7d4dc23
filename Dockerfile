FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Installing Node
RUN apt-get update && apt-get install -y software-properties-common npm
RUN npm install npm@latest -g && \
    npm install n -g && \
    n latest

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN npm i
RUN npm run build-ts
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
COPY --from=build-env App/wwwroot .
COPY --from=build-env App/ctfchallenge.db .
ENTRYPOINT ["dotnet", "PirateConquest.dll"]
