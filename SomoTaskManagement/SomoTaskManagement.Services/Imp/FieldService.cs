using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Field;
using SomoTaskManagement.Domain.Model.Livestock;
using SomoTaskManagement.Domain.Model.Plant;
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
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly SomoTaskManagemnetContext _db;

        public FieldService(IUnitOfWork unitOfWork, IMapper mapper, SomoTaskManagemnetContext db)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _db = db;
        }

        public async Task<IEnumerable<FieldModel>> ListFieldActive()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                 t => t.Zone,
                 t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.IsDelete == false, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }
        public async Task<IEnumerable<FieldModel>> ListField()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                 t => t.Zone,
                 t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.Status == 1 || f.Status == 0, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }
        public async Task<IEnumerable<FieldModel>> ListFieldPlant()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
                t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.Status == 0, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<IEnumerable<FieldModel>> ListFieldLivestock()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
                t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<IEnumerable<FieldModel>> ListFieldPlantActive()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
                t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.Status == 0 && f.IsDelete == false, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<IEnumerable<FieldModel>> ListFieldLivestockActive()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
                t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.Status == 1 && f.IsDelete == false, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<FieldModel> GetZoneField(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetById(id);
            var zone = await _unitOfWork.RepositoryZone.GetById(field.ZoneId);
            var area = await _unitOfWork.RepositoryArea.GetById(zone.AreaId);
            var status = (HabitantTypeStatus)field.Status;
            var statusString = GetHabitantDescription(status);

            var fieldModel = new FieldModel
            {
                Id = id,
                Name = field.Name,
                Status = statusString,
                ZoneName = zone != null ? zone.Name : null,
                ZoneId = zone.Id,
                Code = field.Code,
                AreaId = area.Id,
                AreaName = area.Name,
                Area = field.Area,
            };
            return fieldModel;
        }

        public static string GetHabitantDescription(HabitantTypeStatus status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }
        public async Task<IEnumerable<FieldModel>> GetByZone(int id)
        {
            //var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.ZoneId == id && f.Status == 1 || f.Status == 0);
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
            };

            var fields = await _unitOfWork.RepositoryField
                   .GetData(
                     expression: f => f.ZoneId == id && f.IsDelete == false,
                     includes: new Expression<Func<Field, object>>[]
                     {
                         t => t.Zone,
                         t => t.Zone.Area,
                     });

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<IEnumerable<FieldModel>> GetAllByZone(int id)
        {
            //var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.ZoneId == id);
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
            };

            var fields = await _unitOfWork.RepositoryField
                   .GetData(
                     expression: f => f.ZoneId == id && f.Status != 3,
                     includes: new Expression<Func<Field, object>>[]
                     {
                         t => t.Zone,
                         t => t.Zone.Area,
                     });

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<IEnumerable<FieldModel>> GetPlantFieldByFarm(int id)
        {
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id);
            var areaIds = areas.Select(a => a.Id).ToList();
            var zones = await _unitOfWork.RepositoryZone
               .GetData(expression: z => areaIds.Contains(z.AreaId), includes: null);
            var zoneIds = zones.Select(z => z.Id).ToList();
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
                t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                   .GetData(
                     expression: f => zoneIds.Contains(f.ZoneId) && f.Status == 0,
                     includes: includes);
            fields = fields.OrderByDescending(e => e.Id).ToList();
            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<IEnumerable<FieldModel>> GetLivestockFieldByFarm(int id)
        {
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id);
            var areaIds = areas.Select(a => a.Id).ToList();
            var zones = await _unitOfWork.RepositoryZone
               .GetData(expression: z => areaIds.Contains(z.AreaId), includes: null);
            var zoneIds = zones.Select(z => z.Id).ToList();
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
                t => t.Zone.Area,
            };

            var fields = await _unitOfWork.RepositoryField
                   .GetData(
                     expression: f => zoneIds.Contains(f.ZoneId) && f.Status == 1,
                     includes: includes);
            fields = fields.OrderByDescending(e => e.Id).ToList();
            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<AreaZoneModel> GetAreaZoneByField(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetById(id);

            var zone = await _unitOfWork.RepositoryZone.GetById(field.ZoneId);

            var area = await _unitOfWork.RepositoryArea.GetById(zone.AreaId);

            var areaZoneModel = new AreaZoneModel
            {
                AreaId = area.Id,
                AreaName = area.Name,
                ZoneId = zone.Id,
                ZoneName = zone.Name,
            };
            return areaZoneModel;
        }

        public async Task DeleteFieldByStatus(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetById(id);
            if (field == null)
            {
                throw new Exception("Không tìm thấy field");
            }

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.FieldId == id);

            if (tasks.Any())
            {
                throw new Exception("Có các nhiệm vụ chứa thực thể này không thể xóa");
            }

            var livestock = await _unitOfWork.RepositoryLiveStock.GetData(expression: l => l.FieldId == field.Id);
            var plant = await _unitOfWork.RepositoryPlant.GetData(expression: l => l.FieldId == field.Id);
            var numberLivestock = livestock.Count();
            var numberPlant = plant.Count();
            if ((numberLivestock != 0 || numberPlant != 0) && field.IsDelete == false)
            {
                throw new Exception("Có một hoặc nhiều thực thể ở bên trong nên không thể xóa");
            }
            field.IsDelete = field.IsDelete == true ? false : true;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }

        public async Task AddField(FieldCreateUpdateModel field)
        {
            var fieldNew = new Field
            {
                Name = field.Name,
                Code = field.Code,
                Area = field.Area,
                ZoneId = field.ZoneId,
                IsDelete = false,
            };
            var existCode = await _unitOfWork.RepositoryField.GetSingleByCondition(a => a.Code == field.Code);
            if (existCode != null)
            {
                throw new Exception("Mã không thể trùng");
            }

            var zone = await _unitOfWork.RepositoryZone.GetById(field.ZoneId);
            if (zone.ZoneTypeId == 1)
            {
                fieldNew.Status = 0;
            }
            else if (zone.ZoneTypeId == 2)
            {
                fieldNew.Status = 1;
            }

            await _unitOfWork.RepositoryField.Add(fieldNew);
            await _unitOfWork.RepositoryField.Commit();
        }

        public async Task UpdateField(int id, FieldCreateUpdateModel field)
        {
            var fieldUpdate = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.Id == id);
            if (fieldUpdate == null)
            {
                throw new Exception("Không tìm thấy");
            }

            var initialCode = fieldUpdate.Code;

            fieldUpdate.Code = field.Code;
            fieldUpdate.Name = field.Name;
            fieldUpdate.Area = field.Area;
            fieldUpdate.ZoneId = field.ZoneId;

            if (fieldUpdate.Code != initialCode)
            {
                var existCode = await _unitOfWork.RepositoryField.GetSingleByCondition(a => a.Code == field.Code);
                if (existCode != null)
                {
                    throw new Exception("Mã không thể trùng");
                }
            }

            //var zone = await _unitOfWork.RepositoryZone.GetById(field.ZoneId);
            //if (zone.ZoneTypeId == 1)
            //{
            //    fieldUpdate.Status = 1;
            //}
            //else if (zone.ZoneTypeId == 2)
            //{
            //    fieldUpdate.Status = 0;
            //}

            await _unitOfWork.RepositoryField.Commit();
        }
        public async Task DeleteField(int fieldId)
        {
            var field = await _unitOfWork.RepositoryField.GetById(fieldId);
            if (field == null)
            {
                throw new Exception("Không tìm thấy field");
            }

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.FieldId == fieldId);

            if (tasks.Any())
            {
                throw new Exception("Có các nhiệm vụ chứa thực thể này không thể xóa");
            }

            var livestock = await _unitOfWork.RepositoryLiveStock.GetData(expression: l => l.FieldId == fieldId);
            var plant = await _unitOfWork.RepositoryPlant.GetData(expression: l => l.FieldId == fieldId);
            var numberLivestock = livestock.Count();
            var numberPlant = plant.Count();
            if (numberLivestock != 0 || numberPlant != 0)
            {
                throw new Exception("Có một hoặc nhiều thực thể ở bên trong nên không thể xóa");
            }

            _unitOfWork.RepositoryField.Delete(a => a.Id == fieldId);
            await _unitOfWork.RepositoryField.Commit();
        }


        public async Task<Field> GetByCode(string code)
        {
            return await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.Code == code);
        }

        public async Task<IEnumerable<LiveStockModel>> GetLivestockByField(int fieldId)
        {
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f=>f.Id ==fieldId && f.Status == 1) ?? throw new Exception("Không tìm thấy chuồng");

            var includes = new Expression<Func<LiveStock, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };
            var liveStocks = await _unitOfWork.RepositoryLiveStock
                .GetData(expression: l => l.FieldId == fieldId , includes: includes);

            liveStocks = liveStocks.OrderBy(ls => ls.CreateDate).ToList();
            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<LiveStockModel>>(liveStocks);

        }

        public async Task<IEnumerable<PlantModel>> GetPlantByField(int fieldId)
        {
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.Id == fieldId && f.Status == 0) ?? throw new Exception("Không tìm thấy vườn");

            var includes = new Expression<Func<Plant, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };
            var plants = await _unitOfWork.RepositoryPlant
                .GetData(expression: l => l.FieldId == fieldId, includes: includes);

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<PlantModel>>(plants);
        }
    }
}
