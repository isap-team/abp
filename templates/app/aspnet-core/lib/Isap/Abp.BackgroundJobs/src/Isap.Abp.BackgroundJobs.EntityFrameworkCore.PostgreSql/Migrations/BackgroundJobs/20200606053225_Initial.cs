using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql.Migrations.BackgroundJobs
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpJobQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpJobQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpJobConcurrencyLocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    QueueId = table.Column<Guid>(nullable: false),
                    ConcurrencyKey = table.Column<string>(maxLength: 128, nullable: false),
                    LockId = table.Column<Guid>(nullable: false),
                    LockTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpJobConcurrencyLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpJobConcurrencyLocks_AbpJobQueues_QueueId",
                        column: x => x.QueueId,
                        principalTable: "AbpJobQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpJobQueueProcessors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    NodeId = table.Column<int>(nullable: false),
                    QueueId = table.Column<Guid>(nullable: false),
                    ApplicationRole = table.Column<string>(maxLength: 128, nullable: false),
                    LastActivityTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpJobQueueProcessors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpJobQueueProcessors_AbpJobQueues_QueueId",
                        column: x => x.QueueId,
                        principalTable: "AbpJobQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    QueueId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    ArgumentsKey = table.Column<string>(maxLength: 40, nullable: false),
                    Arguments = table.Column<string>(type: "jsonb", nullable: false),
                    ConcurrencyKey = table.Column<string>(maxLength: 128, nullable: false),
                    Priority = table.Column<byte>(nullable: false, defaultValue: (byte)15),
                    State = table.Column<int>(nullable: false, defaultValue: 0),
                    TryCount = table.Column<int>(nullable: false, defaultValue: 0),
                    NextTryTime = table.Column<DateTime>(nullable: false),
                    LastTryTime = table.Column<DateTime>(nullable: true),
                    LockId = table.Column<Guid>(nullable: true),
                    LockTime = table.Column<DateTime>(nullable: true),
                    Result = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpJobs_AbpJobQueues_QueueId",
                        column: x => x.QueueId,
                        principalTable: "AbpJobQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpJobExecutionLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    JobId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    LockId = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Log = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpJobExecutionLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpJobExecutionLog_AbpJobs_JobId",
                        column: x => x.JobId,
                        principalTable: "AbpJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobConcurrencyLocks_QueueId",
                table: "AbpJobConcurrencyLocks",
                column: "QueueId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobConcurrencyLocks_TenantId_QueueId_ConcurrencyKey",
                table: "AbpJobConcurrencyLocks",
                columns: new[] { "TenantId", "QueueId", "ConcurrencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobExecutionLog_JobId",
                table: "AbpJobExecutionLog",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobQueueProcessors_QueueId",
                table: "AbpJobQueueProcessors",
                column: "QueueId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobQueues_Name",
                table: "AbpJobQueues",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobs_ArgumentsKey",
                table: "AbpJobs",
                column: "ArgumentsKey");

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobs_CreationTime",
                table: "AbpJobs",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobs_LastTryTime",
                table: "AbpJobs",
                column: "LastTryTime",
                filter: "\"State\" = 2");

            migrationBuilder.CreateIndex(
                name: "IX_AbpJobs_QueueId",
                table: "AbpJobs",
                column: "QueueId",
                filter: "\"State\" = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpJobConcurrencyLocks");

            migrationBuilder.DropTable(
                name: "AbpJobExecutionLog");

            migrationBuilder.DropTable(
                name: "AbpJobQueueProcessors");

            migrationBuilder.DropTable(
                name: "AbpJobs");

            migrationBuilder.DropTable(
                name: "AbpJobQueues");
        }
    }
}
