using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Models
{
    public class PhoneNumberModel
    {
        [Required, MinLength(10), MaxLength(10)]
        public string Number { get; set; }
    }
}
