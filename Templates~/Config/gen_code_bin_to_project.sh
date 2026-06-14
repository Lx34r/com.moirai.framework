#!/bin/bash

cd "$(dirname "$0")"
echo "当前目录: $(pwd)"

source path_export.sh

cp -R "${CONFIG_SCRIPT_SOURCE}" "${CONFIG_SCRIPT_TARGET}"
cp -R "${CONFIGINIT_SCRIPT_SOURCE}" "${CONFIGINIT_SCRIPT_TARGET}"
cp -R "${EXTERNALTYPEUTIL_SCRIPT_SOURCE}" "${EXTERNALTYPEUTIL_SCRIPT_TARGET}"

# 使用 grep -i 不区分大小写
if [ -z "$1" ]; then
    export TEMPLATE_SUFFIX="LazyLoad"
elif echo "$1" | grep -qi "^true$"; then
    export TEMPLATE_SUFFIX="LazyLoad"
else
    export TEMPLATE_SUFFIX="Default"
fi

dotnet "${LUBAN_DLL}" \
    -t client \
    -c cs-bin \
    -d bin \
    --conf "${CONF}" \
    --customTemplateDir "${CONF_ROOT}\CustomTemplate\CustomTemplate_Client_${TEMPLATE_SUFFIX}" \
    -x code.lineEnding=crlf \
    -x pathValidator.rootDir="${PATH_VALIDATOR_ROOT"" \
    -x outputCodeDir="${CODE_OUTPUT_PATH_CLIENT}" \
    -x outputDataDir="${DATA_OUTPUT_PATH_CLIENT}" \
    -x l10n.provider="default" \
    -x l10n.textFile.path="${L10N_TEXTFILE_PATH}" \
    -x l10n.textFile.keyFieldName="key"

echo "操作完成，按任意键退出..."
read -k1
