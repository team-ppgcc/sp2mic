import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UtilsService } from '../../../shared/util/utils.service';
import { Microsservico } from '../../../shared/models/microsservico';
import { GenericService } from '../../../shared/services/generic-service';
import { MicrosservicoUpdate } from '../../../shared/models/microsservico-update';
import { MicrosservicoAdd } from '../../../shared/models/microsservico-add';
import { map, startWith } from 'rxjs/operators';
import { FormControl } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class MicrosservicoService extends GenericService {
  public override api = `${environment.apiUrl}/analysis/microsservicos`;

  constructor(public override http: HttpClient) {
    super(http);
  }

  ping(): Observable<any> {
    const url = `${this.api}/ping`;
    return this.http.get(url, this.httpOptions);
  }

  public findByFilter(
    filtrosMicrosservico?: Microsservico,
  ): Observable<Microsservico[]> {
    const url = `${this.api}/find-by-filter-async`;
    const queryParams: HttpParams =
      UtilsService.buildQueryParams(filtrosMicrosservico);
    return this.http.get<Microsservico[]>(url, {
      headers: this.headers,
      params: queryParams,
    });
  }

  post(microsservicoAdd: MicrosservicoAdd): Observable<HttpResponse<any>> {
    const url = `${this.api}/add`;
    return this.http.post(url, JSON.stringify(microsservicoAdd), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  update(
    id: number,
    microsservicoUpdate: MicrosservicoUpdate,
  ): Observable<any> {
    const url = `${this.api}/update/${id}`;
    return this.http.put(url, JSON.stringify(microsservicoUpdate), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  override findById(id: number): Observable<Microsservico> {
    const url = `${this.api}/find-by-id-async/${id}`;
    return this.http.get<Microsservico>(url, this.httpOptions);
  }

  override delete(id: number): Observable<any> {
    const url = `${this.api}/delete/${id}`;
    return this.http.delete(url, this.httpOptions);
  }

  recuperarIdMicroserviceSelecionado(
    microserviceFormControl: FormControl<string | Microsservico | null>,
  ): number {
    let obj = microserviceFormControl.value;
    const idObj = typeof obj === 'string' ? obj : obj?.id;
    if (typeof idObj !== 'string') {
      return idObj;
    }
    return idObj.length === 0 ? null : 0;
  }

  recuperarFilteredOptions(
    microserviceFormControl: FormControl<string | Microsservico | null>,
    microserviceOptions: Microsservico[],
  ) {
    return microserviceFormControl.valueChanges.pipe(
      startWith(''),
      map((value) => {
        const name = typeof value === 'string' ? value : value?.noMicrosservico;
        return name
          ? this._microserviceFilter(name as string, microserviceOptions)
          : microserviceOptions.slice();
      }),
    );
  }

  private _microserviceFilter(
    name: string,
    microserviceOptions: Microsservico[],
  ): Microsservico[] {
    const filterValue = name.toLowerCase();
    return microserviceOptions.filter((option) =>
      option.noMicrosservico.toLowerCase().includes(filterValue),
    );
  }
}
