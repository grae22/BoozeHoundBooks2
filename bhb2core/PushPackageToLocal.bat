echo off
cls

cd bhb2core/bin/Debug

rmdir /S /Q C:\Users\graemeb\.nuget\packages\bhb2core

copy *.nupkg c:\dev\prj\other\LocalNugetPackages /Y

pause