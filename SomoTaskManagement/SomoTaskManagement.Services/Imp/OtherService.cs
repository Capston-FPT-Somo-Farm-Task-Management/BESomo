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
    public class OtherService: IOtherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OtherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<IEnumerable<Other>> ListOther()
        {
            return _unitOfWork.RepositoryOther.GetData(null);
        }
        public Task<Other> GetOther(int id)
        {
            return _unitOfWork.RepositoryOther.GetById(id);
        }
        public async Task AddOther(Other other)
        {
            other.Status = 1;
            await _unitOfWork.RepositoryOther.Add(other);
            await _unitOfWork.RepositoryOther.Commit();
        }
        public async Task UpdateOther(Other other)
        {
            _unitOfWork.RepositoryOther.Update(other);
            await _unitOfWork.RepositoryOther.Commit();
        }
        public async Task DeleteOther(Other other)
        {
            _unitOfWork.RepositoryOther.Delete(a => a.Id == other.Id);
            await _unitOfWork.RepositoryOther.Commit();
        }
    }
}
