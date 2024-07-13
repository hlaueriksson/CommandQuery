# CommandQuery Sample Code

This code has been written with *Visual Studio 2022*.

The unit tests are using [NUnit](https://github.com/nunit/nunit).

Try out the sample projects via the Postman Workspace:

* <https://www.postman.com/hlaueriksson/workspace/commandquery/overview>

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/30609-f711b607-24cc-4b17-955e-c24e8dbeab99?action=collection%2Ffork&collection-url=entityId%3D30609-f711b607-24cc-4b17-955e-c24e8dbeab99%26entityType%3Dcollection%26workspaceId%3D3787ce92-42c3-4e2f-9534-6ea64eb639b3)

Or via the `.http` files in Visual Studio:

* Open the `CommandQuery.Samples.sln`
* Open a `.http` file in the `HttpFiles` folder
* Select en environment in the HTTP Environments (F6) dropdown:
  * `AspNetCore` | `AWSLambda` | `AzureFunctions` | `GoogleCloudFunctions` | `http://localhost:7071`
* Click the `Send request` link
  * [Send an HTTP request](https://learn.microsoft.com/en-us/aspnet/core/test/http-files?view=aspnetcore-8.0#send-an-http-request)

![Visual Studio - HttpFiles](HttpFiles.png)

## Command + Query + Handlers

Sample code:

* `CommandQuery.Sample.Contracts`
* `CommandQuery.Sample.Handlers`

## ASP.NET Core

Sample code:

* `CommandQuery.Sample.AspNetCore`
* `CommandQuery.Sample.AspNetCore.Tests`

## AWS Lambda

Sample code:

* `CommandQuery.Sample.AWSLambda`
* `CommandQuery.Sample.AWSLambda.Tests`

## Azure Functions

Sample code:

* `CommandQuery.Sample.AzureFunctions`
* `CommandQuery.Sample.AzureFunctions.Tests`

## Google Cloud Functions

Sample code:

* `CommandQuery.Sample.GoogleCloudFunctions`
* `CommandQuery.Sample.GoogleCloudFunctions.Tests`

## Client

Sample code:

* `CommandQuery.Sample.Client`
