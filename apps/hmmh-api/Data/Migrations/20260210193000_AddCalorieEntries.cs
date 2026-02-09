using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hmmh.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCalorieEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the calorie entries table for per-meal logging.
            migrationBuilder.CreateTable(
                name: "CalorieEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EntryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Calories = table.Column<int>(type: "integer", nullable: false),
                    FoodName = table.Column<string>(type: "text", nullable: true),
                    PartOfDay = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalorieEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalorieEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalorieEntries_UserId_EntryDate",
                table: "CalorieEntries",
                columns: new[] { "UserId", "EntryDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the calorie entries table.
            migrationBuilder.DropTable(
                name: "CalorieEntries");
        }
    }
}
