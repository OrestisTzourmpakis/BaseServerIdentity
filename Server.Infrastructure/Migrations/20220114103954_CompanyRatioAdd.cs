using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    public partial class CompanyRatioAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsRatio",
                table: "Companies");

            migrationBuilder.AddColumn<double>(
                name: "EuroToPointsRatio",
                table: "Companies",
                type: "float",
                nullable: false,
                defaultValue: 0.20000000000000001);

            migrationBuilder.AddColumn<double>(
                name: "PointsToEuroRatio",
                table: "Companies",
                type: "float",
                nullable: false,
                defaultValue: 0.001);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EuroToPointsRatio",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PointsToEuroRatio",
                table: "Companies");

            migrationBuilder.AddColumn<double>(
                name: "PointsRatio",
                table: "Companies",
                type: "float",
                nullable: false,
                defaultValue: 1.0);
        }
    }
}
