using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
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
        private readonly SomoTaskManagemnetContext _context;

        public EmployeeService(IUnitOfWork unitOfWork, SomoTaskManagemnetContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public Task<IEnumerable<Employee>> ListEmployee()
        {
            return _unitOfWork.RepositoryEmployee.GetData(null);
        }

        public Task<Employee> GetEmployee(int id)
        {
            return _unitOfWork.RepositoryEmployee.GetById(id);
        }
        public async Task AddEmployee(List<int> taskTypeIds, Employee employee)
        {
            var employeeNew = new Employee
            {
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                Name = employee.Name,
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
                    EmployeeId = employee.Id,
                    TaskTypeId = taskType.Id,
                    Status = true,
                };

                employeeNew.Employee_TaskTypes.Add(employee_TaskType);
            }

            await _unitOfWork.RepositoryEmployee.Add(employeeNew);
            await _unitOfWork.RepositoryEmployee.Commit();
        }
        public async Task<IEnumerable<Employee>> GetByTaskType(int id)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(id);
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetSingleByCondition(e => e.TaskTypeId == taskType.Id);
            var employee = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.Id == employee_taskType.EmployeeId, includes: null);

            return employee;
        }

        public Task<IEnumerable<Employee>> ListEmployeeByFarm(int id)
        {
            return _unitOfWork.RepositoryEmployee.GetData(expression: e => e.FarmId == id, includes: null);
        }

        public async Task<IEnumerable<Employee>> ListByTaskTypeFarm(int taskTypeid, int farmId)
        {
            var taskType = await _context.TaskType.SingleOrDefaultAsync(t => t.Id == taskTypeid);
            if(taskType == null)
            {
                throw new Exception("not found");
            }
            var employee_taskType = await _context.Employee_TaskType.SingleOrDefaultAsync(t => t.TaskTypeId == taskType.Id);

            if (employee_taskType != null)
            {
                var employees = await _context.Employee
                    .Where(t => t.Id == employee_taskType.EmployeeId && t.FarmId == farmId)
                    .ToListAsync();

                return employees;
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
    }
}
