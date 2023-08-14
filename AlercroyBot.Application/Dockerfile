FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AlercroyBot.Application/AlercroyBot.Application.csproj", "AlercroyBot.Application/"]
RUN dotnet restore "AlercroyBot.Application/AlercroyBot.Application.csproj"
COPY . .
WORKDIR "/src/AlercroyBot.Application"
RUN dotnet build "AlercroyBot.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlercroyBot.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlercroyBot.Application.dll"]
