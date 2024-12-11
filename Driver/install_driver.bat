@echo off
wdi-simple.exe --vid 0x01FC9 --pid 0x0094 --type 3 --name OpenGD77
pause
pnputil.exe /add-driver DFU\sttube.inf /install
pause
