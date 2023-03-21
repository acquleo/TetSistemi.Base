call "setEnvironment.bat"
echo %~dp0
cd /D "%~dp0"

dotnet build -c release
REM "MSBuild.exe" /property:Configuration=Release /t:clean
REM "MSBuild.exe" /property:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 PAUSE
