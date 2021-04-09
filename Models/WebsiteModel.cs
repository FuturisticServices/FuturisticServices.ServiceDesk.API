using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Models
{
    public class WebsiteModel
    {
        [Required, MaxLength(50)]
        public string Url { get; set; }
    }
}
