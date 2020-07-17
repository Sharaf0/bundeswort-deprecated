using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Scraper
{
    public class DocumentsDbContext : DbContext
    {
        public DbSet<VideoDocument> Documents { get; set; }
        public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : base(options) { }
    }
    public class VideoDocument
    {
        [Key]
        public string VideoId { get; set; }
        [Required]
        public string Document { get; set; }
        [Required]
        public string Language { get; set; }
    }
}