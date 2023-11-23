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
        Task AddEmployee(EmployeeCreateModel employee);
        Task DeleteEmployee(Employee employee);
        Task<byte[]> ExportEmployeeEffort(int emplopyeeId, int month, int year);
        Task<byte[]> ExportEmployeesEffortToExcel(int farmId, int month, int year);
        Task<byte[]> ExportEmployeesToExcel(int farmId);
        Task<IEnumerable<EmployeeListModel>> GetByTaskType(int id);
        Task<List<EmployeeEffortInTask>> GetEffortEmployeeInTask(int id, int month, int year);
        Task<EmployeeListModel> GetEmployee(int id);
        Task<List<EmployeeEffortInMonthModel>> GetTotalEffortEmployee(int id, int month, int year);
        Task ImportEmployeesFromExcel(Stream excelFileStream);
        Task<IEnumerable<EmployeeListModel>> ListByTaskTypeFarm(int taskTypeid, int farmId);
        Task<IEnumerable<EmployeeListModel>> ListEmployee();
        Task<IEnumerable<EmployeeListModel>> ListEmployeeActive();
        Task<IEnumerable<EmployeeFarmModel>> ListEmployeeActiveByFarm(int id);
        Task<IEnumerable<EmployeeFarmModel>> ListEmployeeByFarm(int id);
        Task<IEnumerable<EmployeeFarmModel>> ListTaskEmployee(int taskId);
        Task UpdateEmployee(int id, EmployeeCreateModel employeeModel);
        Task UpdateStatus(int id);
    }
}
