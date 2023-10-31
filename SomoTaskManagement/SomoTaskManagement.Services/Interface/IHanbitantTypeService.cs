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
        Task AddHabitantType(HabitantType habitantType);
        //Task DeleteHabitantType(HabitantType habitantType);
        Task<HabitantType> GetHabitant(int id);
        Task<IEnumerable<HabitantTypeModel>> ListHabitantType();
        Task<IEnumerable<HabitantTypeModel>> ListHabitantTypeActive();
        Task<IEnumerable<HabitantTypeModel>> ListLiveStock();
        Task<IEnumerable<HabitantTypeModel>> ListLiveStockActive();
        Task<IEnumerable<HabitantTypeModel>> ListPlantType();
        Task<IEnumerable<HabitantTypeModel>> ListPlantTypeActive();
        Task UpdateHabitantType(int id, HabitantTypeCUModel habitantType);
        Task UpdateStatus(int id);
    }
}
