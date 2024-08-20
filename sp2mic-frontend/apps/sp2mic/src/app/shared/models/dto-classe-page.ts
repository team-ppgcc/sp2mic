import { Pageable } from './pageable';
import { PageSort } from './page-sort';
import { DtoClasse } from './dto-classe';

export class DtoClassePage {
  content: DtoClasse[];
  totalElements: number;
  //totalPages: number;
  last: boolean;
  first: boolean;
  size: number;
  // número da página atual
  number: number;
  //numberOfElements: number;
  empty: boolean;
  sort: PageSort;
  pageable: Pageable;
}
