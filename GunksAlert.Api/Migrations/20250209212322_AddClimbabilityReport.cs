using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GunksAlert.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClimbabilityReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                schema: "public",
                table: "ClimbableConditions");

            migrationBuilder.AddColumn<int>(
                name: "CragId",
                schema: "public",
                table: "ClimbableConditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClimbabilityReport",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CragId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TempGood = table.Column<bool>(type: "boolean", nullable: false),
                    WindGood = table.Column<bool>(type: "boolean", nullable: false),
                    CloudsGood = table.Column<bool>(type: "boolean", nullable: false),
                    HumidityGood = table.Column<bool>(type: "boolean", nullable: false),
                    ChanceDry = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimbabilityReport", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClimbabilityReport",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "CragId",
                schema: "public",
                table: "ClimbableConditions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Date",
                schema: "public",
                table: "ClimbableConditions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
