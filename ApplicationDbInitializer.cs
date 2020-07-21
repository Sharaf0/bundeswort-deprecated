using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scraper;
using System.Linq;
using Bundeswort.Controllers;

namespace Bundeswort
{
    public class ApplicationDbInitializer
    {
        public static void SeedVideos(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var clear = false;
                var context = scope.ServiceProvider.GetRequiredService<VideosDbContext>();
                var controller = scope.ServiceProvider.GetRequiredService<AddVideoController>();
                string[] videos = {
                        "YMwWxeEbKH0",
                        "Io7D-yR155g",
                        "JS_ZB3ykdhE",
                        "ZMSaEcYBixM",
                        "0BiD9V8nPCY",
                        "QlaeirHJpns",
                        "yrUlgF5q6Mo",
                        "jwhCtkssg00",
                        "UF8uR6Z6KLc",
                        "pxBQLFLei70",
                        "vsMydMDi3rI",
                        "V80-gPkpH6M",
                        "Qbel5MhtDq4",
                        "MxZpaJK74Y4",
                        "mfjGmBVAL-o",
                        "BmCTQ_mkzHU",
                        "9ofED6BInFs",
                        "jDaZu_KEMCY",
                        "zPx5N6Lh3sw",
                        "wHGqp8lz36c"
                    };
                foreach (var v in videos)
                {
                    var res = controller.AddVideo(new VideoDetails { VideoId = v, Language = "EN" }, clear).Result;
                }
            }
        }
    }
}

