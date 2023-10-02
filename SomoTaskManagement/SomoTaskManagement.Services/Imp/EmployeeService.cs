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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeListModel>> ListEmployee()
        {
            var employees = await _unitOfWork.RepositoryEmployee.GetData(null);
            employees = employees.OrderBy(e => e.Name.Split(' ').Last()).ToList();
            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
        }
        public async Task<IEnumerable<EmployeeListModel>> ListEmployeeActive()
        {
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.Status == 1, includes: null);
            employees = employees.OrderBy(e => e.Name.Split(' ').Last()).ToList();

            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
        }
        public async Task<EmployeeListModel> GetEmployee(int id)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }
            var status = (EnumStatus)employee.Status;
            var statusString = status == EnumStatus.Active ? "Active" : "Inactive";
            var employeeModel = new EmployeeListModel
            {
                Name = employee.Name,
                Status = statusString,
                PhoneNumber = employee.PhoneNumber,
                FarmId = employee.FarmId,
                Address = employee.Address,
            };
            return employeeModel;
        }

        public async Task AddEmployee(List<int> taskTypeIds, EmployeeCreateModel employeeModel)
        {
            var employeeNew = new Employee
            {
                PhoneNumber = employeeModel.PhoneNumber,
                Address = employeeModel.Address,
                Name = employeeModel.Name,
                FarmId = employeeModel.FarmId,
                Status = 1,
            };

            for (int i = 0; i < taskTypeIds.Count; i++)
            {
                var taskTypeId = taskTypeIds[i];

                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskTypeId);

                if (taskType == null)
                {
                    throw new Exception($"Task type with ID '{taskTypeId}' not found.");
                }

                var employee_TaskType = new Employee_TaskType
                {
                    EmployeeId = employeeNew.Id,
                    TaskTypeId = taskType.Id,
                    Status = true,
                };

                employeeNew.Employee_TaskTypes.Add(employee_TaskType);
            }

            await _unitOfWork.RepositoryEmployee.Add(employeeNew);
            await _unitOfWork.RepositoryEmployee.Commit();
        }
        public async Task<IEnumerable<EmployeeListModel>> GetByTaskType(int id)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(id);
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetSingleByCondition(e => e.TaskTypeId == taskType.Id);
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.Id == employee_taskType.EmployeeId && e.Status == 1, includes: null);
            employees = employees.OrderBy(e => e.Name).ToList();

            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
        }

        public async Task<IEnumerable<EmployeeFarmModel>> ListEmployeeByFarm(int id)
        {

            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.FarmId == id , includes: null);
            employees = employees.OrderBy(e => e.Name.Split(' ').Last()).ToList();

            var map = new Dictionary<Employee, EmployeeFarmModel>();
            var employeeModel = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeFarmModel>>(employees);

            foreach (var pair in employees.Zip(employeeModel, (ft, ftModel) => new { ft, ftModel }))
            {
                map.Add(pair.ft, pair.ftModel);
            };
            foreach (var employee in employees)
            {
                var taskTypeName = await ListTaskTypeEmployee(employee.Id);

                if (taskTypeName != null && map.ContainsKey(employee))
                {
                    map[employee].TaskTypeName = string.Join(", ", taskTypeName);
                }
            }
            return map.Values;
        }

        public async Task<IEnumerable<string>> ListTaskTypeEmployee(int employeeId)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);
            if (employee == null)
            {
                throw new Exception("Task not found");
            }
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetData(expression: e => e.EmployeeId == employee.Id, includes: null);
            var taskTypeIds = employee_taskType.Select(x => x.TaskTypeId).ToList();
            if (employee_taskType != null)
            {
                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => taskTypeIds.Contains(e.Id));

                return taskType.Select(e => e.Name);
            }

            return null;
        }

        public async Task<IEnumerable<EmployeeListModel>> ListByTaskTypeFarm(int taskTypeid, int farmId)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskTypeid);
            if (taskType == null)
            {
                throw new Exception("Task type not found");
            }
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetData(expression: e => e.TaskTypeId == taskType.Id, includes: null);
            var employeeIds = employee_taskType.Select(x => x.EmployeeId).ToList();
            if (employee_taskType != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id) && e.Status == 1);
                employees = employees.OrderBy(e => e.Name.Split(' ').Last()).ToList();

                return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
            }

            return null;
        }
        public async Task<IEnumerable<string>> ListTaskEmployee(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var employeeIds = employee_task.Select(x => x.EmployeeId).ToList();
            if (employee_task != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));
                employees = employees.OrderBy(e => e.Name).ToList();

                return employees.Select(e => e.Name);
            }

            return null;
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var employeeUpdate = await _unitOfWork.RepositoryEmployee.GetSingleByCondition(e => e.Id == employee.Id);

            employeeUpdate.PhoneNumber = employee.PhoneNumber;
            employeeUpdate.Address = employee.Address;
            employeeUpdate.Name = employee.Name;
            employeeUpdate.Status = employee.Status;

            await _unitOfWork.RepositoryEmployee.Commit();
        }
        public async Task DeleteEmployee(Employee employee)
        {
            _unitOfWork.RepositoryEmployee.Delete(a => a.Id == employee.Id);
            await _unitOfWork.RepositoryEmployee.Commit();
        }

        public async Task UpdateStatus(int id)
        {
            var liveStock = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (liveStock == null)
            {
                throw new Exception("Employee not found");
            }
            liveStock.Status = liveStock.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryLiveStock.Commit();
        }
    }
}
