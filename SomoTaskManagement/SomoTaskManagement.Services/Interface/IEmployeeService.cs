using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
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
        Task<IEnumerable<EmployeeFarmModel>> ListEmployeeByFarm(int id);
        Task<IEnumerable<string>> ListTaskEmployee(int taskId);
        Task UpdateEmployee(Employee employee);
        Task UpdateStatus(int id);
    }
}
