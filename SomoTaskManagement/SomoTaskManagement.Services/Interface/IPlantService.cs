using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IPlantService
    {
        Task Add(PlantCreateModel plant);
        Task DeleteHabitant(Plant plant);
        Task<PlantModel> Get(int id);
        Task<IEnumerable<ExternalIdModel>> GetExternalIds(int id);
        Task<IEnumerable<PlantModel>> GetList();
        Task<IEnumerable<PlantModel>> GetListActive();
        Task<IEnumerable<PlantModel>> GetPlantFarm(int farmId);
        Task Update(Plant plant);
        Task UpdateStatus(int id);
    }
}
