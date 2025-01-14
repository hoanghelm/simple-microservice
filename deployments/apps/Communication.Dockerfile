#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Communication/Communication.Consumer/Communication.Consumer.csproj", "Communication/Communication.Consumer/"]
COPY ["Shared/Mail/Mail.csproj", "Shared/Mail/"]
COPY ["Shared/Common/Common.csproj", "Shared/Common/"]
COPY ["Communication/Communication.Services/Communication.Services.csproj", "Communication/Communication.Services/"]
RUN dotnet restore "Communication/Communication.Consumer/Communication.Consumer.csproj"
COPY . .
WORKDIR "/src/Communication/Communication.Consumer"
RUN dotnet build "Communication.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Communication.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Communication.Consumer.dll"]