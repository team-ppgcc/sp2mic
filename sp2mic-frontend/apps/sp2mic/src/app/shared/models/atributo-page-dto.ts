import { Pageable } from './pageable';
import { PageSort } from './page-sort';
import { AtributoDto } from './atributo-dto';

export class PageAtributoDTO {
  content: AtributoDto[];
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
