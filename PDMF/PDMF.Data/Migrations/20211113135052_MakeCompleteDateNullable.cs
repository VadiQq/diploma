using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PDMF.Data.Migrations
{
    public partial class MakeCompleteDateNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "state",
                table: "AspNetUsers",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "AspNetUsers",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "complete_date",
                table: "parse_task",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "complete_date",
                table: "modeling_task",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "complete_date",
                table: "forecast_task",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "AspNetUsers",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "AspNetUsers",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "complete_date",
                table: "parse_task",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "complete_date",
                table: "modeling_task",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "complete_date",
                table: "forecast_task",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
