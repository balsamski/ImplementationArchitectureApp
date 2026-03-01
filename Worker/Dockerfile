FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Worker/Worker.csproj", "Worker/"]
RUN dotnet restore "Worker/Worker.csproj"
COPY . .
WORKDIR "/src/Worker"
RUN dotnet publish "Worker.csproj" -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
WORKDIR /home/site/wwwroot
COPY --from=build /home/site/wwwroot .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
