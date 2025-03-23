using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GunksAlert.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConditionsReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClimbabilityReport",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "ConditionsReport",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CragId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TempFeelsLike = table.Column<double>(type: "double precision", nullable: false),
                    TempMin = table.Column<double>(type: "double precision", nullable: false),
                    TempMax = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDegree = table.Column<int>(type: "integer", nullable: false),
                    Clouds = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    EstimatedSnowpack = table.Column<double>(type: "double precision", nullable: false),
                    PreciptationDayBefore = table.Column<double>(type: "double precision", nullable: false),
                    PreciptationDayOf = table.Column<double>(type: "double precision", nullable: false),
                    ChanceDry = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionsReport", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_CragId",
                schema: "public",
                table: "Alert",
                column: "CragId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alert_Crag_CragId",
                schema: "public",
                table: "Alert",
                column: "CragId",
                principalSchema: "public",
                principalTable: "Crag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alert_Crag_CragId",
                schema: "public",
                table: "Alert");

            migrationBuilder.DropTable(
                name: "ConditionsReport",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Alert_CragId",
                schema: "public",
                table: "Alert");

            migrationBuilder.CreateTable(
                name: "ClimbabilityReport",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChanceDry = table.Column<double>(type: "double precision", nullable: false),
                    CloudsGood = table.Column<bool>(type: "boolean", nullable: false),
                    CragId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    HumidityGood = table.Column<bool>(type: "boolean", nullable: false),
                    TempGood = table.Column<bool>(type: "boolean", nullable: false),
                    WindGood = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimbabilityReport", x => x.Id);
                });
        }
    }
}
