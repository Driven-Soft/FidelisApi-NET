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
[SwaggerTag("Diário do pet armazenado no MongoDB (NoSQL): humor, observações e tags livres do dia a dia.")]
public class RegistroDiarioController : HateoasController
{
    private const string IdRoute = "{id:length(24)}";

    private readonly IRegistroDiarioService _service;

    public RegistroDiarioController(IRegistroDiarioService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista registros do diário com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (dataRegistro, titulo, humor, petId, criadoEm), sortDirection (asc|desc) e filtros: petId, humor, tag, texto (busca em título e descrição), dataInicio, dataFim. Ordenação padrão: dataRegistro desc.")]
    [ProducesResponseType(typeof(PagedResponse<RegistroDiarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] RegistroDiarioQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, registro => registro.Id));
    }

    [HttpGet(IdRoute)]
    [SwaggerOperation(Summary = "Busca um registro do diário por id (ObjectId)")]
    [ProducesResponseType(typeof(ResourceResponse<RegistroDiarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(string id)
    {
        var registro = _service.GetById(id);
        return registro is null ? NotFound() : Ok(ToResource(registro, registro.Id));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria um registro no diário do pet",
        Description = "Humores aceitos: Feliz, Calmo, Agitado, Ansioso, Triste, Doente. O pet informado precisa existir no banco relacional.")]
    [ProducesResponseType(typeof(ResourceResponse<RegistroDiarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] RegistroDiarioRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut(IdRoute)]
    [SwaggerOperation(Summary = "Atualiza todos os dados de um registro do diário")]
    [ProducesResponseType(typeof(ResourceResponse<RegistroDiarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(string id, [FromBody] RegistroDiarioUpdateRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch(IdRoute)]
    [SwaggerOperation(Summary = "Atualiza parcialmente um registro do diário")]
    [ProducesResponseType(typeof(ResourceResponse<RegistroDiarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(string id, [FromBody] RegistroDiarioPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete(IdRoute)]
    [SwaggerOperation(Summary = "Remove um registro do diário")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(string id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
