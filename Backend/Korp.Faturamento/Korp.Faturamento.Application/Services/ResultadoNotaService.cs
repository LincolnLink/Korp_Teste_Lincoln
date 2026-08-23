using Korp.Faturamento.Application.Exceptions;
using Korp.Faturamento.Domain.Enums;
using Korp.Faturamento.Domain.Interfaces;
using Korp.Faturamento.Domain.Messages;

namespace Korp.Faturamento.Application.Services
{
    public class ResultadoNotaService : IResultadoNotaService
    {
        private readonly INotaFiscalRepository _repository;

        public ResultadoNotaService(
            INotaFiscalRepository repository)
        {
            _repository = repository;
        }

        public async Task ProcessarResultadoAsync(
            ResultadoProcessamentoNotaMessage message)
        {
            var nota =
                await _repository.ObterPorIdAsync(
                    message.NotaFiscalId);

            if (nota is null)
                throw new NotFoundException(
                    $"Nota fiscal {message.NotaFiscalId} não encontrada.");

            if (!message.Sucesso)
            {
                // permanece Aberta
                return;
            }

            nota.Status = StatusNotaFiscal.Fechada;

            await _repository.AtualizarAsync(nota);
        }
    }
}
