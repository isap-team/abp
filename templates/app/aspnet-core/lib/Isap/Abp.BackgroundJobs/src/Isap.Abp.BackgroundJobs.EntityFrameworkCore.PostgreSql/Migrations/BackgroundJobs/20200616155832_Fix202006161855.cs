using Microsoft.EntityFrameworkCore.Migrations;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql.Migrations.BackgroundJobs
{
    public partial class Fix202006161855 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyKey",
                table: "AbpJobs",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyKey",
                table: "AbpJobs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);
        }
    }
}
