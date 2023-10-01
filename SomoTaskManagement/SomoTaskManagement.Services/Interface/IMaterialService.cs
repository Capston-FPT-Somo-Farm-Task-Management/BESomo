using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IMaterialService
    {
        Task AddMaterial(Material material);
        Task DeleteMaterial(Material material);
        Task<Material> GetMaterial(int id);
        Task<IEnumerable<Material>> ListMaterial();
        Task UpdateMaterial(Material material);
    }
}
