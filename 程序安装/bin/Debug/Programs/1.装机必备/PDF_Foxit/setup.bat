@echo off
CD %~dp0
if exist %SYSTEMROOT%\Yhc_App\Foxit_Reader (
   echo "�Ѿ������ļ���"
) else (
md %SYSTEMROOT%\Foxit_Reader
echo "�ļ��� �����ڣ����ڴ���"
)

echo d|xcopy Foxit_Reader.exe %SYSTEMROOT%\Yhc_App\Foxit_Reader /c /e /h /k /r /y
cd %SYSTEMROOT%\Yhc_App\Foxit_Reader
Foxit_Reader.exe -Register
exit