using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class FarmService: IFarmService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FarmService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Farm>> ListFarm()
        {
            return await _unitOfWork.RepositoryFarm.GetData(null);
        }

        public async Task<Farm> GetFarmById(int id)
        {
            return await _unitOfWork.RepositoryFarm.GetById(id);
        }

        public async Task CreateFarm(Farm farm)
        {
            farm.Status = 1;
            await _unitOfWork.RepositoryFarm.Add(farm);
            await _unitOfWork.RepositoryFarm.Commit();
        }

        public async Task UpdateFarm(Farm farm)
        {
            _unitOfWork.RepositoryFarm.Update(farm);
            await _unitOfWork.RepositoryFarm.Commit();
        }

        public async Task DeleteFarm(Farm farm)
        {
            _unitOfWork.RepositoryFarm.Delete(f=>f.Id ==farm.Id);
            await _unitOfWork.RepositoryFarm.Commit();
        }

    }
}
