FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WindowsAuthFileExplorer/WindowsAuthFileExplorer.csproj", "WindowsAuthFileExplorer/"]
RUN dotnet restore "WindowsAuthFileExplorer/WindowsAuthFileExplorer.csproj"
COPY . .
WORKDIR "/src/WindowsAuthFileExplorer"
RUN dotnet build "WindowsAuthFileExplorer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WindowsAuthFileExplorer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WindowsAuthFileExplorer.dll"]
