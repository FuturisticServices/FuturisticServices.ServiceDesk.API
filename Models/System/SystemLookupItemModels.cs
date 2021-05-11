using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemLookupItemModel : BaseModel
    {
        public SystemLookupItemModel() 
        {
            Enabled = true;    
        }

        public SystemLookupItemModel(SystemLookupItem entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CloneToAdminDatabase = entity.CloneToAdminDatabase;
            Values = SystemLookupItemValueModel.Construct(entity.Values);
            Enabled = entity.Enabled;
        }

        public static IEnumerable<SystemLookupItemModel> Construct(IEnumerable<SystemLookupItem> entities)
        {
            List<SystemLookupItemModel> model = new List<SystemLookupItemModel>();
            foreach (SystemLookupItem lookupItem in entities)
            {
                model.Add(new SystemLookupItemModel(lookupItem));
            }
            return model;
        }

        /// <summary>
        /// Container parition key. *** CANNOT CHANGE VALUE (used by Cosmos DB)!!!!
        /// Group this setting is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Whether to clone this item when a admin database is being created.
        /// </summary>
        [JsonProperty(PropertyName = "cloneToAdminDatabase", Required = Required.Default)]
        [DefaultValue(false), DisplayName("Clone to admin database")]
        public bool CloneToAdminDatabase { get; set; }

        /// <summary>
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "values", Required = Required.Default)]
        [DisplayName("Values")]
        public IEnumerable<SystemLookupItemValueModel> Values { get; set; }
    }

    public class SystemLookupItemValueModel : BaseModel
    {
        public SystemLookupItemValueModel() { }

        public SystemLookupItemValueModel(SystemLookupItemValue entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
            Description = entity.Description;
        }

        public static IEnumerable<SystemLookupItemValueModel> Construct(IEnumerable<SystemLookupItemValue> values)
        {
            List<SystemLookupItemValueModel> model = new List<SystemLookupItemValueModel>();
            foreach (SystemLookupItemValue value in values)
            {
                model.Add(new SystemLookupItemValueModel(value));
            }
            return model;
        }

        /// <summary>
        /// Name of the item.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        [MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Value of the item.
        /// </summary>
        [JsonProperty(PropertyName = "abbreviation", Required = Required.Default)]
        [MaxLength(10), DisplayName("Abbreviation")]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Description and/or purpose of the value.
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.Default)]
        [DisplayName("Description")]
        public string Description { get; set; }
    }
}
