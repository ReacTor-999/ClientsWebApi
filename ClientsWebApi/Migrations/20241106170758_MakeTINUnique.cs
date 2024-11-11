using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientsWebApi.Migrations
{
    public partial class MakeTINUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Founders_INN",
                table: "Founders",
                column: "TIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_INN",
                table: "Clients",
                column: "TIN",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Founders_INN",
                table: "Founders");

            migrationBuilder.DropIndex(
                name: "IX_Clients_INN",
                table: "Clients");
        }
    }
}
