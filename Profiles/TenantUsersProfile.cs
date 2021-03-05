using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Models;

namespace TangledServices.ServiceDesk.API.Profiles
{
    public class TenantUserProfile : Profile
    {
        public TenantUserProfile()
        {
            //CreateMap<zTenantUser, zTenantUserModel>().ReverseMap();
        }
    }
}
