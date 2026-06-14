:: 第1个参数：lazyload=true
cd /d %~dp0

call path_export.bat

echo F |xcopy /s /e /i /y "%CONFIG_SCRIPT_SOURCE%" "%CONFIG_SCRIPT_TARGET%"
echo F |xcopy /s /e /i /y "%CONFIGINIT_SCRIPT_SOURCE%" "%CONFIGINIT_SCRIPT_TARGET%"
echo F |xcopy /s /e /i /y "%EXTERNALTYPEUTIL_SCRIPT_SOURCE%" "%EXTERNALTYPEUTIL_SCRIPT_TARGET%"

:: /i 表示不区分大小写
if "%1"=="" (set TEMPLATE_SUFFIX=LazyLoad) else if /i "%1"=="true" (set TEMPLATE_SUFFIX=LazyLoad) else (set TEMPLATE_SUFFIX=Default)

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin^
    --conf %CONF% ^
    --customTemplateDir %CONF_ROOT%\CustomTemplate\CustomTemplate_Client_%TEMPLATE_SUFFIX% ^
    -x code.lineEnding=crlf ^
    -x pathValidator.rootDir=%PATH_VALIDATOR_ROOT% ^
    -x outputCodeDir=%CODE_OUTPUT_PATH_CLIENT% ^
    -x outputDataDir=%DATA_OUTPUT_PATH_CLIENT% ^
    -x l10n.provider=default ^
    -x l10n.textFile.path=%L10N_TEXTFILE_PATH% ^
    -x l10n.textFile.keyFieldName=key
pause