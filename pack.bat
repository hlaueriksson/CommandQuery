dotnet build -c Release

:: Fix for extra bin folder
xcopy /y .\src\CommandQuery.AzureFunctions\bin\Release\net461\bin\CommandQuery.AzureFunctions.* .\src\CommandQuery.AzureFunctions\bin\Release\net461\
xcopy /y .\src\CommandQuery.AzureFunctions\bin\Release\netstandard2.0\bin\CommandQuery.AzureFunctions.* .\src\CommandQuery.AzureFunctions\bin\Release\netstandard2.0\

dotnet pack -c Release --no-build
