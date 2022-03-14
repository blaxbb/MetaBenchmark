FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY .git/ ./.git/
COPY MetaBenchmark.sln .
COPY MetaBenchmark/MetaBenchmark.csproj ./MetaBenchmark/MetaBenchmark.csproj
RUN dotnet restore MetaBenchmark.sln
COPY . .
RUN dotnet build MetaBenchmark.sln -c Release -o /app/build

FROM build AS publish
RUN dotnet publish MetaBenchmark.sln -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY MetaBenchmark/nginx.conf /etc/nginx/nginx.conf
