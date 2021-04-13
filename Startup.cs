using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using AutoMapper;

using TangledServices.ServicePortal.API.Managers;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(webHostEnvironment.ContentRootPath + @"\ConfigurationFiles")
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("system-reset.json", optional: true)
            .AddJsonFile("tenant-setup.json", optional: true)
            .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();
            //Configuration = configuration;

            WebHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            //services.AddSwaggerGen((options) => {
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Futuristic Service Desk API", Version = "v1" });
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v3", new OpenApiInfo { Title = "Futuristic API Library", Version = "v3" });
                c.UseInlineDefinitionsForEnums();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
                {
                    Description = "Provide a username and password to invoke the \"/api/system/user/login\" API",
                    Name = "Authenticate",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "BasicAuth", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "basic",
                            Name = "Basic auth requirement",
                            In = ParameterLocation.Header,
                        },new List<string>()
                    }
                });

                c.AddSecurityDefinition("JWT Bearer", new OpenApiSecurityScheme
                {
                    Description = "Provide a JWT to be sent in the Authorization header using the Bearer scheme for all \"locked\" APIs.",
                    Name = "JWT Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "JWT Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "oauth2", 
                            Name = "JWT Bearer",
                            In = ParameterLocation.Header,
                        },new List<string>()
                    }
                });
            });

            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = new HeaderApiVersionReader("x-version");
            });

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddControllersAsServices();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());    //  Makes enums display string values opposed to associated numeric value.
                //options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            //  https://wildermuth.com/2018/04/10/Using-JwtBearer-Authentication-in-an-API-only-ASP-NET-Core-Project
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(a =>
            {
                a.RequireHttpsMetadata = false;
                a.SaveToken = true;
                a.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "tangled.services",
                    ValidAudience = "tangled.services",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:secretKey"])),
                    ClockSkew = TimeSpan.Zero,
                };
            });

            //services.AddControllers();

            services.AddSingleton<IWebHostEnvironment>(WebHostEnvironment);

            services.AddSingleton<IConfiguration>(Configuration);

            //var profile = string.Empty;
            //switch (WebHostEnvironment.IsDevelopment())
            //{
            //    case false:
            //        profile = "TangledServices.ServicePortal.CosmosDb";
            //        break;
            //    default:
            //        profile = "Localhost.Tenants.CosmosDb";
            //        break;
            //}
            //services.AddSingleton<ITenantService>(TenantFactoryAsync(Configuration.GetSection(profile)).GetAwaiter().GetResult());

            services.AddHttpContextAccessor();

            //  Cosmos DB services.
            services.AddSingleton<ICosmosDbService, CosmosDbService>();

            //  Cosmos DB managers.
            services.AddSingleton<ICosmosDbManager, CosmosDbManager>();

            //  Security services.
            services.AddSingleton<IHashingService, HashingService>();

            //  System services.
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<ISystemTenantsService, SystemTenantsService>();
            //services.AddSingleton<ISystemTenantRegistrationService, SystemTenantRegistrationService>();
            services.AddSingleton<ISystemLookupItemService, SystemLookupItemService>();
            services.AddSingleton<ISystemSubscriptionService, SystemSubscriptionService>();
            services.AddSingleton<ISystemUsersService, SystemUsersService>();

            //  System managers.
            services.AddSingleton<ISystemManager, SystemManager>();
            services.AddSingleton<ISystemLookupItemManager, SystemLookupItemManager>();
            services.AddSingleton<ISystemSubscriptionManager, SystemSubscriptionManager>();
            services.AddSingleton<ISystemUsersManager, SystemUsersManager>();
            services.AddSingleton<ISystemTenantsManager, SystemTenantsManager>();

            //  Tenant services.
            services.AddSingleton<ITenantService, TenantService>();
            services.AddSingleton<ITenantSetupService, TenantSetupService>();
            services.AddSingleton<ITenantLookupGroupService, TenantLookupGroupService>();
            services.AddSingleton<ITenantLookupItemService, TenantLookupItemService>();
            services.AddSingleton<ITenantSubscriptionService, TenantSubscriptionService>();
            services.AddSingleton<ITenantUserService, TenantUserService>();

            //  Tenant managers.
            services.AddSingleton<ITenantManager, TenantManager>();
            services.AddSingleton<ITenantLookupItemManager, TenantLookupItemManager>();
            services.AddSingleton<ITenantSubscriptionsManager, TenantSubscriptionsManager>();
            services.AddSingleton<ITenantUserManager, TenantUserManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //  Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v3/swagger.json", "API V3");
                //c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region Private Methods
        ///// <summary>
        ///// Initializes a Cosmos DB connection to "TangledServices.ServicePortal" database, "Subscriptions" container.
        ///// </summary>
        ///// <param name="configurationSection"></param>
        ///// <returns></returns>
        //private static async Task<TenantService> TenantFactoryAsync(IConfigurationSection configurationSection)
        //{
        //    string databaseName = configurationSection.GetSection("DatabaseName").Value;
        //    string containerName = configurationSection.GetSection("ContainerName").Value;
        //    string uri = configurationSection.GetSection("URI").Value;
        //    string primaryKey = configurationSection.GetSection("PrimaryKey").Value;
        //    string partitionKeyName = configurationSection.GetSection("PartitionKeyName").Value;
        //    CosmosClientBuilder clientBuilder = new CosmosClientBuilder(uri, primaryKey);
        //    CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
        //    TenantService cosmosDbService = new TenantService(client, databaseName, containerName);
        //    //DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        //    //await database.Database.CreateContainerIfNotExistsAsync(containerName, "/" + partitionKeyName);

        //    return cosmosDbService;
        //}
        #endregion Private Methods
    }
}
