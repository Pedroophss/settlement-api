using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RetSettlementDates.Domain;
using RetSettlementDates.Domain.Abstractions;
using RetSettlementDates.Domain.Binders;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace RetSettlementDates.Api
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
            services.AddDomain();

            // File Input
            var assembly = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(assembly.Location);
            var path = Path.Combine(directory, "resources", "DataResults.txt");

            services.AddSingleton(new CsvFileBinder.Input(path, ';', CultureInfo.GetCultureInfo("pt-br")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ret Settlement Dates API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Pedro Santos",
                        Email = "pedro.hssop@gmail.com",
                        Url = new Uri("https://github.com/Pedroophss"),
                    },
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ret Settlement Dates API V1");
            });

            RunBinders(app.ApplicationServices);
        }

        public void RunBinders(IServiceProvider services)
        {
            var binders = services.GetServices<IBinderService>();
            foreach (var binder in binders)
            {
                binder.Bind(CancellationToken.None).Wait();
            }
        }
    }
}
