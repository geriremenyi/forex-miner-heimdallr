namespace ForexMiner.Heimdallr.Common.ServiceConfiguration
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public static class ProblemDetailsConfiguration
    {
        public static void AddProblemDetailsExceptionHandling(this IServiceCollection services)
        {
            services.AddProblemDetails(setup =>
            {
                setup.Map<Data.Exceptions.ProblemDetailsException>(exception => new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{(int)exception.Status}",
                    Title = exception.Status.ToString(),
                    Detail = exception.Message,
                    Status = (int)exception.Status
                });
            });
        }

        public static void UseProblemDetails(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseProblemDetails();
        }
    }
}
