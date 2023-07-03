using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using PismoWebInput.Core.Infrastructure.Common.Exceptions;
using PismoWebInput.Core.Infrastructure.Common.Extensions;

namespace PismoWebInput.Core.Infrastructure.Helpers
{
    public static class ProblemMapper
    {
        public static StatusCodeProblemDetails DynamicStatusCode(HttpContext context, HttpStatusCodeException e)
        {
            return new StatusCodeProblemDetails(e.StatusCode)
            {
                Detail = e.GetBaseErrorMessage(),
                Instance = context.Request.Path
            };
        }

        public static Func<HttpContext, Exception, StatusCodeProblemDetails> StatusCode(int statusCode)
        {
            return (ctx, e) => new StatusCodeProblemDetails(statusCode)
            {
                Detail = e.GetBaseErrorMessage(),
                Instance = ctx.Request.Path
            };
        }
    }
}
