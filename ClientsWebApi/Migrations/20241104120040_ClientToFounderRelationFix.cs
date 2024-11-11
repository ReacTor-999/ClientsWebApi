using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientsWebApi.Migrations
{
    public partial class ClientToFounderRelationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Founders_Clients_ClientId",
                table: "Founders");

            migrationBuilder.DropIndex(
                name: "IX_Founders_ClientId",
                table: "Founders");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Founders");

            migrationBuilder.CreateTable(
                name: "ClientFounder",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "int", nullable: false),
                    FoundersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFounder", x => new { x.ClientsId, x.FoundersId });
                    table.ForeignKey(
                        name: "FK_ClientFounder_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientFounder_Founders_FoundersId",
                        column: x => x.FoundersId,
                        principalTable: "Founders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientFounder_FoundersId",
                table: "ClientFounder",
                column: "FoundersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientFounder");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Founders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Founders_ClientId",
                table: "Founders",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Founders_Clients_ClientId",
                table: "Founders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
