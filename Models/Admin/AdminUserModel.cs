using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AdminUserModel : BaseModel
    {
        public AdminUserModel() { }

        public AdminUserModel(AdminUser entity)
        {
            Id = entity.Id;
            EmployeeId = entity.EmployeeId;
            NamePrefix = entity.NamePrefix;
            NameFirst = entity.NameFirst;
            NameLast = entity.NameLast;
            NameSuffix = entity.NameSuffix;
            Username = entity.Username;
            Password = entity.Password;
            DisplayName = entity.DisplayName;
            ProfileImageUrl = entity.ProfileImageUrl;
            MustChangePasswordAtNextLogin = entity.MustChangePasswordAtNextLogin;
            PasswordExpirationDateTime = entity.PasswordExpirationDateTime;
            CloneToATenantDatabase = entity.CloneToATenantDatabase;
            Enabled = entity.Enabled;
            EmailAddresses = EmailAddressModel.Construct(entity.EmailAddresses);
            PhoneNumbers = PhoneNumberModel.Construct(entity.PhoneNumbers);
            Roles = entity.Roles;
        }

        public AdminUserModel(SystemUser entity)
        {
            Id = entity.Id;
            EmployeeId = entity.EmployeeId;
            NamePrefix = entity.NamePrefix;
            NameFirst = entity.NameFirst;
            NameLast = entity.NameLast;
            NameSuffix = entity.NameSuffix;
            Username = entity.Username;
            Password = entity.Password;
            DisplayName = entity.DisplayName;
            ProfileImageUrl = entity.ProfileImageUrl;
            MustChangePasswordAtNextLogin = entity.MustChangePasswordAtNextLogin;
            PasswordExpirationDateTime = entity.PasswordExpirationDateTime;
            CloneToATenantDatabase = entity.CloneToAdminDatabase;
            Enabled = entity.Enabled;
            EmailAddresses = EmailAddressModel.Construct(entity.EmailAddresses);
            PhoneNumbers = PhoneNumberModel.Construct(entity.PhoneNumbers);
            Roles = entity.Roles;
        }

        /// <summary>
        /// Suffix of the user.
        /// </summary>
        [JsonProperty(PropertyName = "employeeId", Required = Required.Always)]
        [Required, DisplayName("Employee ID")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// Suffix of the user.
        /// </summary>
        [JsonProperty(PropertyName = "namePrefix", Required = Required.Default)]
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
        [JsonProperty(PropertyName = "nameSuffix", Required = Required.Default)]
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
        [JsonProperty(PropertyName = "displayName", Required = Required.Default)]
        [MinLength(3), MaxLength(25), DisplayName("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// URL of the user's profile image.
        /// </summary>
        [JsonProperty(PropertyName = "profileImageUrl", Required = Required.Default)]
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
        [JsonProperty(PropertyName = "passwordExpirationDateTime", Required = Required.Default)]
        [DisplayName("Password expiration date and time")]
        public DateTime? PasswordExpirationDateTime { get; set; }

        /// <summary>
        /// Controls whether this user can attempt to login.
        /// </summary>
        [JsonProperty(PropertyName = "cloneToATenantDatabase", Required = Required.Default)]
        [DefaultValue(false), DisplayName("Clone to tenant database")]
        public bool CloneToATenantDatabase { get; set; }

        /// <summary>
        /// Controls whether this user can attempt to login.
        /// </summary>
        [JsonProperty(PropertyName = "enabled", Required = Required.Always)]
        [Required, DisplayName("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Email addresses associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "emailAddresses", Required = Required.Default)]
        [DisplayName("Email Addresses")]
        public List<EmailAddressModel> EmailAddresses { get; set; }

        /// <summary>
        /// Phone numbers associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumbers", Required = Required.Default)]
        [DisplayName("Phone numbers")]
        public List<PhoneNumberModel> PhoneNumbers { get; set; }

        [JsonProperty(PropertyName = "roles", Required = Required.Default)]
        [DisplayName("Roles")]
        public List<string> Roles { get; set; }
    }
}