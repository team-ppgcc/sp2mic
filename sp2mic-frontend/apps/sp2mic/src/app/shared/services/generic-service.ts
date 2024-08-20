import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export class GenericService {
  protected headers = {
    'Content-Type': 'application/json-patch+json',
    accept: 'application/json',
  };
  protected pesquisar: boolean;
  protected api = `${environment.apiUrl}`;
  protected httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json-patch+json',
      accept: 'application/json',
      'Cache-Control': 'no-cache',
      'If-Modified-Since': '0',
    }),
  };

  constructor(public http: HttpClient) {}

  findById(id: number): Observable<any> {
    const url = `${this.api}/${id}`;
    return this.http.get(url, this.httpOptions);
  }

  delete(id: number): Observable<any> {
    const url = `${this.api}/${id}`;
    return this.http.delete(url, this.httpOptions);
  }
}
