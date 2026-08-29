export interface ItemNotaFiscal {
  produtoId: string;
  quantidade: number;
}

export interface NotaFiscal {
  id: string;
  numero: number;
  status: number;
  dataCriacao: string;
  itens: ItemNotaFiscal[];
}

export interface CriarNotaFiscal {
  itens: ItemNotaFiscal[];
}