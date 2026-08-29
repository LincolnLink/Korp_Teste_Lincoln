import {
  Component,
  EventEmitter,
  inject,
  OnInit,
  Output
} from '@angular/core';

import {
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzMessageService } from 'ng-zorro-antd/message';

import {
  Produto
} from '../../../../core/models/produto.model';

import {
  ProdutoService
} from '../../../../core/services/produto.service';

import {
  NotaFiscalService
} from '../../../../core/services/nota-fiscal.service';

import {
  CriarNotaFiscal
} from '../../../../core/models/nota-fiscal.model';

@Component({
  selector: 'app-nota-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,

    NzButtonModule,
    NzFormModule,
    NzSelectModule,
    NzInputNumberModule
  ],
  templateUrl: './nota-form.components.html',
  styleUrl: './nota-form.components.scss'
})
export class NotaFormComponents implements OnInit {

  private readonly fb =
    inject(FormBuilder);

  private readonly produtoService =
    inject(ProdutoService);

  private readonly notaFiscalService =
    inject(NotaFiscalService);

  private readonly message =
    inject(NzMessageService);


  @Output()
  salvo = new EventEmitter<void>();


  @Output()
  cancelado = new EventEmitter<void>();


  produtos: Produto[] = [];

  salvando = false;


  form = this.fb.group({

    itens: this.fb.array([])

  });


  ngOnInit(): void {

    this.carregarProdutos();

    this.adicionarItem();

  }


  get itens(): FormArray {

    return this.form.controls.itens;

  }


  carregarProdutos(): void {

    this.produtoService.listar().subscribe({

      next: (produtos) => {

        this.produtos = produtos;

      },

      error: () => {

        this.message.error(
          'Não foi possível carregar os produtos.'
        );

      }

    });

  }


  adicionarItem(): void {

    const item = this.fb.nonNullable.group({

      produtoId: [
        '',
        Validators.required
      ],

      quantidade: [
        1,
        [
          Validators.required,
          Validators.min(1)
        ]
      ]

    });


    this.itens.push(item);

  }


  removerItem(index: number): void {

    if (this.itens.length === 1) {
      return;
    }

    this.itens.removeAt(index);

  }


  salvar(): void {

    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;

    }


    const dto: CriarNotaFiscal = {

      itens: this.itens.getRawValue()

    };


    this.salvando = true;


    this.notaFiscalService.criar(dto).subscribe({

      next: () => {

        this.salvando = false;

        this.message.success(
          'Nota fiscal criada com sucesso.'
        );

        this.salvo.emit();

      },

      error: () => {

        this.salvando = false;

        this.message.error(
          'Não foi possível criar a nota fiscal.'
        );

      }

    });

  }

}