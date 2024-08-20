import { TipoDadoEnum } from '../enums/tipo-dado-enum';

export class StoredProcedureFilter {
  id?: number;
  noSchema?: string;
  noStoredProcedure?: string;
  coTipoDadoRetorno?: TipoDadoEnum;
  snSucessoParser?: boolean;
  snAnalisada?: boolean;
  snRetornoLista?: boolean;
  idDtoClasse?: number;
}

export class StoredProcedureFilterPaginado extends StoredProcedureFilter {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
