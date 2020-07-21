using System;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using Scraper;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bundeswort
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

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddDbContextPool<VideosDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("MyWebApiConection"));
            });

            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options => options.Configuration = "localhost:6379");

            services.AddMvc().AddControllersAsServices();

            var client = GetElasticSearchClient(Configuration);
            services.AddSingleton(client);
        }

        private ElasticClient GetElasticSearchClient(IConfiguration configuration)
        {
            var uris = Configuration
            .GetSection("ElasticSearchConnections")
            .AsEnumerable()
            .Where(u => u.Value != null)
            .Select(u => new Uri(u.Value));

            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool).DefaultIndex("caption-index");

            var client = new ElasticClient(settings);
            // var index = await client.Indices.ExistsAsync("caption-index");
            // if (!index.Exists)
            // {
            //     var response = await client.Indices.CreateAsync("caption-index", c => c
            //                     .Map<Caption>(m => m
            //                         .AutoMap<Caption>()
            //                     )
            //                 );
            // }
            return client;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<VideosDbContext>();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            ApplicationDbInitializer.SeedVideos(Configuration, app.ApplicationServices);
        }
    }
}
