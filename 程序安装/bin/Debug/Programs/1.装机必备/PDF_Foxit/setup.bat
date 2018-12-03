@echo off
CD %~dp0
if exist %SYSTEMROOT%\Yhc_App\Foxit_Reader (
   echo "已经存在文件夹"
) else (
md %SYSTEMROOT%\Foxit_Reader
echo "文件夹 不存在，正在创建"
)

echo d|xcopy Foxit_Reader.exe %SYSTEMROOT%\Yhc_App\Foxit_Reader /c /e /h /k /r /y
cd %SYSTEMROOT%\Yhc_App\Foxit_Reader
Foxit_Reader.exe -Register
exit