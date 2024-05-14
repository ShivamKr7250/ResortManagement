using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResortManagement.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IVillaRepository Villa {  get; }
        IVillaNumberRepository VillaNumber { get; }
        IAmenity Amenity { get; }
        IBookingRepository Booking { get; }
        void Save();
    }
}
