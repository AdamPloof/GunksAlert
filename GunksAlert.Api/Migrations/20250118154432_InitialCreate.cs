using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GunksAlert.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "AlertPeriod",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClimbableConditions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TempMin = table.Column<int>(type: "integer", nullable: false),
                    TempMax = table.Column<int>(type: "integer", nullable: false),
                    WindSpeed = table.Column<int>(type: "integer", nullable: false),
                    WindGust = table.Column<int>(type: "integer", nullable: false),
                    WindDegree = table.Column<int>(type: "integer", nullable: false),
                    Clouds = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    Pop = table.Column<int>(type: "integer", nullable: false),
                    Rain = table.Column<double>(type: "double precision", nullable: false),
                    Snow = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimbableConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crag",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    StateProvince = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyCondition",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Main = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IconDay = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IconNight = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyCondition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forecast",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CragId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TempLow = table.Column<double>(type: "double precision", nullable: false),
                    TempHigh = table.Column<double>(type: "double precision", nullable: false),
                    TempFeelsLikeDay = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindGust = table.Column<double>(type: "double precision", nullable: false),
                    WindDegree = table.Column<int>(type: "integer", nullable: false),
                    Clouds = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    Pop = table.Column<double>(type: "double precision", nullable: false),
                    Rain = table.Column<double>(type: "double precision", nullable: false),
                    Snow = table.Column<double>(type: "double precision", nullable: false),
                    DailyConditionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecast", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherHistory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CragId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TempLow = table.Column<double>(type: "double precision", nullable: false),
                    TempHigh = table.Column<double>(type: "double precision", nullable: false),
                    Clouds = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    Precipitation = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDegree = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertCriteria",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CragId = table.Column<int>(type: "integer", nullable: false),
                    ClimbableConditionsId = table.Column<int>(type: "integer", nullable: false),
                    AlertPeriodId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertCriteria_AlertPeriod_AlertPeriodId",
                        column: x => x.AlertPeriodId,
                        principalSchema: "public",
                        principalTable: "AlertPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertCriteria_ClimbableConditions_ClimbableConditionsId",
                        column: x => x.ClimbableConditionsId,
                        principalSchema: "public",
                        principalTable: "ClimbableConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertCriteria_Crag_CragId",
                        column: x => x.CragId,
                        principalSchema: "public",
                        principalTable: "Crag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertCriteria_AlertPeriodId",
                schema: "public",
                table: "AlertCriteria",
                column: "AlertPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertCriteria_ClimbableConditionsId",
                schema: "public",
                table: "AlertCriteria",
                column: "ClimbableConditionsId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertCriteria_CragId",
                schema: "public",
                table: "AlertCriteria",
                column: "CragId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertCriteria",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DailyCondition",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Forecast",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WeatherHistory",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AlertPeriod",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClimbableConditions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Crag",
                schema: "public");
        }
    }
}
