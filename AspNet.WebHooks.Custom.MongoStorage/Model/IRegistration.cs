﻿
namespace Microsoft.AspNet.WebHooks.Storage
{
    /// <summary>
    /// Defines the WebHook registration data model for rows stored in Mongo DB.
    /// </summary>
    public interface IRegistration
    {
        /// <summary>
        /// Gets or sets the user ID for this WebHook registration.
        /// </summary>
        string User { get; set; }

        /// <summary>
        /// Gets or sets the ID of this WebHook registration.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the data included in this WebHook registration. Note that this is encrypted 
        /// as it contains sensitive information.
        /// </summary>
        string ProtectedData { get; set; }
    }
}
