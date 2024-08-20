import { TipoDadoEnum } from '../enums/tipo-dado-enum';

export class Variavel {
  id: number;
  noVariavel: string;
  coTipoDado: TipoDadoEnum;
  // 1-Tipo nao mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long,
  // 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal.
  //coTipoEscopo: number;
  // 1-Parametro da Stored Procedure, 2-Local, 3-Parametro do Endpoint
  //nuTamanho: number;
  idStoredProcedure?: number;
  idEndpoint?: number;
}

export class VariavelPaginado extends Variavel {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
