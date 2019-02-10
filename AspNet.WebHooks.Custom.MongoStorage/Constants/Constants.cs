
namespace AspNet.WebHooks.Custom.MongoStorage.Constants
{
    public static class Constants
    {
        #region ErrorMessages
        public const string EmptyString = "Argument {0} must not be the empty string or null.";
        public const string ConcurrencyError = "Concurrency failure during '{0}' operation: '{1}'.";
        public const string BadWebHook = "The '{0}' could not be retrieved from the SQL store: {1}";
        public const string NoConnectionString = "Please provide a SQL connection string with name '{0}' in the configuration string section of the 'Web.Config' file.";
        public const string OperationFailed = "General error during '{0}' operation: '{1}'.";
        public const string MongoOperationFailed = "Operation '{0}' failed with error: '{1}'.";
        #endregion
    }
}
