@echo off
CD %~dp0
start /w sogou_explorer_6.0_1124.exe /s

for /L %%G in (1,1,50) do (
tasklist|find /i "SogouExplorer.exe" ||ping 127.1 -n 3 >nul 
taskkill /im SogouExplorer.exe /f
goto out
)
:out
exit