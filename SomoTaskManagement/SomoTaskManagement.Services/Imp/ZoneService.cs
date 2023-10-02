using AutoMapper;
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
    public class ZoneService : IZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ZoneService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ZoneModel>> ListZone()
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };
            var zone = await _unitOfWork.RepositoryZone.GetData(expression: null, includes: includes);
            zone = zone.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zone);
        }
        public async Task<IEnumerable<ZoneModel>> ListZoneActive()
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };
            var zone = await _unitOfWork.RepositoryZone.GetData(expression: z => z.Status == 1, includes: includes);
            zone = zone.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zone);
        }

        public Task<Zone> GetZone(int id)
        {
            return _unitOfWork.RepositoryZone.GetById(id);
        }
        public async Task<string> GetZoneNameById(int id)
        {
            var zone = await _unitOfWork.RepositoryZone.GetById(id);

            return zone.Name;
        }
        public async Task AddZone(Zone zone)
        {
            zone.Status = 1;
            await _unitOfWork.RepositoryZone.Add(zone);
            await _unitOfWork.RepositoryZone.Commit();
        }
        public async Task UpdateZone(Zone zone)
        {
            var zoneUpdate = await _unitOfWork.RepositoryZone.GetSingleByCondition(z => z.Id == zone.Id);
            if (zoneUpdate != null)
            {
                zoneUpdate.FarmArea = zone.FarmArea;
                zoneUpdate.ZoneTypeId = zone.ZoneTypeId;
                zoneUpdate.AreaId = zone.AreaId;
                zoneUpdate.Name = zone.Name;
                zoneUpdate.Status = zone.Status;

                await _unitOfWork.RepositoryZone.Commit();
            }
        }

        public async Task DeleteZone(Zone zone)
        {
            _unitOfWork.RepositoryZone.Delete(a => a.Id == zone.Id);
            await _unitOfWork.RepositoryZone.Commit();
        }

        public async Task<IEnumerable<ZoneModel>> GetByArea(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.AreaId == id && z.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);

        }
        public async Task<IEnumerable<ZoneModel>> GetAllByArea(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.AreaId == id, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);

        }
        public async Task<IEnumerable<ZoneModel>> GetByAreaAndPlant(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.AreaId == id && z.ZoneTypeId == 2 && z.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }

        public async Task<IEnumerable<ZoneModel>> GetByAreaAndLivestock(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.AreaId == id && z.ZoneTypeId == 1 && z.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }


        public async Task<IEnumerable<ZoneModel>> GetByZoneTypeId(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.ZoneTypeId == id, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }
        public async Task<IEnumerable<ZoneModel>> GetByFarmId(int id)
        {
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id);
            var areaIds = areas.Select(a => a.Id).ToList();
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => areaIds.Contains(z.AreaId) && z.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }

        public async Task UpdateStatus(int id)
        {
            var zone = await _unitOfWork.RepositoryZone.GetById(id);
            if (zone == null)
            {
                throw new Exception("Zone not found");
            }
            zone.Status = zone.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryZone.Commit();
        }

    }
}
