import { PageSort } from './page-sort';
import { Pageable } from './pageable';
import { StoredProcedure } from './stored-procedure';

export class StoredProcedurePageDto {
  content: StoredProcedure[];
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
