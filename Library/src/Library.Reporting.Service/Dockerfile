FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["./Library.Reporting.Service/Library.Reporting.Service.csproj", "./Library.Reporting.Service/"]
RUN dotnet restore "./Library.Reporting.Service/Library.Reporting.Service.csproj"
COPY . .
WORKDIR "/src/Library.Reporting.Service"
RUN dotnet build "./Library.Reporting.Service.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "./Library.Reporting.Service.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.Reporting.Service.dll"]
