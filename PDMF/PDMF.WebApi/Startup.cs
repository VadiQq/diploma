using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PDMF.Data.Utilities;
using PDMF.Data.Utilities.Configuration;
using PDMF.Data.Utilities.Configuration.Managers;
using PDMF.WebApi.Configuration;
using PDMF.WebApi.Models.Identity.Data;

namespace PDMF.WebApi
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            
            services.AddIdentityCore<PDMFUser>()
                .AddSignInManager()
                .AddEntityFrameworkStores<ApplicationContext>();
            
            services.AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = JWTOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = JWTOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = JWTOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            Configurations.InitializeConfigurationManager(new AppSettingsConfigurationManager(Configuration));
            VirtualPathMapper.Initialize(WebHostEnvironment.WebRootPath);
            
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
                        
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PDMF.WebApi", Version = "v1" });
            });
            
            DIConfiguration.Register(services);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PDMF.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
