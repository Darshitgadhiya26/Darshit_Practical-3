using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practical_3.DataAccess.Migrations
{
    public partial class removetotalfromorderitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OrderItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "OrderItems",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
