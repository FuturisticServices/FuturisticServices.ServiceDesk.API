using System.ComponentModel.DataAnnotations;

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

        [Required, MaxLength(50)]
        public string Url { get; set; }
    }
}
