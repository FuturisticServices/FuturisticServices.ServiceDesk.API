using System;
using System.Collections.Generic;
using TangledServices.ServiceDesk.API.Entities;

namespace TangledServices.ServiceDesk.API.Models
{
    public class UserModel : BaseModel
    {
        /// <summary>
        /// Unique ID associated to a user by the tenant.
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// User's name prefix.
        /// </summary>
        public string NamePrefix { get; set; }

        /// <summary>
        /// User's first name.
        /// </summary>
        public string NameFirst { get; set; }

        /// <summary>
        /// User's last name.
        /// </summary>
        public string NameLast { get; set; }

        /// <summary>
        /// User's name suffix.
        /// </summary>
        public string NameSuffix { get; set; }

        /// <summary>
        /// User's login user ID.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User's login password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User's display name as it appears in the user interfaces.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// URL of user's image displayed in the user interfaces.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Determines if user is forced to change their password upon the next successful login.
        /// </summary>
        public bool MustChangePasswordAtNextLogin { get; set; }

        /// <summary>
        /// The date when the user is forced to the change their password upon the 1st successful login.
        /// </summary>
        public DateTime? PasswordExpirationDateTime { get; set; }

        /// <summary>
        /// Controls if the user's account is active or disabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// List of user email addresses.
        /// </summary>
        public List<EmailAddress> EmailAddresses { get; set; }

        /// <summary>
        /// List of user phone numbers.
        /// </summary>
        public List<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// List if groups the user is assigned/associated to/with.
        /// </summary>
        public List<Group> Groups { get; set; }
    }
}
