#!/bin/bash

# 切换到脚本所在目录
cd "$(dirname "$0")"

# 读取配置文件（跳过注释和空行）
# IFS='=' 按等号分割。
while IFS='=' read -r key value; do
    # 跳过注释和空行
    [[ -z "$key" || "$key" =~ ^# ]] && continue
    # 去除两端空格
    key=$(echo "$key" | xargs)
    value=$(echo "$value" | xargs)
    # 导出为环境变量
    export "$key=$value"
done < "path_export.conf"

# 测试输出
# echo "CONF_ROOT=$CONF_ROOT"
# echo "LUBAN_DLL=$LUBAN_DLL"
# echo "PATH_VALIDATOR_ROOT=$PATH_VALIDATOR_ROOT"