#!/bin/bash

# Parameters to named parameters
token=$1

# Parameter checks
if [ "$token" = "" ] 
then
    echo "[ERROR] No token given. Please pass an Oanda token as the first parameter."
    exit 1
fi

# Prepare things
src_folder="$(readlink -f $(dirname "$0")/../src)"
secrets_open=$'{\n\t'
secrets_close=$'\n}'
secrets_content="$secrets_open\"Oanda-MasterToken\": \"$token\"$secrets_close"

# Set the token for Instruments.Worker
echo "$secrets_content" > "$src_folder/Instruments/Instruments.Worker/appsettings.Secrets.json"

# Set the token for Connections.Worker
echo "$secrets_content" > "$src_folder//Connections/Connections.Worker/appsettings.Secrets.json"