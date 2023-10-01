using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IOtherService
    {
        Task AddOther(Other other);
        Task DeleteOther(Other other);
        Task<Other> GetOther(int id);
        Task<IEnumerable<Other>> ListOther();
        Task UpdateOther(Other other);
    }
}
