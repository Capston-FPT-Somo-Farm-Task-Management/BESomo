using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SomoTaskManagement.Domain.Model.Reponse;

namespace SomoTaskManagement.Api.ExceptionModelState
{
    public class ModelStateValidationFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var errors = GetModelStateErrors(context.ModelState);

            context.Result = new BadRequestObjectResult(new ApiResponseModel
            {
                Message = "Validation Failed: " + errors,
                Success = false
            });

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        private string GetModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = modelState.Values
              .SelectMany(x => x.Errors)
              .Select(x => x.ErrorMessage);

            return string.Join(", ", errors);
        }

    }
}
