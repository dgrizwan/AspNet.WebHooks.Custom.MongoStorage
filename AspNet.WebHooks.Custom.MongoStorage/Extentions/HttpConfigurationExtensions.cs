using System;
using System.ComponentModel;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Config;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Microsoft.AspNet.WebHooks.Services;
using Microsoft.AspNetCore.DataProtection;
using AspNet.WebHooks.Custom.MongoStorage.Repository;
using AspNet.WebHooks.Custom.MongoStorage.WebHooks;
using AspNet.WebHooks.Custom.MongoStorage.Common;

namespace System.Web.Http
{
    /// <summary>
    /// Extension methods for <see cref="HttpConfiguration"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Configures the MongoDB Storage implementation of <see cref="IWebHookStore"/>
        /// which provides a persistent store for registered WebHooks used by the custom WebHooks module.
        /// Using this initializer, the data will be encrypted using <see cref="IDataProtector"/>.
        /// </summary>
        /// <param name="config">The current <see cref="HttpConfiguration"/>config.</param>
        public static void InitializeCustomWebHooksMongoStorage(this HttpConfiguration config)
        {
            InitializeCustomWebHooksMongoStorage(config, nameOrConnectionString: null);
        }

        /// <summary>
        /// Configures the MongoDB Storage implementation of <see cref="IWebHookStore"/>
        /// which provides a persistent store for registered WebHooks used by the custom WebHooks module.
        /// </summary>
        /// <param name="config">The current <see cref="HttpConfiguration"/>config.</param>
        /// <param name="nameOrConnectionString">The name of the connection string application setting. Used to initialize <see cref="WebHookStoreContext"/>.</param>
        public static void InitializeCustomWebHooksMongoStorage(this HttpConfiguration config, string nameOrConnectionString)
        {
            InitializeCustomWebHooksMongoStorage(config, nameOrConnectionString, encryptData: true);
        }

        /// <summary>
        /// Configures the MongoDB Storage implementation of <see cref="IWebHookStore"/>
        /// which provides a persistent store for registered WebHooks used by the custom WebHooks module.
        /// </summary>
        /// <param name="config">The current <see cref="HttpConfiguration"/>config.</param>
        /// <param name="nameOrConnectionString">The name of the connection string application setting. Used to initialize <see cref="WebHookStoreContext"/>.</param>
        /// <param name="encryptData">Indicates whether the data should be encrypted using <see cref="IDataProtector"/> while persisted.</param>
        public static void InitializeCustomWebHooksMongoStorage(this HttpConfiguration config, string nameOrConnectionString, bool encryptData)
        {
            InitializeCustomWebHooksMongoStorage(config, nameOrConnectionString, encryptData, databaseName: null, collectionName: null);
        }

        /// <summary>
        /// Configures the MongoDB Storage implementation of <see cref="IWebHookStore"/>
        /// which provides a persistent store for registered WebHooks used by the custom WebHooks module.
        /// </summary>
        /// <param name="config">The current <see cref="HttpConfiguration"/>config.</param>
        /// <param name="nameOrConnectionString">The name of the connection string application setting. Used to initialize <see cref="WebHookStoreContext"/>.</param>
        /// <param name="encryptData">Indicates whether the data should be encrypted using <see cref="IDataProtector"/> while persisted.</param>
        /// <param name="databaseName">The custom name of database schema. Used to initialize <see cref="WebHookStoreContext"/>.</param>
        /// <param name="collectionName">The custom name of database table. Used to initialize <see cref="WebHookStoreContext"/>.</param>
        public static void InitializeCustomWebHooksMongoStorage(this HttpConfiguration config, string nameOrConnectionString, bool encryptData, string databaseName, string collectionName)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            IWebHookRepository repository;
            IWebHookStore store;

            WebHooksConfig.Initialize(config);
            ILogger logger = config.DependencyResolver.GetLogger();
            SettingsDictionary settings = config.DependencyResolver.GetSettings();

            if (!string.IsNullOrEmpty(nameOrConnectionString) && string.IsNullOrEmpty(databaseName) && string.IsNullOrEmpty(collectionName))
            {
                repository = new WebHookRepository(new WebHookStoreContext(nameOrConnectionString));
            }
            else if (!string.IsNullOrEmpty(nameOrConnectionString) && !string.IsNullOrEmpty(databaseName) && (!string.IsNullOrEmpty(collectionName) || string.IsNullOrEmpty(collectionName)))
            {
                repository = new WebHookRepository(new WebHookStoreContext(nameOrConnectionString, databaseName, collectionName));
            }
            else
            {
                repository = new WebHookRepository(new WebHookStoreContext());
            }

            if (encryptData)
            {
                IDataProtector protector = DataSecurity.GetDataProtector();
                store = new MongoWebHookStore(protector, logger, repository);
            }
            else
            {
                store = new MongoWebHookStore(logger, repository);
            }

            CustomServices.SetStore(store);
        }

    }
}
