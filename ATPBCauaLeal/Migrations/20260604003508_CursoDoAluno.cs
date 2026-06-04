using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATPBCauaLeal.Migrations
{
    /// <inheritdoc />
    public partial class CursoDoAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CursoId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CursoId",
                table: "AspNetUsers",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Cursos_CursoId",
                table: "AspNetUsers",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Cursos_CursoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CursoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "AspNetUsers");
        }
    }
}
