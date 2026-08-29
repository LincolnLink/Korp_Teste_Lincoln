import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  AtualizarProduto,
  CriarProduto,
  Produto
} from '../models/produto.model';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiEstoque}/Produtos`;

  listar(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }

  buscarPorId(id: string): Observable<Produto> {
    return this.http.get<Produto>(
      `${this.apiUrl}/${id}`
    );
  }

  criar(produto: CriarProduto): Observable<Produto> {
    return this.http.post<Produto>(
      this.apiUrl,
      produto
    );
  }

  atualizar(
    id: string,
    produto: AtualizarProduto
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      produto
    );
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}