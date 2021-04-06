using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Profiles
{
    public class TicketsProfile : Profile
    {
        public TicketsProfile()
        {
            CreateMap<Ticket, TicketsModel>().ReverseMap();
        }
    }
}
