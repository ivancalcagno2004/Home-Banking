using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBanking.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyIsGiftClimed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGiftClaimed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGiftClaimed",
                table: "Users");
        }
    }
}
