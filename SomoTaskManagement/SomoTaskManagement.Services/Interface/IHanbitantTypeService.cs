using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HabitantType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IHanbitantTypeService
    {
        Task AddHabitantType(HabitantTypeCUModel habitantType);
        //Task DeleteHabitantType(HabitantType habitantType);
        Task<HabitantType> GetHabitant(int id);
        Task<IEnumerable<HabitantTypeModel>> ListHabitantType();
        Task<IEnumerable<HabitantTypeModel>> ListHabitantTypeActive(int farmId);
        Task<IEnumerable<HabitantTypeModel>> ListLiveStock(int farmId);
        Task<IEnumerable<HabitantTypeModel>> ListLiveStockActive(int farmId);
        Task<IEnumerable<HabitantTypeModel>> ListPlantType(int farmId);
        Task<IEnumerable<HabitantTypeModel>> ListPlantTypeActive(int farmId);
        Task UpdateHabitantType(int id, HabitantTypeCUModel habitantType);
        Task UpdateStatus(int id);
    }
}
