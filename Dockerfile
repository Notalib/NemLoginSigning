FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && apt-get upgrade -y && apt-get install -y locales-all

# Add Nota CA certificate (Bux https for dev is signed by this)
COPY cert/root/*.crt /usr/local/share/ca-certificates
RUN update-ca-certificates

# Create non-root user  
RUN adduser --disabled-password --disabled-login --gecos "" service

# To avoid anymore NemID/.NET Culture issues, define Danish locale.
ENV LANG="en_DK.UTF-8"
ENV LC_ALL="en_DK.UTF-8"

#---------------------------
#| Compile signing service |
#---------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
ADD ./ /src

RUN --mount=type=secret,id=NuGet.Config dotnet restore NemLoginSigningService.sln --configfile /run/secrets/NuGet.Config

RUN dotnet publish /src/src/NemLoginSigningWebApp/NemLoginSigningWebApp.csproj -c Release -o /build

#------------------------
#| Build runtime Image  |
#------------------------
FROM base AS runtime

# Copy over build binaries
COPY --from=build /build /app/
COPY cert /app/cert

USER service
WORKDIR /app

# Set some defaults, override from docker compose file
ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://*:5000"

# Image entrypoint
STOPSIGNAL SIGTERM
EXPOSE 5000
EXPOSE 5001

CMD dotnet NemLoginSigningWebApp.dll
