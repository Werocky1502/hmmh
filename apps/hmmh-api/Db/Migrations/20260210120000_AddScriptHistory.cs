using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hmmh.Api.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddScriptHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScriptHistory",
                columns: table => new
                {
                    ScriptName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    ScriptHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AppliedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptHistory", x => x.ScriptName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScriptHistory");
        }
    }
}
