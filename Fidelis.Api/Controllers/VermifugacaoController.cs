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
[SwaggerTag("Controle de vermifugações dos pets.")]
public class VermifugacaoController : HateoasController
{
    private readonly IVermifugacaoService _service;

    public VermifugacaoController(IVermifugacaoService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista vermifugações com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (qualquer campo do recurso), sortDirection (asc|desc) e filtros: petId, veterinarioId, produto, proximaAte.")]
    [ProducesResponseType(typeof(PagedResponse<VermifugacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] VermifugacaoQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, vermifugacao => vermifugacao.Id));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca uma vermifugação por id")]
    [ProducesResponseType(typeof(ResourceResponse<VermifugacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var vermifugacao = _service.GetById(id);
        return vermifugacao is null ? NotFound() : Ok(ToResource(vermifugacao, vermifugacao.Id));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra uma nova vermifugação")]
    [ProducesResponseType(typeof(ResourceResponse<VermifugacaoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] VermifugacaoRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza todos os dados de uma vermifugação")]
    [ProducesResponseType(typeof(ResourceResponse<VermifugacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] VermifugacaoRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza parcialmente uma vermifugação")]
    [ProducesResponseType(typeof(ResourceResponse<VermifugacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] VermifugacaoPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove uma vermifugação")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
