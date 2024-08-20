import { DtoClasse } from './dto-classe';
import { EndpointListagem } from './endpoint-listagem';

export class Microsservico {
  id: number;
  noMicrosservico: string;
  snProntoParaGerar: boolean;
  qtdEndpoints: number;
  dtoClasses?: DtoClasse[];
  endpoints?: EndpointListagem[];
}

export class MicrosservicoPaginado extends Microsservico {
  page: number;
  linesPerPage: number;
  orderBy: string;
  direction: string;
}
