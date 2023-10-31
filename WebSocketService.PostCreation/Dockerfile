FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebSocketService.PostCreation/WebSocketService.PostCreation.csproj", "WebSocketService.PostCreation/"]
RUN dotnet restore "WebSocketService.PostCreation/WebSocketService.PostCreation.csproj"
COPY . .
WORKDIR "/src/WebSocketService.PostCreation"
RUN dotnet build "WebSocketService.PostCreation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebSocketService.PostCreation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebSocketService.PostCreation.dll"]
