using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twidder.Migrations
{
    public partial class filepath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureFilePath",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureFilePath",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                schema: "Identity",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
