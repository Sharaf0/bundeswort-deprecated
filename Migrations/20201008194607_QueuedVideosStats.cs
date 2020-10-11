using Microsoft.EntityFrameworkCore.Migrations;

namespace Bundeswort.Migrations
{
    public partial class QueuedVideosStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommentCount",
                table: "QueuedVideos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DislikeCount",
                table: "QueuedVideos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FavoriteCount",
                table: "QueuedVideos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LikeCount",
                table: "QueuedVideos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ViewCount",
                table: "QueuedVideos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "QueuedVideos");

            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "QueuedVideos");

            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "QueuedVideos");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "QueuedVideos");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "QueuedVideos");
        }
    }
}
