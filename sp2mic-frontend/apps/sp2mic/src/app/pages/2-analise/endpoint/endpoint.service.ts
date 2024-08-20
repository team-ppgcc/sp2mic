import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Endpoint } from '../../../shared/models/endpoint';
import { Observable } from 'rxjs';
import { UtilsService } from '../../../shared/util/utils.service';
import { GenericService } from '../../../shared/services/generic-service';
import { EndpointUpdate } from '../../../shared/models/endpoint-update';
import { EndpointListagem } from '../../../shared/models/endpoint-listagem';
import { EndpointFilter } from '../../../shared/models/endpoint-filter';

@Injectable({
  providedIn: 'root',
})
export class EndpointService extends GenericService {
  public override api = `${environment.apiUrl}/analysis/endpoints`;

  constructor(public override http: HttpClient) {
    super(http);
  }

  public findByFilter(
    filtrosEndpoint?: EndpointFilter,
  ): Observable<EndpointListagem[]> {
    const url = `${this.api}/find-by-filter-async`;
    const queryParams: HttpParams =
      UtilsService.buildQueryParams(filtrosEndpoint);
    return this.http.get<EndpointListagem[]>(url, {
      headers: this.headers,
      params: queryParams,
    });
  }

  update(id: number, endpointUpdate: EndpointUpdate): Observable<any> {
    const url = `${this.api}/update/${id}`;
    return this.http.put(url, JSON.stringify(endpointUpdate), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  override findById(id: number): Observable<Endpoint> {
    const url = `${this.api}/find-by-id-async/${id}`;
    return this.http.get<Endpoint>(url, this.httpOptions);
  }

  override delete(id: number): Observable<any> {
    const url = `${this.api}/delete/${id}`;
    return this.http.delete(url, this.httpOptions);
  }
}
