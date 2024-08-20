export class StoredProcedureListagem {
  id: number;
  noSchema: string;
  noStoredProcedure: string;
  noTipoDadoRetorno: string;
  noSucessoParser: string;
  snAnalisada: boolean;
  qtdEndpoints: number;
  tabelasAssociadas: string;
}

export class StoredProcedureListagemPaginado extends StoredProcedureListagem {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
