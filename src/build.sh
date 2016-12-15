#!/bin/bash

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $SCRIPT_DIR


echo "Downloading Cake bootstrap script"
curl -Lsfo buildcake.sh http://cakebuild.net/bootstrapper/linux
chmod u+x ./buildcake.sh

echo "Running cake"
./buildcake.sh -s "build.cake" $@