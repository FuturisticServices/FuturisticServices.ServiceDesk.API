﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class WebsiteModel : BaseModel
    {
        public WebsiteModel() { }

        public WebsiteModel (Website entity)
        {
            Id = entity.Id;
            Url = entity.Url;
        }

        /// <summary>
        /// Associated LookupItem.WebsiteTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public LookupItemEntity Type { get; set; }

        [Required, MaxLength(50)]
        public string Url { get; set; }
    }
}
