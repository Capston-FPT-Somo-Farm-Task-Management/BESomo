using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
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
    public class PlantService : IPlantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PlantModel>> GetList()
        {
            var includes = new Expression<Func<Plant, object>>[]
            {
                t =>t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };
            var plants = await _unitOfWork.RepositoryPlant
                .GetData(expression: null, includes: includes);

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<PlantModel>>(plants);
        }


        public async Task<IEnumerable<ExternalIdModel>> GetExternalIds(int id)
        {
            var plants = await _unitOfWork.RepositoryPlant.GetData(expression: l => l.FieldId == id, includes: null);

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<ExternalIdModel>>(plants);
        }


        public async Task<PlantModel> Get(int id)
        {
            var plant = await _unitOfWork.RepositoryPlant.GetById(id);
            if (plant == null)
            {
                throw new Exception(" Not found plant");
            }
            var habitantType = await _unitOfWork.RepositoryPlant.GetSingleByCondition(h => h.Id == plant.HabitantTypeId);
            var field = await _unitOfWork.RepositoryField.GetSingleByCondition(h => h.Id == plant.FieldId);
            var zone = await _unitOfWork.RepositoryZone.GetSingleByCondition(h => h.Id == field.ZoneId);
            var area = await _unitOfWork.RepositoryArea.GetById(zone.AreaId);

            var status = (EnumStatus)plant.Status;
            var statusString = status == EnumStatus.Active ? "Active" : "Inactive";

            var plantModel = new PlantModel
            {
                Id = plant.Id,
                Name = plant.Name,
                Status = statusString,
                ExternalId = plant.ExternalId,
                CreateDate = plant.CreateDate,
                Height = plant.Height,
                HabitantTypeName = habitantType.Name,
                FieldName = field.Name,
                ZoneName = zone.Name,
                AreaName = area.Name,
            };

            return plantModel;
        }
        public async Task Add(Plant plant)
        {
            plant.Status = 1;
            await _unitOfWork.RepositoryPlant.Add(plant);
            await _unitOfWork.RepositoryPlant.Commit();
        }
        public async Task Update(Plant plant)
        {
            var plantUpdate = await _unitOfWork.RepositoryPlant.GetSingleByCondition(p => p.Id == plant.Id);
            if (plantUpdate != null)
            {
                plantUpdate.ExternalId = plant.ExternalId;
                plantUpdate.CreateDate = plant.CreateDate;
                plantUpdate.Height = plant.Height;
                plantUpdate.HabitantTypeId = plant.HabitantTypeId;
                plantUpdate.FieldId = plant.FieldId;
                plantUpdate.Name = plant.Name;
                plantUpdate.Status = plant.Status;

                await _unitOfWork.RepositoryPlant.Commit();
            }
        }

        public async Task UpdateStatus(int id)
        {
            var plant = await _unitOfWork.RepositoryPlant.GetSingleByCondition(f => f.Id == id);
            if (plant != null)
            {
                plant.Status = plant.Status == 1 ? 0 : 1;
                await _unitOfWork.RepositoryLiveStock.Commit();
            }
        }


        public async Task DeleteHabitant(Plant plant)
        {
            _unitOfWork.RepositoryPlant.Delete(a => a.Id == plant.Id);
            await _unitOfWork.RepositoryPlant.Commit();
        }
    }
}
