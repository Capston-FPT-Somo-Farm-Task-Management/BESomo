using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IFarmService
    {
        Task CreateFarm(Farm farm);
        Task DeleteFarm(Farm farm);
        Task<Farm> GetFarmById(int id);
        Task<IEnumerable<Farm>> ListFarm();
        Task UpdateFarm(Farm farm);
    }
}
