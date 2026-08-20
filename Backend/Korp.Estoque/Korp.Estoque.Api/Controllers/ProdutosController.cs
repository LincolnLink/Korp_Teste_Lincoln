using Korp.Estoque.Application.DTOs;
using Korp.Estoque.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Estoque.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoResponseDto>> Cadastrar(
        ProdutoRequestDto request)
    {
        var produto = await _produtoService.CadastrarAsync(request);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = produto.Id },
            produto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> ObterTodos()
    {
        var produtos = await _produtoService.ObterTodosAsync();

        return Ok(produtos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponseDto>> ObterPorId(Guid id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);

        if (produto is null)
            return NotFound();

        return Ok(produto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(
        Guid id,
        ProdutoRequestDto request)
    {
        var atualizado = await _produtoService.AtualizarAsync(id, request);

        if (!atualizado)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var excluido = await _produtoService.ExcluirAsync(id);

        if (!excluido)
            return NotFound();

        return NoContent();
    }
}