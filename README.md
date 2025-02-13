# Payment Processing System

## Overview

This is a simple payment processing system that uses RabbitMQ as a message broker and CosmosDB as a database.

## Getting Started

### RabbitMQ

Ensure you have Docker installed on your machine [Docker Desktop](https://www.docker.com/products/docker-desktop)

Download [RabbitMQ Docker Image](https://hub.docker.com/_/rabbitmq)

```bash
docker pull rabbitmq:4-management
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

[RabbitMQ Management Console](http://localhost:15672) with default credentials `guest:guest`

### CosmosDB

Download [CosmosDB emulator](https://aka.ms/cosmosdb-emulator)

Run via powershell ( admin )

```ps1
Import-Module "$env:ProgramFiles\Azure Cosmos DB Emulator\PSModules\Microsoft.Azure.CosmosDB.Emulator"
Start-CosmosDbEmulator
```

Check status

```ps1
Get-CosmosDbEmulatorStatus
```

[After few mins explorer should be working](https://localhost:8081/_explorer/index.html)

## Credits

- https://www.svix.com/resources/guides/rabbitmq-docker-setup-guide/
- https://hub.docker.com/_/rabbitmq
- https://code-maze.com/masstransit-rabbitmq-aspnetcore/
- https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-entity-framework-integration?tabs=package-reference
- https://jeremylindsayni.wordpress.com/2019/02/25/getting-started-with-azure-cosmos-db-and-net-core-part-1-installing-the-cosmos-emulator/
- https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-windows-arguments
- https://stackoverflow.com/questions/69872189/how-to-add-cosmosdb-to-net-6-webapi
- https://brettmckenzie.net/posts/the-input-content-is-invalid-because-the-require-properties-id-are-missing/
- https://medium.com/codenx/implement-mediator-pattern-with-mediatr-in-c-8a271d7b9901
- https://stackoverflow.com/questions/52939211/the-ssl-connection-could-not-be-established
- https://masstransit.io/documentation/concepts/messages

## Misc

```
dotnet new gitignore
```

```
dotnet dev-certs https --trust
```