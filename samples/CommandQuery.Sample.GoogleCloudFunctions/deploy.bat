cd ..

CMD /C gcloud functions deploy commandquery-sample-googlecloudfunctions-command --runtime dotnet3 --trigger-http --entry-point=CommandQuery.Sample.GoogleCloudFunctions.Command --set-build-env-vars=GOOGLE_BUILDABLE=CommandQuery.Sample.GoogleCloudFunctions
CMD /C gcloud functions deploy commandquery-sample-googlecloudfunctions-query --runtime dotnet3 --trigger-http --entry-point=CommandQuery.Sample.GoogleCloudFunctions.Query --set-build-env-vars=GOOGLE_BUILDABLE=CommandQuery.Sample.GoogleCloudFunctions

cd CommandQuery.Sample.GoogleCloudFunctions
