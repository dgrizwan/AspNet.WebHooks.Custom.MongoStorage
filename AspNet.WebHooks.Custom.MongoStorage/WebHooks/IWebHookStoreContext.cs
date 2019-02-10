using MongoDB.Driver;

namespace AspNet.WebHooks.Custom.MongoStorage.WebHooks
{
    public interface IWebHookStoreContext
    {
        IMongoCollection<TEntity> GetCollection<TEntity>();
    }
}
