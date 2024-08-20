import { TipoDadoEnum } from '../enums/tipo-dado-enum';

export class StoredProcedureUpdate {
  noStoredProcedure: string;
  noSchema: string;
  coTipoDadoRetorno: TipoDadoEnum;
  snRetornoLista: boolean;
  snAnalisada: boolean;
  idDtoClasse?: number;
}
