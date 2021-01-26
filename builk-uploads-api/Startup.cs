using builk_uploads_api.DataContext;
using builk_uploads_api.FileData.Repositories;
using builk_uploads_api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace builk_uploads_api
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
            services.AddScoped<DataRepository>();


            string conexion = Configuration["ConnectionStrings:FileDataUploadDB"].ToString();
            services.AddDbContext<DataConfigContext>(options =>
            {
                options.UseSqlServer(conexion);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Configuration.GetSection("Cors").GetSection("AllowOrigins").Get<string[]>()).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                var formats = Configuration.GetSection("AppSettings").GetSection("AllowedFileFormats").Get<string[]>();
                options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Data Export API",
                    Version = "v1",
                    Description = $"This API allows data uploads by sending information on {string.Join(",", formats)} files format only.",
                    Contact = new OpenApiContact
                    {
                        Email = "",
                        Name = "Johanny vivas",
                    }

                });
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization by x-api-key inside request's header",
                    Scheme = "ApiKeyScheme"
                });
            });

            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("./v1/swagger.json", "Export API V1");
            });

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
