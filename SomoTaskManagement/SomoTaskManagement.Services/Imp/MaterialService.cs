using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Material;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MaterialService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialModel>> ListMaterial()
        {
            var material = await _unitOfWork.RepositoryMaterial.GetData(null);
            material = material.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Material>, IEnumerable<MaterialModel>>(material); ;
        }

        public async Task<IEnumerable<MaterialModel>> ListMaterialActive()
        {
            var material = await _unitOfWork.RepositoryMaterial.GetData(expression: m => m.Status == 1);
            material = material.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Material>, IEnumerable<MaterialModel>>(material); ;
        }

        public Task<Material> GetMaterial(int id)
        {
            return _unitOfWork.RepositoryMaterial.GetById(id);
        }
        public async Task AddMaterial(Material material)
        {
            material.Status = 1;
            await _unitOfWork.RepositoryMaterial.Add(material);
            await _unitOfWork.RepositoryMaterial.Commit();
        }
        public async Task UpdateMaterial(int id,Material material)
        {
            var materialUpdate = await _unitOfWork.RepositoryMaterial.GetById(id);
            if(materialUpdate != null)
            {
                materialUpdate.Status = 1;
                materialUpdate.Id = id;
                materialUpdate.Name = material.Name;
                await _unitOfWork.RepositoryMaterial.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy vật dụng");
            }
        }
        public async Task DeleteMaterial(Material material)
        {
            _unitOfWork.RepositoryMaterial.Delete(a => a.Id == material.Id);
            await _unitOfWork.RepositoryMaterial.Commit();
        }
        public async Task DeleteByStatus(int id)
        {
            var material = await _unitOfWork.RepositoryMaterial.GetById(id);
            if (material == null)
            {
                throw new Exception("Livestock not found");
            }
            material.Status = material.Status == 1 ? 0 : 1; ;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
    }
}
