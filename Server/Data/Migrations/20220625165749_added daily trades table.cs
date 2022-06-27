using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorApp1.Server.Data.Migrations
{
    public partial class addeddailytradestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company_Daily_Trades",
                columns: table => new
                {
                    IdCompany = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company_Daily_Trades", x => new { x.IdCompany, x.DateTime });
                    table.ForeignKey(
                        name: "FK_Company_Daily_Trades_Company_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Company",
                        principalColumn: "IdCompany",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Company_Daily_Trades");
        }
    }
}
