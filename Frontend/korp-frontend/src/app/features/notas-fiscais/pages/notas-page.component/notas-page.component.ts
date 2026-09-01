import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit
} from '@angular/core';

import { finalize } from 'rxjs';

import { DatePipe } from '@angular/common';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzMessageService } from 'ng-zorro-antd/message';

import { NotaFiscal } from '../../../../core/models/nota-fiscal.model';

import { NotaFiscalService } from '../../../../core/services/nota-fiscal.service';

import { NotaFormComponents } from '../../components/nota-form.components/nota-form.components';

import { HttpErrorService } from '../../../../core/services/http-error.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-notas-page',
  standalone: true,
  imports: [
    DatePipe,

    NzButtonModule,
    NzTableModule,
    NzModalModule,
    NzTagModule,

    NotaFormComponents
  ],
  templateUrl: './notas-page.component.html',
  styleUrl: './notas-page.component.scss'
})
export class NotasPageComponent implements OnInit {

  private readonly notaFiscalService = inject(NotaFiscalService);

  private readonly message = inject(NzMessageService);

  private readonly cdr = inject(ChangeDetectorRef);

  private readonly httpErrorService = inject(HttpErrorService);

  notas: NotaFiscal[] = [];

  carregando = false;

  modalAberto = false;

  processandoNotaId?: string;


  ngOnInit(): void { this.carregarNotas(); }

  carregarNotas(): void {
    this.carregando = true;

    this.notaFiscalService
      .listar()
      .pipe(
        finalize(() => {
          this.carregando = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: (notas) => {
          this.notas = notas;
        },
        error: (erro: HttpErrorResponse) => {
          this.message.error(
            this.httpErrorService.obterMensagem(erro, 'Não foi possível carregar as notas fiscais.')
          );
        }
      });
  }


  novaNota(): void {

    this.modalAberto = true;

  }


  fecharModal(): void {

    this.modalAberto = false;

  }


  notaCriada(): void {

    this.fecharModal();

    this.carregarNotas();

  }


  imprimir(nota: NotaFiscal): void {
    this.processandoNotaId = nota.id;

    this.notaFiscalService
      .imprimir(nota.id)
      .subscribe({
        next: () => {
          this.message.info(
            `Nota ${nota.numero} enviada para processamento.`
          );

          this.verificarStatusNota(nota);
        },
        error: (erro: HttpErrorResponse) => {
          this.processandoNotaId = undefined;

          this.message.error(
            this.httpErrorService.obterMensagem(
              erro,
              'Não foi possível processar a nota fiscal.'
            )
          );

          this.cdr.markForCheck();
        }
      });
  }

  private verificarStatusNota(nota: NotaFiscal): void {
    const intervalo = setInterval(() => {

      this.notaFiscalService
        .buscarPorId(nota.id)
        .subscribe({
          next: (notaAtualizada) => {

            if (notaAtualizada.status === 2) {
              clearInterval(intervalo);

              this.processandoNotaId = undefined;

              this.message.success(
                `Nota ${nota.numero} processada com sucesso.`
              );
              this.cdr.markForCheck();
              this.carregarNotas();
            }

          },
          error: (erro: HttpErrorResponse) => {
            clearInterval(intervalo);
            this.processandoNotaId = undefined;

            this.message.error(
              this.httpErrorService.obterMensagem(
                erro,
                'Erro ao consultar o processamento da nota.'
              )
            );

            this.cdr.markForCheck();
          }
        });

    }, 1000);
  }

  statusDescricao(status: number): string {

    return status === 2
      ? 'Fechada'
      : 'Aberta';

  }

}