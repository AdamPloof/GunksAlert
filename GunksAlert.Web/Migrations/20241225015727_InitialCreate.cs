using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GunksAlert.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertPeriod",
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
                name: "AlertCriteria",
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
                        principalTable: "AlertPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertCriteria_ClimbableConditions_ClimbableConditionsId",
                        column: x => x.ClimbableConditionsId,
                        principalTable: "ClimbableConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertCriteria_Crag_CragId",
                        column: x => x.CragId,
                        principalTable: "Crag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Forecast",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TempLow = table.Column<int>(type: "integer", nullable: false),
                    TempHigh = table.Column<int>(type: "integer", nullable: false),
                    TempFeelsLike = table.Column<int>(type: "integer", nullable: false),
                    WindSpeed = table.Column<int>(type: "integer", nullable: false),
                    WindGust = table.Column<int>(type: "integer", nullable: false),
                    WindDegree = table.Column<int>(type: "integer", nullable: false),
                    Clouds = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    Pop = table.Column<int>(type: "integer", nullable: false),
                    Rain = table.Column<double>(type: "double precision", nullable: false),
                    Snow = table.Column<double>(type: "double precision", nullable: false),
                    DailyConditionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecast", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forecast_DailyCondition_DailyConditionId",
                        column: x => x.DailyConditionId,
                        principalTable: "DailyCondition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Temp = table.Column<int>(type: "integer", nullable: false),
                    WindSpeed = table.Column<int>(type: "integer", nullable: false),
                    WindDegree = table.Column<int>(type: "integer", nullable: false),
                    Clouds = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    Rain = table.Column<double>(type: "double precision", nullable: false),
                    Snow = table.Column<double>(type: "double precision", nullable: false),
                    DailyConditionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherHistory_DailyCondition_DailyConditionId",
                        column: x => x.DailyConditionId,
                        principalTable: "DailyCondition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertCriteria_AlertPeriodId",
                table: "AlertCriteria",
                column: "AlertPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertCriteria_ClimbableConditionsId",
                table: "AlertCriteria",
                column: "ClimbableConditionsId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertCriteria_CragId",
                table: "AlertCriteria",
                column: "CragId");

            migrationBuilder.CreateIndex(
                name: "IX_Forecast_DailyConditionId",
                table: "Forecast",
                column: "DailyConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherHistory_DailyConditionId",
                table: "WeatherHistory",
                column: "DailyConditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertCriteria");

            migrationBuilder.DropTable(
                name: "Forecast");

            migrationBuilder.DropTable(
                name: "WeatherHistory");

            migrationBuilder.DropTable(
                name: "AlertPeriod");

            migrationBuilder.DropTable(
                name: "ClimbableConditions");

            migrationBuilder.DropTable(
                name: "Crag");

            migrationBuilder.DropTable(
                name: "DailyCondition");
        }
    }
}
