using Newtonsoft.Json;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Reponse;

namespace SomoTaskManagement.Api.ExceptionModelState
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 400)
            {
                var errorResponse = new ApiResponseModel
                {
                    Message = "Lỗi khi nhập",
                    Success = false
                };

                var json = JsonConvert.SerializeObject(errorResponse);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
        }
    }
}
