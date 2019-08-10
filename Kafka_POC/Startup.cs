using Confluent.Kafka;
using Kafka_POC.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Kafka_POC
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHostedService<Worker>();

            services.AddSwaggerGen(swaggerConfig =>
            {
                swaggerConfig.SwaggerDoc("v1", Configuration.GetSection("SwaggerDoc:Info").Get<Info>());
                swaggerConfig.DescribeAllEnumsAsStrings();
            });

            services.AddSingleton(Configuration.GetSection("ProducerConfig").Get<ProducerConfig>());
            services.AddSingleton(Configuration.GetSection("ConsumerConfig").Get<ConsumerConfig>());
            services.AddSingleton<KafkaConsumer>();

            services.AddScoped<KafkaProducer>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kafka_POC API V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}