#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MetaBenchmark/Server/MetaBenchmark.Server.csproj", "MetaBenchmark/Server/"]
COPY ["MetaBenchmark/Client/MetaBenchmark.Client.csproj", "MetaBenchmark/Client/"]
COPY ["MetaBenchmark/Shared/MetaBenchmark.Shared.csproj", "MetaBenchmark/Shared/"]
RUN dotnet restore "MetaBenchmark/Server/MetaBenchmark.Server.csproj"
COPY . .
WORKDIR "/src/MetaBenchmark/Server"
RUN dotnet build "MetaBenchmark.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MetaBenchmark.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MetaBenchmark.Server.dll"]