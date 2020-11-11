using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Spatial;
using Newtonsoft.Json;

using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    public class User : EntityBase
    {
        /// <summary>
        /// Suffix of the user.
        /// </summary>
        [JsonProperty(PropertyName = "employeeId", Required = Required.Always)]
        [Required, DisplayName("Employee ID")]
        public string EmployeeID { get; set; }

        /// <summary>
        /// Suffix of the user.
        /// </summary>
        [JsonProperty(PropertyName = "namePrefix", Required = Required.AllowNull)]
        [MinLength(1), MaxLength(10), DisplayName("Prefix")]
        public string NamePrefix { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        [JsonProperty(PropertyName = "nameFirst", Required = Required.Always)]
        [Required, MinLength(1), MaxLength(50), DisplayName("First name")]
        public string NameFirst { get; set; }

        /// <summary>
        /// Last name of the user.
        /// </summary>
        [JsonProperty(PropertyName = "nameLast", Required = Required.Always)]
        [Required, MinLength(1), MaxLength(50), DisplayName("Last name")]
        public string NameLast { get; set; }

        /// <summary>
        /// Suffix of the user.
        /// </summary>
        [JsonProperty(PropertyName = "nameSuffix", Required = Required.AllowNull)]
        [MinLength(1), MaxLength(10), DisplayName("Suffix")]
        public string NameSuffix { get; set; }

        /// <summary>
        /// Unique user identifier used for authentication.
        /// </summary>
        [JsonProperty(PropertyName = "username", Required = Required.Always)]
        [Required, MinLength(3), MaxLength(25), DisplayName("Username")]
        public string Username { get; set; }

        /// <summary>
        /// Hashed password used for authentication.
        /// </summary>
        [JsonProperty(PropertyName = "password", Required = Required.Always)]
        [Required, MinLength(8), MaxLength(50), DisplayName("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Unique identifier used when displaying the 'user' within the interface.
        /// </summary>
        [JsonProperty(PropertyName = "displayName", Required = Required.Always)]
        [Required, MinLength(3), MaxLength(25), DisplayName("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// URL of the user's profile image.
        /// </summary>
        [JsonProperty(PropertyName = "profileImageUrl", Required = Required.AllowNull)]
        [MinLength(3), MaxLength(25), DisplayName("Profile image URL")]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Admin setting used to force the user to change their password.
        /// </summary>
        [JsonProperty(PropertyName = "mustChangePasswordAtNextLogin", Required = Required.Always)]
        [Required, DisplayName("Must change password at next login")]
        public bool MustChangePasswordAtNextLogin { get; set; }

        /// <summary>
        /// Admin setting used to force the user to change their password.
        /// If value is null, the password does NOT expire.
        /// </summary>
        [JsonProperty(PropertyName = "passwordExpirationDateTime", Required = Required.AllowNull)]
        [Required, DisplayName("Password expiration date and time")]
        public DateTime? PasswordExpirationDateTime { get; set; }

        /// <summary>
        /// Controls whether this user can attempt to login.
        /// </summary>
        [JsonProperty(PropertyName = "enabled", Required = Required.Always)]
        [Required, DisplayName("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Email addresses associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "emailAddresses", Required = Required.Always)]
        [Required, DisplayName("Email Addresses")]
        public List<EmailAddress> EmailAddresses { get; set; }

        /// <summary>
        /// Phone numbers associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumbers", Required = Required.Always)]
        [Required, DisplayName("Phone numbers")]
        public List<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// Group(s) the user is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "groups", Required = Required.Always)]
        [Required, DisplayName("Groups")]
        public List<Group> Groups { get; set; }
    }
}