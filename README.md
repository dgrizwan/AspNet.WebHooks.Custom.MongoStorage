# AspNet.WebHooks.Custom.MongoStorage

This project provides the support for persisting your custom WebHooks registrations in a MongoDB storage medium.

Add this project as a reference in your main WebApi/ MVC project.

Resolve its dependency using httpConfiguration object in the WebApiConfig.cs and provide your connection string name as 'config.InitializeCustomWebHooksMongoStorage("connection string name")'.

You can also provide the names of the Mongo Collection or Document. Also have a look of the optional parameters of this method.

You also need to add the WebHooks nuget packages to make your solution run for the WebHooks using the MongoDB.

You also need to add the following nuget packages in your main project to make this project run if you already haven't added them:

      id="Microsoft.AspNet.WebHooks.Common" version="1.2.1"
      id="Microsoft.AspNet.WebHooks.Custom" version="1.2.1"
      id="MongoDB.Bson" version="2.7.2" />
      id="MongoDB.Driver" version="2.7.2"
      
You can also add the nuget package for this storage from the https://www.nuget.org/packages/AspNet.WebHooks.Custom.MongoStorage.
It will automatically resolve all the dependencies and you need not do anything at all.

This project is open for any contribution i.e. you can generate a pull request or create an issue etc.


Thanks!

Muhammad Rizwan  
dgrizwan@gmail.com
