using SomoTaskManagement.Data;
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
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AreaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<Area>> ListArea()
        {
            return _unitOfWork.RepositoryArea.GetData(null);
        }

        public Task<Area> GetAreaByFarmId(int id)
        {
            return _unitOfWork.RepositoryArea.GetSingleByCondition(a => a.FarmId == id);
        }

        public Task<Area> GetArea(int id)
        {
            return _unitOfWork.RepositoryArea.GetById(id);
        }

        public async Task<IEnumerable<Area>> GetAreaByFarm(int id)
        {
            var area = await _unitOfWork.RepositoryArea
                .GetData(expression: f => f.FarmId == id, includes: null);

            return area;
        }

        public async Task AddArea(Area area)
        {
            area.Status = 1;
            await _unitOfWork.RepositoryArea.Add(area);
            await _unitOfWork.RepositoryArea.Commit();
        }
        public async Task UpdateArea(Area area)
        {
            _unitOfWork.RepositoryArea.Update(area);
            await _unitOfWork.RepositoryArea.Commit();
        }
        public async Task DeleteArea(Area area)
        {
            _unitOfWork.RepositoryArea.Delete(a => a.Id == area.Id);
            await _unitOfWork.RepositoryArea.Commit();
        }
    }
}
