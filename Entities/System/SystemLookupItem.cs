using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class SystemLookupItem: BaseEntity
    {
        public SystemLookupItem() 
        {
            Enabled = true;
        }

        public SystemLookupItem(SystemLookupItemModel model)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            Name = model.Name;
            CloneToAdminDatabase = model.CloneToAdminDatabase;
            Values = SystemLookupItemValue.Construct(model.Values);
            Enabled = model.Enabled;
        }

        public static IEnumerable<SystemLookupItem> Construct(IEnumerable<SystemLookupItemModel> models)
        {
            List<SystemLookupItem> model = new List<SystemLookupItem>();
            foreach (SystemLookupItemModel lookupItemModel in models)
            {
                model.Add(new SystemLookupItem(lookupItemModel));
            }
            return model;
        }

        /// <summary>
        /// Container parition key. *** CANNOT CHANGE VALUE (used by Cosmos DB)!!!!
        /// Group this setting is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required=Required.Always)]
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
        public IEnumerable<SystemLookupItemValue> Values { get; set; }
    }

    public class SystemLookupItemValue : BaseEntity
    {
        public SystemLookupItemValue() { }

        public SystemLookupItemValue(SystemLookupItemValueModel model) 
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
            Description = model.Description;
            Enabled = model.Enabled;
        }

        public static IEnumerable<SystemLookupItemValue> Construct(IEnumerable<SystemLookupItemValueModel> values)
        {
            List<SystemLookupItemValue> model = new List<SystemLookupItemValue>();
            foreach (SystemLookupItemValueModel value in values)
            {
                model.Add(new SystemLookupItemValue(value));
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
