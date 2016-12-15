#!/bin/bash

get_script_dir () {
     SOURCE="${BASH_SOURCE[0]}"
     while [ -h "$SOURCE" ]; do
          DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
          SOURCE="$( readlink "$SOURCE" )"
          [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE"
     done
     $( cd -P "$( dirname "$SOURCE" )" )
     pwd
}

cd "$(get_script_dir)"

echo "Downloading Cake bootstrap script"
curl -Lsfo buildcake.sh http://cakebuild.net/bootstrapper/linux
chmod u+x ./buildcake.sh

echo "Running cake"
./buildcake.sh -s "build.cake" $@