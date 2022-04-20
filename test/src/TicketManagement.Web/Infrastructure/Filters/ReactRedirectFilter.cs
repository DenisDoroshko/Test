using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using TicketManagement.Web.JwtTokenAuth;

namespace TicketManagement.Web.Infrastructure.Filters
{
    public class ReactRedirectFilterAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var featureManager = context.HttpContext.RequestServices.GetService<IFeatureManager>();
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            if (await featureManager.IsEnabledAsync("ReactFeature"))
            {
                var path = context.HttpContext.Request.Path;
                var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var redirectUrl = configuration["ReactAppAddress"] + path + $"?lang={lang}";
                var token = context.HttpContext.Request.Cookies[JwtTokenConstants.JwtCookieKey];
                if (!string.IsNullOrEmpty(token))
                {
                    redirectUrl += $"&token={token}";
                }

                context.HttpContext.Response.Redirect(redirectUrl);
            }

            await next();
        }
    }
}
