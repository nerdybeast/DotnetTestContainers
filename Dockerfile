FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet publish DotnetTestContainers/ -c Release -f net8.0 -o /out

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS runtime
COPY --from=build /out /home/site/wwwroot
