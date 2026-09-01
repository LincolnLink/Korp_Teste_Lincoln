import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HttpErrorService {

  obterMensagem( erro: HttpErrorResponse,  mensagemPadrao: string ): string {
    return erro.error?.mensagem
      ?? erro.error?.message
      ?? mensagemPadrao;
  }
}