using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class FarmTaskService : IFarmTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FarmTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FarmTaskModel>> GetList()
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t =>t.Member,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t=>t.Other,
                t=>t.TaskType,
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                .GetData(expression: null, includes: includes);

            return _mapper.Map<IEnumerable<FarmTask>, IEnumerable<FarmTaskModel>>(farmTasks);
        }

        public async Task<IEnumerable<FarmTaskModel>> GetListActive()
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t =>t.Member,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t=>t.Other,
                t=>t.TaskType,
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                .GetData(expression: t => t.Status == 0 || t.Status == 1 || t.Status == 2 || t.Status == 3, includes: includes) ;

            return _mapper.Map<IEnumerable<FarmTask>, IEnumerable<FarmTaskModel>>(farmTasks);
        }

        public async Task<FarmTaskModel> Get(int id)
        {
            var farmTask = await _unitOfWork.RepositoryFarmTask.GetById(id);

            if (farmTask == null)
            {
                throw new Exception("Not found task");
            }

            var member = await _unitOfWork.RepositoryMember.GetById(farmTask.MemberId);
            var plant = await _unitOfWork.RepositoryPlant.GetById(farmTask.PlantId);
            var liveStock = await _unitOfWork.RepositoryLiveStock.GetById(farmTask.LiveStockId);
            var field = await _unitOfWork.RepositoryField.GetById(farmTask.FieldId);
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(farmTask.TaskTypeId);
            var other = await _unitOfWork.RepositoryOther.GetById(farmTask.OtherId);

            var zone = await _unitOfWork.RepositoryZone.GetSingleByCondition(h => h.Id == field.ZoneId);
            var area = await _unitOfWork.RepositoryArea.GetById(zone.AreaId);

            var status = (TaskStatusEnum)farmTask.Status;
            var statusString = GetTaskStatusDescription(status);

            var priority = (PriorityEnum)farmTask.Priority;
            var priorityString = GetPriorityDescription(priority);

            var farmTaskModel = new FarmTaskModel
            {
                Id = farmTask.Id,
                CreateDate = farmTask.CreateDate,
                StartDate = farmTask.StartDate,
                EndDate = farmTask.EndDate,
                Description = farmTask.Description,
                Priority = priorityString,
                Repeat = farmTask.Repeat,
                Iterations = farmTask.Iterations,
                ReceiverName = member != null ? member.Name : null,
                MemberName = member != null ? member.Name : null,
                PlantName = plant != null ? plant.Name : null,
                liveStockName = liveStock != null ? liveStock.Name : null,
                FieldName = field != null ? field.Name : null,
                OtherName = other != null ? other.Name : null,
                TaskTypeName = taskType != null ? taskType.Name : null,
                Remind = farmTask.Remind,
                Name = farmTask.Name,
                Status = statusString,
                ZoneName = zone.Name,
                AreaName = area.Name,
                ExternalId = liveStock != null ? liveStock.ExternalId : null
            };

            return farmTaskModel;
        }

        public static string GetTaskStatusDescription(TaskStatusEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }
        public static string GetPriorityDescription(PriorityEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public async Task<IEnumerable<FarmTaskModel>> GetTaskByDay(DateTime date)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t =>t.Member,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field,
                t=>t.Other,
                t=>t.TaskType,
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                .GetData(expression: task => task.StartDate.Date == date.Date || task.EndDate.Date == date.Date || task.CreateDate.Date == date.Date,
                        includes: includes
                        );
            return _mapper.Map<IEnumerable<FarmTask>, IEnumerable<FarmTaskModel>>(farmTasks);
        }

        public async Task<IEnumerable<FarmTaskModel>> GetTaskByMemberId(int id)
        {
            var includes = new Expression<Func<FarmTask, object>>[]
            {
                t =>t.Member,
                t => t.Plant,
                t => t.LiveStrock,
                t => t.Field.Zone,
                t => t.Field.Zone.Area,
                t => t.Field,
                t=>t.Other,
                t=>t.TaskType,
            };

            var farmTasks = await _unitOfWork.RepositoryFarmTask
                .GetData(expression: t => t.MemberId == id, includes: includes);

            return _mapper.Map<IEnumerable<FarmTask>, IEnumerable<FarmTaskModel>>(farmTasks);


        }

        public async Task Add(int memberId, TaskCreateUpdateModel farmTaskmodel, List<int> employeeIds, List<int> materialIds)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(memberId) ?? throw new Exception("Member not found");

            int totalDays = (farmTaskmodel.EndDate - farmTaskmodel.StartDate).Days;

            int currentDayIndex = 0;
            while (currentDayIndex <= totalDays)
            {
                var currentDay = farmTaskmodel.StartDate.AddDays(currentDayIndex);

                switch (farmTaskmodel.Repeat)
                {
                    case "Hằng tuần":
                        DateTime nextWeek = farmTaskmodel.StartDate.AddDays(7 * farmTaskmodel.Iterations);
                        currentDay = currentDay >= nextWeek ? nextWeek : currentDay;
                        break;

                    case "Hằng tháng":
                        DateTime nextMonth = farmTaskmodel.StartDate.AddMonths(1 * farmTaskmodel.Iterations);
                        currentDay = currentDay >= nextMonth ? nextMonth : currentDay;
                        break;

                    case "Hằng ngày":
                        currentDay = currentDay.AddDays(1 * farmTaskmodel.Iterations);
                        break;
                    case "None":
                        currentDay = farmTaskmodel.StartDate;
                        break;
                    default:
                        break;
                }


                var farmTaskNew = new FarmTask
                {
                    CreateDate = DateTime.Now,
                    StartDate = currentDay,
                    EndDate = farmTaskmodel.EndDate,
                    Description = farmTaskmodel.Description,
                    Priority = (int)ParsePriorityFromString(farmTaskmodel.Priority),
                    ReceiverId = farmTaskmodel.ReceiverId,
                    FieldId = farmTaskmodel.FieldId,
                    TaskTypeId = farmTaskmodel.TaskTypeId,
                    MemberId = member.Id,
                    OtherId = farmTaskmodel.OtherId == 0 ? (int?)null : farmTaskmodel.OtherId,
                    PlantId = farmTaskmodel.PlantId == 0 ? (int?)null : farmTaskmodel.PlantId,
                    LiveStockId = farmTaskmodel.LiveStockId == 0 ? (int?)null : farmTaskmodel.LiveStockId,
                    Name = farmTaskmodel.Name,
                    Status = 0,
                    Repeat = farmTaskmodel.Repeat,
                    Iterations = farmTaskmodel.Iterations,
                    Remind = farmTaskmodel.Remind
                };



                for (int i = 0; i < employeeIds.Count; i++)
                {
                    var employeeId = employeeIds[i];

                    var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                    if (employee == null)
                    {
                        throw new Exception($"Employee with ID '{employeeId}' not found.");
                    }

                    var employee_Task = new Employee_Task
                    {
                        EmployeeId = employee.Id,
                        TaskId = farmTaskNew.Id,
                        Status = true,
                    };

                    farmTaskNew.Employee_Tasks.Add(employee_Task);
                }


                for (int i = 0; i < materialIds.Count; i++)
                {
                    var materialId = materialIds[i];

                    var material = await _unitOfWork.RepositoryMaterial.GetById(materialId);

                    if (material == null)
                    {
                        throw new Exception($"Employee with ID '{materialId}' not found.");
                    }

                    var material_Task = new Material_Task
                    {
                        MaterialId = material.Id,
                        TaskId = farmTaskNew.Id,
                        Status = true,
                    };

                    farmTaskNew.Material_Tasks.Add(material_Task);
                }

                currentDayIndex++;

                await _unitOfWork.RepositoryFarmTask.Add(farmTaskNew);
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
        }

        public PriorityEnum ParsePriorityFromString(string priorityString)
        {
            Console.WriteLine($"Attempting to parse priority: '{priorityString}'");

            switch (priorityString.Trim())
            {
                case "Thấp nhất":
                    return PriorityEnum.Shortest;
                case "Thấp":
                    return PriorityEnum.Short;
                case "Trung bình":
                    return PriorityEnum.Medium;
                case "Cao":
                    return PriorityEnum.High;
                case "Cao nhất":
                    return PriorityEnum.Tallest;
                default:
                    Console.WriteLine("Invalid priority string.");
                    throw new ArgumentException("Invalid priority string.");
            }
        }


        public async Task Update(int farmTaskId, int memberId, FarmTask farmTaskUpdate, List<int> employeeIds, List<int> materialIds)
        {
            var taskOfMember = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.MemberId == memberId);
            if (taskOfMember == null)
            {
                throw new Exception("Task of member not found");
            }

            var farmTask = await _unitOfWork.RepositoryFarmTask.GetById(taskOfMember.Id);

            if (farmTask == null)
            {
                throw new Exception($"Farm task with ID '{farmTaskId}' not found.");
            }

            farmTask.Name = farmTaskUpdate.Name;
            farmTask.Description = farmTaskUpdate.Description;
            farmTask.Priority = farmTaskUpdate.Priority;
            farmTask.ReceiverId = farmTaskUpdate.ReceiverId;
            farmTask.FieldId = farmTaskUpdate.FieldId;
            farmTask.TaskTypeId = farmTaskUpdate.TaskTypeId;
            farmTask.OtherId = farmTaskUpdate.OtherId;
            farmTask.PlantId = farmTaskUpdate.PlantId;
            farmTask.LiveStockId = farmTaskUpdate.LiveStockId;
            farmTask.StartDate = farmTaskUpdate.StartDate;
            farmTask.EndDate = farmTaskUpdate.EndDate;
            farmTask.Status = farmTaskUpdate.Status;
            farmTask.Repeat = farmTaskUpdate.Repeat;
            farmTask.Iterations = farmTaskUpdate.Iterations;
            farmTask.Remind = farmTaskUpdate.Remind;
            farmTask.Employee_Tasks.Clear();

            foreach (var employeeId in employeeIds)
            {
                var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);

                if (employee == null)
                {
                    throw new Exception($"Employee with ID {employeeId} not found");
                }

                var employeeTask = new Employee_Task
                {
                    EmployeeId = employee.Id,
                    TaskId = farmTask.Id,
                    Status = true
                };

                farmTask.Employee_Tasks.Add(employeeTask);
            }

            farmTask.Material_Tasks.Clear();

            foreach (var materialId in materialIds)
            {
                var material = await _unitOfWork.RepositoryMaterial.GetById(materialId);

                if (material == null)
                {
                    throw new Exception($"Material with ID {materialId} not found");
                }

                var materialTask = new Material_Task
                {
                    MaterialId = material.Id,
                    TaskId = farmTask.Id,
                    Status = true
                };

                farmTask.Material_Tasks.Add(materialTask);
            }

            await _unitOfWork.RepositoryFarmTask.Commit();
        }


        public async Task UpdateStatus(int id, int status)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(f => f.Id == id);
            if (task != null)
            {
                task.Status = status;
                await _unitOfWork.RepositoryFarmTask.Commit();
            }
        }

        public async Task Delete(FarmTask farmTask)
        {
            _unitOfWork.RepositoryFarmTask.Delete(a => a.Id == farmTask.Id);
            await _unitOfWork.RepositoryFarmTask.Commit();
        }

    }
}
