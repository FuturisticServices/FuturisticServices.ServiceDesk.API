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
    public class LookupItem: BaseEntity
    {
        public LookupItem() { }

        public LookupItem(LookupItemModel model)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            Name = model.Name;
            DisplayAs = model.DisplayAs;
            CloneToAdminDatabase = model.CloneToAdminDatabase;
            Values = LookupItemValue.ConvertModelToEntity(model.Values);
        }

        /// <summary>
        /// Container parition key. *** CANNOT CHANGE VALUE (used by Cosmos DB)!!!!
        /// Group this setting is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required=Required.Always)]
        [Required, MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Friendly value to use for display.
        /// </summary>
        [JsonProperty(PropertyName = "displayAs", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Display as")]
        public string DisplayAs { get; set; }

        /// <summary>
        /// Friendly value to use for display.
        /// </summary>
        [JsonProperty(PropertyName = "cloneToAdminDatabase", Required = Required.Default)]
        [DefaultValue(false), DisplayName("Clone to admin database")]
        public bool CloneToAdminDatabase { get; set; }

        /// <summary>
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "values", Required = Required.Default)]
        [DisplayName("Values")]
        public IEnumerable<LookupItemValue> Values { get; set; }
    }

    public class LookupItemValue : BaseEntity
    {
        public LookupItemValue() { }

        public LookupItemValue(LookupItemValueModel model) 
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
        }

        public static IEnumerable<LookupItemValue> ConvertModelToEntity(IEnumerable<LookupItemValueModel> values)
        {
            List<LookupItemValue> model = new List<LookupItemValue>();
            foreach (LookupItemValueModel value in values)
            {
                model.Add(new LookupItemValue(value));
            }
            return model;
        }

        /// <summary>
        /// System value. *** SHOULD NOT CHANGE!!!!
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

        ///// <summary>
        ///// List of values.
        ///// </summary>
        //[JsonProperty(PropertyName = "items", Required = Required.Default)]
        //[DisplayName("Items")]
        //public IEnumerable<LookupItemValue> Items { get; set; }
    }
}
