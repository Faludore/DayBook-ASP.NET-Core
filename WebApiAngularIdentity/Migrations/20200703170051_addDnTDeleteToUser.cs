using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiAngularIdentity.Migrations
{
    public partial class addDnTDeleteToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DnTDelete",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DnTDelete",
                table: "AspNetUsers");
        }
    }
}
