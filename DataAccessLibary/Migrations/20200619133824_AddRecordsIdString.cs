using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiAngularIdentity.Migrations
{
    public partial class AddRecordsIdString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_AspNetUsers_UserId1",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_UserId1",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Records");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Records",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Records_UserId",
                table: "Records",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_AspNetUsers_UserId",
                table: "Records",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_AspNetUsers_UserId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_UserId",
                table: "Records");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Records",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Records",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Records_UserId1",
                table: "Records",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_AspNetUsers_UserId1",
                table: "Records",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
