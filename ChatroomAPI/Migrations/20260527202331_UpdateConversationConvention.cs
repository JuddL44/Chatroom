using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatroomAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Conversations");
        }
    }
}
