export class EndpointListagem {
  id: number;
  noMetodoEndpoint: string;
  noPath: string;
  noMicrosservico?: string;
  noTipoSqlDml: string;
  noTipoDadoRetorno: string;
  snAnalisado: boolean;

  idStoredProcedure: number;
  noStoredProcedure: string;

  tabelasAssociadas: string;
}

export class EndpointListagemPaginado extends EndpointListagem {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
