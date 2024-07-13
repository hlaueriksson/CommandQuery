:: Deploy from the root directory
cd ..\..

CMD /C gcloud functions deploy commandquery-sample-googlecloudfunctions-command --gen2 --region=europe-north1 --runtime=dotnet8 --trigger-http --entry-point=CommandQuery.Sample.GoogleCloudFunctions.Command --set-build-env-vars=GOOGLE_BUILDABLE=samples/CommandQuery.Sample.GoogleCloudFunctions
CMD /C gcloud functions deploy commandquery-sample-googlecloudfunctions-query --gen2 --region=europe-north1 --runtime=dotnet8 --trigger-http --entry-point=CommandQuery.Sample.GoogleCloudFunctions.Query --set-build-env-vars=GOOGLE_BUILDABLE=samples/CommandQuery.Sample.GoogleCloudFunctions

cd samples\CommandQuery.Sample.GoogleCloudFunctions
