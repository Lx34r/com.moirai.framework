cd /d %~dp0

call path_export.bat

dotnet %LUBAN_DLL% ^
    -t server^
    -c cs-bin ^
    -d bin^
    --conf %CONF% ^
    -x code.lineEnding=crlf ^
    -x outputCodeDir=%CODE_OUTPUT_PATH_SERVER% ^
    -x outputDataDir=%DATA_OUTPUT_PATH_SERVER% ^
    -x outputSaver.bin.cleanUpOutputDir=1 ^
    -x outputSaver.json.cleanUpOutputDir=1 ^
    -x outputSaver.cs-bin.cleanUpOutputDir=1 
pause