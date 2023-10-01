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
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FieldService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FieldModel>> ListField()
        {
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: null, includes: includes);

            return _mapper.Map<IEnumerable<Field>, IEnumerable<FieldModel>>(fields);
        }

        public async Task<FieldModel> GetZoneField(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetById(id);
            var zone = await _unitOfWork.RepositoryZone.GetById(field.ZoneId);

            var fieldModel = new FieldModel
            {
                Id = id,
                Name = field.Name,
                Status = field.Status,
                ZoneName = zone != null ? zone.Name : null,
                Area = field.Area,
            };
            return fieldModel;
        }

        public async Task<IEnumerable<FieldModel>> GetByZone(int id)
        {
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(f=>f.ZoneId == id);
            var includes = new Expression<Func<Field, object>>[]
            {
                t => t.Zone,
            };

            var fields = await _unitOfWork.RepositoryField
                .GetData(expression: f=>f.ZoneId == id, includes: includes);

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
                ZoneName= zone.Name,
            };
            return areaZoneModel;
        }

        public async Task AddField(Field field)
        {
            field.Status = 1;
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
