@echo off
CD %~dp0
start PureCodec_2015.06.03.1444287551.exe /S /D
CD %SYSTEMROOT%\System32\drivers\etc
attrib -r hosts
ping 127.1 -n 3 >nul
findstr /b /n /r "127.0.0.1 get.daum.net" "hosts" &&echo ÕÒµ½||echo 127.0.0.1 get.daum.net>>hosts
exit