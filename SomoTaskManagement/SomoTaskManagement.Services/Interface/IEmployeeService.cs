using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IEmployeeService
    {
        Task AddEmployee(List<int> taskTypeIds, Employee employee);
        Task DeleteEmployee(Employee employee);
        Task<IEnumerable<Employee>> GetByTaskType(int id);
        Task<Employee> GetEmployee(int id);
        Task<IEnumerable<Employee>> ListByTaskTypeFarm(int taskTypeid, int farmId);
        Task<IEnumerable<Employee>> ListEmployee();
        Task<IEnumerable<Employee>> ListEmployeeByFarm(int id);
        Task UpdateEmployee(Employee employee);
    }
}
