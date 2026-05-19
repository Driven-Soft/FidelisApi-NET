using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fidelis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migrations_003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comportamentos_Pets_PetId",
                table: "Comportamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Pets_PetId",
                table: "Consultas");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Veterinarios_VeterinarioId",
                table: "Consultas");

            migrationBuilder.DropForeignKey(
                name: "FK_Exames_Consultas_ConsultaId",
                table: "Exames");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoPesos_Pets_PetId",
                table: "HistoricoPesos");

            migrationBuilder.DropForeignKey(
                name: "FK_Lembretes_Pets_PetId",
                table: "Lembretes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lembretes_Tutores_TutorId",
                table: "Lembretes");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicamentos_Prescricoes_PrescricaoId",
                table: "Medicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Clinicas_ClinicaId",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Tutores_TutorId",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescricoes_Consultas_ConsultaId",
                table: "Prescricoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recomendacoes_Pets_PetId",
                table: "Recomendacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacinacoes_Pets_PetId",
                table: "Vacinacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacinacoes_Veterinarios_VeterinarioId",
                table: "Vacinacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Vermifugacoes_Pets_PetId",
                table: "Vermifugacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Vermifugacoes_Veterinarios_VeterinarioId",
                table: "Vermifugacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Veterinarios_Clinicas_ClinicaId",
                table: "Veterinarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Veterinarios",
                table: "Veterinarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vermifugacoes",
                table: "Vermifugacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacinacoes",
                table: "Vacinacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tutores",
                table: "Tutores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recomendacoes",
                table: "Recomendacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescricoes",
                table: "Prescricoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pets",
                table: "Pets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medicamentos",
                table: "Medicamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lembretes",
                table: "Lembretes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exames",
                table: "Exames");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultas",
                table: "Consultas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comportamentos",
                table: "Comportamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clinicas",
                table: "Clinicas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricoPesos",
                table: "HistoricoPesos");

            migrationBuilder.RenameTable(
                name: "Veterinarios",
                newName: "VETERINARIOS");

            migrationBuilder.RenameTable(
                name: "Vermifugacoes",
                newName: "VERMIFUGACOES");

            migrationBuilder.RenameTable(
                name: "Vacinacoes",
                newName: "VACINACOES");

            migrationBuilder.RenameTable(
                name: "Tutores",
                newName: "TUTORES");

            migrationBuilder.RenameTable(
                name: "Recomendacoes",
                newName: "RECOMENDACOES");

            migrationBuilder.RenameTable(
                name: "Prescricoes",
                newName: "PRESCRICOES");

            migrationBuilder.RenameTable(
                name: "Pets",
                newName: "PETS");

            migrationBuilder.RenameTable(
                name: "Medicamentos",
                newName: "MEDICAMENTOS");

            migrationBuilder.RenameTable(
                name: "Lembretes",
                newName: "LEMBRETES");

            migrationBuilder.RenameTable(
                name: "Exames",
                newName: "EXAMES");

            migrationBuilder.RenameTable(
                name: "Consultas",
                newName: "CONSULTAS");

            migrationBuilder.RenameTable(
                name: "Comportamentos",
                newName: "COMPORTAMENTOS");

            migrationBuilder.RenameTable(
                name: "Clinicas",
                newName: "CLINICAS");

            migrationBuilder.RenameTable(
                name: "HistoricoPesos",
                newName: "HISTORICO_PESOS");

            migrationBuilder.RenameIndex(
                name: "IX_Veterinarios_ClinicaId",
                table: "VETERINARIOS",
                newName: "IX_VETERINARIOS_ClinicaId");

            migrationBuilder.RenameIndex(
                name: "IX_Vermifugacoes_VeterinarioId",
                table: "VERMIFUGACOES",
                newName: "IX_VERMIFUGACOES_VeterinarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Vermifugacoes_PetId",
                table: "VERMIFUGACOES",
                newName: "IX_VERMIFUGACOES_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_Vacinacoes_VeterinarioId",
                table: "VACINACOES",
                newName: "IX_VACINACOES_VeterinarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Vacinacoes_PetId",
                table: "VACINACOES",
                newName: "IX_VACINACOES_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_Recomendacoes_PetId",
                table: "RECOMENDACOES",
                newName: "IX_RECOMENDACOES_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescricoes_ConsultaId",
                table: "PRESCRICOES",
                newName: "IX_PRESCRICOES_ConsultaId");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_TutorId",
                table: "PETS",
                newName: "IX_PETS_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_ClinicaId",
                table: "PETS",
                newName: "IX_PETS_ClinicaId");

            migrationBuilder.RenameIndex(
                name: "IX_Medicamentos_PrescricaoId",
                table: "MEDICAMENTOS",
                newName: "IX_MEDICAMENTOS_PrescricaoId");

            migrationBuilder.RenameIndex(
                name: "IX_Lembretes_TutorId",
                table: "LEMBRETES",
                newName: "IX_LEMBRETES_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_Lembretes_PetId",
                table: "LEMBRETES",
                newName: "IX_LEMBRETES_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_Exames_ConsultaId",
                table: "EXAMES",
                newName: "IX_EXAMES_ConsultaId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultas_VeterinarioId",
                table: "CONSULTAS",
                newName: "IX_CONSULTAS_VeterinarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultas_PetId",
                table: "CONSULTAS",
                newName: "IX_CONSULTAS_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_Comportamentos_PetId",
                table: "COMPORTAMENTOS",
                newName: "IX_COMPORTAMENTOS_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricoPesos_PetId",
                table: "HISTORICO_PESOS",
                newName: "IX_HISTORICO_PESOS_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VETERINARIOS",
                table: "VETERINARIOS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VERMIFUGACOES",
                table: "VERMIFUGACOES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VACINACOES",
                table: "VACINACOES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TUTORES",
                table: "TUTORES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RECOMENDACOES",
                table: "RECOMENDACOES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PRESCRICOES",
                table: "PRESCRICOES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PETS",
                table: "PETS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MEDICAMENTOS",
                table: "MEDICAMENTOS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LEMBRETES",
                table: "LEMBRETES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EXAMES",
                table: "EXAMES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CONSULTAS",
                table: "CONSULTAS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_COMPORTAMENTOS",
                table: "COMPORTAMENTOS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CLINICAS",
                table: "CLINICAS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HISTORICO_PESOS",
                table: "HISTORICO_PESOS",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_COMPORTAMENTOS_PETS_PetId",
                table: "COMPORTAMENTOS",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CONSULTAS_PETS_PetId",
                table: "CONSULTAS",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CONSULTAS_VETERINARIOS_VeterinarioId",
                table: "CONSULTAS",
                column: "VeterinarioId",
                principalTable: "VETERINARIOS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EXAMES_CONSULTAS_ConsultaId",
                table: "EXAMES",
                column: "ConsultaId",
                principalTable: "CONSULTAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HISTORICO_PESOS_PETS_PetId",
                table: "HISTORICO_PESOS",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LEMBRETES_PETS_PetId",
                table: "LEMBRETES",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LEMBRETES_TUTORES_TutorId",
                table: "LEMBRETES",
                column: "TutorId",
                principalTable: "TUTORES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MEDICAMENTOS_PRESCRICOES_PrescricaoId",
                table: "MEDICAMENTOS",
                column: "PrescricaoId",
                principalTable: "PRESCRICOES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PETS_CLINICAS_ClinicaId",
                table: "PETS",
                column: "ClinicaId",
                principalTable: "CLINICAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PETS_TUTORES_TutorId",
                table: "PETS",
                column: "TutorId",
                principalTable: "TUTORES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PRESCRICOES_CONSULTAS_ConsultaId",
                table: "PRESCRICOES",
                column: "ConsultaId",
                principalTable: "CONSULTAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RECOMENDACOES_PETS_PetId",
                table: "RECOMENDACOES",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VACINACOES_PETS_PetId",
                table: "VACINACOES",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VACINACOES_VETERINARIOS_VeterinarioId",
                table: "VACINACOES",
                column: "VeterinarioId",
                principalTable: "VETERINARIOS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VERMIFUGACOES_PETS_PetId",
                table: "VERMIFUGACOES",
                column: "PetId",
                principalTable: "PETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VERMIFUGACOES_VETERINARIOS_VeterinarioId",
                table: "VERMIFUGACOES",
                column: "VeterinarioId",
                principalTable: "VETERINARIOS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VETERINARIOS_CLINICAS_ClinicaId",
                table: "VETERINARIOS",
                column: "ClinicaId",
                principalTable: "CLINICAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_COMPORTAMENTOS_PETS_PetId",
                table: "COMPORTAMENTOS");

            migrationBuilder.DropForeignKey(
                name: "FK_CONSULTAS_PETS_PetId",
                table: "CONSULTAS");

            migrationBuilder.DropForeignKey(
                name: "FK_CONSULTAS_VETERINARIOS_VeterinarioId",
                table: "CONSULTAS");

            migrationBuilder.DropForeignKey(
                name: "FK_EXAMES_CONSULTAS_ConsultaId",
                table: "EXAMES");

            migrationBuilder.DropForeignKey(
                name: "FK_HISTORICO_PESOS_PETS_PetId",
                table: "HISTORICO_PESOS");

            migrationBuilder.DropForeignKey(
                name: "FK_LEMBRETES_PETS_PetId",
                table: "LEMBRETES");

            migrationBuilder.DropForeignKey(
                name: "FK_LEMBRETES_TUTORES_TutorId",
                table: "LEMBRETES");

            migrationBuilder.DropForeignKey(
                name: "FK_MEDICAMENTOS_PRESCRICOES_PrescricaoId",
                table: "MEDICAMENTOS");

            migrationBuilder.DropForeignKey(
                name: "FK_PETS_CLINICAS_ClinicaId",
                table: "PETS");

            migrationBuilder.DropForeignKey(
                name: "FK_PETS_TUTORES_TutorId",
                table: "PETS");

            migrationBuilder.DropForeignKey(
                name: "FK_PRESCRICOES_CONSULTAS_ConsultaId",
                table: "PRESCRICOES");

            migrationBuilder.DropForeignKey(
                name: "FK_RECOMENDACOES_PETS_PetId",
                table: "RECOMENDACOES");

            migrationBuilder.DropForeignKey(
                name: "FK_VACINACOES_PETS_PetId",
                table: "VACINACOES");

            migrationBuilder.DropForeignKey(
                name: "FK_VACINACOES_VETERINARIOS_VeterinarioId",
                table: "VACINACOES");

            migrationBuilder.DropForeignKey(
                name: "FK_VERMIFUGACOES_PETS_PetId",
                table: "VERMIFUGACOES");

            migrationBuilder.DropForeignKey(
                name: "FK_VERMIFUGACOES_VETERINARIOS_VeterinarioId",
                table: "VERMIFUGACOES");

            migrationBuilder.DropForeignKey(
                name: "FK_VETERINARIOS_CLINICAS_ClinicaId",
                table: "VETERINARIOS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VETERINARIOS",
                table: "VETERINARIOS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VERMIFUGACOES",
                table: "VERMIFUGACOES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VACINACOES",
                table: "VACINACOES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TUTORES",
                table: "TUTORES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RECOMENDACOES",
                table: "RECOMENDACOES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PRESCRICOES",
                table: "PRESCRICOES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PETS",
                table: "PETS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MEDICAMENTOS",
                table: "MEDICAMENTOS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LEMBRETES",
                table: "LEMBRETES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EXAMES",
                table: "EXAMES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CONSULTAS",
                table: "CONSULTAS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_COMPORTAMENTOS",
                table: "COMPORTAMENTOS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CLINICAS",
                table: "CLINICAS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HISTORICO_PESOS",
                table: "HISTORICO_PESOS");

            migrationBuilder.RenameTable(
                name: "VETERINARIOS",
                newName: "Veterinarios");

            migrationBuilder.RenameTable(
                name: "VERMIFUGACOES",
                newName: "Vermifugacoes");

            migrationBuilder.RenameTable(
                name: "VACINACOES",
                newName: "Vacinacoes");

            migrationBuilder.RenameTable(
                name: "TUTORES",
                newName: "Tutores");

            migrationBuilder.RenameTable(
                name: "RECOMENDACOES",
                newName: "Recomendacoes");

            migrationBuilder.RenameTable(
                name: "PRESCRICOES",
                newName: "Prescricoes");

            migrationBuilder.RenameTable(
                name: "PETS",
                newName: "Pets");

            migrationBuilder.RenameTable(
                name: "MEDICAMENTOS",
                newName: "Medicamentos");

            migrationBuilder.RenameTable(
                name: "LEMBRETES",
                newName: "Lembretes");

            migrationBuilder.RenameTable(
                name: "EXAMES",
                newName: "Exames");

            migrationBuilder.RenameTable(
                name: "CONSULTAS",
                newName: "Consultas");

            migrationBuilder.RenameTable(
                name: "COMPORTAMENTOS",
                newName: "Comportamentos");

            migrationBuilder.RenameTable(
                name: "CLINICAS",
                newName: "Clinicas");

            migrationBuilder.RenameTable(
                name: "HISTORICO_PESOS",
                newName: "HistoricoPesos");

            migrationBuilder.RenameIndex(
                name: "IX_VETERINARIOS_ClinicaId",
                table: "Veterinarios",
                newName: "IX_Veterinarios_ClinicaId");

            migrationBuilder.RenameIndex(
                name: "IX_VERMIFUGACOES_VeterinarioId",
                table: "Vermifugacoes",
                newName: "IX_Vermifugacoes_VeterinarioId");

            migrationBuilder.RenameIndex(
                name: "IX_VERMIFUGACOES_PetId",
                table: "Vermifugacoes",
                newName: "IX_Vermifugacoes_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_VACINACOES_VeterinarioId",
                table: "Vacinacoes",
                newName: "IX_Vacinacoes_VeterinarioId");

            migrationBuilder.RenameIndex(
                name: "IX_VACINACOES_PetId",
                table: "Vacinacoes",
                newName: "IX_Vacinacoes_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_RECOMENDACOES_PetId",
                table: "Recomendacoes",
                newName: "IX_Recomendacoes_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_PRESCRICOES_ConsultaId",
                table: "Prescricoes",
                newName: "IX_Prescricoes_ConsultaId");

            migrationBuilder.RenameIndex(
                name: "IX_PETS_TutorId",
                table: "Pets",
                newName: "IX_Pets_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_PETS_ClinicaId",
                table: "Pets",
                newName: "IX_Pets_ClinicaId");

            migrationBuilder.RenameIndex(
                name: "IX_MEDICAMENTOS_PrescricaoId",
                table: "Medicamentos",
                newName: "IX_Medicamentos_PrescricaoId");

            migrationBuilder.RenameIndex(
                name: "IX_LEMBRETES_TutorId",
                table: "Lembretes",
                newName: "IX_Lembretes_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_LEMBRETES_PetId",
                table: "Lembretes",
                newName: "IX_Lembretes_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_EXAMES_ConsultaId",
                table: "Exames",
                newName: "IX_Exames_ConsultaId");

            migrationBuilder.RenameIndex(
                name: "IX_CONSULTAS_VeterinarioId",
                table: "Consultas",
                newName: "IX_Consultas_VeterinarioId");

            migrationBuilder.RenameIndex(
                name: "IX_CONSULTAS_PetId",
                table: "Consultas",
                newName: "IX_Consultas_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_COMPORTAMENTOS_PetId",
                table: "Comportamentos",
                newName: "IX_Comportamentos_PetId");

            migrationBuilder.RenameIndex(
                name: "IX_HISTORICO_PESOS_PetId",
                table: "HistoricoPesos",
                newName: "IX_HistoricoPesos_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Veterinarios",
                table: "Veterinarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vermifugacoes",
                table: "Vermifugacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacinacoes",
                table: "Vacinacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tutores",
                table: "Tutores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recomendacoes",
                table: "Recomendacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescricoes",
                table: "Prescricoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pets",
                table: "Pets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medicamentos",
                table: "Medicamentos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lembretes",
                table: "Lembretes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exames",
                table: "Exames",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultas",
                table: "Consultas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comportamentos",
                table: "Comportamentos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clinicas",
                table: "Clinicas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricoPesos",
                table: "HistoricoPesos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comportamentos_Pets_PetId",
                table: "Comportamentos",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Pets_PetId",
                table: "Consultas",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Veterinarios_VeterinarioId",
                table: "Consultas",
                column: "VeterinarioId",
                principalTable: "Veterinarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exames_Consultas_ConsultaId",
                table: "Exames",
                column: "ConsultaId",
                principalTable: "Consultas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoPesos_Pets_PetId",
                table: "HistoricoPesos",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lembretes_Pets_PetId",
                table: "Lembretes",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lembretes_Tutores_TutorId",
                table: "Lembretes",
                column: "TutorId",
                principalTable: "Tutores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medicamentos_Prescricoes_PrescricaoId",
                table: "Medicamentos",
                column: "PrescricaoId",
                principalTable: "Prescricoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Clinicas_ClinicaId",
                table: "Pets",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Tutores_TutorId",
                table: "Pets",
                column: "TutorId",
                principalTable: "Tutores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricoes_Consultas_ConsultaId",
                table: "Prescricoes",
                column: "ConsultaId",
                principalTable: "Consultas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recomendacoes_Pets_PetId",
                table: "Recomendacoes",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacinacoes_Pets_PetId",
                table: "Vacinacoes",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacinacoes_Veterinarios_VeterinarioId",
                table: "Vacinacoes",
                column: "VeterinarioId",
                principalTable: "Veterinarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vermifugacoes_Pets_PetId",
                table: "Vermifugacoes",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vermifugacoes_Veterinarios_VeterinarioId",
                table: "Vermifugacoes",
                column: "VeterinarioId",
                principalTable: "Veterinarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veterinarios_Clinicas_ClinicaId",
                table: "Veterinarios",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
