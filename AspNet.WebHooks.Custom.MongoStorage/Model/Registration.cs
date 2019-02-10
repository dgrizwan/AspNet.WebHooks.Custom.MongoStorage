using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.WebHooks.Storage
{
    /// <summary>
    /// Defines the WebHook registration data model for rows stored in Mongo DB.
    /// </summary>
    [Table("WebHooks")]
    public class Registration : IRegistration
    {
        /// <inheritdoc />
        [Key]
        [StringLength(256)]
        [Column(Order = 0)]
        public string User { get; set; }

        /// <inheritdoc />
        [Key]
        [StringLength(64)]
        [Column(Order = 1)]
        public string Id { get; set; }

        /// <inheritdoc />
        [Required]
        public string ProtectedData { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
