import { PageSort } from './page-sort';
import { Pageable } from './pageable';
import { Variavel } from './variavel';

export class VariavelPageDto {
  content: Variavel[];
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
