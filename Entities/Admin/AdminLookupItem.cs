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
    public class AdminLookupItem : BaseEntity
    {
        public AdminLookupItem() { }

        public AdminLookupItem(AdminLookupItemModel model)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            Name = model.Name;
            Values = AdminLookupItemValue.Convert(model.Values);
        }

        public static IEnumerable<AdminLookupItem> Construct(IEnumerable<AdminLookupItemModel> models)
        {
            List<AdminLookupItem> model = new List<AdminLookupItem>();
            foreach (AdminLookupItemModel lookupItemModel in models)
            {
                model.Add(new AdminLookupItem(lookupItemModel));
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
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "values", Required = Required.Default)]
        [DisplayName("Values")]
        public IEnumerable<AdminLookupItemValue> Values { get; set; }
    }

    public class AdminLookupItemValue : BaseEntity
    {
        public AdminLookupItemValue() { }

        public AdminLookupItemValue(AdminLookupItemValueModel model)
        {
            Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
        }

        public static IEnumerable<AdminLookupItemValue> Convert(IEnumerable<AdminLookupItemValueModel> values)
        {
            List<AdminLookupItemValue> model = new List<AdminLookupItemValue>();
            foreach (AdminLookupItemValueModel value in values)
            {
                model.Add(new AdminLookupItemValue(value));
            }
            return model;
        }

        /// <summary>
        /// Admin value. *** SHOULD NOT CHANGE!!!!
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
    }
}
