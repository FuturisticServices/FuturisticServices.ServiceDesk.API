﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Models
{
    public class BaseModel
    {
        /// <summary>
        /// A unique GUID identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        [Required, DisplayName("ID")]
        public string Id { get; set; }
    }
}
