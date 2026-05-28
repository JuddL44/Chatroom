using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatroomAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Conversations");
        }
    }
}
