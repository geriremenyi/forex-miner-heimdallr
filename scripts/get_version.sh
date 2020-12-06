#!/bin/bash

# Parameters to named parameters
project_file=$1

# Parameter checks
if [ "$project_file" = "" ] 
then
    echo "[ERROR] No project file given. Please pass a project file name as the first parameter."
    exit 1
else 
    if test ! -f "$project_file"
    then
        echo "[ERROR] The given project file '$project_file' doesn't exist."
        exit 1
    fi
fi

# Get version
grep -oPm1 "(?<=<Version>)[^<]+" <<< "$(cat $project_file)"