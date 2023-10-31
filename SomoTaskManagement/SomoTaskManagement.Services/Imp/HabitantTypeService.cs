﻿using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class HabitantTypeService: IHanbitantTypeService
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
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(null);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListHabitantTypeActive()
        {
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression:h=>h.IsActive == true);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }

        public async Task<IEnumerable<HabitantTypeModel>> ListPlantType()
        {
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression:p=>p.Status == 0);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListLiveStock()
        {
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 1);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }


        public async Task<IEnumerable<HabitantTypeModel>> ListPlantTypeActive()
        {
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 0 && p.IsActive == true);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }
        public async Task<IEnumerable<HabitantTypeModel>> ListLiveStockActive()
        {
            var habitantType = await _unitOfWork.RepositoryHabitantType.GetData(expression: p => p.Status == 1 && p.IsActive == true);
            return _mapper.Map<IEnumerable<HabitantType>, IEnumerable<HabitantTypeModel>>(habitantType);
        }


        public Task<HabitantType> GetHabitant(int id)
        {
            return _unitOfWork.RepositoryHabitantType.GetById(id);
        }
        public async Task AddHabitantType(HabitantType habitantType)
        {
            habitantType.IsActive = true;
            await _unitOfWork.RepositoryHabitantType.Add(habitantType);
            await _unitOfWork.RepositoryHabitantType.Commit();
        }
        //public async Task UpdateHabitantType(HabitantType habitantType)
        //{
        //    _unitOfWork.RepositoryHabitantType.Update(habitantType);
        //    await _unitOfWork.RepositoryHabitantType.Commit();
        //}
        public async Task UpdateHabitantType(int id,HabitantTypeCUModel habitantType)
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
                habitantTypeUpdate.IsActive = true;

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
