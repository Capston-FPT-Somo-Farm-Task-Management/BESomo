﻿using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class MaterialService: IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaterialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<Material>> ListMaterial()
        {
            return _unitOfWork.RepositoryMaterial.GetData(null);
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
        public async Task UpdateMaterial(Material material)
        {
            _unitOfWork.RepositoryMaterial.Update(material);
            await _unitOfWork.RepositoryMaterial.Commit();
        }
        public async Task DeleteMaterial(Material material)
        {
            _unitOfWork.RepositoryMaterial.Delete(a => a.Id == material.Id);
            await _unitOfWork.RepositoryMaterial.Commit();
        }
    }
}