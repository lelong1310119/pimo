using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PismoWebInput.Core.Infrastructure.Helpers
{
    public class IdentityProblemDetails : ProblemDetails
    {
        public IdentityProblemDetails(IList<string> errors)
        {
            Status = StatusCodes.Status400BadRequest;
            Title = "Bad Request";
            Detail = "One or more errors occurred.";
            Errors = errors;
        }

        public IList<string> Errors { get; }
    }
}
