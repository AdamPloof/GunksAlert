using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GunksAlert.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClimbingConditionsFieldTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "WindSpeed",
                schema: "public",
                table: "ClimbableConditions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "WindGust",
                schema: "public",
                table: "ClimbableConditions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "TempMin",
                schema: "public",
                table: "ClimbableConditions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "TempMax",
                schema: "public",
                table: "ClimbableConditions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Pop",
                schema: "public",
                table: "ClimbableConditions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WindSpeed",
                schema: "public",
                table: "ClimbableConditions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "WindGust",
                schema: "public",
                table: "ClimbableConditions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "TempMin",
                schema: "public",
                table: "ClimbableConditions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "TempMax",
                schema: "public",
                table: "ClimbableConditions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Pop",
                schema: "public",
                table: "ClimbableConditions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
