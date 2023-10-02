using AutoMapper;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AreaService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AreaModel>> ListArea()
        {
            var includes = new Expression<Func<Area, object>>[]
               {
                    t => t.Farm,
               };
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: null, includes: includes);
            areas = areas.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areas);
        }
        public async Task<IEnumerable<AreaModel>> ListAreaActive()
        {
            var includes = new Expression<Func<Area, object>>[]
               {
                    t => t.Farm,
               };
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a=>a.Status == 1, includes: includes);
            areas = areas.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areas);
        }

        public async Task<IEnumerable<AreaModel>> GetAreaByFarmId(int id)
        {
            var includes = new Expression<Func<Area, object>>[]
              {
                    t => t.Farm,
              };
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id && a.Status == 1, includes: includes);
            areas = areas.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areas);
        }

        public async Task<IEnumerable<AreaModel>> GetAllAreaByFarmId(int id)
        {
            var includes = new Expression<Func<Area, object>>[]
              {
                    t => t.Farm,
              };
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id, includes: includes);
            areas = areas.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areas);
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

        public async Task DeleteByStatus(int id)
        {
            var area = await _unitOfWork.RepositoryArea.GetById(id);
            if (area == null)
            {
                throw new Exception("Area not found");
            }
            area.Status = area.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryLiveStock.Commit();
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
