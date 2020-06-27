namespace ForexMiner.Heimdallr.UserManager.Configuration
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public static class ProblemDetailsConfiguration
    {
        public static void AddProblemDetailsExceptionHandling(this IServiceCollection services)
        {
            // ProblemDetails
            services.AddProblemDetails(setup =>
            {
                setup.Map<Data.Exceptions.ProblemDetailsException>(exception => new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{((int)exception.Status)}",
                    Title = exception.Status.ToString(),
                    Detail = exception.Message,
                    Status = (int)exception.Status
                });
            });
        }
    }
}
