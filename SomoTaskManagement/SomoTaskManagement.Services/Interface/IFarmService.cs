using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Farm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IFarmService
    {
        Task CreateFarm(FarmCreateUpdateModel farm);
        Task DeleteFarm(Farm farm);
        Task<Farm> GetFarmById(int id);
        Task<IEnumerable<Farm>> ListFarm();
        Task UpdateFarm(int id, FarmCreateUpdateModel farm);
    }
}
