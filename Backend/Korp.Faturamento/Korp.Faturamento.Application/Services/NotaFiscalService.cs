using FluentValidation;
using Korp.Faturamento.Application.DTOs;
using Korp.Faturamento.Application.Exceptions;
using Korp.Faturamento.Application.Interfaces;
using Korp.Faturamento.Application.Messages;
using Korp.Faturamento.Domain.Entities;
using Korp.Faturamento.Domain.Enums;
using Korp.Faturamento.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Korp.Faturamento.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _notaFiscalRepository;        
        private readonly IValidator<CriarNotaFiscalDto> _validator;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public NotaFiscalService(
            INotaFiscalRepository notaFiscalRepository,
            IValidator<CriarNotaFiscalDto> validator,
            IRabbitMqPublisher rabbitMqPublisher)
        {
            _notaFiscalRepository = notaFiscalRepository;
            _validator = validator;
        }

        public async Task<NotaFiscalResponseDto> CadastrarAsync(CriarNotaFiscalDto request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var proximoNumero =
                await _notaFiscalRepository.ObterProximoNumeroAsync();

            var notaFiscal = new NotaFiscal
            {
                Id = Guid.NewGuid(),
                Numero = proximoNumero,
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = DateTime.UtcNow
            };

            foreach (var itemRequest in request.Itens)
            {
                notaFiscal.Itens.Add(new ItemNotaFiscal
                {
                    Id = Guid.NewGuid(),
                    NotaFiscalId = notaFiscal.Id,
                    ProdutoId = itemRequest.ProdutoId,
                    Quantidade = itemRequest.Quantidade
                });
            }

            await _notaFiscalRepository.AdicionarAsync(notaFiscal);

            return MapearNotaFiscal(notaFiscal);
        }

        public async Task<IEnumerable<NotaFiscalResponseDto>> ObterTodosAsync()
        {
            var notas =
                await _notaFiscalRepository.ObterTodosAsync();

            return notas.Select(MapearNotaFiscal);
        }

        public async Task<NotaFiscalResponseDto> ObterPorIdAsync(Guid id)
        {
            var nota =
                await _notaFiscalRepository.ObterPorIdAsync(id);

            if (nota is null)
            {
                throw new NotFoundException(
                    $"Nota fiscal com Id {id} não encontrada.");
            }

            return MapearNotaFiscal(nota);
        }

        public async Task ImprimirAsync(Guid id)
        {
            var nota =
                await _notaFiscalRepository.ObterPorIdAsync(id);

            if (nota is null)
            {
                throw new NotFoundException(
                    $"Nota fiscal com Id {id} não encontrada.");
            }

            if (nota.Status != StatusNotaFiscal.Aberta)
            {
                throw new BusinessException(
                    "Somente notas fiscais abertas podem ser impressas.");
            }

            var message = new ProcessarNotaFiscalMessage
            {
                NotaFiscalId = nota.Id,

                Itens = nota.Itens
                    .Select(item => new ItemNotaFiscalMessage
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade
                    })
                    .ToList()
            };

            await _rabbitMqPublisher
                .PublicarProcessamentoNotaAsync(message);
        }

        private static NotaFiscalResponseDto MapearNotaFiscal(NotaFiscal nota)
        {
            return new NotaFiscalResponseDto
            {
                Id = nota.Id,
                Numero = nota.Numero,
                Status = nota.Status,
                DataCriacao = nota.DataCriacao,

                Itens = nota.Itens.Select(item =>
                    new ItemNotaFiscalResponseDto
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade
                    })
            };
        }
    }
}
