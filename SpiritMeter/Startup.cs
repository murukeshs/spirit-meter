using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using static SpiritMeter.Data.Common;

namespace SpiritMeter
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
          
                //For JWTAuthentication
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
                 //Add Authentication header in swagger
                 services.AddSwaggerGen(c =>
                 {
                     c.SwaggerDoc("v1", new Info { Title = "Spirit Meter", Description = "Swagger Core API", Version = "v1" });
                     c.DescribeAllEnumsAsStrings();
                     c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                     {
                         Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                         In = "header",
                         Name = "Authorization",
                         Type = "apiKey"
                     });

                     c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                     { "Bearer", Enumerable.Empty<string>() },
                     });

                     c.OperationFilter<FileUploadOperation>(); //Register File Upload Operation Filter

                     //Set the comments path for the Swagger JSON and UI.

                     //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                     //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                     //c.IncludeXmlComments(xmlPath);

                     //var FilePath = _env.WebRootPath + Path.DirectorySeparatorChar.ToString()
                     //            + "XmlFile"
                     //            + Path.DirectorySeparatorChar.ToString()
                     //            + "ApiDescription.xml";
                     //c.IncludeXmlComments(FilePath);

                 });

                //Enablecars for React
                 services.AddCors(options =>
                 {
                     options.AddPolicy("AllowAll",
                         builder =>
                         {
                             builder
                             .AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader();
                         });

                 });
                 services.Configure<MvcOptions>(options =>
                 {
                     options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAll"));
                 });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sprirt Meter");
            });
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
