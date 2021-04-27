using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class CustomerEntity : BaseEntity
    {
        public CustomerEntity() { }

        //  Create entity from model.
        public CustomerEntity(CustomerModel model)
        {
            Id = model.Id;
            LegalName = model.LegalName;
            Name = model.Name;
            AdminMoniker = model.AdminMoniker;
            TenantMonikers = model.TenantMonikers;
            Address = new Address(model.Address);
            PhoneNumber = new PhoneNumber(model.PhoneNumber);
            Website = new Website(model.Website);
            PointOfContact = new PointOfContact(model.PointOfContact);
            Enabled = model.Enabled;
        }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "legalName", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Legal name")]
        public string LegalName { get; set; }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "adminMoniker", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Admin moniker")]
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
        public Address Address { get; set; }

        /// <summary>
        /// Primary phone number of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumber", Required = Required.Always)]
        [Required]
        public PhoneNumber PhoneNumber { get; set; }

        /// <summary>
        /// Company public website URL.
        /// </summary>
        [JsonProperty(PropertyName = "website", Required = Required.Always)]
        [Required]
        public Website Website { get; set; }

        /// <summary>
        /// Tenant point of contact.
        /// </summary>
        [JsonProperty(PropertyName = "pointOfContact", Required = Required.Always)]
        [Required]
        public PointOfContact PointOfContact { get; set; }

        /// <summary>
        /// Determines if the tenant account is currently active.
        /// </summary>
        [JsonProperty(PropertyName = "enabled", Required = Required.Always)]
        [Required, DisplayName("Enabled")]
        public bool Enabled { get; set; }
    }
}
