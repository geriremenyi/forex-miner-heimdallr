# forex-miner-heimdallr

Management component implementation for forex-miner.com. 

The repo's name is coming from the Norse mythology, in which [Heimdallr](https://en.wikipedia.org/wiki/Heimdallr) is a god who is attested as possessing foreknowledge, keen eyesight and hearing, and keeps watch for invaders and the onset of Ragnarök while drinking fine mead in his dwelling Himinbjörg, located where the burning rainbow bridge Bifröst meets the sky.

## Getting started

### Prerequisites

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download) to build, run etc.
- [Docker](https://www.docker.com/products/docker-desktop) (optional) to contenarize
- [Visual Studio](https://visualstudio.microsoft.com) (optional) as the IDE.

### Local setup

1. Clone this repo
```bash
# HTTPS
https://github.com/geriremenyi/forex-miner-heimdallr.git
# SSH
git@github.com:geriremenyi/forex-miner-heimdallr.git
```

2. Navigate to the root of the project and restore nuget packages for all sub-component
```bash
dotnet restore src/ForexMiner.Heimdallr.sln
```

### Run

To run the different components you can execute the CLI's run command for each sub-component

User API
```bash
dotnet run --project src/Users/Users.Api/Users.Api.csproj
```

Connections API
```bash
dotnet run --project src/Connections/Connections.Api/Connections.Api.csproj
```

Connections worker
```bash
dotnet run --project src/Connections/Connections.Worker/Connections.Worker.csproj
```

Instruments API
```bash
dotnet run --project src/Instruments/Instruments.Api/Instruments.Api.csproj
```

Instruments worker
```bash
dotnet run --project src/Instruments/Instruments.Worker/Instruments.Worker.csproj
```

However it is recommened to use docker-compose instead, which takes care of the dependencies between the components and creates the required resources as well.
```bash
docker-compose -f src/docker-compose.yml up 
```
:warning: Note: There is a private docker image `forex-miner-thor` (the trading engine) defined in the docker-compose.yml. This means that either you need that image built locally (from the [forex-miner-thor repository](https://github.com/geriremenyi/forex-miner-thor)) or you need to be logged in to the GitHub Container Registry with the credentials given in the documentation.

If there are changes in the datamodel there is a need to create a new data migration which can be done with the following command:
```bash
dotnet ef migrations add {MigrationName} -c ForexMiner.Heimdallr.Common.Data.Database.Context.ForexMinerHeimdallrDbContext -p ./src/Common/Common.Data -o Database/Migrations -s ./src/Users/Users.Api 
```

### Tests

Unit tests are located in the projects ending with `.Tests`. To run all tests for the entire solution just execute the following command:
```bash
dotnet test src/ForexMiner.Heimdallr.sln
```

## Deployment

This chapter guides you through the CI/CD setup and the deployment steps for the engine.

### GitHub Actions

There are continuous integration and deployment steps setup as GitHub actions to be able to test on every pull-request and to be able to deliver fast. 

#### Continuous integration

All pull request opened against any branches triggers a continuous integration workflow to run.

There is a continuous integration defined for each separate project.

- [Common.Caching](.github/workflows/common_caching_continuous_integration.yaml)
- [Common.Data](.github/workflows/common_data_continuous_integration.yaml)
- [Common.Extensions](.github/workflows/common_extensions_continuous_integration.yaml)
- [Connections.Api](.github/workflows/connections_api_continuous_integration.yaml)
- [Connections.Secret](.github/workflows/connections_secret_continuous_integration.yaml)
- [Connections.Worker](.github/workflows/connections_worker_continuous_integration.yaml)
- [Instruments.Api](.github/workflows/instruments_api_continuous_integration.yaml)
- [Instruments.Storage](.github/workflows/instruments_storage_continuous_integration.yaml)
- [Instruments.Worker](.github/workflows/instruments_worker_continuous_integration.yaml)
- [Users.Api](.github/workflows/users_api_continuous_integration.yaml)

#### Continuous deployment

All changes on the [master branch](https://github.com/geriremenyi/forex-miner-heimdallr/tree/master) triggers a deployment to the [kubernetes cluster behind the `forex-miner.com` domain](https://github.com/geriremenyi/forex-miner-asgard).

The steps are defined separately for all deployable component.

- [Connections.Api](.github/workflows/connections_api_continuous_deployment.yaml)
- [Connections.Worker](.github/workflows/connections_worker_continuous_deployment.yaml)
- [Instruments.Api](.github/workflows/instruments_api_continuous_deployment.yaml)
- [Instruments.Worker](.github/workflows/instruments_worker_continuous_deployment.yaml)
- [Users.Api](.github/workflows/users_api_continuous_deployment.yaml)

To actually trigger a new release with a new version, please checkout from [develop branch](https://github.com/geriremenyi/forex-miner-heimdallr/tree/develop) to a release branch (like: `git checkout -b releases/0.0.1`). On this branch run the [`./scripts/bump_version.sh`](scripts/bump_version.sh) script with any of the following parameters `patch|minor|major`. If no parameter given, the `patch` version will be bumped. After this update and merge the [master branch](https://github.com/geriremenyi/forex-miner-heimdallr/tree/master) to the release branch and accept all current changes (to resolve merge conflicts coming from master). If finished push the release branch to GitHub and open PR against the [develop branch](https://github.com/geriremenyi/forex-miner-heimdallr/tree/develop) and the [master branch](https://github.com/geriremenyi/forex-miner-heimdallr/tree/master). After completing the PR against the master branch the CD workflows will automatically kick in. This will create a new [tag](https://github.com/geriremenyi/forex-miner-heimdallr/tags) and a new [release](https://github.com/geriremenyi/forex-miner-heimdallr/releases).

### Kubernetes Cluster

The kubernetes deployments are defined under each deployable project's Kubernetes folder.

To deploy the latest docker images to a kubernetes cluster run the following commands.
1. If not yet created, create a namespace called `forex-miner` and add the GitHub Container Registry pull secret to it (can be found in the documentation)
```bash
kubectl create namespace forex-miner
kubectl create secret ghcr-secret pullsecret --docker-server=https://ghcr.io/ --docker-username=notneeded --docker-password={PULL_SECRET_VALUE}
```
2. Deploy all apps (a.k.a.: management components)
```bash
kubectl apply -f ./src/Users/Users.Api/Kubernetes/app.yaml
kubectl apply -f ./src/Connections/Connections.Api/Kubernetes/app.yaml
kubectl apply -f ./src/Connections/Connections.Worker/Kubernetes/app.yaml
kubectl apply -f ./src/Instruments/Instruments.Api/app.yaml
kubectl apply -f ./src/Instruments/Instruments.Worker/Kubernetes/app.yaml
```
4. Deploy all services (to expose all APIs)
```bash
kubectl apply -f ./src/Users/Users.Api/Kubernetes/service.yaml
kubectl apply -f ./src/Connections/Connections.Api/Kubernetes/service.yaml
kubectl apply -f ./src/Instruments/Instruments.Api/service.yaml
```