using Microsoft.EntityFrameworkCore.Migrations;

namespace PDMF.Data.Migrations
{
    public partial class AddTypeToDataset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "dataset",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "dataset");
        }
    }
}
