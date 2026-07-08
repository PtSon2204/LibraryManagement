using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFineTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FineTemplates",
                columns: table => new
                {
                    FineTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FineType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Fixed"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FineTemplates", x => x.FineTemplateId);
                });

            migrationBuilder.InsertData(
                table: "FineTemplates",
                columns: new[] { "FineTemplateId", "Amount", "CreatedAt", "FineType", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("a1000001-0000-0000-0000-000000000001"), 5000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PerDay", true, "Quá hạn trả sách" },
                    { new Guid("a1000001-0000-0000-0000-000000000002"), 50000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fixed", true, "Sách bị rách bìa" },
                    { new Guid("a1000001-0000-0000-0000-000000000003"), 100000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fixed", true, "Sách bị ướt / hư hỏng nặng" },
                    { new Guid("a1000001-0000-0000-0000-000000000004"), 200000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fixed", true, "Sách bị mất" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FineTemplates");
        }
    }
}
