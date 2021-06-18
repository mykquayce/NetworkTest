# Network Test
## Settings
1. `appsettings.json`
1. environment variables
1. user secrets (id: `dotnet-NetworkTest.WorkerService-A2357F95-287D-411F-8BB9-BA4CBDCDBD60`)
1. Docker secrets

|Key|Type|Default Value|Description|
|:---|:---:|:---:|:---|
|Interval|int|1800000|time between tests|
|Target|string|lse.packetlosstest.com|where to ping|
|Ping:Timeout|int|2000||
|Database:Server|string|localhost||
|Database:Database|string|networktest||
|Database:Port|uint|3306||
|Database:UserId|string|networktest||
|Database:Password|string|password||
