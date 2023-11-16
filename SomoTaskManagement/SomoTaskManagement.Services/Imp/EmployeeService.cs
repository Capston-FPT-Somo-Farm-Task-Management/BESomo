using AutoMapper;
using AutoMapper.Execution;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
                if (employeeModel.ImageFile != null)
                {
                    var urlImage = await UploadImageToFirebaseAsync(employeeNew, employeeModel.ImageFile);
                    employeeNew.Avatar = urlImage;
                }
                else
                {
                    employeeNew.Avatar = "default_image_url";
                }
                
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

        public async Task ImportEmployeesFromExcel(Stream excelFileStream)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                using (var package = new ExcelPackage(excelFileStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var taskTypeIdsString = worksheet.Cells[row, 8].Value?.ToString();

                        var taskTypeIds = new List<int>();

                        if (!string.IsNullOrEmpty(taskTypeIdsString))
                        {
                            var taskTypeIdsArray = taskTypeIdsString.Split(',');

                            foreach (var idString in taskTypeIdsArray)
                            {
                                if (int.TryParse(idString, out int id))
                                {
                                    taskTypeIds.Add(id);
                                }
                            }
                        }

                        var employeeModel = new EmployeeImportExcelModel
                        {
                            Code = worksheet.Cells[row, 1].Value?.ToString(),
                            Name = worksheet.Cells[row, 2].Value?.ToString(),
                            PhoneNumber = worksheet.Cells[row, 3].Value?.ToString(),
                            Address = worksheet.Cells[row, 4].Value?.ToString(),
                            FarmId = Convert.ToInt32(worksheet.Cells[row, 5].Value),
                            Gender = worksheet.Cells[row, 6].Value != null ? (bool)worksheet.Cells[row, 5].Value : false,
                            DateOfBirth = Convert.ToDateTime(worksheet.Cells[row, 7].Value),
                            TaskTypeIds = taskTypeIds,
                            ImageUrl = worksheet.Cells[row, 9].Value?.ToString()
                        };

                        var existCode = await _unitOfWork.RepositoryEmployee.GetSingleByCondition(a => a.Code == employeeModel.Code);
                        if (existCode != null)
                        {
                            throw new Exception($"Duplicate code found: {employeeModel.Code}");
                        }

                        var employeeNew = new Employee
                        {
                            PhoneNumber = employeeModel.PhoneNumber,
                            Address = employeeModel.Address,
                            Name = employeeModel.Name,
                            FarmId = employeeModel.FarmId,
                            Gender = employeeModel.Gender,
                            Code = employeeModel.Code,
                            Status = 1,
                            DateOfBirth = employeeModel.DateOfBirth,
                            Avatar = employeeModel.ImageUrl
                        };

                        foreach (var taskTypeId in employeeModel.TaskTypeIds)
                        {
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
                    }

                    await _unitOfWork.RepositoryEmployee.Commit();
                    _unitOfWork.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception($"Error during employee import: {ex.Message}");
            }
        }

        public async Task<byte[]> ExportEmployeesToExcel(int farmId)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                worksheet.Cells[1, 1].Value = "Mã nhân viên";
                worksheet.Cells[1, 2].Value = "Họ tên";
                worksheet.Cells[1, 3].Value = " Số điện thoại";
                worksheet.Cells[1, 4].Value = "Địa chỉ";
                worksheet.Cells[1, 5].Value = "Trang trại";
                worksheet.Cells[1, 6].Value = "Giới tính";
                worksheet.Cells[1, 7].Value = "Ngày sinh";
                worksheet.Cells[1, 8].Value = "Kỹ năng";
                worksheet.Cells[1, 9].Value = "Hình ảnh";

                var employees = await _unitOfWork.RepositoryEmployee.GetData(e => e.FarmId == farmId && e.Status == 1);

                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.Code;
                    worksheet.Cells[row, 2].Value = employee.Name;
                    worksheet.Cells[row, 3].Value = employee.PhoneNumber;
                    worksheet.Cells[row, 4].Value = employee.Address;

                    var farm = await _unitOfWork.RepositoryFarm.GetById(farmId);
                    worksheet.Cells[row, 5].Value = farm.Name;

                    var gender = (bool)employee.Gender ? EmployeeGenderEnum.Male : EmployeeGenderEnum.Female;
                    var genderString = GetGenderDescription(gender);
                    worksheet.Cells[row, 6].Value = genderString;
                    worksheet.Cells[row, 7].Value = employee.DateOfBirth;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "yyyy-mm-dd";

                    var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetData(et => et.EmployeeId == employee.Id);
                    var taskTypeIds = employee_taskType.Select(t => t.TaskTypeId).ToList();
                    var taskTypeOfEmployee = await _unitOfWork.RepositoryTaskTaskType.GetData(et => taskTypeIds.Contains(et.Id));
                    var taskTypeName = taskTypeOfEmployee.Select(t => t.Name).ToList();
                    worksheet.Cells[row, 8].Value = string.Join(",", taskTypeName);

                    worksheet.Cells[row, 9].Value = employee.Avatar;

                    row++;
                }

                return package.GetAsByteArray();
            }
        }
        public static string GetGenderDescription(EmployeeGenderEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }


        private async Task<string> UploadImageToFirebaseAsync(Employee employee, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
                };

                string uniqueIdentifier = Guid.NewGuid().ToString();
                string fileName = $"{employee.Id}_{uniqueIdentifier}";
                string fileExtension = Path.GetExtension(imageFile.FileName);

                var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                    .Child("images")
                    .Child("EmployeeAvatar")
                    .Child(fileName + fileExtension);

                using (var stream = imageFile.OpenReadStream())
                {
                    await firebaseStorage.PutAsync(stream);
                }

                var imageUrl = await firebaseStorage.GetDownloadUrlAsync();

                return imageUrl;
            }
            else
            {
                return "default_image_url";
            }
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
