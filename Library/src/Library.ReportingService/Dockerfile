FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["./Library.ReportingService/Library.ReportingService.csproj", "./Library.ReportingService/"]
RUN dotnet restore "./Library.ReportingService/Library.ReportingService.csproj"
COPY . .
WORKDIR "/src/Library.ReportingService"
RUN dotnet build "./Library.ReportingService.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "./Library.ReportingService.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.ReportingService.dll"]
