# CommandQuery Sample Code

This code has been written with *Visual Studio 2017* and *Visual Studio Code*.

The unit tests are using [Machine.Specifications](https://github.com/machine/machine.specifications/).

Manual testing can be done with [Postman](https://www.getpostman.com).
[Import](https://www.getpostman.com/docs/v6/postman/collections/data_formats#importing-postman-data) the collections in this folder to get started.

## Command + Query + Handlers

Sample code:

* `CommandQuery.Sample`

## ASP.NET Web API 2

Sample code:

* `CommandQuery.Sample.AspNet.WebApi`

Tests:

* `CommandQuery.Sample.AspNet.WebApi.Specs`

Postman collection:

* `CommandQuery.Sample.AspNet.WebApi.postman_collection.json`

## ASP.NET Core

Sample code:

* `CommandQuery.Sample.AspNetCore`

Tests:

* `CommandQuery.Sample.Specs/AspNetCore`

Postman collection:

* `CommandQuery.Sample.AspNetCore.postman_collection.json`

## AWS Lambda

Sample code:

* `CommandQuery.Sample.AWSLambda`

Tests:

* `CommandQuery.Sample.Specs/AWSLambda`

Postman collection:

* `CommandQuery.Sample.AWSLambda.postman_collection.json`

## Azure Functions

Functions can be created in both Visual Studio and Visual Studio Code, and run on both .NET Framework and .NET Core

Sample code:

* `CommandQuery.Sample.AzureFunctions.Vs1`
    * Visual Studio with `v1` (.NET Framework) runtime
* `CommandQuery.Sample.AzureFunctions.Vs2`
    * Visual Studio with `v2` (.NET Core) runtime
* `CommandQuery.Sample.AzureFunctions.VsCode1`
    * Visual Studio Code with `~1` (.NET Framework) runtime
* `CommandQuery.Sample.AzureFunctions.VsCode2`
    * Visual Studio Code with `beta` (.NET Core) runtime

Tests:

* `CommandQuery.Sample.Specs/AzureFunctions.Vs1`
* `CommandQuery.Sample.Specs/AzureFunctions.Vs2`

Postman collection:

* `CommandQuery.Sample.AzureFunctions.postman_collection.json`
