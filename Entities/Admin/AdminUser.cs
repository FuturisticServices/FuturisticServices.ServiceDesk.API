using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class AdminUser : BaseEntity
    {
        public AdminUser() { }

        public AdminUser(AdminUserModel model)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            NamePrefix = model.NamePrefix;
            NameFirst = model.NameFirst;
            NameLast = model.NameLast;
            NameSuffix = model.NameSuffix;
            DisplayAs = model.DisplayAs;
            ProfileImageUrl = model.ProfileImageUrl;
            MustChangePasswordAtNextLogin = model.MustChangePasswordAtNextLogin;
            PasswordExpirationDateTime = model.PasswordExpirationDateTime;
            Enabled = model.Enabled;
            EmailAddresses = AdminEmailAddress.Construct(model.EmailAddresses);
            PhoneNumbers = AdminPhoneNumber.Construct(model.PhoneNumbers);
            Roles = model.Roles;
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
        [JsonProperty(PropertyName = "displayAs", Required = Required.Default)]
        [MinLength(3), MaxLength(25), DisplayName("Display as")]
        public string DisplayAs { get; set; }

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
        public string PasswordExpirationDateTime { get; set; }

        /// <summary>
        /// Email addresses associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "emailAddresses", Required = Required.Default)]
        [DisplayName("Email Addresses")]
        public List<AdminEmailAddress> EmailAddresses { get; set; }

        /// <summary>
        /// Phone numbers associated to the user.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumbers", Required = Required.Default)]
        [DisplayName("Phone numbers")]
        public List<AdminPhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// Role(s) the user is authorized to perform.
        /// </summary>
        [JsonProperty(PropertyName = "roles", Required = Required.Default)]
        [DisplayName("Roles")]
        public List<string> Roles { get; set; }
    }

    public class AdminAuthenticateUser : AdminUser
    {
        public AdminAuthenticateUser() { }

        public AdminAuthenticateUser(AdminAuthenticateUserModel model)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            NamePrefix = model.NamePrefix;
            NameFirst = model.NameFirst;
            NameLast = model.NameLast;
            NameSuffix = model.NameSuffix;
            Username = model.Username;
            Password = model.Password;
            DisplayAs = model.DisplayAs;
            ProfileImageUrl = model.ProfileImageUrl;
            MustChangePasswordAtNextLogin = model.MustChangePasswordAtNextLogin;
            PasswordExpirationDateTime = model.PasswordExpirationDateTime;
            Enabled = model.Enabled;
            EmailAddresses = AdminEmailAddress.Construct(model.EmailAddresses);
            PhoneNumbers = AdminPhoneNumber.Construct(model.PhoneNumbers);
            Roles = model.Roles;
        }

        public AdminAuthenticateUser(AdminUserModel model, AdminAuthenticateUser entity)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            NamePrefix = model.NamePrefix;
            NameFirst = model.NameFirst;
            NameLast = model.NameLast;
            NameSuffix = model.NameSuffix;
            Username = entity.Username;
            Password = entity.Password;
            DisplayAs = model.DisplayAs;
            ProfileImageUrl = model.ProfileImageUrl;
            MustChangePasswordAtNextLogin = model.MustChangePasswordAtNextLogin;
            PasswordExpirationDateTime = model.PasswordExpirationDateTime;
            Enabled = model.Enabled;
            EmailAddresses = AdminEmailAddress.Construct(model.EmailAddresses);
            PhoneNumbers = AdminPhoneNumber.Construct(model.PhoneNumbers);
            Roles = model.Roles;
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
        [Required, MinLength(8), MaxLength(50), DisplayName("Password")]
        public string Password { get; set; }
    }

    public class AdminUserResetUsername : BaseEntity
    {
        public AdminUserResetUsername() { }

        public AdminUserResetUsername(AdminUserResetUsernameModel entity)
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

    public class AdminUserResetPassword : BaseEntity
    {
        public AdminUserResetPassword() { }

        public AdminUserResetPassword(AdminUserResetPasswordModel entity)
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
