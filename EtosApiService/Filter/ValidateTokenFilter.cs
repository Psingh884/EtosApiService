using EtosApiServices.Business.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EtosApiService.Filter
{
    public class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly IAuthenticationService _authenticationService;

        public ValidateTokenFilter(IAuthenticationService authenticationService)
        {
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string AppKey = string.Empty, UserName = string.Empty, Password = string.Empty;
            if (context.HttpContext.Request.Headers.TryGetValue(FirmInfo.AppKey, out var AppKeyHeader))
                AppKey = AppKeyHeader.ToString();

            if (context.HttpContext.Request.Headers.TryGetValue(FirmInfo.UserName, out var UserNameHeader))
                UserName = UserNameHeader.ToString();

            if (context.HttpContext.Request.Headers.TryGetValue(FirmInfo.Password, out var PasswordHeader))
                Password = PasswordHeader.ToString();
            try
            {
                var ValidationResponse = await _authenticationService.ValidateCompanyUserAsync(UserName, Password, AppKey);
                if (ValidationResponse != null && ValidationResponse.PlayerId > 0)
                {
                    context.HttpContext.Request.Headers[FirmInfo.ComapnyIdHeader] = Convert.ToString(ValidationResponse.CompanyId);
                    context.HttpContext.Request.Headers[FirmInfo.PlayerIdHeader] = Convert.ToString(ValidationResponse.PlayerId);
                    context.HttpContext.Request.Headers[FirmInfo.DBConnectionStringHeader] = Convert.ToString(ValidationResponse.DBConnectionString);
                }
                else
                {
                    context.Result = new ContentResult
                    {
                        Content = "Invalid UserName, Password or AppKey",
                        ContentType = "application/text",
                        StatusCode = 400
                    };
                    return;
                }
                await next();
            }
            catch (Exception ex)
            {
                context.Result = new ContentResult
                {
                    Content = "Invalid UserName, Password or AppKey",
                    ContentType = "application/text",
                    StatusCode = 400
                };
                return;
            }
        }
    }
}
