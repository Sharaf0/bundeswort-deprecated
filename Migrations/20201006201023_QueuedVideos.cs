using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bundeswort.Migrations
{
    public partial class QueuedVideos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueuedVideos",
                columns: table => new
                {
                    VideoId = table.Column<string>(nullable: false),
                    Etag = table.Column<string>(nullable: false),
                    ChannelId = table.Column<string>(nullable: false),
                    ChannelTitle = table.Column<string>(nullable: false),
                    HighThumbnail = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    VideoTitle = table.Column<string>(nullable: false),
                    PublishedAt = table.Column<DateTime>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedVideos", x => x.VideoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueuedVideos");
        }
    }
}
