namespace ForexMiner.Heimdallr.Instruments.Api.Instruments.Api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ForexMiner.Heimdallr.Instruments.Api.Configuration;
    using System.Text.Json.Serialization;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Routing
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddApiVersioning();

            // Instruments.Api services
            services.AddInstrumentsApiServices(_environment, _configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ForexMinerHeimdallrDbContext dbContext)
        {
            // Custom
            app.UseInstrumentsApiServices(_environment, dbContext);

            // System
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
