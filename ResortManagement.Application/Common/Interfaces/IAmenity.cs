﻿using ResortManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResortManagement.Application.Common.Interfaces
{
    public interface IAmenity : IRepository<Amenity>
    {
        void Update(Amenity entity);
    }
}
