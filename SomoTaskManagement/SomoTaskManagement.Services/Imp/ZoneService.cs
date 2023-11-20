using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Zone;
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
        private readonly SomoTaskManagemnetContext _db;

        public ZoneService(IUnitOfWork unitOfWork, IMapper mapper, SomoTaskManagemnetContext db)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _db = db;
        }
        public async Task<IEnumerable<ZoneModel>> ListZone()
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };
            var zones = await _unitOfWork.RepositoryZone.GetData(expression: null, includes: includes);
            zones = zones.OrderBy(x => x.Name).ToList();
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones); 
        }
        public async Task<IEnumerable<ZoneModel>> ListActiveZone()
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };
            var zones = await _unitOfWork.RepositoryZone.GetData(expression: z => z.Status == 1, includes: includes);
            zones = zones.OrderBy(x => x.Name).ToList();

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }

        public async Task<ZoneModel> GetZone(int id)
        {
            var  zone = await _unitOfWork.RepositoryZone.GetById(id);
            if(zone == null)
            {
                throw new Exception("Zone not found");
            }
            var zoneType = await _unitOfWork.RepositoryZoneType.GetById(zone.ZoneTypeId);
            var area = await _unitOfWork.RepositoryArea.GetById(zone.AreaId);
            string name = $"{zone.Code} -" + $" {zone.Name}";
            var status = (EnumStatus)area.Status;
            var statusString = status == EnumStatus.Active ? "Active" : "Inactive";
            var zoneModel = new ZoneModel
            {
                Id = zone.Id,
                Name = name,
                ZoneTypeName = zoneType.Name,
                Status = statusString,
                AreaName = area.Name
            };
            return zoneModel;
        }
        public async Task<string> GetZoneNameById(int id)
        {
            var zone = await _unitOfWork.RepositoryZone.GetById(id);

            return zone.Name;
        }
        public async Task AddZone(ZoneCreateUpdateModel zone)
        {
            var areaNew = new Zone
            {
                Name = zone.Name,
                Code = zone.Code,
                FarmArea = zone.FarmArea,
                AreaId = zone.AreaId,
                ZoneTypeId = zone.ZoneTypeId,
                Status = 1,
            };
            var existCode = await _unitOfWork.RepositoryZone.GetSingleByCondition(a => a.Code == zone.Code);
            if (existCode != null)
            {
                throw new Exception("Mã khu vực không thể trùng");

            }
            await _unitOfWork.RepositoryZone.Add(areaNew);
            await _unitOfWork.RepositoryZone.Commit();
        }
        public async Task UpdateZone(int id ,ZoneCreateUpdateModel zone)
        {
            var zoneUpdate = await _unitOfWork.RepositoryZone.GetById(id);
            if (zoneUpdate != null)
            {
                var initialCode = zoneUpdate.Code;

                zoneUpdate.Id = id;
                zoneUpdate.FarmArea = zone.FarmArea;
                zoneUpdate.ZoneTypeId = zone.ZoneTypeId;
                zoneUpdate.Code = zone.Code;
                zoneUpdate.FarmArea = zone.FarmArea;
                zoneUpdate.AreaId = zone.AreaId;
                zoneUpdate.Name = zone.Name;

                if (zoneUpdate.Code != initialCode)
                {
                    var existCode = await _unitOfWork.RepositoryZone.GetSingleByCondition(a => a.Code == zone.Code);
                    if (existCode != null)
                    {
                        throw new Exception("Mã không thể trùng");
                    }
                }
                await _unitOfWork.RepositoryZone.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy vùng");
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
            var area = await _unitOfWork.RepositoryArea.GetById(id) ?? throw new Exception("Không tìm thấy khu vực");

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.AreaId == id && z.ZoneTypeId == 1 && z.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }

        public async Task<IEnumerable<ZoneModel>> GetByAreaAndLivestock(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };
            var area = await _unitOfWork.RepositoryArea.GetById(id) ?? throw new Exception("Không tìm thấy khu vực");

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.AreaId == id && z.ZoneTypeId == 2 && z.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }


        public async Task<IEnumerable<ZoneModel>> GetByZoneTypeId(int id)
        {
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };
            var zoneType = await _unitOfWork.RepositoryZoneType.GetById(id) ?? throw new Exception("Không tìm thấy loại công việc");
            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => z.ZoneTypeId == id, includes: includes);

            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }
        public async Task<IEnumerable<ZoneModel>> GetByFarmId(int id)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(id) ?? throw new Exception("Không tìm thấy trang trại");
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id);
            var areaIds = areas.Select(a => a.Id).ToList();
            var includes = new Expression<Func<Zone, object>>[]
            {
                t => t.Area,
                t =>t.ZoneType
            };

            var zones = await _unitOfWork.RepositoryZone
                .GetData(expression: z => areaIds.Contains(z.AreaId), includes: includes);
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }

        public async Task<IEnumerable<ZoneModel>> GetActiveByFarmId(int id)
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
            if (zones == null)
            {
                throw new Exception("Không tìm thấy");
            }
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ZoneModel>>(zones);
        }

        public async Task UpdateStatus(int id)
        {
            var zone = await _unitOfWork.RepositoryZone.GetById(id) ?? throw new Exception("Không tìm thấy vùng");

            if (zone.Status == 1)
            {
                var fields = await _unitOfWork.RepositoryField.GetData(f => f.ZoneId == id && f.Status == 1);

                if (fields.Any())
                {
                    throw new Exception("Không thể xóa vùng này khi còn thực thể bên trong");
                }
            }

            zone.Status = zone.Status == 1 ? 0 : 1;

            await _unitOfWork.RepositoryZone.Commit();
        }



        public async Task Delete(int zoneId)
        {
            var fields = await _unitOfWork.RepositoryField.GetData(z => z.ZoneId == zoneId && z.Status == 1);
            if (fields.Count() > 0)
            {
                throw new Exception("Không thể xóa vùng này  khi còn thực thể bên trong");
            }
            _unitOfWork.RepositoryZone.Delete(a => a.Id == zoneId);
            await _unitOfWork.RepositoryZone.Commit();
        }
    }
}
