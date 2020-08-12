using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiAngularIdentity.Migrations
{
    public partial class addInviteCodeToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "AspNetUsers",
                newName: "InviteCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InviteCode",
                table: "AspNetUsers",
                newName: "Code");
        }
    }
}
