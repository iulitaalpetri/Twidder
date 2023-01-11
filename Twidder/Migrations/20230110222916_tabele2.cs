using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Twidder.Migrations
{
    public partial class tabele2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friends",
                schema: "Identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendshipId = table.Column<int>(type: "int", nullable: false),
                    RequestFrom_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestTo_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    friends = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.id);
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_RequestFrom_Id",
                        column: x => x.RequestFrom_Id,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_RequestTo_Id",
                        column: x => x.RequestTo_Id,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_RequestFrom_Id",
                schema: "Identity",
                table: "Friends",
                column: "RequestFrom_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_RequestTo_Id",
                schema: "Identity",
                table: "Friends",
                column: "RequestTo_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends",
                schema: "Identity");
        }
    }
}
