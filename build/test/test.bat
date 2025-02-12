cd "../../wwwroot/housebrokerapp/net8.0/"

dotnet vstest "HouseBrokerApp.Tests.dll" /logger:console;verbosity=detailed --parallel
::pause