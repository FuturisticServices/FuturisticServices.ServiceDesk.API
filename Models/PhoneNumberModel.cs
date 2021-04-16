using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class PhoneNumberModel : BaseModel
    {
        public PhoneNumberModel() { }

        public PhoneNumberModel(PhoneNumber entity)
        {
            Id = entity.Id;
            Number = entity.Number;
        }

        [Required, MinLength(10), MaxLength(10)]
        public string Number { get; set; }
    }
}
