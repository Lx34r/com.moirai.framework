#!/bin/bash

cd "$(dirname "$0")"
echo "当前目录: $(pwd)"

source path_export.sh

dotnet "${LUBAN_DLL}" \
    -t server \
    -c cs-bin \
    -d bin \
    --conf "${CONF_ROOT}/luban.conf" \
    -x code.lineEnding=crlf \
    -x outputCodeDir="${CODE_OUTPUT_PATH_SERVER}" \
    -x outputDataDir="${DATA_OUTPUT_PATH_SERVER}" \
    -x outputSaver.bin.cleanUpOutputDir=1 \
    -x outputSaver.json.cleanUpOutputDir=1 \
    -x outputSaver.cs-bin.cleanUpOutputDir=1

echo "操作完成，按任意键退出..."
read -k1
