FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY . .
RUN dotnet restore --nologo --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json --verbosity minimal
RUN dotnet publish NetworkTest.WorkerService/NetworkTest.WorkerService.csproj --configuration Release --nologo --output /app/publish --runtime linux-x64 --verbosity minimal

FROM mcr.microsoft.com/dotnet/runtime:6.0
ENV DOTNET_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "NetworkTest.WorkerService.dll"]
