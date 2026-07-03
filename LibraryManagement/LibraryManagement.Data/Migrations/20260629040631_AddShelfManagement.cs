using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShelfManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "BookCopies");

            migrationBuilder.AddColumn<Guid>(
                name: "ShelfSlotId",
                table: "BookCopies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    FloorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    FloorNumber = table.Column<int>(type: "int", nullable: false),
                    FloorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.FloorId);
                });

            migrationBuilder.CreateTable(
                name: "Bookshelves",
                columns: table => new
                {
                    BookshelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    FloorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShelfCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookshelves", x => x.BookshelfId);
                    table.ForeignKey(
                        name: "FK_Bookshelves_Floors",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "FloorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookshelfCategories",
                columns: table => new
                {
                    BookshelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookshelfCategories", x => new { x.BookshelfId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_BookshelfCategories_Bookshelves",
                        column: x => x.BookshelfId,
                        principalTable: "Bookshelves",
                        principalColumn: "BookshelfId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookshelfCategories_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shelves",
                columns: table => new
                {
                    ShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    BookshelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShelfNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelves", x => x.ShelfId);
                    table.ForeignKey(
                        name: "FK_Shelves_Bookshelves",
                        column: x => x.BookshelfId,
                        principalTable: "Bookshelves",
                        principalColumn: "BookshelfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShelfSlots",
                columns: table => new
                {
                    SlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    ShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShelfSlots", x => x.SlotId);
                    table.ForeignKey(
                        name: "FK_ShelfSlots_Shelves",
                        column: x => x.ShelfId,
                        principalTable: "Shelves",
                        principalColumn: "ShelfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_ShelfSlotId",
                table: "BookCopies",
                column: "ShelfSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfCategories_CategoryId",
                table: "BookshelfCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "UQ_Bookshelves_FloorShelfCode",
                table: "Bookshelves",
                columns: new[] { "FloorId", "ShelfCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Floors_FloorNumber",
                table: "Floors",
                column: "FloorNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ShelfSlots_ShelfSlotCode",
                table: "ShelfSlots",
                columns: new[] { "ShelfId", "SlotCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Shelves_BookshelfShelfNumber",
                table: "Shelves",
                columns: new[] { "BookshelfId", "ShelfNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_ShelfSlots",
                table: "BookCopies",
                column: "ShelfSlotId",
                principalTable: "ShelfSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_ShelfSlots",
                table: "BookCopies");

            migrationBuilder.DropTable(
                name: "BookshelfCategories");

            migrationBuilder.DropTable(
                name: "ShelfSlots");

            migrationBuilder.DropTable(
                name: "Shelves");

            migrationBuilder.DropTable(
                name: "Bookshelves");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_ShelfSlotId",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "ShelfSlotId",
                table: "BookCopies");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "BookCopies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
