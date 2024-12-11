@echo off
cd /d "%~dp0"
chcp 1251
wdi-simple.exe --vid 0x01FC9 --pid 0x0094 --type 3 --name OpenGD77
pnputil.exe /add-driver DFU\sttube.inf /install
pause


