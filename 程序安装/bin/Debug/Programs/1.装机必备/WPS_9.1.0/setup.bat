@echo off
CD %~dp0
start /w WPS_9.1.0.5218_setup.1446794216.exe /s

for /L %%G in (1,1,50) do (
tasklist|find /i "wps.exe" ||ping 127.1 -n 3 >nul 
taskkill /im wps.exe /f
goto out
)
:out
exit