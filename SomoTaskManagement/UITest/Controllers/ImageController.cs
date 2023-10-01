using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace UITest.Controllers
{
    public class ImageController : Controller
    {
        private readonly HttpClient client;
        private readonly string ImageUrl;

        public ImageController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ImageUrl = "https://localhost:7103/api/EvidenceImage";
        }

        public IActionResult Create()
        {
            return View();

        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Images/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EvidenceImage evidenceImage, IFormFile file)
        {

            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                string fileExtension = Path.GetExtension(file.FileName);

                var json = JsonSerializer.Serialize(evidenceImage);
                var contentImage = new StringContent(json, Encoding.UTF8, "application/json");

                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream()), "imageFile", fileName + fileExtension);
                content.Add(contentImage);

                HttpResponseMessage response = await client.PostAsync($"{ImageUrl}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index","Home");
                }
            }
            return View();
        }


    }
}
