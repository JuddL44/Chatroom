using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatroomAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConsoleToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Console",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Console",
                table: "Messages");
        }
    }
}
