echo off
cls

cd bhb2core/bin/Debug

dotnet nuget push *.nupkg --source "github"

pause