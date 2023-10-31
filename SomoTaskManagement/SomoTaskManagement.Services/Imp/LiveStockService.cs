using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Domain.Model.Livestock;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class LiveStockService : ILiveStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LiveStockService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<LiveStockModel>> GetList()
        {
            var includes = new Expression<Func<LiveStock, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone, 
                t => t.Field.Zone.Area 
            };
            var liveStocks = await _unitOfWork.RepositoryLiveStock
                .GetData(expression: null, includes: includes);

            liveStocks = liveStocks.OrderBy(ls => ls.CreateDate).ToList();
            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<LiveStockModel>>(liveStocks);
        }
        public async Task<IEnumerable<LiveStockModel>> GetListActive()
        {
            var includes = new Expression<Func<LiveStock, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };
            var liveStocks = await _unitOfWork.RepositoryLiveStock
                .GetData(expression: l=>l.Status ==1, includes: includes);

            liveStocks = liveStocks.OrderBy(ls => ls.CreateDate).ToList();
            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<LiveStockModel>>(liveStocks);
        }

        public async Task<IEnumerable<LiveStockModel>> GetLiveStockFarm(int farmId)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(farmId);
            if(farm == null)
            {
                throw new Exception("Không tìm tháy trang trại");
            }
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == farmId);

            var areaId = areas.Select(z => z.Id).ToList();

            var zones = await _unitOfWork.RepositoryZone.GetData(expression: a => areaId.Contains(a.AreaId));

            var zoneId = zones.Select(f=> f.Id).ToList();

            var fields = await _unitOfWork.RepositoryField.GetData(expression: a => zoneId.Contains(a.ZoneId));

            var fieldId = fields.Select(f => f.Id).ToList();

            var includes = new Expression<Func<LiveStock, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };

            var liveStocks = await _unitOfWork.RepositoryLiveStock
                .GetData(expression: l => fieldId.ToList().Contains(l.FieldId), includes: includes);
            if ( liveStocks == null)
            {
                throw new Exception("Không tìm thấy động vật");
            }
            liveStocks = liveStocks.OrderBy(ls => ls.CreateDate).ToList();

            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<LiveStockModel>>(liveStocks);
        }

        public async Task<IEnumerable<LiveStockModel>> GetLiveStockActiveFarm(int farmId)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(farmId);
            if (farm == null)
            {
                throw new Exception("Không tìm thấy trang trại");
            }
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == farmId);

            var areaId = areas.Select(z => z.Id).ToList();

            var zones = await _unitOfWork.RepositoryZone.GetData(expression: a => areaId.Contains(a.AreaId));

            var zoneId = zones.Select(f => f.Id).ToList();

            var fields = await _unitOfWork.RepositoryField.GetData(expression: a => zoneId.Contains(a.ZoneId));

            var fieldId = fields.Select(f => f.Id).ToList();

            var includes = new Expression<Func<LiveStock, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };

            var liveStocks = await _unitOfWork.RepositoryLiveStock
                .GetData(expression: l => fieldId.ToList().Contains(l.FieldId) && l.Status == 1, includes: includes);
            //liveStocks = liveStocks.OrderBy(ls => ls.CreateDate).ToList();
            if (liveStocks == null)
            {
                throw new Exception("Không tìm thấy động vật");
            }
            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<LiveStockModel>>(liveStocks);
        }


        public async Task<LiveStockModel> Get(int id)
        {
            var liveStock = await _unitOfWork.RepositoryLiveStock.GetById(id);

            if (liveStock == null)
            {
                throw new Exception("Không tìm thấy động vật");
            }

            var habitantType = await _unitOfWork.RepositoryHabitantType.GetSingleByCondition(h => h.Id == liveStock.HabitantTypeId);
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(h => h.Id == liveStock.FieldId);
            var zone = await _unitOfWork.RepositoryZone.GetSingleByCondition(h => h.Id == field.ZoneId);
            var area = await _unitOfWork.RepositoryArea.GetById(zone.AreaId);

            var status = (EnumStatus)liveStock.Status;
            var statusString = status == EnumStatus.Active ? "Active" : "Inactive";

            var genderEnum = liveStock.Gender ? GenderEnum.Male : GenderEnum.Female;
            var genderString = GetGenderDescription(genderEnum);

            var liveStockModel = new LiveStockModel
            {
                Id = liveStock.Id,
                Name = liveStock.Name,
                Status = statusString,
                ExternalId = liveStock.ExternalId,
                CreateDate = liveStock.CreateDate,
                Weight = liveStock.Weight,
                Gender = genderString,
                HabitantTypeName = habitantType.Name,
                FieldName = field.Name,
                ZoneName = zone.Name,
                AreaName = area.Name,
                AreaId = area.Id,
                FieldId = field.Id,
                ZoneId = zone.Id,
                HabitantTypeId = habitantType.Id,
            };

            return liveStockModel;
        }
        public static string GetGenderDescription(GenderEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public async Task<IEnumerable<ExternalIdModel>> GetExternalIds(int id)
        {
            var livestock = await _unitOfWork.RepositoryLiveStock.GetData(expression:l=>l.FieldId==id && l.Status == 1,includes:null);

            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<ExternalIdModel>>(livestock);
        }


        public async Task Add(LivestockCreateModel liveStock)
        {
            var livestockNew = new LiveStock
            {
                Name = liveStock.Name,
                Status = 1,
                IsActive = true,
                ExternalId = liveStock.ExternalId,
                CreateDate = DateTime.Now,
                Weight = liveStock.Weight,
                Gender = liveStock.Gender,
                HabitantTypeId = liveStock.HabitantTypeId,
                FieldId = liveStock.FieldId
            };

            var existCode = await _unitOfWork.RepositoryLiveStock.GetSingleByCondition(a => a.ExternalId == liveStock.ExternalId);
            if (existCode != null)
            {
                throw new Exception("Mã động vật không thể trùng");
            }

            await _unitOfWork.RepositoryLiveStock.Add(livestockNew);
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
        public async Task Update(int id, LivestockCreateModel liveStock)
        {
            var liveStockUpdate = await _unitOfWork.RepositoryLiveStock.GetSingleByCondition(p => p.Id == id);
            if (liveStockUpdate != null)
            {
                var initialExternalId = liveStockUpdate.ExternalId;

                liveStockUpdate.Id = id;
                liveStockUpdate.ExternalId = liveStock.ExternalId;
                liveStockUpdate.Weight = liveStock.Weight;
                liveStockUpdate.HabitantTypeId = liveStock.HabitantTypeId;
                liveStockUpdate.FieldId = liveStock.FieldId;
                liveStockUpdate.Name = liveStock.Name;
                liveStockUpdate.Status = 1;
                liveStockUpdate.Gender = liveStock.Gender;
                liveStockUpdate.IsActive = true;

                if (liveStockUpdate.ExternalId != initialExternalId)
                {
                    var existCode = await _unitOfWork.RepositoryLiveStock.GetSingleByCondition(a => a.ExternalId == liveStock.ExternalId);
                    if (existCode != null)
                    {
                        throw new Exception("Mã động vật không thể trùng");
                    }
                }

                await _unitOfWork.RepositoryLiveStock.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy động vật");
            }
        }


        public async Task UpdateStatus(int id)
        {
            var liveStock = await _unitOfWork.RepositoryLiveStock.GetById(id);
            if (liveStock == null)
            {
                throw new Exception("Livestock not found");
            }
            liveStock.Status = liveStock.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }

        public async Task DeleteHabitant(LiveStock liveStock)
        {
            _unitOfWork.RepositoryLiveStock.Delete(a => a.Id == liveStock.Id);
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
    }
}
