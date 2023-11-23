using AutoMapper;
using OfficeOpenXml;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.TaskType;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class TaskTypeService: ITaskTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskTypeModel>> ListTaskType()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(null);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypePlant()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression:e=>e.Status ==0 && e.IsDelete == false);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeActive()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.IsDelete == false);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeLivestock()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.Status == 1&& e.IsDelete == false);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }

        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeOther()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.Status == 2 && e.IsDelete == false);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }

        public Task<TaskType> GetTaskType(int id)
        {
            return _unitOfWork.RepositoryTaskTaskType.GetById(id);
        }
        public async Task AddTaskType(TaskTypeCreateUpdateModel taskType)
        {
            var taskTypeNew = new TaskType
            {
                Name = taskType.Name,
                Description = taskType.Description,
                Status = taskType.Status,
                IsDelete = false
            };
            await _unitOfWork.RepositoryTaskTaskType.Add(taskTypeNew);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
        public async Task UpdateTaskType(int taskTypeId, TaskTypeCreateUpdateModel taskType)
        {
            var taskTypeUpdate = await  _unitOfWork.RepositoryTaskTaskType.GetById(taskTypeId) ?? throw new Exception("Không tìm thấy loại công việc");
            taskTypeUpdate.Name = taskType.Name;
            taskTypeUpdate.Description = taskType.Description;
            taskTypeUpdate.Status = taskType.Status;

            //_unitOfWork.RepositoryTaskTaskType.Update(taskType);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
        public async Task DeleteTaskType(int taskTypeId)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(taskTypeId) ?? throw new Exception("Không tìm thấy công việc");
            var taskTypeOfTask = await _unitOfWork.RepositoryFarmTask.GetData(t => t.TaskTypeId == taskTypeId);
            var taskTypeOfEmployee = await _unitOfWork.RepositoryEmployee_TaskType.GetData(t => t.TaskTypeId == taskTypeId);
            if(taskTypeOfEmployee != null || taskTypeOfTask != null)
            {
                throw new Exception("Không thể xóa loại công việc đang được sử dụng.");
            }
            _unitOfWork.RepositoryTaskTaskType.Delete(a => a.Id == taskTypeId);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }

        public async Task<byte[]> ExportTaskTypeToExcel()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaskTypes");

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã";
                worksheet.Cells[1, 3].Value = "Tên";
                worksheet.Cells[1, 4].Value = "Loại công việc";
                worksheet.Cells[1, 5].Value = "Mô tả";

                var taskTypes = await _unitOfWork.RepositoryTaskTaskType.GetData(e => e.IsDelete == false);

                int row = 2;
                int sequence = 1; 

                foreach (var taskType in taskTypes)
                {
                    worksheet.Cells[row, 1].Value = sequence;
                    worksheet.Cells[row, 2].Value = taskType.Id;
                    worksheet.Cells[row, 3].Value = taskType.Name;
                    worksheet.Cells[row, 4].Value = GetEnumDescription((PlantLivestockEnum)taskType.Status);
                    worksheet.Cells[row, 5].Value = taskType.Description;

                    row++;
                    sequence++;
                }

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

        public async Task ImportTaskTypeFromExcel(Stream excelFileStream)
        {
            try
            {
                using (var package = new ExcelPackage(excelFileStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var taskTypeImport = new TaskType
                        {
                            Name = worksheet.Cells[row, 3].Value?.ToString(),
                            Description = worksheet.Cells[row, 5].Value?.ToString(),
                        };

                        int excelTaskTypeId = Convert.ToInt32(worksheet.Cells[row, 2].Value);

                        var existingTaskType = await _unitOfWork.RepositoryTaskTaskType
                            .GetSingleByCondition(tt => tt.Id == excelTaskTypeId);

                        if (existingTaskType == null)
                        {
                            var taskTypeNew = new TaskType
                            {
                                Name = taskTypeImport.Name,
                                Description = taskTypeImport.Description,
                                IsDelete = false,
                            };

                            string statusText = worksheet.Cells[row, 4].Value?.ToString();
                            PlantLivestockEnum statusEnum = GetEnumValueFromDescription<PlantLivestockEnum>(statusText);
                            taskTypeNew.Status = (int)statusEnum;
                            await _unitOfWork.RepositoryTaskTaskType.Add(taskTypeNew);
                        }
                        else 
                        {
                            existingTaskType.Name = taskTypeImport.Name;
                            existingTaskType.Description = taskTypeImport.Description;

                            string statusText = worksheet.Cells[row, 4].Value?.ToString();
                            PlantLivestockEnum statusEnum = GetEnumValueFromDescription<PlantLivestockEnum>(statusText);

                            existingTaskType.Status = (int)statusEnum;


                            _unitOfWork.RepositoryTaskTaskType.Update(existingTaskType);
                        }
                    }

                    await _unitOfWork.RepositoryTaskTaskType.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during task type import: {ex.Message}");
            }
        }


        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(T)} must be an enumerated type");
            }

            foreach (var field in type.GetFields())
            {
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

                if (attribute != null && string.Equals(attribute.Description, description, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)field.GetValue(null);
                }
                else if (string.Equals(field.Name, description, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"Giá trị hợp lệ là: 'Công việc cây trồng' , 'Công việc chăn nuôi', 'Công việc khác'");

        }

        public async Task UpdateStatus(int id)
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetById(id);
            if (taskType == null)
            {
                throw new Exception("Không tìm thấy loại công việc");
            }
            taskType.IsDelete = taskType.IsDelete == true ? false : true;
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
    }
}
