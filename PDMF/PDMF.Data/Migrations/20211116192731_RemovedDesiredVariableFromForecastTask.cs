using Microsoft.EntityFrameworkCore.Migrations;

namespace PDMF.Data.Migrations
{
    public partial class RemovedDesiredVariableFromForecastTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredVariable",
                table: "forecast_task");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesiredVariable",
                table: "forecast_task",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
