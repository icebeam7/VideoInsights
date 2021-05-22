using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VideoInsights.Api.Helpers;
using VideoInsights.Api.Services;
using VideoInsights.Api.Contexts;

using Microsoft.EntityFrameworkCore;

namespace VideoInsights.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VideoInsights.Api", Version = "v1" });
            });

            services.AddSingleton<IVideoIndexerService, VideoIndexerService>();

            var dbConnection = Configuration.GetSection("VideoDatabase")["ConnectionString"];
            services.AddDbContext<VideoInsightsDbContext>(options =>
                options.UseNpgsql(dbConnection));

            services.AddOptions<VideoIndexerOptions>()
                .Configure(Configuration.GetSection("VideoIndexer").Bind)
                .ValidateDataAnnotations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VideoInsights.Api v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}