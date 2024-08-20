import { Pageable } from './pageable';
import { PageSort } from './page-sort';
import { Endpoint } from './endpoint';

export class PageEndpointDTO {
  content: Endpoint[];
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
