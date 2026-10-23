using Fidelis.Api.Hateoas;
using Fidelis.Application.DTOs;
using Fidelis.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Fidelis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
[SwaggerTag("Trilha de auditoria (MongoDB) gerada automaticamente para toda requisição POST, PUT, PATCH e DELETE.")]
public class AuditoriaController : HateoasController
{
    private readonly IAuditoriaService _service;

    public AuditoriaController(IAuditoriaService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista os logs de auditoria com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (dataHora, metodo, caminho, statusCode, duracaoMs), sortDirection (asc|desc) e filtros: metodo, caminho, statusCode, usuarioId, dataInicio, dataFim. Ordenação padrão: dataHora desc.")]
    [ProducesResponseType(typeof(PagedResponse<AuditoriaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] AuditoriaQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, log => log.Id));
    }

    [HttpGet("{id:length(24)}")]
    [SwaggerOperation(Summary = "Busca um log de auditoria por id (ObjectId)")]
    [ProducesResponseType(typeof(ResourceResponse<AuditoriaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(string id)
    {
        var log = _service.GetById(id);
        return log is null ? NotFound() : Ok(ToResource(log, log.Id));
    }
}
