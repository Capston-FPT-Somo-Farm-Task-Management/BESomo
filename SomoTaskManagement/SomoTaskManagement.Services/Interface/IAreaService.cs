using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Area;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IAreaService
    {
        Task AddArea(AreaCreateUpdateModel area);
        Task DeleteArea(int areaId);
        Task DeleteByStatus(int id);
        Task<IEnumerable<AreaModel>> GetAllAreaByFarmId(int id);
        Task<AreaModel> GetArea(int id);
        Task<IEnumerable<Area>> GetAreaByFarm(int id);
        Task<IEnumerable<AreaModel>> GetAreaByFarmId(int id);
        Task<IEnumerable<AreaModel>> GetAreaWithZoneTypeLiveStock(int farmId);
        Task<IEnumerable<AreaModel>> GetAreaWithZoneTypePlant(int farmId);
        Task<IEnumerable<AreaModel>> ListArea();
        Task<IEnumerable<AreaModel>> ListAreaActive();
        Task UpdateArea(int id, AreaCreateUpdateModel area);
    }
}
