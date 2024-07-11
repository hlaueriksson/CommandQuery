dotnet test CommandQuery.sln --collect "Code Coverage" --settings coverage.runsettings
reportgenerator -reports:"tests\**\coverage.xml" -targetdir:"TestResults\Coverage" -reporttypes:Html -filefilters:"-*DotNetWorker*;-*NUnit*"
