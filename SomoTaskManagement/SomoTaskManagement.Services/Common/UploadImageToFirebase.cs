using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Common
{
    public class UploadImageToFirebase
    {
        private readonly IConfiguration _configuration;

        public UploadImageToFirebase(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        public async Task<List<EvidenceImage>> UploadEvidenceImages<T>(int id, T evidenceCreateUpdateModel)
        {
            var uploadedImages = new List<EvidenceImage>();

            // Kiểm tra xem kiểu T có thuộc tính ImageFile không
            var imageFilesProperty = typeof(T).GetProperty("ImageFile");
            if (imageFilesProperty != null)
            {
                var imageFiles = (IEnumerable<IFormFile>)imageFilesProperty.GetValue(evidenceCreateUpdateModel);

                foreach (var imageFile in imageFiles)
                {
                    var imageEvidence = new EvidenceImage
                    {
                        TaskEvidenceId = id,
                    };

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        string fileExtension = Path.GetExtension(imageFile.FileName);

                        var options = new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
                        };

                        var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
                            .Child("images")
                            .Child(fileName + fileExtension);

                        await firebaseStorage.PutAsync(imageFile.OpenReadStream());

                        string imageUrl = await firebaseStorage.GetDownloadUrlAsync();

                        imageEvidence.ImageUrl = imageUrl;
                    }
                    else
                    {
                        imageEvidence.ImageUrl = null;
                    }

                    uploadedImages.Add(imageEvidence);
                }
            }

            return uploadedImages;
        }

    }
}
