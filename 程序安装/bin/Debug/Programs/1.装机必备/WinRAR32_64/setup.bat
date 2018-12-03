@echo off
CD %~dp0
if /i "%PROCESSOR_IDENTIFIER:~0,3%"=="X86" (
	call WinRAR_X86\setup.bat
) ELSE (
	call WinRAR_X64\setup.bat
)
