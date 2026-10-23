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
[SwaggerTag("Cadastro de clínicas veterinárias.")]
public class ClinicaController : HateoasController
{
    private readonly IClinicaService _service;

    public ClinicaController(IClinicaService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista clínicas com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (qualquer campo do recurso), sortDirection (asc|desc) e filtros: nome, cnpj, email.")]
    [ProducesResponseType(typeof(PagedResponse<ClinicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] ClinicaQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, clinica => clinica.Id));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca uma clínica por id")]
    [ProducesResponseType(typeof(ResourceResponse<ClinicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var clinica = _service.GetById(id);
        return clinica is null ? NotFound() : Ok(ToResource(clinica, clinica.Id));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra uma nova clínica")]
    [ProducesResponseType(typeof(ResourceResponse<ClinicaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ClinicaRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza todos os dados de uma clínica")]
    [ProducesResponseType(typeof(ResourceResponse<ClinicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ClinicaRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza parcialmente uma clínica")]
    [ProducesResponseType(typeof(ResourceResponse<ClinicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] ClinicaPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove uma clínica")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
