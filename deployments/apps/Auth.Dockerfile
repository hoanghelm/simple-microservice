#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Auth/Auth.API/Auth.API.csproj", "Auth/Auth.API/"]
COPY ["Auth/Auth.Infrastructure/Auth.Infrastructure.csproj", "Auth/Auth.Infrastructure/"]
COPY ["Auth/Auth.Data/Auth.Data.csproj", "Auth/Auth.Data/"]
COPY ["Auth/Auth.Domain/Auth.Domain.csproj", "Auth/Auth.Domain/"]
COPY ["Shared/Common/Common.csproj", "Shared/Common/"]
COPY ["Auth/Auth.Service/Auth.Service.csproj", "Auth/Auth.Service/"]
RUN dotnet restore "Auth/Auth.API/Auth.API.csproj"
COPY . .
WORKDIR "/src/Auth/Auth.API"
RUN dotnet build "Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auth.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.API.dll"]