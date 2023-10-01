using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
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

        public Task<HabitantType> GetHabitant(int id)
        {
            return _unitOfWork.RepositoryHabitantType.GetById(id);
        }
        public async Task AddHabitantType(HabitantType habitantType)
        {
            habitantType.Status = 1;
            await _unitOfWork.RepositoryHabitantType.Add(habitantType);
            await _unitOfWork.RepositoryHabitantType.Commit();
        }
        public async Task UpdateHabitantType(HabitantType habitantType)
        {
            _unitOfWork.RepositoryHabitantType.Update(habitantType);
            await _unitOfWork.RepositoryHabitantType.Commit();
        }
        public async Task DeleteHabitantType(HabitantType habitantType)
        {
            _unitOfWork.RepositoryHabitantType.Delete(a => a.Id == habitantType.Id);
            await _unitOfWork.RepositoryHabitantType.Commit();
        }
    }
}
