using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceIntegration.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CheckIn = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    CheckOut = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TotalHours = table.Column<decimal>(type: "TEXT", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "TEXT", nullable: false),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Processed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_CompanyId_Date_EmployeeId",
                table: "AttendanceRecords",
                columns: new[] { "CompanyId", "Date", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "AuditLogs");
        }
    }
}
