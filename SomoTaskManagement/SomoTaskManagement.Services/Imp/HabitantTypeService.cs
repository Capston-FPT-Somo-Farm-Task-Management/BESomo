using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class HabitantTypeService : IHanbitantTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HabitantTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListHabitantType()
        {
            var includes = new Expression<Func<HabitantType, object>>[]
            {
                t =>t.Farm,
            };
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression:null, includes: includes);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListHabitantTypeActive(int farmId)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(farmId)?? throw new Exception("Không tìm thấy trang trại");
            var includes = new Expression<Func<HabitantType, object>>[]
            {
                t =>t.Farm,
            };
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: h => h.IsActive == true && h.FarmId == farmId,includes:includes);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }

        public async Task<IEnumerable<HabitantTypeModel>> ListPlantType(int farmId)
        {
            var includes = new Expression<Func<HabitantType, object>>[]
            {
                t =>t.Farm,
            };
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 0 && p.FarmId == farmId, includes: includes);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListLiveStock(int farmId)
        {
            var includes = new Expression<Func<HabitantType, object>>[]
            {
                t =>t.Farm,
            };
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 1 && p.FarmId == farmId, includes: includes);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }


        public async Task<IEnumerable<HabitantTypeModel>> ListPlantTypeActive(int farmId)
        {
            var includes = new Expression<Func<HabitantType, object>>[]
           {
                t =>t.Farm,
           };
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 0 && p.IsActive == true && p.FarmId == farmId, includes: includes);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListLiveStockActive(int farmId)
        {
            var includes = new Expression<Func<HabitantType, object>>[]
          {
                t =>t.Farm,
          };
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 1 && p.IsActive == true && p.FarmId == farmId, includes: includes);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }


        public Task<HabitantType> GetHabitant(int id)
        {
            return _unitOfWork.RepositoryHabitantType.GetById(id);
        }
        public async Task AddHabitantType(HabitantTypeCUModel habitantType)
        {
            var habitantTypeNew = new HabitantType
            {
                Name = habitantType.Name,
                Origin = habitantType.Origin,
                Environment = habitantType.Environment,
                Description = habitantType.Description,
                Status = habitantType.Status,
                IsActive = true,
                FarmId = habitantType.FarmId
            };
            await _unitOfWork.RepositoryHabitantType.Add(habitantTypeNew);
            await _unitOfWork.RepositoryHabitantType.Commit();
        }

        //public async Task UpdateHabitantType(HabitantType habitantType)
        //{
        //    _unitOfWork.RepositoryHabitantType.Update(habitantType);
        //    await _unitOfWork.RepositoryHabitantType.Commit();
        //}
        public async Task UpdateHabitantType(int id, HabitantTypeCUModel habitantType)
        {
            var habitantTypeUpdate = await _unitOfWork.RepositoryHabitantType.GetSingleByCondition(p => p.Id == id);
            if (habitantTypeUpdate != null)
            {
                habitantTypeUpdate.Id = id;
                habitantTypeUpdate.Name = habitantType.Name;
                //liveStockUpdate.CreateDate = liveStock.CreateDate;
                habitantTypeUpdate.Origin = habitantType.Origin;
                habitantTypeUpdate.Environment = habitantType.Environment;
                habitantTypeUpdate.Description = habitantType.Description;
                habitantTypeUpdate.Status = habitantType.Status;

                await _unitOfWork.RepositoryLiveStock.Commit();
            }
            else
            {
                throw new Exception("Không tìm thấy động vật");
            }
        }

        public async Task UpdateStatus(int id)
        {
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetById(id);
            if (habitantType == null)
            {
                throw new Exception("Loại không tìm thấy");
            }
            var livestock = await _unitOfWork.RepositoryLiveStock.GetData(expression: l => l.HabitantTypeId == habitantType.Id);
            var plant = await _unitOfWork.RepositoryPlant.GetData(expression: l => l.HabitantTypeId == habitantType.Id);

            var numberLivestock = livestock.Count();
            var numberPlant = plant.Count();

            if (numberLivestock != 0 || numberPlant != 0 && habitantType.IsActive == true)
            {
                throw new Exception("Còn thực thể xuất hiện trong loại này nên không thể xoá");
            }
            habitantType.IsActive = habitantType.IsActive == true ? false : true;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
    }
}
