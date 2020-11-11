using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TCE.ProjectVolume.Ticketing.API.Entities;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class TenantUserAuthenticationModel
    {
        [JsonConstructor]
        public TenantUserAuthenticationModel() { }

        public string username { get; set; }
        public string password { get; set; }
    }

    /// <summary>
    /// Model used for displaying ALL properties of a tenant tenantUser.
    /// </summary>
    public class zTenantUserModel
    {
        [JsonConstructor]
        public zTenantUserModel() { }

        public zTenantUserModel(zTenantUser tenantUser)
        {
            Id = string.IsNullOrEmpty(tenantUser.Id) ? Guid.NewGuid().ToString() : tenantUser.Id;
            PartitionKeyId = tenantUser.PartitionKeyId.Trim();
            Username = tenantUser.Username.Trim();
            Password = tenantUser.Password.Trim();
            Enabled = tenantUser.Enabled;
            ForcePasswordChangeAtNextLogin = tenantUser.ForcePasswordChangeAtNextLogin;
            FirstName = tenantUser.FirstName.Trim();
            LastName = tenantUser.LastName.Trim();
            Title = tenantUser.Title.Trim();
            UserRoles = tenantUser.TenantUserRoles;
            EmailAddresses = tenantUser.EmailAddresses;
            PhoneNumbers = tenantUser.PhoneNumbers;
        }

        public string Id { get; set; }
        public string PartitionKeyId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public bool ForcePasswordChangeAtNextLogin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string[] UserRoles { get; set; }
        public List<zEmailAddressModel> EmailAddresses { get; set; }
        public List<zPhoneNumberModel> PhoneNumbers { get; set; }
        public List<zAddressModel> Addresses { get; set; }

        //public string FullName { get; set; }
    }

    public class TenantUserIdModel
    {
        [JsonConstructor]
        public TenantUserIdModel() { }

        public string Id { get; set; }
    }
}
