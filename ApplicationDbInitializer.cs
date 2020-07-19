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
                var context = scope.ServiceProvider.GetRequiredService<VideosDbContext>();
                var controller = scope.ServiceProvider.GetRequiredService<AddVideoController>();
                if (context.Videos.Count() == 0)
                {
                    string[] videos = {
                        "YMwWxeEbKH0",
                        "Io7D-yR155g",
                        "JS_ZB3ykdhE",
                        "ZMSaEcYBixM",
                        "0BiD9V8nPCY",
                        "QlaeirHJpns",
                        "yrUlgF5q6Mo",
                        "jwhCtkssg00",
                    };
                    foreach (var v in videos)
                    {
                        var res = controller.AddVideo(new VideoDetails { VideoId = v, Language = "EN" }).Result;
                    }
                }
            }
        }
    }
}

