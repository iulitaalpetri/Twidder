using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twidder.Migrations
{
    public partial class Groups1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                schema: "Identity",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                schema: "Identity",
                table: "Groups",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                schema: "Identity",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupDescription",
                schema: "Identity",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GroupId",
                schema: "Identity",
                table: "Posts",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupId",
                schema: "Identity",
                table: "Posts",
                column: "GroupId",
                principalSchema: "Identity",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_GroupId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "GroupId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "Identity",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupDescription",
                schema: "Identity",
                table: "Groups");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                schema: "Identity",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
