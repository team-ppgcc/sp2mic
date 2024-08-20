import { DtoClasse } from './dto-classe';
import { TipoDadoEnum } from '../enums/tipo-dado-enum';

export class AtributoDto {
  id?: number;
  noAtributo?: string;
  coTipoDado?: TipoDadoEnum;
  noTipoDado?: string;
  idDtoClasse?: number;
  //noDtoClasse?= "";
  noMicrosservico?: string;
  dtoClasse?: DtoClasse;
}

export class AtributoDtoPaginado extends AtributoDto {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
