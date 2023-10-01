using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IZoneTypeService
    {
        Task AddZoneType(ZoneType zoneType);
        Task DeleteZoneType(ZoneType zoneType);
        Task<ZoneType> GetZoneType(int id);
        Task<IEnumerable<ZoneType>> ListZone();
        Task UpdateZoneType(ZoneType zoneType);
    }
}
