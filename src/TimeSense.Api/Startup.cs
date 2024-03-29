﻿using System.Text.Json.Serialization;
using TimeSense.Repository.Configuration;
using TimeSense.Serialization.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeSense.Mapping.Configuration;

namespace TimeSense.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                _ => _.AddDefaultPolicy(
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                ));
            services.AddHealthChecks();
            services.AddControllers()
                .AddNewtonsoftJson()
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSwaggerGen();
            services.AddSerialization();
            services.AddRepositories(Configuration);
            services.AddMapping();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
