using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IFieldService
    {
        Task AddField(Field field);
        Task DeleteField(Field field);
        Task<AreaZoneModel> GetAreaZoneByField(int id);
        Task<IEnumerable<FieldModel>> GetByZone(int id);
        Task<FieldModel> GetZoneField(int id);
        Task<IEnumerable<FieldModel>> ListField();
        Task UpdateField(Field field);
    }
}
