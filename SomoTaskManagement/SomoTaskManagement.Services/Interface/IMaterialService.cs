using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IMaterialService
    {
        Task AddMaterial(MaterialCreateUpdateModel material);
        Task DeleteByStatus(int id);
        Task DeleteMaterial(Material material);
        Task<Material> GetMaterial(int id);
        Task<IEnumerable<MaterialModel>> ListMaterial();
        Task<IEnumerable<MaterialModel>> ListMaterialActive(int farmid);
        Task<IEnumerable<MaterialModel>> ListMaterialByFarm(int farmid);
        Task UpdateMaterial(int id, MaterialCreateUpdateModel material);
    }
}
