using AutoMapper;
using AutoMapper.Execution;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
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
                Avatar = employee.Avatar,
                Code = employee.Code
            };
            return employeeModel;
        }

        public async Task AddEmployee(EmployeeCreateModel employeeModel)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var employeeNew = new Employee
                {
                    PhoneNumber = employeeModel.PhoneNumber,
                    Address = employeeModel.Address,
                    Name = employeeModel.Name,
                    FarmId = employeeModel.FarmId,
                    Gender = employeeModel.Gender,
                    Code = employeeModel.Code,
                    Status = 1,
                    DateOfBirth = employeeModel.DateOfBirth
                };

                var urlImage = await UploadImageToFirebaseAsync(employeeNew, employeeModel.ImageFile);
                employeeNew.Avatar = urlImage;
                for (int i = 0; i < employeeModel.TaskTypeIds.Count; i++)
                {
                    var taskTypeId = employeeModel.TaskTypeIds[i];

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

                var existCode = await _unitOfWork.RepositoryEmployee.GetSingleByCondition(a => a.Code == employeeModel.Code);
                if (existCode != null)
                {
                    throw new Exception("Mã không thể trùng");
                }

                await _unitOfWork.RepositoryEmployee.Add(employeeNew);
                await _unitOfWork.RepositoryEmployee.Commit();
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> UploadImageToFirebaseAsync(Employee employee, IFormFile imageFile)
        {
            var options = new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
            };

            string fileName = employee.Id.ToString();
            string fileExtension = Path.GetExtension(imageFile.FileName);

            var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                .Child("images")
                .Child(fileName + fileExtension);

            using (var stream = imageFile.OpenReadStream())
            {
                await firebaseStorage.PutAsync(stream);
            }

            var imageUrl = await firebaseStorage.GetDownloadUrlAsync();

            return imageUrl;
        }

        public async Task<IEnumerable<EmployeeListModel>> GetByTaskType(int id)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(id);
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetSingleByCondition(e => e.TaskTypeId == taskType.Id);
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.Id == employee_taskType.EmployeeId && e.Status == 1, includes: null);

            if (employees == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }
            employees = employees.OrderBy(e => e.Name).ToList();

            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
        }

        public async Task<IEnumerable<EmployeeFarmModel>> ListEmployeeByFarm(int id)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(id);
            if (farm == null)
            {
                throw new Exception("Không tìm thấy nông trại");
            }
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.FarmId == id, includes: null);
            if (employees == null)
            {
                throw new Exception("không tìm thấy nhân viên");
            }
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
                var taskTypeId = await ListTaskTypeIdEmployee(employee.Id);

                if (taskTypeId != null && map.ContainsKey(employee))
                {
                    map[employee].TaskTypeId = taskTypeId.ToList();
                }
            }
            return map.Values;
        }

        //Active
        public async Task<IEnumerable<EmployeeFarmModel>> ListEmployeeActiveByFarm(int id)
        {
            var farm = await _unitOfWork.RepositoryFarm.GetById(id);
            if (farm == null)
            {
                throw new Exception("Không tìm thấy nông trại");
            }
            var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => e.FarmId == id && e.Status == 1, includes: null);
            if (employees == null)
            {
                throw new Exception("không tìm thấy nhân viên");
            }
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

                var taskTypeId = await ListTaskTypeIdEmployee(employee.Id);

                if (taskTypeId != null && map.ContainsKey(employee))
                {
                    map[employee].TaskTypeId = taskTypeId.ToList();
                }
            }
            return map.Values;
        }

        public async Task<IEnumerable<string>> ListTaskTypeEmployee(int employeeId)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nông trại");
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
        public async Task<IEnumerable<int>> ListTaskTypeIdEmployee(int employeeId)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(employeeId);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nông trại");
            }
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetData(expression: e => e.EmployeeId == employee.Id, includes: null);
            var taskTypeIds = employee_taskType.Select(x => x.TaskTypeId).ToList();
            if (employee_taskType != null)
            {
                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => taskTypeIds.Contains(e.Id));

                return taskType.Select(e => e.Id);
            }

            return null;
        }

        public async Task<IEnumerable<EmployeeListModel>> ListByTaskTypeFarm(int taskTypeid, int farmId)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskTypeid);
            if (taskType == null)
            {
                throw new Exception("Không tìm thấy loại công việc");
            }
            var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetData(expression: e => e.TaskTypeId == taskType.Id, includes: null);
            var employeeIds = employee_taskType.Select(x => x.EmployeeId).ToList();
            if (employee_taskType != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id) && e.Status == 1 && e.FarmId == farmId);
                if (employees == null)
                {
                    throw new Exception("không tìm thấy nhân viên");
                }
                employees = employees.OrderBy(e => e.Name.Split(' ').Last()).ToList();

                return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeListModel>>(employees);
            }

            return null;
        }
        public async Task<IEnumerable<EmployeeFarmModel>> ListTaskEmployee(int taskId)
        {
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            if (task == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ");
            }
            var employee_task = await _unitOfWork.RepositoryEmployee_Task.GetData(expression: e => e.TaskId == task.Id, includes: null);
            var employeeIds = employee_task.Select(x => x.EmployeeId).ToList();
            if (employee_task != null)
            {
                var employees = await _unitOfWork.RepositoryEmployee.GetData(expression: e => employeeIds.Contains(e.Id));
                if (employees == null)
                {
                    throw new Exception("không tìm thấy nhân viên");
                }
                employees = employees.OrderBy(e => e.Name).ToList();
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

            return null;
        }

        public async Task UpdateEmployee(int id, EmployeeCreateModel employeeModel)
        {
            var employeeUpdate = await _unitOfWork.RepositoryEmployee.GetSingleByCondition(e => e.Id == id);
            if (employeeUpdate == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }
            var initialCode = employeeUpdate.Code;

            employeeUpdate.Id = id;
            employeeUpdate.PhoneNumber = employeeModel.PhoneNumber;
            employeeUpdate.Address = employeeModel.Address;
            employeeUpdate.Name = employeeModel.Name;
            employeeUpdate.Code = employeeModel.Code;
            employeeUpdate.Gender = employeeModel.Gender;
            employeeUpdate.Status = 1;

            //if (employeeUpdate.Code != initialCode)
            //{
            //    var existCode = await _unitOfWork.RepositoryEmployee.GetSingleByCondition(a => a.Code == employee.Code);
            //    if (existCode != null)
            //    {
            //        throw new Exception("Mã không thể trùng");
            //    }
            //}
            var urlImage = employeeModel.ImageFile != null
                  ? await UploadImageToFirebaseAsync(employeeUpdate, employeeModel.ImageFile)
                  : employeeUpdate.Avatar;
            employeeUpdate.Avatar = urlImage;

            _unitOfWork.RepositoryEmployee_TaskType.Delete(expression: t => t.EmployeeId == employeeUpdate.Id);
            //_unitOfWork.RepositoryEmployee_TaskType.Commit();

            for (int i = 0; i < employeeModel.TaskTypeIds.Count; i++)
            {
                var taskTypeId = employeeModel.TaskTypeIds[i];

                var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskTypeId);

                if (taskType == null)
                {
                    throw new Exception($"Task type with ID '{taskTypeId}' not found.");
                }

                var employee_TaskType = new Employee_TaskType
                {
                    EmployeeId = employeeUpdate.Id,
                    TaskTypeId = taskType.Id,
                    Status = true,
                };

                employeeUpdate.Employee_TaskTypes.Add(employee_TaskType);
            }
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
