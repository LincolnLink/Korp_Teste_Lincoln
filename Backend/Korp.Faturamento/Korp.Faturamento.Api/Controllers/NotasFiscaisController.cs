using Korp.Faturamento.Application.DTOs;
using Korp.Faturamento.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Faturamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly INotaFiscalService _notaFiscalService;

        public NotasFiscaisController(
            INotaFiscalService notaFiscalService)
        {
            _notaFiscalService = notaFiscalService;
        }

        [HttpPost]
        [ProducesResponseType(
            typeof(NotaFiscalResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotaFiscalResponseDto>> Cadastrar(
            CriarNotaFiscalDto request)
        {
            var nota =
                await _notaFiscalService.CadastrarAsync(request);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = nota.Id },
                nota);
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(IEnumerable<NotaFiscalResponseDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotaFiscalResponseDto>>>
            ObterTodos()
        {
            var notas =
                await _notaFiscalService.ObterTodosAsync();

            return Ok(notas);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(
            typeof(NotaFiscalResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotaFiscalResponseDto>>
            ObterPorId(Guid id)
        {
            var nota =
                await _notaFiscalService.ObterPorIdAsync(id);

            return Ok(nota);
        }

        [HttpPost("{id:guid}/imprimir")]
        [ProducesResponseType(
            StatusCodes.Status202Accepted)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Imprimir(Guid id)
        {
            await _notaFiscalService.ImprimirAsync(id);

            return Accepted(new
            {
                mensagem = "Nota fiscal enviada para processamento."
            });
        }
    }
}
