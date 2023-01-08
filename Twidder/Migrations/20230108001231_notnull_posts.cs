using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twidder.Migrations
{
    public partial class notnull_posts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Profiles_ProfileId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                schema: "Identity",
                table: "Posts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                schema: "Identity",
                table: "Posts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupId",
                schema: "Identity",
                table: "Posts",
                column: "GroupId",
                principalSchema: "Identity",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Profiles_ProfileId",
                schema: "Identity",
                table: "Posts",
                column: "ProfileId",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Profiles_ProfileId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                schema: "Identity",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                schema: "Identity",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupId",
                schema: "Identity",
                table: "Posts",
                column: "GroupId",
                principalSchema: "Identity",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Profiles_ProfileId",
                schema: "Identity",
                table: "Posts",
                column: "ProfileId",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
