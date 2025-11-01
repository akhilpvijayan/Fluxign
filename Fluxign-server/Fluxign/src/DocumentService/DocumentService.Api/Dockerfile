FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files required for restore
COPY DocumentService.Api/*.csproj DocumentService.Api/
COPY DocumentService.Application/*.csproj DocumentService.Application/
COPY DocumentService.Domain/*.csproj DocumentService.Domain/
COPY DocumentService.Persistence/*.csproj DocumentService.Persistence/
COPY DocumentService.Infrastructure/*.csproj DocumentService.Infrastructure/

RUN dotnet restore DocumentService.Api/DocumentService.Api.csproj

# Copy everything else
COPY . .

# Build in Debug mode
RUN dotnet build DocumentService.Api/DocumentService.Api.csproj -c Debug -o /app/build
# RUN dotnet build DocumentService.Api/DocumentService.Api.csproj -c Release -o /app/build

FROM build AS publish

# Publish in Debug mode
RUN dotnet publish DocumentService.Api/DocumentService.Api.csproj -c Debug -o /app/publish
# RUN dotnet publish DocumentService.Api/DocumentService.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "DocumentService.Api.dll"]
