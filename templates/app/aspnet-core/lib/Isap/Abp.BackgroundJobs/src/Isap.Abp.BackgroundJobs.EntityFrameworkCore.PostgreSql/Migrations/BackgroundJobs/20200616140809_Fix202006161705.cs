using Microsoft.EntityFrameworkCore.Migrations;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql.Migrations.BackgroundJobs
{
    public partial class Fix202006161705 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArgumentsType",
                table: "AbpJobs",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResultType",
                table: "AbpJobs",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArgumentsType",
                table: "AbpJobs");

            migrationBuilder.DropColumn(
                name: "ResultType",
                table: "AbpJobs");
        }
    }
}
