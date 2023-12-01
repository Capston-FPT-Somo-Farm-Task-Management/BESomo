using AutoMapper;
using AutoMapper.Execution;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.City;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.SubTask;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            var farm = await _unitOfWork.RepositoryFarm.GetById(employee.FarmId);
            var status = (EnumStatus)employee.Status;
            var statusString = status == EnumStatus.Active ? "Active" : "Inactive";
            var employeeModel = new EmployeeListModel
            {
                Name = employee.Name,
                Status = statusString,
                PhoneNumber = employee.PhoneNumber,
                FarmId = employee.FarmId,
                FarmName = farm.Name,
                Address = employee.Address,
                Avatar = employee.Avatar,
                Code = employee.Code,
                Id = employee.Id
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

                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        throw new Exception("Tài liệu không hợp lệ");
                    }
                    for (int row = 4; row <= rowCount; row++)
                    {
                        var taskTypeIdsString = worksheet.Cells[row, 7].Value?.ToString();

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
                            Gender = worksheet.Cells[row, 5].Value?.ToString(),
                            TaskTypeIds = taskTypeIds,
                            ImageUrl = worksheet.Cells[row, 8].Value?.ToString()
                        };
                        object dateOfBirthValue = worksheet.Cells[row, 6].Value;
                        DateTime dateOfBirth;

                        if (dateOfBirthValue != null && double.TryParse(dateOfBirthValue.ToString(), out double dateAsDouble))
                        {
                            dateOfBirth = DateTime.FromOADate(dateAsDouble);
                        }
                        else
                        {
                            throw new Exception("Invalid date format in Excel.");
                        }

                        employeeModel.DateOfBirth = dateOfBirth;

                        var existCode = await _unitOfWork.RepositoryEmployee.GetSingleByCondition(a => a.Code == employeeModel.Code);

                        if (existCode != null)
                        {
                            existCode.PhoneNumber = employeeModel.PhoneNumber;
                            existCode.Address = employeeModel.Address;
                            existCode.Name = employeeModel.Name;
                            existCode.Gender = string.Equals(worksheet.Cells[row, 5].Value?.ToString(), "Nam", StringComparison.OrdinalIgnoreCase);
                            existCode.Status = 1;
                            existCode.DateOfBirth = employeeModel.DateOfBirth;
                            existCode.Avatar = employeeModel.ImageUrl;

                            _unitOfWork.RepositoryEmployee.Update(existCode);

                            _unitOfWork.RepositoryEmployee_TaskType.Delete(expression: t => t.EmployeeId == existCode.Id);
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
                                    EmployeeId = existCode.Id,
                                    TaskTypeId = taskType.Id,
                                    Status = true,
                                };

                                existCode.Employee_TaskTypes.Add(employee_TaskType);
                            }
                            await _unitOfWork.RepositoryEmployee.Commit();
                        }
                        else
                        {
                            var employeeNew = new Employee
                            {
                                PhoneNumber = employeeModel.PhoneNumber,
                                Address = employeeModel.Address,
                                Name = employeeModel.Name,
                                FarmId = 1,
                                Gender = string.Equals(worksheet.Cells[row, 5].Value?.ToString(), "Nam", StringComparison.OrdinalIgnoreCase),
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
                //var httpClient = new HttpClient();
                //var apiResponseCity = await httpClient.GetStringAsync("https://provinces.open-api.vn/api/?depth=1");
                //var cities = JsonConvert.DeserializeObject<List<City>>(apiResponseCity);

                //var apiResponseDistricts = await httpClient.GetStringAsync("https://provinces.open-api.vn/api/d/");
                //var districts = JsonConvert.DeserializeObject<List<District>>(apiResponseDistricts);

                //var apiResponseWards = await httpClient.GetStringAsync("https://provinces.open-api.vn/api/w/");
                //var wards = JsonConvert.DeserializeObject<List<City>>(apiResponseWards);

                //List<string> provinceNames = cities.Select(city => city.Name).ToList();
                //List<string> districtNames = districts.Select(district => district.Name).ToList();
                //List<string> wardNames = wards.Select(ward => ward.Name).ToList();

                var worksheetEmployee = package.Workbook.Worksheets.Add("Employees");
                var farm = await _unitOfWork.RepositoryFarm.GetById(farmId);
                worksheetEmployee.Cells["B1:G1"].Merge = true;
                worksheetEmployee.Cells[1, 2].Value = $"Thông tin trang trại {farm.Name}";
                worksheetEmployee.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheetEmployee.Cells[3, 1].Value = "Mã nhân viên";
                worksheetEmployee.Cells[3, 2].Value = "Họ tên";
                worksheetEmployee.Cells[3, 3].Value = "Số điện thoại";
                worksheetEmployee.Cells[3, 4].Value = "Địa chỉ";
                //worksheetEmployee.Cells[3, 5].Value = "Tỉnh";
                //worksheetEmployee.Cells[3, 6].Value = "Huyện";
                //worksheetEmployee.Cells[3, 5].Value = "Xã";
                worksheetEmployee.Cells[3, 5].Value = "Giới tính";
                worksheetEmployee.Cells[3, 6].Value = "Ngày sinh";
                worksheetEmployee.Cells[3, 7].Value = "Mã kỹ năng";
                worksheetEmployee.Cells[3, 8].Value = "Hình ảnh";

                var employees = await _unitOfWork.RepositoryEmployee.GetData(e => e.FarmId == farmId && e.Status == 1);

                int row = 4;
                foreach (var employee in employees)
                {
                    worksheetEmployee.Cells[row, 1].Value = employee.Code;
                    worksheetEmployee.Cells[row, 2].Value = employee.Name;
                    worksheetEmployee.Cells[row, 3].Value = employee.PhoneNumber;
                    worksheetEmployee.Cells[row, 4].Value = employee.Address;

                    var gender = (bool)employee.Gender ? EmployeeGenderEnum.Female : EmployeeGenderEnum.Male;
                    var genderString = GetGenderDescription(gender);
                    worksheetEmployee.Cells[row, 5].Value = genderString;
                    worksheetEmployee.Cells[row, 6].Value = employee.DateOfBirth;
                    worksheetEmployee.Cells[row, 6].Style.Numberformat.Format = "yyyy-mm-dd";

                    var employee_taskType = await _unitOfWork.RepositoryEmployee_TaskType.GetData(et => et.EmployeeId == employee.Id);
                    var taskTypeIds = employee_taskType.Select(t => t.TaskTypeId).ToList();
                    worksheetEmployee.Cells[row, 7].Value = string.Join(",", taskTypeIds);

                    worksheetEmployee.Cells[row, 8].Value = employee.Avatar;

                    row++;
                }
                worksheetEmployee.Cells.AutoFitColumns();

                var worksheetTaskType = package.Workbook.Worksheets.Add("TaskTypes");
                worksheetTaskType.Cells[1, 1].Value = "STT";
                worksheetTaskType.Cells[1, 2].Value = "Mã";
                worksheetTaskType.Cells[1, 3].Value = "Tên";
                worksheetTaskType.Cells[1, 4].Value = "Loại công việc";
                worksheetTaskType.Cells[1, 5].Value = "Mô tả";

                var taskTypes = await _unitOfWork.RepositoryTaskTaskType.GetData(e => e.IsDelete == false);

                int rowTaskType = 2;
                int sequence = 1;
                taskTypes.OrderBy(t => t.Status);
                foreach (var taskType in taskTypes)
                {
                    worksheetTaskType.Cells[rowTaskType, 1].Value = sequence;
                    worksheetTaskType.Cells[rowTaskType, 2].Value = taskType.Id;
                    worksheetTaskType.Cells[rowTaskType, 3].Value = taskType.Name;
                    worksheetTaskType.Cells[rowTaskType, 4].Value = GetEnumDescription((PlantLivestockEnum)taskType.Status);
                    worksheetTaskType.Cells[rowTaskType, 5].Value = taskType.Description;

                    rowTaskType++;
                    sequence++;
                }

                worksheetTaskType.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
        public static string GetEnumDescription<T>(T enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }

        public static string GetGenderDescription(EmployeeGenderEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public async Task<byte[]> ExportEmployeesEffortToExcel(int farmId, int month, int year)
        {
            var numberOfDaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("EmployeeImportTemplate");

                worksheet.Cells["D1:N1"].Merge = true;
                worksheet.Cells[1, 4].Value = $"BẢNG GHI NHẬN GIỜ LÀM THỰC TẾ THÁNG {month} NĂM {year}";
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                using (var range = worksheet.Cells["A1:L1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 18;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                worksheet.Cells["A2:A3"].Merge = true;
                worksheet.Cells["B2:B3"].Merge = true;
                worksheet.Cells["C2:C3"].Merge = true;
                worksheet.Cells[2, 1].Value = "STT";
                worksheet.Cells[2, 2].Value = "Mã nhân viên";
                worksheet.Cells[2, 3].Value = "Họ tên";
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Merge = true;
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Value = "NGÀY CÔNG";
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                using (var range = worksheet.Cells["A2:M2"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 12;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int currentColumn = 4;
                for (int day = 1; day <= numberOfDaysInMonth; day++)
                {
                    worksheet.Cells[3, currentColumn].Value = $"{day:00}";

                    currentColumn++;
                }

                //worksheet.Row(3).Height = 35;

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 30;
                for (int col = 4; col <= numberOfDaysInMonth + 2; col++)
                {
                    worksheet.Column(col).Width = 10;
                }
                int currentColumnEffort = 4;
                var employees = await _unitOfWork.RepositoryEmployee.GetData(e => e.FarmId == farmId && e.Status == 1);
                int row = 4;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = row - 3;
                    worksheet.Cells[row, 2].Value = employee.Code;
                    worksheet.Cells[row, 3].Value = employee.Name;

                    var totalEffortList = await GetTotalEffortEmployee(employee.Id, month, year);

                    foreach (var totalEffort in totalEffortList)
                    {
                        worksheet.Cells[row, currentColumnEffort].Value = totalEffort.EffortHour;
                        currentColumnEffort++;
                    }
                    currentColumnEffort = 4;
                    row++;
                }
                worksheet.View.FreezePanes(5, 4);
                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> ExportEmployeeEffort(int emplopyeeId, int month, int year)
        {
            var numberOfDaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            using (var package = new ExcelPackage())
            {
                var employee = await _unitOfWork.RepositoryEmployee.GetById(emplopyeeId);
                var worksheet = package.Workbook.Worksheets.Add("EmployeeImportTemplate");

                worksheet.Cells["D1:N1"].Merge = true;
                worksheet.Cells[1, 4].Value = $"BẢNG GHI NHẬN GIỜ LÀM CỦA THÁNG {month} NĂM {year} CỦA NHÂN VIÊN {employee.Name}";
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                using (var range = worksheet.Cells["A1:L1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 18;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                worksheet.Cells["A2:A3"].Merge = true;
                worksheet.Cells["B2:B3"].Merge = true;
                worksheet.Cells["C2:C3"].Merge = true;
                worksheet.Cells[2, 1].Value = "STT";
                worksheet.Cells[2, 2].Value = "Mã nhiệm vụ";
                worksheet.Cells[2, 3].Value = "Tên nhiệm vụ";
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Merge = true;
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Value = "NGÀY THỰC HIỆN";
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                using (var range = worksheet.Cells["A2:M2"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 12;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int currentColumn = 4;
                for (int day = 1; day <= numberOfDaysInMonth; day++)
                {
                    worksheet.Cells[3, currentColumn].Value = $"{day:00}";

                    currentColumn++;
                }

                //worksheet.Row(3).Height = 35;

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 30;
                for (int col = 4; col <= numberOfDaysInMonth + 2; col++)
                {
                    worksheet.Column(col).Width = 10;
                }
                int currentColumnEffort = 4;
                var taskOfEmployees = await GetEffortEmployeeInTask(emplopyeeId, month, year);
                int row = 4;
                var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(t => t.EmployeeId == emplopyeeId &&
                                          t.DaySubmit.HasValue && t.DaySubmit.Value.Month == month && t.DaySubmit.Value.Year == year);

                var taskIds = subtasks.Select(t => t.TaskId);

                var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id));
                foreach (var taskInDay in tasks)
                {
                    worksheet.Cells[row, 1].Value = row - 3;
                    worksheet.Cells[row, 2].Value = taskInDay.Code;
                    worksheet.Cells[row, 3].Value = taskInDay.Name;

                    foreach (var taskOfEmployee in taskOfEmployees)
                    {
                        var effortTask = taskOfEmployee.TaskEfforts.FirstOrDefault(te => te.CodeTask == taskInDay.Code);

                        if (effortTask != null)
                        {
                            worksheet.Cells[row, currentColumnEffort].Value = effortTask.EffortHour;
                        }

                        currentColumnEffort++;
                    }

                    currentColumnEffort = 4;
                    row++;
                }


                worksheet.View.FreezePanes(5, 4);
                return package.GetAsByteArray();
            }
        }
        public async Task<List<EmployeeEffortInTask>> GetEffortEmployeeInTask(int id, int month, int year)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }

            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(t => t.EmployeeId == id &&
                                                t.DaySubmit.HasValue && t.DaySubmit.Value.Month == month && t.DaySubmit.Value.Year == year);



            var totalEffortList = new List<EmployeeEffortInTask>();

            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
            {
                var subtasksOfDay = subtasks.Where(t => t.DaySubmit?.Day == day).ToList();
                var taskIds = subtasksOfDay.Select(t => t.TaskId);

                var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => taskIds.Contains(t.Id) && (t.Status == 8|| t.Status == 7));

                var tasksAndEffortsOfDay = new List<TaskEffort>();

                foreach (var subtask in subtasks)
                {
                    var taskEmployeeOfDay = tasks.FirstOrDefault(t => t.Id == subtask.TaskId);

                    if (taskEmployeeOfDay != null)
                    {
                        var taskEffort = new TaskEffort
                        {
                            CodeTask = taskEmployeeOfDay.Code,
                            EffortHour = subtask.ActualEfforMinutes / 60.0 + subtask.ActualEffortHour
                        };

                        tasksAndEffortsOfDay.Add(taskEffort);
                    }
                }

                var totalEffortEmployee = new EmployeeEffortInTask
                {
                    Day = day,
                    TaskEfforts = tasksAndEffortsOfDay
                };

                totalEffortList.Add(totalEffortEmployee);
            }
            return totalEffortList;
        }

        public async Task<List<EmployeeEffortInMonthModel>> GetTotalEffortEmployee(int id, int month, int year)
        {
            var employee = await _unitOfWork.RepositoryEmployee.GetById(id);
            if (employee == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }

            var subtasks = await _unitOfWork.RepositoryEmployee_Task.GetData(t => t.EmployeeId == id &&
                                                t.DaySubmit.HasValue && t.DaySubmit.Value.Month == month && t.DaySubmit.Value.Year == year);

            var totalEffortList = new List<EmployeeEffortInMonthModel>();

            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
            {
                var subtasksOfDay = subtasks.Where(t => t.DaySubmit?.Day == day).ToList();

                if (subtasksOfDay.Any())
                {
                    var totalEffortOfDay = 0.0;

                    foreach (var subtask in subtasksOfDay)
                    {
                        var subtaskEffort = await _unitOfWork.RepositoryEmployee_Task.GetSingleByCondition(s => s.SubtaskId == subtask.SubtaskId && s.EmployeeId == id);
                        totalEffortOfDay += subtaskEffort.ActualEfforMinutes / 60.0 + subtaskEffort.ActualEffortHour;
                    }

                    var totalEffortEmployee = new EmployeeEffortInMonthModel
                    {
                        Day = day,
                        EndDate = subtasksOfDay.Max(t => t.DaySubmit),
                        EffortHour = totalEffortOfDay
                    };

                    totalEffortList.Add(totalEffortEmployee);
                }
                else
                {
                    // If no effort on this day, add an entry with zero effort
                    var totalEffortEmployee = new EmployeeEffortInMonthModel
                    {
                        Day = day,
                        EndDate = null, // You might want to set this to an appropriate value
                        EffortHour = 0.0
                    };

                    totalEffortList.Add(totalEffortEmployee);
                }
            }

            return totalEffortList;
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

        public Task<byte[]> ExportEmployeesEffortToExcel(int farmId, DateTime startDay, DateTime endDay)
        {
            throw new NotImplementedException();
        }

        public static bool IsDifferent(object record, object request)
        {
            if (record == null || request == null)
            {
                return true;
            }

            foreach (PropertyInfo property in record.GetType().GetProperties())
            {
                object recordValue = property.GetValue(record);
                object requestValue = property.GetValue(request);

                if (recordValue != requestValue || (recordValue == null && requestValue != null) || (recordValue != null && requestValue == null))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
