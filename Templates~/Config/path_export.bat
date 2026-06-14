@echo off
setlocal enabledelayedexpansion

cd /d "%~dp0"

:: 读取配置文件（跳过 # 开头的注释和空行）
for /f "usebackq eol=# delims=" %%i in ("path_export.conf") do (
    set "line=%%i"
    if defined line (
        for /f "tokens=1,* delims==" %%a in ("!line!") do (
            set "%%a=%%b"
        )
    )
)

:: 测试输出（不要输出 line 或 %%i）
:: echo CONF_ROOT=%CONF_ROOT%
:: echo LUBAN_DLL=%LUBAN_DLL%
:: echo PATH_VALIDATOR_ROOT=%PATH_VALIDATOR_ROOT%
:: pause