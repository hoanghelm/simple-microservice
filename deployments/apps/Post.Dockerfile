#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Post/Post.API/Post.API.csproj", "Post/Post.API/"]
COPY ["Post/Post.Infrastructure/Post.Infrastructure.csproj", "Post/Post.Infrastructure/"]
COPY ["Post/Post.Service/Post.Service.csproj", "Post/Post.Service/"]
COPY ["Post/Post.Data/Post.Data.csproj", "Post/Post.Data/"]
COPY ["Post/Post.Domain/Post.Domain.csproj", "Post/Post.Domain/"]
COPY ["Shared/Paginate/Paginate.csproj", "Shared/Paginate/"]
COPY ["Shared/Common/Common.csproj", "Shared/Common/"]
COPY ["Shared/Logging/Logging.csproj", "Shared/Logging/"]
RUN dotnet restore "Post/Post.API/Post.API.csproj"
COPY . .
WORKDIR "/src/Post/Post.API"
RUN dotnet build "Post.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.API.dll"]