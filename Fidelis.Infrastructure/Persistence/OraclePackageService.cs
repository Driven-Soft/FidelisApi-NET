using System.Data;
using Fidelis.Application.DTOs;
using Fidelis.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Fidelis.Infrastructure.Persistence;

public sealed class OraclePackageService : IOraclePackageService
{
    private readonly FidelisContext _context;

    public OraclePackageService(FidelisContext context)
    {
        _context = context;
    }

    public Task<(string? Resultado, string? Mensagem)> ListarPetsJsonAsync(
        int? id,
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            "PKG_FIDELIS_JSON.SP_LISTAR_PETS_JSON",
            command =>
            {
                AddParameter(command, "p_id_pet", OracleDbType.Int32, ParameterDirection.Input,
                    id.HasValue ? id.Value : DBNull.Value);
                AddParameter(command, "p_resultado", OracleDbType.Clob, ParameterDirection.Output);
                AddParameter(command, "p_msg", OracleDbType.Varchar2, ParameterDirection.Output, size: 4000);
            },
            hasResult: true,
            cancellationToken);
    }

    public Task<(string? Resultado, string? Mensagem)> ResumoTratamentosAsync(
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            "PKG_FIDELIS_RELATORIOS.SP_RESUMO_DIAS_TRATAMENTO",
            command =>
            {
                AddParameter(command, "p_resultado", OracleDbType.Clob, ParameterDirection.Output);
                AddParameter(command, "p_msg", OracleDbType.Varchar2, ParameterDirection.Output, size: 4000);
            },
            hasResult: true,
            cancellationToken);
    }

    public async Task<string?> InserirTutorAsync(
        OracleTutorRequest request,
        CancellationToken cancellationToken)
    {
        var (_, mensagem) = await ExecuteAsync(
            "PKG_FIDELIS_CADASTRO.SP_INSERIR_TUTOR",
            command =>
            {
                AddParameter(command, "p_id", OracleDbType.Int32, ParameterDirection.Input, request.Id);
                AddParameter(command, "p_cpf", OracleDbType.Varchar2, ParameterDirection.Input, request.Cpf, 4000);
                AddParameter(command, "p_nome", OracleDbType.Varchar2, ParameterDirection.Input, request.Nome, 4000);
                AddParameter(command, "p_email", OracleDbType.Varchar2, ParameterDirection.Input, request.Email, 4000);
                AddParameter(command, "p_senha", OracleDbType.Varchar2, ParameterDirection.Input, request.Senha, 4000);
                AddParameter(command, "p_telefone", OracleDbType.Varchar2, ParameterDirection.Input, request.Telefone, 4000);
                AddParameter(command, "p_endereco", OracleDbType.Varchar2, ParameterDirection.Input, request.Endereco, 4000);
                AddParameter(command, "p_msg", OracleDbType.Varchar2, ParameterDirection.Output, size: 4000);
            },
            hasResult: false,
            cancellationToken);

        return mensagem;
    }

    private async Task<(string? Resultado, string? Mensagem)> ExecuteAsync(
        string procedureName,
        Action<OracleCommand> configureParameters,
        bool hasResult,
        CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;

        if (openedHere)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = (OracleCommand)connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            configureParameters(command);

            await command.ExecuteNonQueryAsync(cancellationToken);

            var resultado = hasResult
                ? GetParameterValue(command.Parameters["p_resultado"])
                : null;
            var mensagem = GetParameterValue(command.Parameters["p_msg"]);

            return (resultado, mensagem);
        }
        finally
        {
            if (openedHere)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static void AddParameter(
        OracleCommand command,
        string name,
        OracleDbType type,
        ParameterDirection direction,
        object? value = null,
        int size = 0)
    {
        var parameter = new OracleParameter(name, type)
        {
            Direction = direction
        };

        if (size > 0)
        {
            parameter.Size = size;
        }

        if (direction == ParameterDirection.Input)
        {
            parameter.Value = value ?? DBNull.Value;
        }

        command.Parameters.Add(parameter);
    }

    private static string? GetParameterValue(OracleParameter parameter)
    {
        return parameter.Value switch
        {
            null or DBNull => null,
            OracleClob clob => clob.IsNull ? null : clob.Value,
            string value => value,
            var value => Convert.ToString(value)
        };
    }
}