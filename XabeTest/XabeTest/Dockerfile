#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["XabeTest/XabeTest.csproj", "XabeTest/"]
RUN dotnet restore "XabeTest/XabeTest.csproj"
COPY . .
WORKDIR "/src/XabeTest"
RUN dotnet build "XabeTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XabeTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get install -y ffmpeg
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XabeTest.dll"]