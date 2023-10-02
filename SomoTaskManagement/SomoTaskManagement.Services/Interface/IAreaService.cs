using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IAreaService
    {
        Task AddArea(Area area);
        Task DeleteArea(Area area);
        Task DeleteByStatus(int id);
        Task<IEnumerable<AreaModel>> GetAllAreaByFarmId(int id);
        Task<Area> GetArea(int id);
        Task<IEnumerable<Area>> GetAreaByFarm(int id);
        Task<IEnumerable<AreaModel>> GetAreaByFarmId(int id);
        Task<IEnumerable<AreaModel>> ListArea();
        Task<IEnumerable<AreaModel>> ListAreaActive();
        Task UpdateArea(Area area);
    }
}
