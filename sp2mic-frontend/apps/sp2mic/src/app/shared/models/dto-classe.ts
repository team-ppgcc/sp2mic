import { AtributoDto } from './atributo-dto';

export class DtoClasse {
  id: number;
  noDtoClasse: string;
  //noMicrosservico: string;
  //idMicrosservico: number;
  idStoredProcedure: number;
  noStoredProcedure: string;
  txDtoClasse: string;
  //idMicrosservicoNavigation: Microsservico;
  atributos: AtributoDto[];
}

export class DtoClassePaginado extends DtoClasse {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
