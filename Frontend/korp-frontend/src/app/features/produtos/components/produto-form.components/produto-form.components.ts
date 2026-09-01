import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzMessageService } from 'ng-zorro-antd/message';

import {
  AtualizarProduto,
  CriarProduto,
  Produto
} from '../../../../core/models/produto.model';

import { ProdutoService } from '../../../../core/services/produto.service';

import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { HttpErrorService } from '../../../../core/services/http-error.service';

@Component({
  selector: 'app-produto-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,

    NzFormModule,
    NzInputModule,
    NzInputNumberModule,
    NzButtonModule
  ],
  templateUrl: './produto-form.components.html',
  styleUrl: './produto-form.components.scss'
})
export class ProdutoFormComponents implements OnChanges {

  private readonly fb = inject(FormBuilder);

  private readonly produtoService = inject(ProdutoService);

  private readonly message = inject(NzMessageService);

  private readonly httpErrorService = inject(HttpErrorService);


  @Input()
  produto?: Produto;


  @Output()
  salvo = new EventEmitter<void>();


  @Output()
  cancelado = new EventEmitter<void>();


  salvando = false;


  form = this.fb.nonNullable.group({

    codigo: [
      '',
      [
        Validators.required,
        Validators.maxLength(50)
      ]
    ],

    descricao: [
      '',
      [
        Validators.required,
        Validators.maxLength(200)
      ]
    ],

    saldo: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ]

  });


  ngOnChanges(changes: SimpleChanges): void {

    if (!changes['produto']) {
      return;
    }


    if (this.produto) {
      this.form.patchValue({
        codigo: this.produto.codigo,
        descricao: this.produto.descricao,
        saldo: this.produto.saldo
      });

    } else {

      this.form.reset({
        codigo: '',
        descricao: '',
        saldo: 0
      });

    }

  }


  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando = true;

    if (this.produto) {
      const dto: AtualizarProduto = this.form.getRawValue();

      this.produtoService
        .atualizar(this.produto.id, dto)
        .pipe(
          finalize(() => {
            this.salvando = false;
          })
        )
        .subscribe({
          next: () => {
            this.message.success(
              'Produto atualizado com sucesso.'
            );

            this.salvo.emit();
          },
          error: (erro: HttpErrorResponse) => {
            this.message.error(
              this.httpErrorService.obterMensagem(
                erro,
                'Não foi possível atualizar o produto.'
              )
            );
          }
        });

      return;
    }

    const dto: CriarProduto =
      this.form.getRawValue();

    this.produtoService
      .criar(dto)
      .pipe(
        finalize(() => {
          this.salvando = false;
        })
      )
      .subscribe({
        next: () => {
          this.message.success(
            'Produto cadastrado com sucesso.'
          );

          this.salvo.emit();
        },
        error: (erro: HttpErrorResponse) => {
          this.message.error(
            this.httpErrorService.obterMensagem(
              erro,
              'Não foi possível cadastrar o produto.'
            )
          );
        }
      });
  }

}
