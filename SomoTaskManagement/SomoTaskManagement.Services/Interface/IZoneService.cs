using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IZoneService
    {
        Task AddZone(Zone zone);
        Task DeleteZone(Zone zone);
        Task<IEnumerable<ZoneModel>> GetAllByArea(int id);
        Task<IEnumerable<ZoneModel>> GetByArea(int id);
        Task<IEnumerable<ZoneModel>> GetByAreaAndLivestock(int id);
        Task<IEnumerable<ZoneModel>> GetByAreaAndPlant(int id);
        Task<IEnumerable<ZoneModel>> GetByFarmId(int id);
        Task<IEnumerable<ZoneModel>> GetByZoneTypeId(int id);
        Task<Zone> GetZone(int id);
        Task<IEnumerable<ZoneModel>> ListZone();
        Task<IEnumerable<ZoneModel>> ListZoneActive();
        Task UpdateStatus(int id);
        Task UpdateZone(Zone zone);
    }
}
