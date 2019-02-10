using System;
using System.Configuration;
using System.Globalization;
using MongoDB.Driver;

namespace AspNet.WebHooks.Custom.MongoStorage.WebHooks
{
    public class WebHookStoreContext : IWebHookStoreContext
    {
        internal const string ConnectionStringName = "MongoStoreConnectionString";
        private const string _databaseName = "WebHooks";
        private string _collectionName = "WebHooks";

        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public WebHookStoreContext()
        {
            if (ConfigurationManager.ConnectionStrings[ConnectionStringName] != null)
            {
                MongoUrl url = new MongoUrl(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString);
                _client = new MongoClient(url);

                string databaseName = !string.IsNullOrEmpty(url.DatabaseName) ? url.DatabaseName : _databaseName;
                _database = _client.GetDatabase(databaseName);
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.EmptyString, nameof(ConnectionStringName));
                throw new ArgumentException(message);
            }
        }
        public WebHookStoreContext(string connectionStringName)
        {
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                MongoUrl url = new MongoUrl(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
                _client = new MongoClient(url);

                string databaseName = !string.IsNullOrEmpty(url.DatabaseName) ? url.DatabaseName : _databaseName;
                _database = _client.GetDatabase(databaseName);
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.EmptyString, nameof(connectionStringName));
                throw new ArgumentException(message);
            }
        }
        public WebHookStoreContext(string connectionStringName, string databaseName, string collectionName)
        {

            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                MongoUrl url = new MongoUrl(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
                _client = new MongoClient(url);

                databaseName = !string.IsNullOrEmpty(databaseName) ? databaseName : !string.IsNullOrEmpty(url.DatabaseName) ? url.DatabaseName : _databaseName;
                _database = _client.GetDatabase(databaseName);
                _collectionName = !string.IsNullOrEmpty(collectionName) ? collectionName : _collectionName;
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.EmptyString, nameof(connectionStringName));
                throw new ArgumentException(message);
            }
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return _database.GetCollection<TEntity>(_collectionName);
        }

    }
}