﻿using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IFieldService
    {
        Task AddField(FieldCreateUpdateModel field);
        Task DeleteField(Field field);
        Task DeleteFieldByStatus(int id);
        Task<IEnumerable<FieldModel>> GetAllByZone(int id);
        Task<AreaZoneModel> GetAreaZoneByField(int id);
        Task<Field> GetByCode(string code);
        Task<IEnumerable<FieldModel>> GetByZone(int id);
        Task<IEnumerable<FieldModel>> GetLivestockFieldByFarm(int id);
        Task<IEnumerable<FieldModel>> GetPlantFieldByFarm(int id);
        Task<FieldModel> GetZoneField(int id);
        Task<IEnumerable<FieldModel>> ListField();
        Task<IEnumerable<FieldModel>> ListFieldActive();
        Task<IEnumerable<FieldModel>> ListFieldLivestock();
        Task<IEnumerable<FieldModel>> ListFieldLivestockActive();
        Task<IEnumerable<FieldModel>> ListFieldPlant();
        Task<IEnumerable<FieldModel>> ListFieldPlantActive();
        Task UpdateField(int id, FieldCreateUpdateModel field);
    }
}
