#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["achieve-ADagent.csproj", "achieve-ADagent/"]
RUN dotnet restore "achieve-ADagent/achieve-ADagent.csproj"
COPY . .

WORKDIR "/src/achieve-ADagent"
COPY . .
RUN dotnet build "achieve-ADagent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "achieve-ADagent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "achieve-ADagent.dll"]