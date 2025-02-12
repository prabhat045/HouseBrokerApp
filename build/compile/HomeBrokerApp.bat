cd "../.."

rd /s /q wwwroot

dotnet build HouseBrokerApp.sln /flp:logfile=build\compile\HouseBrokerAppBuildLog.txt

cd "build\compile"