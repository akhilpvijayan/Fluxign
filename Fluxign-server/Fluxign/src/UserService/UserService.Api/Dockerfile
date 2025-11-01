FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files required for restore
COPY UserService.Api/*.csproj UserService.Api/
COPY UserService.Application/*.csproj UserService.Application/
COPY UserService.Domain/*.csproj UserService.Domain/
COPY UserService.Persistence/*.csproj UserService.Persistence/
COPY UserService.Infrastructure/*.csproj UserService.Infrastructure/

RUN dotnet restore UserService.Api/UserService.Api.csproj

# Copy everything else
COPY . .

# Build in Debug mode
RUN dotnet build UserService.Api/UserService.Api.csproj -c Debug -o /app/build
# RUN dotnet build UserService.Api/UserService.Api.csproj -c Release -o /app/build

FROM build AS publish

# Publish in Debug mode
RUN dotnet publish UserService.Api/UserService.Api.csproj -c Debug -o /app/publish
# RUN dotnet publish UserService.Api/UserService.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
