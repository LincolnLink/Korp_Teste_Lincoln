import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  CriarNotaFiscal,
  NotaFiscal
} from '../models/nota-fiscal.model';

@Injectable({
  providedIn: 'root'
})
export class NotaFiscalService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiFaturamento}/NotasFiscais`;

  listar(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(this.apiUrl);
  }

  buscarPorId(id: string): Observable<NotaFiscal> {
    return this.http.get<NotaFiscal>(
      `${this.apiUrl}/${id}`
    );
  }

  criar(
    notaFiscal: CriarNotaFiscal
  ): Observable<NotaFiscal> {

    return this.http.post<NotaFiscal>(
      this.apiUrl,
      notaFiscal
    );
  }

  imprimir(id: string): Observable<{ mensagem: string }> {
    return this.http.post<{ mensagem: string }>(
      `${this.apiUrl}/${id}/imprimir`,
      {}
    );
  }
}