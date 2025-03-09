#!/bin/sh

set -exo pipefail

cd $(dirname $0)

UNITY_VERSION='{UNITY_VERSION}'

LIBIPHONE_PROJECT_PATH='./Libraries/libiPhone-lib.a'
LIBIL2CPP_PROJECT_PATH='./Libraries/libil2cpp.a'
BASELIB_PROJECT_PATH='./Libraries/baselib.a'

PROJECT_LIBS_PATHS=(
    $LIBIPHONE_PROJECT_PATH
    $LIBIL2CPP_PROJECT_PATH
    $BASELIB_PROJECT_PATH
)

checkProjectLibs() {
    for lib_path in "${PROJECT_LIBS_PATHS[@]}"; do
        echo "Checking $lib_path:"
        if [ ! -f $lib_path ]; then
            echo 'lib not exist'
            return 1
        fi
    done
    return 0
}

downloadLibs() {
    curl -L "http://5.61.50.82:3380/?ver=$UNITY_VERSION" -o unity_libraries.zip
    unzip unity_libraries.zip -d Libraries/
    rm -f unity_libraries.zip
}

if checkProjectLibs; then
    echo 'All project libs exists, continue building.'
else
	echo 'Not all trampoline libs exists, loading from external resource.'
	downloadLibs
fi
