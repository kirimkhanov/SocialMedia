FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SocialMedia/SocialMedia.csproj", "SocialMedia/"]
COPY ["SocialMedia.Core/SocialMedia.Core.csproj", "SocialMedia.Core/"]
COPY ["SocialMedia.Infrastructure/SocialMedia.Infrastructure.csproj", "SocialMedia.Infrastructure/"]
RUN dotnet restore "SocialMedia/SocialMedia.csproj"
COPY . .
WORKDIR "/src/SocialMedia"
RUN dotnet build "SocialMedia.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialMedia.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialMedia.dll"]
