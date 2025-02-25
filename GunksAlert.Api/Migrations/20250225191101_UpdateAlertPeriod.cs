using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GunksAlert.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAlertPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DaysOfWeek",
                schema: "public",
                table: "AlertPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Months",
                schema: "public",
                table: "AlertPeriod",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                schema: "public",
                table: "AlertPeriod");

            migrationBuilder.DropColumn(
                name: "Months",
                schema: "public",
                table: "AlertPeriod");
        }
    }
}
