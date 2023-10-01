using SomoTaskManagement.Domain.Entities;
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
        Task<Area> GetArea(int id);
        Task<IEnumerable<Area>> GetAreaByFarm(int id);
        Task<IEnumerable<Area>> ListArea();
        Task UpdateArea(Area area);
    }
}
