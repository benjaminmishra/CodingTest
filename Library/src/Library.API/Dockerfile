# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./Library.API.csproj ./Library.API/
RUN dotnet restore ./Library.API/

# copy everything else and build app
COPY ./ ./Library.API/
WORKDIR /source/Library.API/
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Library.API.dll"]