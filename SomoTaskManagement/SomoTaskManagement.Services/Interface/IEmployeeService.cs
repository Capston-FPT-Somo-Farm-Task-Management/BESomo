using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IEmployeeService
    {
        Task AddEmployee(List<int> taskTypeIds, EmployeeCreateModel employee);
        Task DeleteEmployee(Employee employee);
        Task<IEnumerable<EmployeeListModel>> GetByTaskType(int id);
        Task<EmployeeListModel> GetEmployee(int id);
        Task<IEnumerable<EmployeeListModel>> ListByTaskTypeFarm(int taskTypeid, int farmId);
        Task<IEnumerable<EmployeeListModel>> ListEmployee();
        Task<IEnumerable<EmployeeListModel>> ListEmployeeActive();
        Task<IEnumerable<EmployeeFarmModel>> ListEmployeeActiveByFarm(int id);
        Task<IEnumerable<EmployeeFarmModel>> ListEmployeeByFarm(int id);
        Task<IEnumerable<EmployeeFarmModel>> ListTaskEmployee(int taskId);
        Task UpdateEmployee(int id, EmployeeCreateModel employee, List<int> taskTypeIds);
        Task UpdateStatus(int id);
    }
}
