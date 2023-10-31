using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Area;
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
        private readonly SomoTaskManagemnetContext _db;

        public AreaService(IUnitOfWork unitOfWork, IMapper mapper, SomoTaskManagemnetContext db)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _db = db;
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
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.Status == 1, includes: includes);
            areas = areas.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areas);
        }

        public async Task<IEnumerable<AreaModel>> GetAreaByFarmId(int id)
        {
            var includes = new Expression<Func<Area, object>>[]
            {
                    t => t.Farm,
            };
            var farm = await _unitOfWork.RepositoryFarm.GetById(id);
            if (farm == null)
            {
                throw new Exception("Không tìm thấy nông trại");
            }
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
            var farm = await _unitOfWork.RepositoryFarm.GetById(id);
            if (farm == null)
            {
                throw new Exception("Không tìm thấy nông trại");
            }
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id, includes: includes);
            areas = areas.OrderBy(x => x.Name).ToList();

            return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areas);
        }

        public async Task<AreaModel> GetArea(int id)
        {
            var area = await _unitOfWork.RepositoryArea.GetById(id);
            if (area == null)
            {
                throw new Exception("Vùng không tìm thấy");
            }
            string name = $"{area.Code} -" + $" {area.Name}";
            var farm = await _unitOfWork.RepositoryFarm.GetById(area.FarmId);
            var status = (EnumStatus)area.Status;
            var statusString = status == EnumStatus.Active ? "Active" : "Inactive";

            var areaModel = new AreaModel
            {
                Id = id,
                Name = name,
                Status = statusString,
                FArea = area.FArea,
                FarmName = farm.Name,
                Code = area.Code,
            };
            return areaModel;
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
                throw new Exception("Vùng không tìm thấy");
            }
            area.Status = area.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }

        public async Task AddArea(AreaCreateUpdateModel area)
        {
            var areaNew = new Area
            {
                Name = area.Name,
                Code = area.Code,
                FArea = area.FArea,
                FarmId = area.FarmId,
                Status = 1,
            };

            var existCode = await _unitOfWork.RepositoryArea.GetSingleByCondition(a=>a.Code == area.Code);
            if (existCode != null)
            {
                throw new Exception("Mã khu vực không thể trùng");
            }
            await _unitOfWork.RepositoryArea.Add(areaNew);
            await _unitOfWork.RepositoryArea.Commit();

        }
        public async Task UpdateArea(int id,AreaCreateUpdateModel area)
        {
            var areaUpdate = await _unitOfWork.RepositoryArea.GetById(id);
            if (areaUpdate == null)
            {
                throw new Exception("Không tìm thấy khu vực này ");
            }
            var initialCode = areaUpdate.Code;

            areaUpdate.Id = id;
            areaUpdate.Name = area.Name;
            areaUpdate.Status = 1;
            areaUpdate.Code = area.Code;
            areaUpdate.FarmId = area.FarmId;
            areaUpdate.FArea = area.FArea;

            if (areaUpdate.Code != initialCode)
            {
                var existCode = await _unitOfWork.RepositoryArea.GetSingleByCondition(a => a.Code == area.Code);
                if (existCode != null)
                {
                    throw new Exception("Mã không thể trùng");
                }
            }
            await _unitOfWork.RepositoryArea.Commit();
        }
        public async Task DeleteArea(Area area)
        {
            _unitOfWork.RepositoryArea.Delete(a => a.Id == area.Id);
            await _unitOfWork.RepositoryArea.Commit();
        }
    }
}
