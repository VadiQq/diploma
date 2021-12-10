using Microsoft.EntityFrameworkCore.Migrations;

namespace PDMF.Data.Migrations
{
    public partial class AddModelingtTaskIdToForecastTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelingTaskId",
                table: "forecast_task",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_forecast_task_ModelingTaskId",
                table: "forecast_task",
                column: "ModelingTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_forecast_task_modeling_task_ModelingTaskId",
                table: "forecast_task",
                column: "ModelingTaskId",
                principalTable: "modeling_task",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forecast_task_modeling_task_ModelingTaskId",
                table: "forecast_task");

            migrationBuilder.DropIndex(
                name: "IX_forecast_task_ModelingTaskId",
                table: "forecast_task");

            migrationBuilder.DropColumn(
                name: "ModelingTaskId",
                table: "forecast_task");
        }
    }
}
