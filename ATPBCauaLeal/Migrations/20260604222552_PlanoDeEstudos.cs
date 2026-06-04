using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATPBCauaLeal.Migrations
{
    /// <inheritdoc />
    public partial class PlanoDeEstudos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanosDeEstudos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlunoId = table.Column<string>(type: "TEXT", nullable: false),
                    CursoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConcluidoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanosDeEstudos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanosDeEstudos_AspNetUsers_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanosDeEstudos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanoDeEstudosDisciplinas",
                columns: table => new
                {
                    PlanoDeEstudosId = table.Column<int>(type: "INTEGER", nullable: false),
                    DisciplinaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoDeEstudosDisciplinas", x => new { x.PlanoDeEstudosId, x.DisciplinaId });
                    table.ForeignKey(
                        name: "FK_PlanoDeEstudosDisciplinas_Disciplinas_DisciplinaId",
                        column: x => x.DisciplinaId,
                        principalTable: "Disciplinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanoDeEstudosDisciplinas_PlanosDeEstudos_PlanoDeEstudosId",
                        column: x => x.PlanoDeEstudosId,
                        principalTable: "PlanosDeEstudos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanoDeEstudosDisciplinas_DisciplinaId",
                table: "PlanoDeEstudosDisciplinas",
                column: "DisciplinaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosDeEstudos_AlunoId",
                table: "PlanosDeEstudos",
                column: "AlunoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanosDeEstudos_CursoId",
                table: "PlanosDeEstudos",
                column: "CursoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanoDeEstudosDisciplinas");

            migrationBuilder.DropTable(
                name: "PlanosDeEstudos");
        }
    }
}
