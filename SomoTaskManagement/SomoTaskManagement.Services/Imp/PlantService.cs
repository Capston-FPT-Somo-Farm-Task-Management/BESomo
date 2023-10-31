using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Domain.Model.Plant;
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
            plants = plants.OrderByDescending(p => p.CreateDate).ToList();

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<PlantModel>>(plants);
        }

        public async Task<IEnumerable<PlantModel>> GetListActive()
        {
            var includes = new Expression<Func<Plant, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };
            var plants = await _unitOfWork.RepositoryPlant
                .GetData(expression: l => l.Status == 1, includes: includes);

            plants = plants.OrderByDescending(p => p.CreateDate).ToList();
            return _mapper.Map<IEnumerable<Plant>, IEnumerable<PlantModel>>(plants);
        }

        public async Task<IEnumerable<ExternalIdModel>> GetExternalIds(int id)
        {
            var plants = await _unitOfWork.RepositoryPlant.GetData(expression: l => l.FieldId == id && l.Status == 1, includes: null);

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<ExternalIdModel>>(plants);
        }

        public async Task<IEnumerable<PlantModel>> GetPlantFarm(int farmId)
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

            var includes = new Expression<Func<Plant, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };

            var plants = await _unitOfWork.RepositoryPlant
                .GetData(expression: l => fieldId.ToList().Contains(l.FieldId), includes: includes);
            if (plants == null)
            {
                throw new Exception("Không tìm thấy cây");
            }
            plants = plants.OrderByDescending(p => p.CreateDate).ToList();

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<PlantModel>>(plants);
        }

        public async Task<IEnumerable<PlantModel>> GetPlantActiveFarm(int farmId)
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

            var includes = new Expression<Func<Plant, object>>[]
            {
                t => t.Field,
                t => t.HabitantType,
                t => t.Field.Zone,
                t => t.Field.Zone.Area
            };

            var plants = await _unitOfWork.RepositoryPlant
                .GetData(expression: l => fieldId.ToList().Contains(l.FieldId)&& l.Status == 1, includes: includes);
            if (plants == null)
            {
                throw new Exception("Không tìm thấy cây");
            }
            plants = plants.OrderByDescending(p => p.CreateDate).ToList();

            return _mapper.Map<IEnumerable<Plant>, IEnumerable<PlantModel>>(plants);
        }

        public async Task<PlantModel> Get(int id)
        {
            var plant = await _unitOfWork.RepositoryPlant.GetById(id);
            if (plant == null)
            {
                throw new Exception(" Không tìm thấy cây");
            }
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetSingleByCondition(h => h.Id == plant.HabitantTypeId);
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
                AreaId = area.Id,
                FieldId = field.Id,
                ZoneId = zone.Id,
                HabitantTypeId = habitantType.Id,
            };

            return plantModel;
        }
        public async Task Add(PlantCreateModel plant)
        {
            var plantNew = new Plant
            {
                Name = plant.Name,
                ExternalId = plant.ExternalId,
                CreateDate = DateTime.Now,
                Status = 1,
                HabitantTypeId = plant.HabitantTypeId,
                FieldId = plant.FieldId,
                Height = plant.Height,
                IsActive = true,
            };

            var existCode = await _unitOfWork.RepositoryPlant.GetSingleByCondition(a => a.ExternalId == plant.ExternalId);
            if (existCode != null)
            {
                throw new Exception("Mã cây trồng không thể trùng");
            }
            await _unitOfWork.RepositoryPlant.Add(plantNew);
            await _unitOfWork.RepositoryPlant.Commit();
        }
        public async Task Update(int id, PlantCreateModel plant)
        {
            var plantUpdate = await _unitOfWork.RepositoryPlant.GetSingleByCondition(p => p.Id == id);
            if (plantUpdate != null)
            {
                var initialExternalId = plantUpdate.ExternalId;

                plantUpdate.Id = id;
                plantUpdate.ExternalId = plant.ExternalId;
                //plantUpdate.CreateDate = plant.CreateDate;
                plantUpdate.Height = plant.Height;
                plantUpdate.HabitantTypeId = plant.HabitantTypeId;
                plantUpdate.FieldId = plant.FieldId;
                plantUpdate.Name = plant.Name;
                plantUpdate.Status = 1;
                plantUpdate.IsActive = true;


                if (plantUpdate.ExternalId != initialExternalId)
                {
                    var existCode = await _unitOfWork.RepositoryLiveStock.GetSingleByCondition(a => a.ExternalId == plant.ExternalId);
                    if (existCode != null)
                    {
                        throw new Exception("Mã động vật không thể trùng");
                    }
                }
                await _unitOfWork.RepositoryPlant.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy cây");
            }
        }

        public async Task UpdateStatus(int id)
        {
            var plant = await _unitOfWork.RepositoryPlant.GetById(id);
            if (plant == null)
            {
                throw new Exception("Plant not found");
            }
            plant.Status = plant.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }


        public async Task DeleteHabitant(Plant plant)
        {
            _unitOfWork.RepositoryPlant.Delete(a => a.Id == plant.Id);
            await _unitOfWork.RepositoryPlant.Commit();
        }
    }
}
