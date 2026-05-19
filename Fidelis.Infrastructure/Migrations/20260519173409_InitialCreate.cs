using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fidelis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Cnpj = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(75)", maxLength: 75, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Cpf = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(75)", maxLength: 75, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(75)", maxLength: 75, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veterinarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Crmv = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(75)", maxLength: 75, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(75)", maxLength: 75, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Especialidade = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ClinicaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veterinarios_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Especie = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Raca = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Sexo = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    FotoUrl = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TutorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ClinicaId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pets_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pets_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comportamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Data = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comportamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comportamentos_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DataHora = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Tipo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Diagnostico = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DataRetorno = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    VeterinarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consultas_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoPesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PesoKg = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    DataMedicao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Observacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoPesos_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lembretes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Tipo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DataPrevista = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    TutorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lembretes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lembretes_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lembretes_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recomendacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Tipo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DataRecomendacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recomendacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recomendacoes_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacinacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DataAplicacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataProxima = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    VacinaAplicada = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Observacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VeterinarioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacinacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacinacoes_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vacinacoes_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vermifugacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Produto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DataAplicacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataProxima = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VeterinarioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vermifugacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vermifugacoes_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vermifugacoes_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Tipo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Resultado = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Data = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ConsultaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exames_Consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "Consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescricoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Dosagem = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Frequencia = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DuracaoDias = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Observacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConsultaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescricoes_Consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "Consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PrescricaoId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicamentos_Prescricoes_PrescricaoId",
                        column: x => x.PrescricaoId,
                        principalTable: "Prescricoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comportamentos_PetId",
                table: "Comportamentos",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_PetId",
                table: "Consultas",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_VeterinarioId",
                table: "Consultas",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_ConsultaId",
                table: "Exames",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPesos_PetId",
                table: "HistoricoPesos",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Lembretes_PetId",
                table: "Lembretes",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Lembretes_TutorId",
                table: "Lembretes",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_PrescricaoId",
                table: "Medicamentos",
                column: "PrescricaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ClinicaId",
                table: "Pets",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_TutorId",
                table: "Pets",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescricoes_ConsultaId",
                table: "Prescricoes",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recomendacoes_PetId",
                table: "Recomendacoes",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacinacoes_PetId",
                table: "Vacinacoes",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacinacoes_VeterinarioId",
                table: "Vacinacoes",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Vermifugacoes_PetId",
                table: "Vermifugacoes",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Vermifugacoes_VeterinarioId",
                table: "Vermifugacoes",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarios_ClinicaId",
                table: "Veterinarios",
                column: "ClinicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comportamentos");

            migrationBuilder.DropTable(
                name: "Exames");

            migrationBuilder.DropTable(
                name: "HistoricoPesos");

            migrationBuilder.DropTable(
                name: "Lembretes");

            migrationBuilder.DropTable(
                name: "Medicamentos");

            migrationBuilder.DropTable(
                name: "Recomendacoes");

            migrationBuilder.DropTable(
                name: "Vacinacoes");

            migrationBuilder.DropTable(
                name: "Vermifugacoes");

            migrationBuilder.DropTable(
                name: "Prescricoes");

            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Veterinarios");

            migrationBuilder.DropTable(
                name: "Tutores");

            migrationBuilder.DropTable(
                name: "Clinicas");
        }
    }
}
