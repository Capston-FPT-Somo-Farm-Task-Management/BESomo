﻿using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface ILiveStockService
    {
        Task Add(LiveStock liveStock);
        Task DeleteHabitant(LiveStock liveStock);
        Task<LiveStockModel> Get(int id);
        Task<IEnumerable<ExternalIdModel>> GetExternalIds(int id);
        Task<IEnumerable<LiveStockModel>> GetList();
        Task Update(LiveStock liveStock);
        Task UpdateStatus(int id);
    }
}
