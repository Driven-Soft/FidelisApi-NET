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
[SwaggerTag("Evolução do peso dos pets ao longo do tempo.")]
public class HistoricoPesoController : HateoasController
{
    private readonly IHistoricoPesoService _service;

    public HistoricoPesoController(IHistoricoPesoService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista históricos de peso com paginação, ordenação e filtros",
        Description = "Parâmetros: page, pageSize (máx. 100), sortBy (qualquer campo do recurso), sortDirection (asc|desc) e filtros: petId, pesoMinimo, pesoMaximo, dataInicio, dataFim.")]
    [ProducesResponseType(typeof(PagedResponse<HistoricoPesoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetAll([FromQuery] HistoricoPesoQueryParameters query)
    {
        var result = _service.GetAll(query);
        return Ok(ToPagedResponse(result, historicoPeso => historicoPeso.Id));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca um histórico de peso por id")]
    [ProducesResponseType(typeof(ResourceResponse<HistoricoPesoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var historicoPeso = _service.GetById(id);
        return historicoPeso is null ? NotFound() : Ok(ToResource(historicoPeso, historicoPeso.Id));
    }

    [HttpGet("pet/{petId:int}/avaliacao")]
    [SwaggerOperation(
        Summary = "Avalia a pesagem mais recente do pet com o serviço de IA",
        Description = "Devolve a faixa de peso esperada para o perfil do pet, o risco (VERDE, AMARELO ou VERMELHO) e a justificativa. Somente leitura: não grava recomendações nem lembretes. Responde 404 quando o pet não existe ou não tem pesagens, 400 para espécies que não são cão ou gato e 503 quando o serviço de IA está indisponível.")]
    [ProducesResponseType(typeof(ResourceResponse<AvaliacaoPesoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetAvaliacao(int petId)
    {
        var avaliacao = _service.GetAvaliacao(petId);
        return avaliacao is null ? NotFound() : Ok(ToResource(avaliacao, avaliacao.HistoricoPesoId));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra um novo histórico de peso",
        Description = "Depois de gravar a pesagem, envia o perfil do pet e o peso ao serviço de IA, grava as recomendações e lembretes sugeridos e devolve a avaliação em avaliacaoIa. Se a IA estiver indisponível, a pesagem é gravada mesmo assim e avaliacaoIa vem nula.")]
    [ProducesResponseType(typeof(ResourceResponse<HistoricoPesoCriadoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] HistoricoPesoRequest request)
    {
        var created = _service.Create(request);
        return CreatedResource(created, created.Id);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza todos os dados de um histórico de peso")]
    [ProducesResponseType(typeof(ResourceResponse<HistoricoPesoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] HistoricoPesoRequest request)
    {
        var updated = _service.Update(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpPatch("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza parcialmente um histórico de peso")]
    [ProducesResponseType(typeof(ResourceResponse<HistoricoPesoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] HistoricoPesoPatchRequest request)
    {
        var updated = _service.Patch(id, request);
        return updated is null ? NotFound() : Ok(ToResource(updated, updated.Id));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove um histórico de peso")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
