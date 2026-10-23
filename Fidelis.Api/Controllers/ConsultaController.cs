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
[SwaggerTag("Agendamento e histórico de consultas veterinárias.")]
public class ConsultaController : HateoasController
{
    private readonly IConsultaService _service;

    public ConsultaController(IConsultaService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista consultas com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (qualquer campo do recurso), sortDirection (asc|desc) e filtros: petId, veterinarioId, tipo, dataInicio, dataFim.")]
    [ProducesResponseType(typeof(PagedResponse<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] ConsultaQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, consulta => consulta.Id));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca uma consulta por id")]
    [ProducesResponseType(typeof(ResourceResponse<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var consulta = _service.GetById(id);
        return consulta is null ? NotFound() : Ok(ToResource(consulta, consulta.Id));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra uma nova consulta")]
    [ProducesResponseType(typeof(ResourceResponse<ConsultaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ConsultaRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza todos os dados de uma consulta")]
    [ProducesResponseType(typeof(ResourceResponse<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ConsultaRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza parcialmente uma consulta")]
    [ProducesResponseType(typeof(ResourceResponse<ConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] ConsultaPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove uma consulta")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
