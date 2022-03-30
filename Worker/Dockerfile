FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

RUN --mount=type=secret,id=ca_crt,dst=/usr/local/share/ca-certificates/ca.crt \
	/usr/sbin/update-ca-certificates

WORKDIR /app
COPY . .
RUN dotnet restore --nologo --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json --verbosity minimal
RUN dotnet publish NetworkTest.WorkerService/NetworkTest.WorkerService.csproj --configuration Release --nologo --output /app/publish --verbosity minimal

FROM mcr.microsoft.com/dotnet/runtime:6.0
ENV DOTNET_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "NetworkTest.WorkerService.dll"]
