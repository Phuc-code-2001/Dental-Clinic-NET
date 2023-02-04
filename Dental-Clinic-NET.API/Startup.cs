using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ImageProcessLayer.Services;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Services.Users;
using Microsoft.AspNetCore.OData;
using RealTimeProcessLayer.Services;
using Dental_Clinic_NET.API.Services;
using FileProcessorServices;
using Dental_Clinic_NET.API.Services.Appointments;
using MailServices;
using AutoMapper;
using Dental_Clinic_NET.API.AutoMapperProfiles;
using System.Linq;
using System.Reflection;
using Twilio;
using PhoneVerifyServices;

namespace Dental_Clinic_NET.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddDbContext<AppDbContext>();

            services.AddIdentityCore<BaseUser>(options =>
            {
                // options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<AppDbContext>();

            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;


                // User settings
                options.User.RequireUniqueEmail = false;
            });



            services.AddTransient<DropboxServices>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpClient();
            services.AddTransient<FacebookServices>();

            services.AddTransient<UserServices>();
            services.AddTransient<AppointmentServices>();

            services.AddTransient<ImageKitServices>();
            services.AddTransient<PusherServices>();
            
            services.AddTransient<KickboxServices>();
            services.AddTransient<EmailSender>();

            services.AddTransient<ServicesManager>();

            services.AddTransient<PhoneVerifyService>();

            services.AddRouting();

            services.AddControllers()
            .AddOData(opt =>
            {
                opt.Select().Filter().Count().OrderBy().Expand();
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dental_Clinic_NET.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dental_Clinic_NET.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
