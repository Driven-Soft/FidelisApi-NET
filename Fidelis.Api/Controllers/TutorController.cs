using Fidelis.Application.DTOs;
using Fidelis.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Fidelis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[SwaggerTag("Cadastro de tutores responsáveis pelos pets.")]
public class TutorController : ControllerBase
{
    private readonly ITutorService _service;

    public TutorController(ITutorService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista tutores cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var tutores = _service.GetAll();
        return Ok(tutores);
    }

    /// <summary>
    /// Busca um tutor por id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var tutor = _service.GetById(id);
        return tutor is null ? NotFound() : Ok(tutor);
    }

    /// <summary>
    /// Cria um novo tutor.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TutorRequest request)
    {
        var created = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um tutor existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] TutorRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Atualiza parcialmente um tutor.
    /// </summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] TutorPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Remove um tutor por id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var deleted = _service.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}