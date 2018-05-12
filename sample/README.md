# CommandQuery sample code

An introduction and instructions on how to get started with `CommandQuery` is documented in the main [README](https://github.com/hlaueriksson/CommandQuery/blob/master/README.md).

## Command + Query + Handlers

One command and one query together with corresponding handlers:

* `CommandQuery.Sample`

## ASP.NET Core

Sample code for ASP.NET Core 2.0:

* `CommandQuery.Sample.AspNetCore`

## Azure Functions

Functions can be created in both Visual Studio and Visual Studio Code, and run on both .NET Framework and .NET Core:

* `CommandQuery.Sample.AzureFunctions.Vs1`
    * Visual Studio with `v1` (.NET Framework) runtime
* `CommandQuery.Sample.AzureFunctions.Vs2`
    * Visual Studio with `v2` (.NET Core) runtime
* `CommandQuery.Sample.AzureFunctions.VsCode1`
    * Visual Studio Code with `~1` (.NET Framework) runtime
* `CommandQuery.Sample.AzureFunctions.VsCode2`
    * Visual Studio Code with `beta` (.NET Core) runtime

## Tests

Tests the handlers in the AspNetCore and AzureFunctions projects with `Machine.Specifications`:

* `CommandQuery.Sample.Specs`

## Postman

Postman collections to get started with manual testing:

* `CommandQuery.Sample.AspNetCore.postman_collection.json`
* `CommandQuery.Sample.AzureFunctions.postman_collection.json`
