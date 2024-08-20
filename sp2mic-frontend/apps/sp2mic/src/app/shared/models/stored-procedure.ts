import { TipoDadoEnum } from '../enums/tipo-dado-enum';
import { EndpointListagem } from './endpoint-listagem';

export class StoredProcedure {
  id: number;
  noSchema: string;
  noStoredProcedure: string;
  snRetornoLista: boolean;
  snAnalisada: boolean;
  coTipoDadoRetorno: TipoDadoEnum;
  idDtoClasse?: number;

  txResultadoParser?: string;
  snSucessoParser: boolean;

  endpoints?: EndpointListagem[];
}

export class StoredProcedurePaginado extends StoredProcedure {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
