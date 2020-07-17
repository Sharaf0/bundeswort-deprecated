using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Scraper
{
    public class VideosDbContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<Caption> Captions { get; set; }
        public VideosDbContext(DbContextOptions<VideosDbContext> options) : base(options) { }
    }
    public class Video
    {
        [Key]
        public string VideoId { get; set; }
        [Required]
        public string Language { get; set; }
    }
    public class Caption
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public double Start { get; set; }
        [Required]
        public double Duration { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string VideoId { get; set; }
        public Video Video { get; set; }
    }
}