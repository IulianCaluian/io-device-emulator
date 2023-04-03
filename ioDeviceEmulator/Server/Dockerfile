#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ioDeviceEmulator/Server/ioDeviceEmulator.Server.csproj", "ioDeviceEmulator/Server/"]
COPY ["ioDeviceEmulator/Client/ioDeviceEmulator.Client.csproj", "ioDeviceEmulator/Client/"]
COPY ["ioDeviceEmulator/Shared/ioDeviceEmulator.Shared.csproj", "ioDeviceEmulator/Shared/"]
RUN dotnet restore "ioDeviceEmulator/Server/ioDeviceEmulator.Server.csproj"
COPY . .
WORKDIR "/src/ioDeviceEmulator/Server"
RUN dotnet build "ioDeviceEmulator.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ioDeviceEmulator.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ioDeviceEmulator.Server.dll"]