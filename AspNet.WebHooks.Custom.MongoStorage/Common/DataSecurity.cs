using System;
using System.Threading;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.WebHooks.Custom.MongoStorage.Common
{
    internal static class DataSecurity
    {
        private const string Purpose = "WebHookPersistence";

        private static IDataProtector _dataProtector;

        /// <summary>
        /// This follows the same initialization that is provided when <see cref="IDataProtectionProvider"/>
        /// is initialized within ASP.NET Core 1.0 Dependency Injection.
        /// </summary>
        /// <returns>A fully initialized <see cref="IDataProtectionProvider"/>.</returns>
        public static IDataProtector GetDataProtector()
        {
            if (_dataProtector != null)
            {
                return _dataProtector;
            }

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            IServiceProvider services = serviceCollection.BuildServiceProvider();
            IDataProtectionProvider provider = services.GetDataProtectionProvider();
            IDataProtector instance = provider.CreateProtector(Purpose);
            Interlocked.CompareExchange(ref _dataProtector, instance, null);
            return _dataProtector;
        }
    }
}
