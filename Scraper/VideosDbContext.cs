using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Scraper
{
    public class VideosDbContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<YoutubeChannel> Channels { get; set; }
        public DbSet<Caption> Captions { get; set; }
        public DbSet<QueuedVideo> QueuedVideos { get; set; }
        public VideosDbContext(DbContextOptions<VideosDbContext> options) : base(options) { }
    }
    public class QueuedVideo
    {
        [Key]
        public string VideoId { get; set; }
        [Required]
        public string Etag { get; set; }
        [Required]
        public string ChannelId { get; set; }
        [Required]
        public string ChannelTitle { get; set; }
        [Required]
        public string HighThumbnail { get; set; }
        public string Description { get; set; }
        [Required]
        public string VideoTitle { get; set; }
        [Required]
        public DateTime PublishedAt { get; set; }
        [Required]
        public string Language { get; set; }
    }

    public class Video
    {
        [Key]
        public string VideoId { get; set; }
        [Required]
        public string Language { get; set; }
    }
    public class YoutubeChannel
    {
        [Key]
        public string ChannelId { get; set; }
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