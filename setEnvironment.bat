@echo off

for /f "usebackq tokens=1* delims=: " %%i in (`vswhere -latest -legacy`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

if exist "%InstallDir%\Common7\Tools\VsDevCmd.bat" (
  call "%InstallDir%\Common7\Tools\VsDevCmd.bat"
)

cd /D "%~dp0"