#!/bin/bash

# Parameters to named parameters
version_type=$1

# Parameter checks
if [ "$version_type" = "" ] 
then
    echo "[WARNING] Version type was not given. Setting version type to 'patch'"
    version_type="patch"
else 
    if [ "$version_type" != "patch" ] && [ "$version_type" != "minor" ] && [ "$version_type" != "major" ]
    then
        echo "[ERROR] The first parameter is the version type which should be either 'patch', 'minor' or 'major'."
        exit 1
    fi
fi

# Get current version
semver_regex='[^0-9]*\([0-9]*\)[.]\([0-9]*\)[.]\([0-9]*\)\([0-9A-Za-z-]*\)'
users_api_csproj_file="$(dirname "$0")/../src/Users/Users.Api/Users.Api.csproj"
current_version="$($(dirname "$0")/get_version.sh $users_api_csproj_file)"
major=`echo $current_version | sed -e "s#$semver_regex#\1#"`
minor=`echo $current_version | sed -e "s#$semver_regex#\2#"`
patch=`echo $current_version | sed -e "s#$semver_regex#\3#"`

# Calculate next version
if [ "$version_type" = "patch" ]
then
    let patch+=1
else
    if [ "$version_type" = "minor" ]
    then
        let minor+=1
    else
        let major+=1
    fi
fi
next_version="$major.$minor.$patch"

# Bumping version in Users.Api
users_api_folder="$(dirname "$0")/../src/Users/Users.Api"
sed -i "s/<Version>.*<\/Version>/<Version>$next_version<\/Version>/g" "$users_api_folder/Users.Api.csproj"
sed -i "s/ghcr.io\/geriremenyi\/forex-miner-heimdallr-users-api\:.*/ghcr.io\/geriremenyi\/forex-miner-heimdallr-users-api\:$next_version/g" "$users_api_folder/Kubernetes/app.yaml"

# Bumping version in Instruments.Worker
instruments_worker_folder="$(dirname "$0")/../src/Instruments/Instruments.Worker"
sed -i "s/<Version>.*<\/Version>/<Version>$next_version<\/Version>/g" "$instruments_worker_folder/Instruments.Worker.csproj"
sed -i "s/ghcr.io\/geriremenyi\/forex-miner-heimdallr-instruments-worker\:.*/ghcr.io\/geriremenyi\/forex-miner-heimdallr-instruments-worker\:$next_version/g" "$instruments_worker_folder/Kubernetes/app.yaml"

# Bumping version in Instruments.Api
instruments_api_folder="$(dirname "$0")/../src/Instruments/Instruments.Api"
sed -i "s/<Version>.*<\/Version>/<Version>$next_version<\/Version>/g" "$instruments_api_folder/Instruments.Api.csproj"
sed -i "s/ghcr.io\/geriremenyi\/forex-miner-heimdallr-instruments-api\:.*/ghcr.io\/geriremenyi\/forex-miner-heimdallr-instruments-api\:$next_version/g" "$instruments_api_folder/Kubernetes/app.yaml"

# Bumping version in Connections.Worker
connections_worker_folder="$(dirname "$0")/../src/Connections/Connections.Worker"
sed -i "s/<Version>.*<\/Version>/<Version>$next_version<\/Version>/g" "$connections_worker_folder/Connections.Worker.csproj"
sed -i "s/ghcr.io\/geriremenyi\/forex-miner-heimdallr-connections-worker\:.*/ghcr.io\/geriremenyi\/forex-miner-heimdallr-connections-worker\:$next_version/g" "$connections_worker_folder/Kubernetes/app.yaml"

# Bumping version in Connections.Api
connections_api_folder="$(dirname "$0")/../src/Connections/Connections.Api"
sed -i "s/<Version>.*<\/Version>/<Version>$next_version<\/Version>/g" "$connections_api_folder/Connections.Api.csproj"
sed -i "s/ghcr.io\/geriremenyi\/forex-miner-heimdallr-connections-api\:.*/ghcr.io\/geriremenyi\/forex-miner-heimdallr-connections-api\:$next_version/g" "$connections_api_folder/Kubernetes/app.yaml"