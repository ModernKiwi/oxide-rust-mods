cd referenceFiles
for /F "delims=" %%a in ('dir /B mscorlib.dll') do del "%%a"
for /F "delims=" %%a in ('dir /B System.Core.dll') do del "%%a"
for /F "delims=" %%a in ('dir /B System.Data.dll') do del "%%a"
for /F "delims=" %%a in ('dir /B *.xml') do del "%%a"
pause