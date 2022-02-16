FROM mcr.microsoft.com/dotnet/sdk:latest as build-env
WORKDIR /app
COPY . .
RUN dotnet restore --nologo --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json --verbosity minimal
RUN dotnet publish NetworkTest.WorkerService/NetworkTest.WorkerService.csproj --configuration Release --nologo --output /app/publish --verbosity minimal

FROM mcr.microsoft.com/dotnet/runtime:latest
ENV DOTNET_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "NetworkTest.WorkerService.dll"]
