import { PageSort } from './page-sort';
import { Pageable } from './pageable';
import { Microsservico } from './microsservico';

export class MicrosservicoPageDto {
  content: Microsservico[];
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
