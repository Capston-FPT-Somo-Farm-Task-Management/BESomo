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
    public class ZoneTypeService: IZoneTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ZoneTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        public Task<IEnumerable<ZoneType>> ListZone()
        {
            return _unitOfWork.RepositoryZoneType.GetData(null);
        }

        public async Task<ZoneType> GetZoneType(int id)
        {
            return await _unitOfWork.RepositoryZoneType.GetById(id) ?? throw new Exception("Không tìm thấy loại khu vùng");
        }

        public async Task AddZoneType(ZoneType zoneType)
        {
            zoneType.Status = 1;
            await _unitOfWork.RepositoryZoneType.Add(zoneType);
            await _unitOfWork.RepositoryZoneType.Commit();
        }
        public async Task UpdateZoneType(ZoneType zoneType)
        {
            var zoneTypeUpdate = await _unitOfWork.RepositoryZoneType.GetSingleByCondition(z => z.Id == zoneType.Id);
            if(zoneTypeUpdate != null)
            {
                zoneTypeUpdate.Name = zoneType.Name;
                zoneTypeUpdate.Status = zoneType.Status;

                await _unitOfWork.RepositoryZoneType.Commit();
            }
        }
        public async Task DeleteZoneType(ZoneType zoneType)
        {
            _unitOfWork.RepositoryZoneType.Delete(a => a.Id == zoneType.Id);
            await _unitOfWork.RepositoryZoneType.Commit();
        }
    }
}
