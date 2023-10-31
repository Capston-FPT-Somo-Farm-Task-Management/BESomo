using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.SubTask;
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
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == task.Id && s.Status == true, includes: includes);
            if (!subtask.Any())
            {
                throw new Exception("Nhiệm vụ con rỗng");
            }
            return _mapper.Map<IEnumerable<Employee_Task>, IEnumerable<SubTaskModel>>(subtask);
        }

        public async Task CreateSubTasks(SubTaskCreateModel subTask)
        {
            var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == subTask.TaskId && s.EmployeeId == subTask.EmployeeId);

            if (existingSubTask == null)
            {
                throw new Exception("Không tìm thấy subtask");
            }

            existingSubTask.Description = subTask.Description;
            existingSubTask.Name = subTask.Name;
            existingSubTask.Status = true;
            existingSubTask.ActualEffort = 0;

            _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);

            await _unitOfWork.RepositoryEmployee_Task.Commit();
        }

        public async Task DeleteSubTasks(int taskId, int employeeId)
        {
            var existingSubTask = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.TaskId == taskId && s.EmployeeId == employeeId);

            if (existingSubTask == null)
            {
                throw new Exception("Không tìm thấy subtask");
            }

            existingSubTask.Description = null;
            existingSubTask.Name = null;
            existingSubTask.Status = false;
            existingSubTask.ActualEffort = 0;

            //_unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);

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

                existingSubTask.ActualEffort = employeeEffortTime.EffortTime;

                _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
                await _unitOfWork.RepositoryEmployee_Task.Commit();
            }
            
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
                    if (existingTask != null)
                    {
                        existingTask.Status = statusTask;
                        await _unitOfWork.RepositoryFarmTask.Commit();
                    }
                    else
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

                        existingSubTask.ActualEffort = employeeEffortTime.EffortTime;

                        _unitOfWork.RepositoryEmployee_Task.Update(existingSubTask);
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



        public async Task<IEnumerable<SubtaskEffortModel>> GetEffortByTask(int taskId)
        {
            var includes = new Expression<Func<Employee_Task, object>>[]
            {
                t => t.Employee,
            };
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.TaskId == taskId, includes: includes);
            employee_task = employee_task.OrderBy(s => s.Employee.Name);

            return _mapper.Map<IEnumerable<Employee_Task>, IEnumerable<SubtaskEffortModel>>(employee_task);
        }

        public async Task<TotalEffortModel> GetTotalEffortEmployee(int id, DateTime? startDay, DateTime? endDay)
        {

            var employee = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }
            var subtask = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: s => s.EmployeeId == id);
            var taskIds =subtask.Select(s => s.TaskId);

            var task = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id) && (t.Status == 2 || t.Status == 3) &&
                                                                         (!startDay.HasValue || !endDay.HasValue || (
                                                                            startDay.Value.Year >= t.StartDate.Year &&
                                                                            endDay.Value.Year <= t.EndDate.Year &&
                                                                            startDay.Value.Month >= t.StartDate.Month &&
                                                                            endDay.Value.Month <= t.EndDate.Month &&
                                                                            startDay.Value.Day >= t.StartDate.Day &&
                                                                            endDay.Value.Day <= t.EndDate.Day
                                                                         )));

            var totalTask = task.Count();

            var taskEffortIds = task.Select(t => t.Id);

            var subtaskEffort = await _unitOfWork.RepositoryEmployee_Task.GetData(s => taskEffortIds.Contains(s.TaskId));

            var totalEffort = subtaskEffort
            .Where(s => taskEffortIds.Contains(s.TaskId) && s.EmployeeId == id)
            .Sum(s => s.ActualEffort);


            var totalEffortEmployee = new TotalEffortModel
            {
                EmployeeId = id,
                EmployeeName = employee.Name,
                EmployeeCode  = employee.Code,
                TotalTask = totalTask,
                EffortTime = totalEffort
            };
            return totalEffortEmployee;
        }
    }
}
