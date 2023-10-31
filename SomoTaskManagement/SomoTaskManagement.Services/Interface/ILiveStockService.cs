using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Domain.Model.Livestock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface ILiveStockService
    {
        Task Add(LivestockCreateModel liveStock);
        Task DeleteHabitant(LiveStock liveStock);
        Task<LiveStockModel> Get(int id);
        Task<IEnumerable<ExternalIdModel>> GetExternalIds(int id);
        Task<IEnumerable<LiveStockModel>> GetList();
        Task<IEnumerable<LiveStockModel>> GetListActive();
        Task<IEnumerable<LiveStockModel>> GetLiveStockActiveFarm(int farmId);
        Task<IEnumerable<LiveStockModel>> GetLiveStockFarm(int farmId);
        Task Update(int id, LivestockCreateModel liveStock);
        Task UpdateStatus(int id);
    }
}
