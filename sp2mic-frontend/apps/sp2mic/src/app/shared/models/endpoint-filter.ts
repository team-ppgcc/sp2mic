export class EndpointFilter {
  id: number;
  noMetodoEndpoint: string;
  noPath: string;
  coTipoSqlDml: number;
  coTipoDadoRetorno: number;
  snRetornoLista: boolean;
  snAnalisado: boolean;
  noMicrosservico?: string;
  idMicrosservico: number;
  idStoredProcedure: number;
  idDtoClasse: number;
  idVariavelRetornada: number;
}
