using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    /// <summary>
    /// Model to register a tenant.
    /// </summary>
    public class CustomerModel : BaseModel
    {
        public CustomerModel() { }

        //  Create model from entity.
        public CustomerModel(CustomerEntity entity)
        {
            Id = entity.Id;
            LegalName = entity.LegalEntityName;
            Name = entity.Name;
            AdminMoniker = entity.AdminMoniker;
            TenantMonikers = entity.TenantMonikers;
            Address = new SystemAddressModel(entity.Address);
            PhoneNumber = new SystemPhoneNumberModel(entity.PhoneNumber);
            Website = new SystemWebsiteModel(entity.Website);
            PointOfContact = new PointOfContactModel(entity.PointOfContact);
            Enabled = entity.Enabled;
        }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "legalEntityName", Required = Required.Default)]
        [DataType(DataType.Text), StringLength(50, ErrorMessage = "Legal entity name cannot exceed {1} characters."), DisplayName("Legal entity name")]
        public string LegalName { get; set; }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, DataType(DataType.Text), StringLength(50, ErrorMessage = "Name cannot exceed {1} characters."), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "adminMoniker", Required = Required.Always)]
        [Required, DataType(DataType.Text), StringLength(10, ErrorMessage = "Moniker cannot exceed {1} characters."), DisplayName("Admin moniker")]
        public string AdminMoniker { get; set; }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "tenantMonikers", Required = Required.AllowNull)]
        [DisplayName("Tenant monikers")]
        public List<string> TenantMonikers { get; set; }

        /// <summary>
        /// Primary address of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required]
        public SystemAddressModel Address { get; set; }

        /// <summary>
        /// Primary phone number of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumber", Required = Required.Always)]
        [Required]
        public SystemPhoneNumberModel PhoneNumber { get; set; }

        /// <summary>
        /// Company public website URL.
        /// </summary>
        [JsonProperty(PropertyName = "website", Required = Required.Always)]
        [Required]
        public SystemWebsiteModel Website { get; set; }

        /// <summary>
        /// Tenant point of contact.
        /// </summary>
        [JsonProperty(PropertyName = "pointOfContact", Required = Required.Always)]
        [Required]
        public PointOfContactModel PointOfContact { get; set; }

        /// <summary>
        /// Customer administrative website users.
        /// </summary>
        [JsonProperty(PropertyName = "users", Required = Required.Default)]
        [DisplayName("Users")]
        public List<AdminAuthenticateUserModel> Users { get; set; }
    }
}
