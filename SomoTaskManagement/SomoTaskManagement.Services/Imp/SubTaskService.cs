﻿using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.SubTask;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class SubTaskService : ISubTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static int taskCounter = 1;
        public SubTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SubTaskModel>> SubtaskByTask(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Nhiệm vụ không tìm thấy");
            }
            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId && s.Status == true, includes: includes);
            if (!subtask.Any())
            {
                throw new Exception("Nhiệm vụ con rỗng");
            }
            return _mapper.Map<IEnumerable<Employee_Task>, IEnumerable<SubTaskModel>>(subtask);
        }

        public async Task<IEnumerable<SubTaskModel>> NonSubtaskByTask(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Nhiệm vụ không tìm thấy");
            }
            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId && s.Status == false, includes: includes);
            if (!subtask.Any())
            {
                throw new Exception("Nhiệm vụ con rỗng");
            }
            return _mapper.Map<IEnumerable<Employee_Task>, IEnumerable<SubTaskModel>>(subtask);
        }

        public async Task UpdateSubTasks(int subtaskId, SubTaskUpdateModel subTask)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            string taskCode = GenerateTaskCode(vietnamTime, taskCounter);
            var subtaskUpdate = await _unitOfWork.RepositoryEmployee_Task.GetById(subtaskId);

            if (subtaskUpdate == null)
            {
                throw new Exception("Không tìm thấy công việc con");
            }

            subtaskUpdate.Description = subTask.Description;
            subtaskUpdate.Name = subTask.Name;
            subtaskUpdate.Status = true;
            subtaskUpdate.StartDay = subTask.StartDay;
            subtaskUpdate.EndDay = subTask.EndDay;
            subtaskUpdate.Code = taskCode;


            taskCounter++;
            _unitOfWork.RepositoryEmployee_Task.Update(subtaskUpdate);
            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }

        public async Task CreateSubTasks(SubTaskCreateModel subTask)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            string taskCode = GenerateTaskCode(vietnamTime, taskCounter);
            var subTaskNew = new Employee_Task
            {
                TaskId = subTask.TaskId,
                EmployeeId = subTask.EmployeeId,
                Description = subTask.Description,
                Name = subTask.Name,
                StartDay = subTask.StartDay,
                EndDay = subTask.EndDay,
                Code = taskCode,
                Status = true,
            };

            if (subTask.StartDay < subTaskNew.StartDay || subTask.EndDay > subTaskNew.EndDay)
            {
                throw new Exception("Ngày bắt đầu và kết thúc của subtask không nằm trong khoảng của task");
            }
            var task = await _unitOfWork.RepositoryEmployee_Task.GetData(t => t.TaskId == subTask.TaskId);
            var employeeOfTask = task.Select(t => t.EmployeeId).ToList();
            if (!employeeOfTask.Contains(subTask.EmployeeId))
            {
                throw new Exception("EmployeeId không nằm trong danh sách của task");
            }

            taskCounter++;
            await _unitOfWork.RepositoryEmployee_Task.Add(subTaskNew);
            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }

        private string GenerateTaskCode(DateTime vietnamTime, int counter)
        {
            return "CON" + vietnamTime.Year.ToString().Substring(2) + counter.ToString("D5");
        }
        public async Task DeleteSubTasks(int subtaskId)
        {
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetById(subtaskId);
            if (subtask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ con ");
            }
            _unitOfWork.RepositoryEmployee_Task.Delete(s => s.SubtaskId == subtaskId);

            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }


        public async Task<IEnumerable<EmployeeListModel>> GetEmployeesNoSubtask(int taskId)
        {
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.Status == false && s.TaskId == taskId);
            var employeeIds = employee_task.Select(s => s.EmployeeId).ToList();
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));
            if (employees == null)
            {
                throw new Exception("không tìm thấy nhân viên");
            }

            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
        }

        public async Task UpdateEffortTime(int taskId, List<EmployeeEffortUpdate> employeeEffortTimes)
        {
            var existingTask = await _unitOfWork.RepositoryFarmTask.GetById(taskId);

            if (existingTask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            foreach (var employeeEffortTime in employeeEffortTimes)
            {
                var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == taskId && s.EmployeeId == employeeEffortTime.EmployeeId);

                if (existingSubTask == null)
                {
                    throw new Exception($"Không tìm thấy subtask cho employee có ID {employeeEffortTime.EmployeeId}");
                }

                existingSubTask.ActualEfforMinutes = employeeEffortTime.ActualEfforMinutes;
                existingSubTask.ActualEffortHour = employeeEffortTime.ActualEffortHour;

                _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
                await _unitOfWork.RepositoryEmployee_Task.Commit();
            }
        }

        public async Task UpdateEffortOfSubtask(int subtaskId, EmployeeEffortUpdate employeeEffortTime)
        {
            var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetById(subtaskId);
            if (existingSubTask == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }

            var task = await _unitOfWork.RepositoryFarmTask.GetSingleByCondition(t => t.Id == existingSubTask.TaskId);
            if (task.Status == 0 || task.Status == 4 || task.Status == 5)
            {
                throw new Exception("Chỉ những nhiệm vụ đang thực hiện, hoàn thành và không hoàn thành mới được chấm công");
            }

            existingSubTask.ActualEfforMinutes = employeeEffortTime.ActualEfforMinutes;
            existingSubTask.ActualEffortHour = employeeEffortTime.ActualEffortHour;
            //existingSubTask.TaskId = existingSubTask.TaskId;

            _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }


        public async Task UpdateEffortTimeAndStatusTask(int taskId, List<EmployeeEffortUpdate> employeeEffortTimes, int statusTask)
        {
            try
            {
                if (statusTask != 2 && statusTask != 3)
                {
                    throw new Exception("Chỉ được phép sử dụng giá trị hoàn thành và không hoàn thành");
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var existingTask = await _unitOfWork.RepositoryFarmTask.GetById(taskId);

                    if (existingTask == null)
                    {
                        throw new Exception("Không tìm thấy nhiệm vụ");
                    }
                    var subtaskOfTask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == taskId && s.Status == true);
                    if (subtaskOfTask.Count() > 0)
                    {
                        existingTask.Status = statusTask;
                        await _unitOfWork.RepositoryFarmTask.Commit();
                    }
                    else
                    {
                        existingTask.Status = statusTask;
                        await _unitOfWork.RepositoryFarmTask.Commit();

                        foreach (var employeeEffortTime in employeeEffortTimes)
                        {
                            var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == taskId && s.EmployeeId == employeeEffortTime.EmployeeId && s.Status == false);

                            if (existingSubTask == null)
                            {
                                throw new Exception($"Không tìm thấy subtask cho employee có ID {employeeEffortTime.EmployeeId}");
                            }

                            existingSubTask.ActualEfforMinutes = employeeEffortTime.ActualEfforMinutes;
                            existingSubTask.ActualEffortHour = employeeEffortTime.ActualEffortHour;

                            _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
                            await _unitOfWork.RepositoryEmployee_Task.Commit();
                        }
                    }
                    _unitOfWork.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetEffortByTaskModel> GetEffortByTask(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };
            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId, includes: includes);

            bool checkSubtask = false;
            var nonSubtask = await _unitOfWork.RepositoryEmployee_Task.GetData(s => s.TaskId == taskId && s.Status == true);
            if (nonSubtask.Count() > 0)
            {
                checkSubtask = true;
            }
            var groupedSubtasks = subtasks.GroupBy(s => new { s.TaskId, s.EmployeeId });

            var subtaskEffortModels = groupedSubtasks.Select(group =>
            {
                var subtaskModel = _mapper.Map<Employee_Task, SubtaskEffortModel>(group.First());
                var effortMinutes = group.Sum(s => s.ActualEfforMinutes);
                var effortHours = group.Sum(s => s.ActualEffortHour);

                subtaskModel.TotalActualEfforMinutes = effortMinutes;
                subtaskModel.TotalActualEffortHour = effortHours;

                if (subtaskModel.TotalActualEfforMinutes >= 60)
                {
                    int additionalHours = subtaskModel.TotalActualEfforMinutes / 60;

                    subtaskModel.TotalActualEffortHour += additionalHours;

                    subtaskModel.TotalActualEfforMinutes %= 60;
                }
                return subtaskModel;
            });

            return new GetEffortByTaskModel
            {
                Subtasks = subtaskEffortModels,
                IsHaveSubtask = checkSubtask
            };
        }

        public async Task<TotalEffortModel> GetTotalEffortEmployee(int id, DateTime? startDay, DateTime? endDay)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }

            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.EmployeeId == id);

            var taskIds = subtasks.Select(s => s.TaskId);

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id) && (t.Status == 2 || t.Status == 3) &&
                                                 (!startDay.HasValue || !endDay.HasValue || (
                                                               (startDay.Value.Year <= t.StartDate.Year &&
                                                                endDay.Value.Year >= t.StartDate.Year) &&
                                                               (startDay.Value.Month <= t.StartDate.Month &&
                                                                endDay.Value.Month >= t.StartDate.Month) &&
                                                               (startDay.Value.Day <= t.StartDate.Day &&
                                                                endDay.Value.Day >= t.StartDate.Day)
                                                            ))
                                                );



            var totalTask = tasks.Count();

            var taskEffortIds = tasks.Select(t => t.Id);

            var subtaskEffort = await _unitOfWork.RepositoryEmployee_Task.GetData(s => taskEffortIds.Contains(s.TaskId) && s.EmployeeId == id);

            var effortMinutes = subtaskEffort.Sum(s => s.ActualEfforMinutes);
            var effortHours = subtaskEffort.Sum(s => s.ActualEffortHour);

            var totalEffortEmployee = new TotalEffortModel
            {
                EmployeeId = id,
                EmployeeName = employee.Name,
                EmployeeCode = employee.Code,
                TotalTask = totalTask,
                ActualEfforMinutes = effortMinutes,
                ActualEffortHour = effortHours
            };

            return totalEffortEmployee;
        }



    }
}