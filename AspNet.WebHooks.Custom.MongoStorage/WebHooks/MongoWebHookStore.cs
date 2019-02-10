using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Microsoft.AspNet.WebHooks.Storage;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using AspNet.WebHooks.Custom.MongoStorage.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.WebHooks.Custom.MongoStorage.WebHooks
{
    /// <summary>
    /// Provides an abstract implementation of <see cref="IWebHookStore"/> targeting the MongoDB
    /// </summary>
    public class MongoWebHookStore : WebHookStore
    {
        #region Member Variables
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IWebHookRepository _webHookRepository;
        private readonly IDataProtector _protector;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public MongoWebHookStore(ILogger logger, IWebHookRepository repository)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            _serializerSettings = new JsonSerializerSettings() { Formatting = Formatting.None };
            _webHookRepository = repository;
            _logger = logger;
        }
        public MongoWebHookStore(IDataProtector protector, ILogger logger, IWebHookRepository repository)
            : this(logger, repository)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            _protector = protector;
        }
        #endregion

        #region WebHookStore Methods
        //Register or in Other Words Inserts a New WebHook for a User. Also need to serialize into JSON and encrypt data using ASP.NET Core Protect.
        public override async Task<StoreResult> InsertWebHookAsync(string user, WebHook webHook)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

            user = NormalizeKey(user);

            try
            {
                Registration registration = ConvertFromWebHook(user, webHook);
                return await _webHookRepository.AddOne(registration);
            }
            catch (OptimisticConcurrencyException ocex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.ConcurrencyError, "Insert", ocex.Message);
                _logger.Error(message, ocex);
                return StoreResult.Conflict;
            }
            catch (UpdateException uex)
            {
                var error = uex.GetBaseException().Message;
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.MongoOperationFailed, "Insert", error);
                _logger.Error(message, uex);
                return StoreResult.Conflict;
            }
            catch (DbException dbex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.MongoOperationFailed, "Insert", dbex.Message);
                _logger.Error(message, dbex);
                return StoreResult.OperationError;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Insert", ex.Message);
                _logger.Error(message, ex);
                return StoreResult.InternalError;
            }
        }
        //Update a WebHook for a User. Also need to serialize into JSON and decrypt data using ASP.NET Core Protect.
        public override async Task<StoreResult> UpdateWebHookAsync(string user, WebHook webHook)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

            user = NormalizeKey(user);

            try
            {
                var registration = await _webHookRepository.GetOne<Registration>(webHook.Id, user);
                if (registration == null)
                {
                    return StoreResult.NotFound;
                }

                Registration registrationToUpdate = ConvertToUpdateRegistrationFromWebHook(user, webHook, registration);
                return await _webHookRepository.UpdateOne(registrationToUpdate, webHook.Id, user);
            }
            catch (OptimisticConcurrencyException ocex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.ConcurrencyError, "Update", ocex.Message);
                _logger.Error(message, ocex);
                return StoreResult.Conflict;
            }
            catch (DbException dbex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.MongoOperationFailed, "Update", dbex.Message);
                _logger.Error(message, dbex);
                return StoreResult.OperationError;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Update", ex.Message);
                _logger.Error(message, ex);
                return StoreResult.InternalError;
            }
        }
        // Delete a WebHook against a User and Id
        public override async Task<StoreResult> DeleteWebHookAsync(string user, string id)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            user = NormalizeKey(user);

            try
            {
                var match = await _webHookRepository.GetOne<Registration>(id, user);
                if (match == null)
                {
                    return StoreResult.NotFound;
                }
                return await _webHookRepository.DeleteOne<Registration>(id, user);
            }
            catch (OptimisticConcurrencyException ocex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.ConcurrencyError, "Delete", ocex.Message);
                _logger.Error(message, ocex);
                return StoreResult.Conflict;
            }
            catch (DbException dbex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.MongoOperationFailed, "Delete", dbex.Message);
                _logger.Error(message, dbex);
                return StoreResult.OperationError;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Delete", ex.Message);
                _logger.Error(message, ex);
                return StoreResult.InternalError;
            }
        }
        // Delete all WebHooks of a User
        public override async Task DeleteAllWebHooksAsync(string user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user = NormalizeKey(user);

            try
            {
                await _webHookRepository.DeleteMany<Registration>(user);
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "DeleteAll", ex.Message);
                _logger.Error(message, ex);
                throw new InvalidOperationException(message, ex);
            }
        }
        // Get One WebHooks providing its Id and User
        public override async Task<WebHook> LookupWebHookAsync(string user, string id)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            user = NormalizeKey(user);
            id = NormalizeKey(id);

            try
            {
                var match = await _webHookRepository.GetOne<Registration>(id, user);
                if (match != null)
                    return ConvertToWebHook(match);
                return null;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Lookup", ex.Message);
                _logger.Error(message, ex);
                throw new InvalidOperationException(message, ex);
            }
        }
        // Get all WebHooks of a User for an Action. It is used to send Notifications against all actions of a User
        public override async Task<ICollection<WebHook>> QueryWebHooksAsync(string user, IEnumerable<string> actions, Func<WebHook, string, bool> predicate)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user = NormalizeKey(user);
            predicate = predicate ?? DefaultPredicate;

            try
            {
                var registrations = await _webHookRepository.GetMany<Registration>(user);

                ICollection<WebHook> matches = registrations.Select(r => ConvertToWebHook(r)).Where(w => (MatchesAnyAction(w, actions) && predicate(w, user))).ToArray();
                return matches;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Get", ex.Message);
                _logger.Error(message, ex);
                throw new InvalidOperationException(message, ex);
            }
        }

        // Get all WebHooks of a User
        public override async Task<ICollection<WebHook>> GetAllWebHooksAsync(string user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user = NormalizeKey(user);

            try
            {
                var registrations = await _webHookRepository.GetMany<Registration>(user);

                ICollection<WebHook> matches = registrations.Select(r => ConvertToWebHook(r)).Where(w => w != null).ToArray();
                return matches;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Get", ex.Message);
                _logger.Error(message, ex);
                throw new InvalidOperationException(message, ex);
            }
        }
        // Get all WebHooks against an Action across all Users. Its used to send Notifications to all Parties against an Action
        public override async Task<ICollection<WebHook>> QueryWebHooksAcrossAllUsersAsync(IEnumerable<string> actions, Func<WebHook, string, bool> predicate)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            predicate = predicate ?? DefaultPredicate;

            try
            {
                var registrations = await _webHookRepository.GetAll<Registration>();

                var matches = new List<WebHook>();
                foreach (var registration in registrations)
                {
                    var webHook = ConvertToWebHook(registration);
                    if (MatchesAnyAction(webHook, actions) && predicate(webHook, registration.User))
                    {
                        matches.Add(webHook);
                    }
                }
                return matches;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.OperationFailed, "Get", ex.Message);
                _logger.Error(message, ex);
                throw new InvalidOperationException(message, ex);
            }
        }
        #endregion

        #region Utilities
        // Convert WebHook into Protected Data while inserting into Database
        public virtual Registration ConvertFromWebHook(string user, WebHook webHook)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

            var content = JsonConvert.SerializeObject(webHook, _serializerSettings);
            var protectedData = _protector != null ? _protector.Protect(content) : content;
            var registration = new Registration
            {
                User = user,
                Id = webHook.Id,
                ProtectedData = protectedData,
                CreatedDate = DateTime.UtcNow
            };
            return registration;
        }
        // Convert Protected Data into WebHook while getting from Database
        protected virtual WebHook ConvertToWebHook(Registration registration)
        {
            if (registration == null)
            {
                return null;
            }

            try
            {
                var content = _protector != null ? _protector.Unprotect(registration.ProtectedData) : registration.ProtectedData;
                var webHook = JsonConvert.DeserializeObject<WebHook>(content, _serializerSettings);
                return webHook;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Constants.Constants.BadWebHook, typeof(WebHook).Name, ex.Message);
                _logger.Error(message, ex);
            }
            return null;
        }
        // Update Registered WebHook Data
        protected virtual Registration ConvertToUpdateRegistrationFromWebHook(string user, WebHook webHook, Registration registration)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            registration.User = user;
            registration.Id = webHook.Id;
            var content = JsonConvert.SerializeObject(webHook, _serializerSettings);
            var protectedData = _protector != null ? _protector.Protect(content) : content;
            registration.ProtectedData = protectedData;
            registration.UpdatedDate = DateTime.UtcNow;

            return registration;
        }
        //Default Predicate
        private static bool DefaultPredicate(WebHook webHook, string user)
        {
            return true;
        }
        #endregion

    }
}