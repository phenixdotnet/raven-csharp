#!/bin/bash

echo "Downloading Cake bootstrap script"
curl -Lsfo buildcake.sh http://cakebuild.net/bootstrapper/linux
chmod u+x ./buildcake.sh

echo "Running cake"
./buildcake.sh -s "build.cake" $@