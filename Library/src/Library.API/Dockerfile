FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["./Library.API/Library.API.csproj", "./Library.API/"]
RUN dotnet restore "./Library.API/Library.API.csproj"
COPY . .
WORKDIR "/src/Library.API"
RUN dotnet build "./Library.API.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "./Library.API.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.API.dll"]
