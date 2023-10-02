using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model;
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

        public FieldService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
            
        public async Task<IEnumerable<FieldModel>> ListFieldActive()
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
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f => f.Status == 1, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<FieldModel> GetZoneField(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetById(id);
            var zone = await _unitOfWork.RepositoryZone.GetById(field.ZoneId);

            var status = (HabitantTypeStatus)field.Status;
            var statusString = GetHabitantDescription(status);
            var fieldModel = new FieldModel
            {
                Id = id,
                Name = field.Name,
                Status = statusString,
                ZoneName = zone != null ? zone.Name : null,
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
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.ZoneId == id && f.Status == 1 || f.Status == 0);
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

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields); ;
        }

        public async Task<IEnumerable<FieldModel>> GetAllByZone(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.ZoneId == id);
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

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields); ;
        }

        public async Task<IEnumerable<FieldModel>> GetPlantFieldByFarm(int id)
        {
            var areas = await _unitOfWork.RepositoryArea.GetData(expression: a => a.FarmId == id);
            var areaIds = areas.Select(a => a.Id).ToList();
            var zones = await _unitOfWork.RepositoryZone
               .GetData(expression: z => areaIds.Contains(z.AreaId) , includes: null);
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
            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields); ;
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
            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields); ;
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
                throw new Exception("Livestock not found");
            }
            field.Status = 3;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }

        public async Task AddField(Field field)
        {
            await _unitOfWork.RepositoryField.Add(field);
            await _unitOfWork.RepositoryField.Commit();
        }
        public async Task UpdateField(Field field)
        {
            var fieldUpdate = await _unitOfWork.RepositoryField.GetSingleByCondition(f => f.Id == field.Id);

            fieldUpdate.Status = field.Status;
            fieldUpdate.Name = field.Name;
            fieldUpdate.Area = field.Area;
            fieldUpdate.ZoneId = field.ZoneId;

            await _unitOfWork.RepositoryField.Commit();
        }
        public async Task DeleteField(Field field)
        {
            _unitOfWork.RepositoryField.Delete(a => a.Id == field.Id);
            await _unitOfWork.RepositoryField.Commit();
        }

    }
}
