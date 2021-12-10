using Microsoft.EntityFrameworkCore.Migrations;

namespace PDMF.Data.Migrations
{
    public partial class RenameCompeleteDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "compelete_date",
                table: "parse_task",
                newName: "complete_date");

            migrationBuilder.RenameColumn(
                name: "compelete_date",
                table: "modeling_task",
                newName: "complete_date");

            migrationBuilder.RenameColumn(
                name: "compelete_date",
                table: "forecast_task",
                newName: "complete_date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "complete_date",
                table: "parse_task",
                newName: "compelete_date");

            migrationBuilder.RenameColumn(
                name: "complete_date",
                table: "modeling_task",
                newName: "compelete_date");

            migrationBuilder.RenameColumn(
                name: "complete_date",
                table: "forecast_task",
                newName: "compelete_date");
        }
    }
}
