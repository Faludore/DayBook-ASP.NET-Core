using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiAngularIdentity.Migrations
{
    public partial class RecordStringImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Records",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Records",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
