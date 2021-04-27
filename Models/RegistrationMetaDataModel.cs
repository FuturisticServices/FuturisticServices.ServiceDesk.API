﻿using System.Collections.Generic;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class RegistrationMetaDataModel
    {
        /// <summary>
        /// List of states.
        /// </summary>
        public List<LookupItem> States { get; set; }

        /// <summary>
        /// List of countries.
        /// </summary>
        public List<LookupItem> Countries { get; set; }

        /// <summary>
        /// List of subscriptions.
        /// </summary>
        public List<Subscription> Subscriptions { get; set; }
    }
}
