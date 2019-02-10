using System.Collections.Generic;
using Microsoft.AspNet.WebHooks;
using System.Threading.Tasks;

namespace AspNet.WebHooks.Custom.MongoStorage.Repository
{
    public interface IWebHookRepository
    {
        /// <summary>
        /// A generic AddOne method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<StoreResult> AddOne<TEntity>(TEntity item) where TEntity : class, new();

        /// <summary>
        /// A generic UpdateOne method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="update"></param>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<StoreResult> UpdateOne<TEntity>(TEntity update, string id, string user) where TEntity : class, new();

        /// <summary>
        /// A generic GetOne method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<TEntity> GetOne<TEntity>(string id, string user) where TEntity : class, new();

        /// <summary>
        /// A generic DeleteOne method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<StoreResult> DeleteOne<TEntity>(string id, string user) where TEntity : class, new();

        /// <summary>
        /// A generic DeleteMany method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        Task DeleteMany<TEntity>(string user) where TEntity : class, new();

        /// <summary>
        /// A generic GetMany method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetMany<TEntity>(string user) where TEntity : class, new();

        /// <summary>
        /// A generic GetAll method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<List<TEntity>> GetAll<TEntity>() where TEntity : class, new();
    }
}
