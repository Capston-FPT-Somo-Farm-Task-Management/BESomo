using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvidenceImageController : ControllerBase
    {
        private readonly IEvidenceImageService _evidenceImageService;
        private readonly IConfiguration _configuration;

        public EvidenceImageController(IEvidenceImageService evidenceImageService, IConfiguration configuration)
        {
            _evidenceImageService = evidenceImageService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _evidenceImageService.ListEvidenceImage());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound("EvidenceImage is not found");
                }
                var area = await _evidenceImageService.GetEvidenceImage(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "EvidenceImage is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EvidenceImageModel evidenceImageModel)
        {
            try
            {
                var response = new ApiResponseModel();
                var imageEvidecne = new EvidenceImage
                {
                    Name = evidenceImageModel.Name,
                    TaskEvidenceId = evidenceImageModel.TaskEvidenceId,
                };
                if (ModelState.IsValid)
                {
                    if (evidenceImageModel.ImageFile != null && evidenceImageModel.ImageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        string fileExtension = Path.GetExtension(evidenceImageModel.ImageFile.FileName);

                        var options = new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
                        };

                        var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                            .Child("images")
                            .Child(fileName + fileExtension);

                        await firebaseStorage.PutAsync(evidenceImageModel.ImageFile.OpenReadStream());

                        string imageUrl = await firebaseStorage.GetDownloadUrlAsync();

                        imageEvidecne.ImageUrl = imageUrl;
                    }
                    await _evidenceImageService.AddEvidenceImage(imageEvidecne);
                    var responseData = new ApiResponseModel
                    {
                        Data = imageEvidecne,
                        Message = "EvidenceImage is added",
                        Success = true,
                    };
                    return Ok(responseData);
                }
                else
                {
                    var errorMessages = new List<string>();
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errorMessages.Add(modelError.ErrorMessage);
                    }

                    response.Message = "Invalid EvidenceImage data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EvidenceImage evidenceImage)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _evidenceImageService.GetEvidenceImage(id);
                    if (existingArea == null)
                    {
                        response.Message = "Farm not found";
                        return NotFound(response);
                    }
                    await _evidenceImageService.UpdateEvidenceImage(evidenceImage);
                    var responseData = new ApiResponseModel
                    {
                        Data = evidenceImage,
                        Message = "EvidenceImage is updated",
                        Success = true,
                    };
                    return Ok(responseData);
                }
                else
                {
                    var errorMessages = new List<string>();
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errorMessages.Add(modelError.ErrorMessage);
                    }

                    response.Message = "Invalid EvidenceImage data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = new ApiResponseModel();
                var existingArea = await _evidenceImageService.GetEvidenceImage(id);
                if (existingArea == null)
                {
                    response.Message = "EvidenceImage not found";
                    return NotFound(response);
                }

                await _evidenceImageService.DeleteEvidenceImage(existingArea);
                response.Message = "EvidenceImage is deleted";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
