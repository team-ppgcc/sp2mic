import { DtoClasse } from './dto-classe';
import { Variavel } from './variavel';
import { TipoDadoEnum } from '../enums/tipo-dado-enum';

export class Endpoint {
  id: number;
  noMetodoEndpoint: string;
  noPath: string;
  snRetornoLista: boolean;
  snAnalisado: boolean;
  txEndpointTratado: string;
  coTipoDadoRetorno: TipoDadoEnum;
  idDtoClasse?: number;
  noVariavelRetornda: string;
  noStoredProcedure?: string;
  idMicrosservico?: number;
  noMicrosservico?: string;

  idStoredProcedure: number;

  idVariavelRetornadaNavigation: Variavel;
  idDtoClasseNavigation: DtoClasse;
}
