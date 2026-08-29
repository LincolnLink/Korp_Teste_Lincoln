import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit
} from '@angular/core';

import { finalize } from 'rxjs';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';

import { ProdutoService } from '../../../../core/services/produto.service';
import { Produto } from '../../../../core/models/produto.model';

import { ProdutoFormComponents } from '../../components/produto-form.components/produto-form.components';

@Component({
  selector: 'app-produtos-page',
  standalone: true,
  imports: [
    NzButtonModule,
    NzTableModule,
    NzModalModule,
    NzPopconfirmModule,

    ProdutoFormComponents
  ],
  templateUrl: './produtos-page.component.html',
  styleUrl: './produtos-page.component.scss'
})
export class ProdutosPageComponent implements OnInit {

  private readonly produtoService = inject(ProdutoService);
  private readonly message = inject(NzMessageService);
  private readonly cdr = inject(ChangeDetectorRef);

  produtos: Produto[] = [];

  carregando = false;

  modalAberto = false;

  produtoSelecionado?: Produto;



  ngOnInit(): void {
    this.carregarProdutos();
  }


  carregarProdutos(): void {
    this.carregando = true;

    this.produtoService
      .listar()
      .pipe(
        finalize(() => {
          this.carregando = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: (produtos) => {
          this.produtos = produtos;
        },
        error: () => {
          this.message.error(
            'Não foi possível carregar os produtos.'
          );
          this.cdr.markForCheck();
        }
      });
  }


  novoProduto(): void {

    this.produtoSelecionado = undefined;

    this.modalAberto = true;
  }


  editarProduto(produto: Produto): void {

    this.produtoSelecionado = produto;

    this.modalAberto = true;
  }


  fecharModal(): void {

    this.modalAberto = false;

    this.produtoSelecionado = undefined;
  }


  produtoSalvo(): void {

    this.fecharModal();

    this.carregarProdutos();
  }


  excluirProduto(produto: Produto): void {

    this.produtoService.excluir(produto.id).subscribe({

      next: () => {

        this.message.success(
          'Produto excluído com sucesso.'
        );

        this.carregarProdutos();
      },

      error: () => {

        this.message.error(
          'Não foi possível excluir o produto.'
        );
        this.cdr.markForCheck();
      }

    });

  }

}