import { PageSort } from './page-sort';

export class Pageable {
  sort: PageSort;
  pageNumber: number;
  pageSize: number;
  offset: number;
  paged: boolean;
  //unpaged: boolean;
}
