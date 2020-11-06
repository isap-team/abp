using Microsoft.EntityFrameworkCore.Migrations;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql.Migrations.BackgroundJobs
{
	public partial class Jobs_AbpUpTo305: Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobs",
				maxLength: 40,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobQueues",
				maxLength: 40,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobQueueProcessors",
				maxLength: 40,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobExecutionLog",
				maxLength: 40,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobConcurrencyLocks",
				maxLength: 40,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobs",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 40,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobQueues",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 40,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobQueueProcessors",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 40,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobExecutionLog",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 40,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ConcurrencyStamp",
				table: "AbpJobConcurrencyLocks",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldMaxLength: 40,
				oldNullable: true);
		}
	}
}
