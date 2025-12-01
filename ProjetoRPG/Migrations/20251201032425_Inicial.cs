using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoRPG.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Missoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    Dificuldade = table.Column<int>(type: "INTEGER", nullable: false),
                    RecompensaOuro = table.Column<int>(type: "INTEGER", nullable: false),
                    DataLimite = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Classe = table.Column<int>(type: "INTEGER", nullable: false),
                    Nivel = table.Column<int>(type: "INTEGER", nullable: false),
                    Vida = table.Column<int>(type: "INTEGER", nullable: false),
                    Mana = table.Column<int>(type: "INTEGER", nullable: false),
                    Moral = table.Column<int>(type: "INTEGER", nullable: false),
                    PatenteGuilda = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonagensMissoes",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(type: "INTEGER", nullable: false),
                    MissaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    FuncaoNoGrupo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    DataAtribuicao = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagensMissoes", x => new { x.PersonagemId, x.MissaoId });
                    table.ForeignKey(
                        name: "FK_PersonagensMissoes_Missoes_MissaoId",
                        column: x => x.MissaoId,
                        principalTable: "Missoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagensMissoes_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonagensMissoes_MissaoId",
                table: "PersonagensMissoes",
                column: "MissaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonagensMissoes");

            migrationBuilder.DropTable(
                name: "Missoes");

            migrationBuilder.DropTable(
                name: "Personagens");
        }
    }
}
