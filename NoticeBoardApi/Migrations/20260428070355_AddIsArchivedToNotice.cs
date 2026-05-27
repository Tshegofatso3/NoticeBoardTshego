using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoticeBoardApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedToNotice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Notices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Notices");
        }
    }
}
