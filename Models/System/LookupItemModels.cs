using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class LookupItemModel : BaseModel
    {
        public LookupItemModel() { }

        public LookupItemModel(LookupItem entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            DisplayAs = entity.DisplayAs;
            Clone = entity.Clone;
            Values = LookupItemValueModel.Construct(entity.Values);
        }

        public static IEnumerable<LookupItemModel> Construct(IEnumerable<LookupItem> entities)
        {
            List<LookupItemModel> model = new List<LookupItemModel>();
            foreach (LookupItem lookupItem in entities)
            {
                model.Add(new LookupItemModel(lookupItem));
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
        /// Friendly value to use for display.
        /// </summary>
        [JsonProperty(PropertyName = "displayAs", Required=Required.Always)]
        [Required, MaxLength(50), DisplayName("Display as")]
        public string DisplayAs { get; set; }

        /// <summary>
        /// Friendly value to use for display.
        /// </summary>
        [JsonProperty(PropertyName = "clone", Required = Required.Default)]
        [DefaultValue(false), DisplayName("Clone")]
        public bool Clone { get; set; }

        /// <summary>
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "values", Required = Required.Default)]
        [DisplayName("Values")]
        public IEnumerable<LookupItemValueModel> Values { get; set; }
    }

    public class LookupItemValueModel : BaseModel
    {
        public LookupItemValueModel() { }

        public LookupItemValueModel(LookupItemValue entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public static IEnumerable<LookupItemValueModel> Construct(IEnumerable<LookupItemValue> values)
        {
            List<LookupItemValueModel> model = new List<LookupItemValueModel>();
            foreach (LookupItemValue value in values)
            {
                model.Add(new LookupItemValueModel(value));
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
        //public IEnumerable<LookupItemValueModel> Items { get; set; }
    }
}
