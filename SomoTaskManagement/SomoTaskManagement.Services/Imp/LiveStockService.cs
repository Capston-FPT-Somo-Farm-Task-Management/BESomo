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
            var liveStock = await _unitOfWork.RepositoryLiveStock
                .GetData(expression: null, includes: includes);

            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<LiveStockModel>>(liveStock);
        }

        public async Task<LiveStockModel> Get(int id)
        {
            var liveStock = await _unitOfWork.RepositoryLiveStock.GetById(id);

            if (liveStock == null)
            {
                throw new Exception("Not found live stock");
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
                DateOfBirth = liveStock.DateOfBirth,
                Gender = genderString,
                HabitantTypeName = habitantType.Name,
                FieldName = field.Name,
                ZoneName = zone.Name,
                AreaName = area.Name,
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
            var livestock = await _unitOfWork.RepositoryLiveStock.GetData(expression:l=>l.FieldId==id,includes:null);

            return _mapper.Map<IEnumerable<LiveStock>, IEnumerable<ExternalIdModel>>(livestock);
        }


        public async Task Add(LiveStock liveStock)
        {
            liveStock.Status = 1;
            await _unitOfWork.RepositoryLiveStock.Add(liveStock);
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
        public async Task Update(LiveStock liveStock)
        {
            var liveStockUpdate = await _unitOfWork.RepositoryLiveStock.GetSingleByCondition(p => p.Id == liveStock.Id);
            if (liveStockUpdate != null)
            {
                liveStockUpdate.ExternalId = liveStock.ExternalId;
                liveStockUpdate.CreateDate = liveStock.CreateDate;
                liveStockUpdate.Weight = liveStock.Weight;
                liveStockUpdate.HabitantTypeId = liveStock.HabitantTypeId;
                liveStockUpdate.FieldId = liveStock.FieldId;
                liveStockUpdate.Name = liveStock.Name;
                liveStockUpdate.Status = liveStock.Status;
                liveStockUpdate.Gender = liveStock.Gender;
                liveStockUpdate.DateOfBirth = liveStock.DateOfBirth;

                await _unitOfWork.RepositoryLiveStock.Commit();
            }

        }

        public async Task UpdateStatus(int id)
        {
            var liveStock = await _unitOfWork.RepositoryLiveStock.GetSingleByCondition(f => f.Id == id);
            if (liveStock != null)
            {
                liveStock.Status = liveStock.Status == 1 ? 0 : 1;
                await _unitOfWork.RepositoryLiveStock.Commit();
            }
        }

        public async Task DeleteHabitant(LiveStock liveStock)
        {
            _unitOfWork.RepositoryLiveStock.Delete(a => a.Id == liveStock.Id);
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
    }
}
