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
[SwaggerTag("Cadastro de pets e seus dados principais.")]
public class PetController : HateoasController
{
    private readonly IPetService _service;

    public PetController(IPetService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista pets com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (qualquer campo do recurso), sortDirection (asc|desc) e filtros: nome, especie, raca, sexo, status, tutorId, clinicaId.")]
    [ProducesResponseType(typeof(PagedResponse<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] PetQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, pet => pet.Id));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca um pet por id")]
    [ProducesResponseType(typeof(ResourceResponse<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var pet = _service.GetById(id);
        return pet is null ? NotFound() : Ok(ToResource(pet, pet.Id));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um novo pet")]
    [ProducesResponseType(typeof(ResourceResponse<PetResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] PetRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza todos os dados de um pet")]
    [ProducesResponseType(typeof(ResourceResponse<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] PetRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza parcialmente um pet")]
    [ProducesResponseType(typeof(ResourceResponse<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] PetPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove um pet")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
