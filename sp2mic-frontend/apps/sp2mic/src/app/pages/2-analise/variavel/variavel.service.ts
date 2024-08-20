import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Variavel, VariavelPaginado } from '../../../shared/models/variavel';
import { Observable } from 'rxjs';
import { UtilsService } from '../../../shared/util/utils.service';
import { GenericService } from '../../../shared/services/generic-service';
import { VariavelPageDto } from '../../../shared/models/variavel-page-dto';

@Injectable({
  providedIn: 'root',
})
export class VariavelService extends GenericService {
  public override api = `${environment.apiUrl}/analysis/variaveis`;
  public filtro: Variavel;

  constructor(public override http: HttpClient) {
    super(http);
  }

  public getByFilters(
    filtrosVariavel?: VariavelPaginado,
  ): Observable<VariavelPageDto> {
    this.filtro = filtrosVariavel;
    const url = `${this.api}/find-by-filter-async`;
    const queryParams: HttpParams =
      UtilsService.buildQueryParams(filtrosVariavel);
    return this.http.get<VariavelPageDto>(url, {
      headers: this.headers,
      params: queryParams,
    });
  }

  post(variavel: Variavel): Observable<HttpResponse<any>> {
    const url = `${this.api}/add`;
    return this.http.post(url, JSON.stringify(variavel), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  put(id: number, variavel: Variavel): Observable<any> {
    const url = `${this.api}/update/${id}`;
    return this.http.put(url, JSON.stringify(variavel), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  override findById(id: number): Observable<Variavel> {
    const url = `${this.api}/find-by-id-async/${id}`;
    return this.http.get<Variavel>(url, this.httpOptions);
  }

  override delete(id: number): Observable<any> {
    const url = `${this.api}/delete/${id}`;
    return this.http.delete(url, this.httpOptions);
  }
}
