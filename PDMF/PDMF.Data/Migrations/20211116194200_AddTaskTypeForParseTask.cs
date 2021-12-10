using Microsoft.EntityFrameworkCore.Migrations;

namespace PDMF.Data.Migrations
{
    public partial class AddTaskTypeForParseTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskType",
                table: "parse_task",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "parse_task");
        }
    }
}
