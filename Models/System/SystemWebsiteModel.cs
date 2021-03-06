﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemWebsiteModel : BaseModel
    {
        public SystemWebsiteModel() { }

        public SystemWebsiteModel (SystemWebsite entity)
        {
            Url = entity.Url;
        }

        /// <summary>
        /// Associated LookupItem.WebsiteTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DataType(DataType.Url), DisplayName("Type")]
        public SystemLookupItemValueModel Type { get; set; }

        [Required, DataType(DataType.Url)]
        public string Url { get; set; }
    }
}
