using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json;

using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// A website accessible from a modern web browser.
    /// </summary>
    public class Website : EntityBase
    {
        public Website() { }

        public Website(WebsiteModel model, LookupItem type)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            Url = model.Url;
        }

        /// <summary>
        /// Purpose/usage of the website.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required]
        public LookupItem Type { get; set; }

        /// <summary>
        /// The website URL.
        /// </summary>
        [JsonProperty(PropertyName = "url", Required = Required.Always), DataType(DataType.Url)]
        [Required]
        public string Url { get; set; }
    }
}