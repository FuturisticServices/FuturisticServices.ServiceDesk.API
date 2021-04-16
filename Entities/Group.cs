using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Common;

namespace TangledServices.ServicePortal.API.Entities
{
    public class Group : BaseEntity
    {
        /// <summary>
        /// The parent group ID this group belongs to (null ~ top level group).
        /// </summary>
        [JsonProperty(PropertyName = "parentGroupId", Required = Required.Always)]
        [Required, DisplayName("Parent group")]
        public string ParentGroupId { get; set; }

        /// <summary>
        /// Group(s) the user is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// A descriptoon/purpose of the group.
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.Always)]
        [Required, DisplayName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// URL of the user's profile image.
        /// </summary>
        [JsonProperty(PropertyName = "profileImageUrl", Required = Required.Always)]
        [Required, MinLength(3), MaxLength(25), DisplayName("Profile image URL")]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// The manager {User} of the group.
        /// </summary>
        [JsonProperty(PropertyName = "managerUserId", Required = Required.Always)]
        [Required, DisplayName("Manager ID")]
        public string ManagerUserId { get; set; }

        /// <summary>
        /// The manager {User} of the group.
        /// </summary>
        [JsonProperty(PropertyName = "users", Required = Required.Always)]
        [Required, DisplayName("Users")]
        public List<User> Users { get; set; }

        /// <summary>
        /// Create/update history of this object.
        /// </summary>
        [JsonProperty(PropertyName = "objectHistory", Required = Required.Always)]
        [Required, DisplayName("Object history")]
        public List<ObjectHistory> ObjectHistory { get; set; }
    }
}
