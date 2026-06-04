using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATPBCauaLeal.Migrations
{
    /// <inheritdoc />
    public partial class EscolhaOrientador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrientadorId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrientadorId",
                table: "AspNetUsers",
                column: "OrientadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_OrientadorId",
                table: "AspNetUsers",
                column: "OrientadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_OrientadorId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrientadorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrientadorId",
                table: "AspNetUsers");
        }
    }
}
