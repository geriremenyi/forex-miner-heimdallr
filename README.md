# forex-miner-heimdallr

Backend API implementation for forex-miner.com. 

The repo's name is coming from the Norse mythology, in which [Heimdallr](https://en.wikipedia.org/wiki/Heimdallr) is a god who is attested as possessing foreknowledge, keen eyesight and hearing, and keeps watch for invaders and the onset of Ragnarök while drinking fine mead in his dwelling Himinbjörg, located where the burning rainbow bridge Bifröst meets the sky.

## Getting started

### Prerequisites

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download) to build, run etc.
- A API client tool like curl, Postman
- An IDE for development, recommended to use [Visual Studio](https://visualstudio.microsoft.com) as there are solution and project files stored for it in this repo 

### Run, test, build

DB migration

```bash
dotnet ef migrations add <MigrationName> -c ForexMiner.Heimdallr.Common.Data.Database.Context.ForexMinerHeimdallrDbContext -o Database\Migrations -s .\Users\Users.Api -p .\Common\Data
```

TBD

## Deployment

TBD

### Pipeline

TBD

### Environments

TBD

#### Staging

TBD

#### Production

TBD