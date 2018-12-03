@echo off
CD %~dp0
start /w baofeng_5.54.1029.1111_setup.1446107282.exe /s

for /L %%G in (1,1,50) do (
tasklist|find /i "StormPlayer.exe" ||ping 127.1 -n 3 >nul 
taskkill /im StormPlayer.exe /f
taskkill /im BaofengPlatform.exe /f
goto out
)
:out
exit