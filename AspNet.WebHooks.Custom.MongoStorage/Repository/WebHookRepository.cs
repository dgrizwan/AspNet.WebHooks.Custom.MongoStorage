using Microsoft.AspNet.WebHooks;
using MongoDB.Bson;
using MongoDB.Driver;
using AspNet.WebHooks.Custom.MongoStorage.WebHooks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNet.WebHooks.Custom.MongoStorage.Repository
{
    public class WebHookRepository : IWebHookRepository
    {
        #region Member Variables
        private readonly IWebHookStoreContext _webHookStoreContext;
        #endregion

        #region Constructor
        public WebHookRepository(IWebHookStoreContext webHookStoreContext)
        {
            _webHookStoreContext = webHookStoreContext;
        }
        #endregion

        #region Database Related CRUD Operations
        // Insert new webHook for a user in DB. 
        public async Task<StoreResult> AddOne<TEntity>(TEntity item) where TEntity : class, new()
        {
            var collection = GetCollection<TEntity>();
            await collection.InsertOneAsync(item);
            return StoreResult.Success;
        }
        // Update webHook for a user in DB.
        public async Task<StoreResult> UpdateOne<TEntity>(TEntity update, string id, string user) where TEntity : class, new()
        {
            var filter = ((new FilterDefinitionBuilder<TEntity>().Eq("Id", id)) & (new FilterDefinitionBuilder<TEntity>().Eq("User", user)));

            var collection = GetCollection<TEntity>();
            await collection.ReplaceOneAsync(filter, update);
            return StoreResult.Success;
        }
        // Get One Record Based on User and Id.
        public async Task<TEntity> GetOne<TEntity>(string id, string user) where TEntity : class, new()
        {
            var filter = ((new FilterDefinitionBuilder<TEntity>().Eq("Id", id)) & (new FilterDefinitionBuilder<TEntity>().Eq("User", user)));

            var collection = GetCollection<TEntity>();
            return await collection.Find(filter).FirstOrDefaultAsync();
        }
        // Get all Record Based on User.
        public async Task<List<TEntity>> GetMany<TEntity>(string user) where TEntity : class, new()
        {
            var filter = (new FilterDefinitionBuilder<TEntity>().Eq("User", user));

            var collection = GetCollection<TEntity>();
            return await collection.Find(filter).ToListAsync();
        }
        //Get all Records
        public async Task<List<TEntity>> GetAll<TEntity>() where TEntity : class, new()
        {
            var collection = GetCollection<TEntity>();
            return await collection.Find(new BsonDocument()).ToListAsync();
        }
        // Delete a Unique Subscription
        public async Task<StoreResult> DeleteOne<TEntity>(string id, string user) where TEntity : class, new()
        {
            var filter = ((new FilterDefinitionBuilder<TEntity>().Eq("Id", id)) & (new FilterDefinitionBuilder<TEntity>().Eq("User", user)));

            var collection = GetCollection<TEntity>();
            await collection.DeleteOneAsync(filter);
            return StoreResult.Success;
        }
        // Delete all Subscriptions of a User
        public async Task DeleteMany<TEntity>(string user) where TEntity : class, new()
        {
            var filter = (new FilterDefinitionBuilder<TEntity>().Eq("User", user));

            var collection = GetCollection<TEntity>();
            await collection.DeleteManyAsync(filter);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// The private GetCollection method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return _webHookStoreContext.GetCollection<TEntity>();
        }
        #endregion

    }
}