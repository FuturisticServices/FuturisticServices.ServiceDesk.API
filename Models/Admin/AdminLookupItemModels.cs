using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AdminLookupItemModel : BaseModel
    {
        public AdminLookupItemModel() { }

        public AdminLookupItemModel(AdminLookupItem entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Values = AdminLookupItemValueModel.Construct(entity.Values);
        }

        public static IEnumerable<AdminLookupItemModel> Construct(IEnumerable<AdminLookupItem> entities)
        {
            List<AdminLookupItemModel> model = new List<AdminLookupItemModel>();
            foreach (AdminLookupItem lookupItem in entities)
            {
                model.Add(new AdminLookupItemModel(lookupItem));
            }
            return model;
        }

        public AdminLookupItemModel(SystemLookupItem entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Values = AdminLookupItemValueModel.Construct(entity.Values);
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
        public IEnumerable<AdminLookupItemValueModel> Values { get; set; }
    }

    public class AdminLookupItemValueModel : BaseModel
    {
        public AdminLookupItemValueModel() { }

        public AdminLookupItemValueModel(AdminLookupItemValue entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public static IEnumerable<AdminLookupItemValueModel> Construct(IEnumerable<AdminLookupItemValue> values)
        {
            List<AdminLookupItemValueModel> model = new List<AdminLookupItemValueModel>();
            foreach (AdminLookupItemValue value in values)
            {
                model.Add(new AdminLookupItemValueModel(value));
            }
            return model;
        }

        public AdminLookupItemValueModel(SystemLookupItemValue entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public static IEnumerable<AdminLookupItemValueModel> Construct(IEnumerable<SystemLookupItemValue> values)
        {
            List<AdminLookupItemValueModel> model = new List<AdminLookupItemValueModel>();
            foreach (SystemLookupItemValue value in values)
            {
                model.Add(new AdminLookupItemValueModel(value));
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
    }
}
