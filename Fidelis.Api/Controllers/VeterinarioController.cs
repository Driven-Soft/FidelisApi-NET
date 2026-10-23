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
[SwaggerTag("Cadastro de veterinários e profissionais responsáveis.")]
public class VeterinarioController : HateoasController
{
    private readonly IVeterinarioService _service;

    public VeterinarioController(IVeterinarioService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista veterinários com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (qualquer campo do recurso), sortDirection (asc|desc) e filtros: nome, crmv, especialidade, clinicaId.")]
    [ProducesResponseType(typeof(PagedResponse<VeterinarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] VeterinarioQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, veterinario => veterinario.Id));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca um veterinário por id")]
    [ProducesResponseType(typeof(ResourceResponse<VeterinarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var veterinario = _service.GetById(id);
        return veterinario is null ? NotFound() : Ok(ToResource(veterinario, veterinario.Id));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um novo veterinário")]
    [ProducesResponseType(typeof(ResourceResponse<VeterinarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] VeterinarioRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza todos os dados de um veterinário")]
    [ProducesResponseType(typeof(ResourceResponse<VeterinarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] VeterinarioRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza parcialmente um veterinário")]
    [ProducesResponseType(typeof(ResourceResponse<VeterinarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] VeterinarioPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove um veterinário")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
