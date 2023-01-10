using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twidder.Migrations
{
    public partial class migr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId1",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId2",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Profiles_ProfileId",
                schema: "Identity",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Profiles_ProfileId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ProfileId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ProfileId",
                schema: "Identity",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileId1",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileId2",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                schema: "Identity",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                schema: "Identity",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileId1",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileId2",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "PrivateProfile",
                schema: "Identity",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateProfile",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                schema: "Identity",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                schema: "Identity",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId1",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId2",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "Identity",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeletedByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    PrivateProfile = table.Column<bool>(type: "bit", nullable: false),
                    ProfileDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignUpDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK_Profiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ProfileId",
                schema: "Identity",
                table: "Posts",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ProfileId",
                schema: "Identity",
                table: "Groups",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileId",
                schema: "Identity",
                table: "AspNetUsers",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileId1",
                schema: "Identity",
                table: "AspNetUsers",
                column: "ProfileId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileId2",
                schema: "Identity",
                table: "AspNetUsers",
                column: "ProfileId2");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                schema: "Identity",
                table: "Profiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId",
                schema: "Identity",
                table: "AspNetUsers",
                column: "ProfileId",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId1",
                schema: "Identity",
                table: "AspNetUsers",
                column: "ProfileId1",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Profiles_ProfileId2",
                schema: "Identity",
                table: "AspNetUsers",
                column: "ProfileId2",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Profiles_ProfileId",
                schema: "Identity",
                table: "Groups",
                column: "ProfileId",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Profiles_ProfileId",
                schema: "Identity",
                table: "Posts",
                column: "ProfileId",
                principalSchema: "Identity",
                principalTable: "Profiles",
                principalColumn: "ProfileId");
        }
    }
}
