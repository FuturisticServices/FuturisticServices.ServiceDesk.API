using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemUserModel : BaseModel
    {
        public SystemUserModel() { }

        public SystemUserModel(SystemUser entity)
        {
            NamePrefix = entity.NamePrefix;
            NameFirst = entity.NameFirst;
            NameLast = entity.NameLast;
            NameSuffix = entity.NameSuffix;
            DisplayName = entity.DisplayName;
            ProfileImageUrl = entity.ProfileImageUrl;
            MustChangePasswordAtNextLogin = entity.MustChangePasswordAtNextLogin;
            PasswordExpirationDateTime = entity.PasswordExpirationDateTime;
            Enabled = entity.Enabled;
            EmailAddresses = SystemEmailAddressModel.Construct(entity.EmailAddresses);
            PhoneNumbers = SystemPhoneNumberModel.Construct(entity.PhoneNumbers);
            Roles = entity.Roles;
            CloneToAdminDatabase = entity.CloneToAdminDatabase;
        }

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
        [JsonProperty(PropertyName = "enabled", Required = Required.Always)]
        [Required, DefaultValue(true), DisplayName("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Email addresses associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "emailAddresses", Required = Required.Default)]
        [DisplayName("Email Addresses")]
        public List<SystemEmailAddressModel> EmailAddresses { get; set; }

        /// <summary>
        /// Phone numbers associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumbers", Required = Required.Default)]
        [DisplayName("Phone numbers")]
        public List<SystemPhoneNumberModel> PhoneNumbers { get; set; }

        /// <summary>
        /// Role(s) the user is authorized to perform.
        /// </summary>
        [JsonProperty(PropertyName = "roles", Required = Required.Default)]
        [DisplayName("Roles")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// Controls whether this user can attempt to login.
        /// </summary>
        [JsonProperty(PropertyName = "cloneToAdminDatabase", Required = Required.Default)]
        [DefaultValue(false), DisplayName("Clone to admin database")]
        public bool CloneToAdminDatabase { get; set; }
    }

    public class SystemUserAuthenticateModel : SystemUserModel
    {
        public SystemUserAuthenticateModel() { }

        public SystemUserAuthenticateModel(SystemAuthenticateUser entity)
        {
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
            Enabled = entity.Enabled;
            EmailAddresses = SystemEmailAddressModel.Construct(entity.EmailAddresses);
            PhoneNumbers = SystemPhoneNumberModel.Construct(entity.PhoneNumbers);
            Roles = entity.Roles;
            CloneToAdminDatabase = entity.CloneToAdminDatabase;
        }

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
        [Required, DisplayName("Password")]
        public string Password { get; set; }
    }

    public class SystemUserResetUsernameModel : BaseModel
    {
        public SystemUserResetUsernameModel() { }

        public SystemUserResetUsernameModel(SystemUserResetUsername entity)
        {
            ResetUsername = entity.ResetUsername;
            ConfirmPassword = entity.ConfirmPassword;
        }

        /// <summary>
        /// Unique user identifier used for authentication.
        /// </summary>
        [JsonProperty(PropertyName = "resetUsername", Required = Required.Always)]
        [Required, MinLength(3), MaxLength(25), DisplayName("Reset username")]
        public string ResetUsername { get; set; }

        /// <summary>
        /// Hashed password used for authentication.
        /// </summary>
        [JsonProperty(PropertyName = "confirmPassword", Required = Required.Always)]
        [Required, DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }

    public class SystemUserResetPasswordModel : BaseModel
    {
        public SystemUserResetPasswordModel() { }

        public SystemUserResetPasswordModel(SystemUserResetPassword entity)
        {
            ResetPassword = entity.ResetPassword;
            ConfirmPassword = entity.ConfirmPassword;
        }

        /// <summary>
        /// Unique user identifier used for authentication.
        /// </summary>
        [JsonProperty(PropertyName = "resetPassword", Required = Required.Always)]
        [Required, MinLength(3), MaxLength(25), DisplayName("Reset password")]
        public string ResetPassword { get; set; }

        /// <summary>
        /// Hashed password used for authentication.
        /// </summary>
        [JsonProperty(PropertyName = "confirmPassword", Required = Required.Always)]
        [Required, DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
