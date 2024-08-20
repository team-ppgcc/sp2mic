import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UtilsService } from '../../../shared/util/utils.service';
import { StoredProcedure } from '../../../shared/models/stored-procedure';
import { GenericService } from '../../../shared/services/generic-service';
import { StoredProcedureUpdate } from '../../../shared/models/stored-procedure-update';
import { StoredProcedureFilter } from '../../../shared/models/stored-procedure-filter';
import { StoredProcedureListagem } from '../../../shared/models/stored-procedure-listagem';
import { StoredProcedureView } from '../../../shared/models/stored-procedure-view';
import { FormControl } from '@angular/forms';
import { map, startWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class StoredProcedureService extends GenericService {
  public override api = `${environment.apiUrl}/analysis/stored-procedures`;

  constructor(public override http: HttpClient) {
    super(http);
  }

  ping(): Observable<any> {
    const url = `${this.api}/ping`;
    return this.http.get(url, this.httpOptions);
  }

  findByFilter(
    filtrosStoredProcedure?: StoredProcedureFilter,
  ): Observable<StoredProcedureListagem[]> {
    const url = `${this.api}/find-by-filter-async`;
    const queryParams: HttpParams = UtilsService.buildQueryParams(
      filtrosStoredProcedure,
    );
    return this.http.get<StoredProcedureListagem[]>(url, {
      headers: this.headers,
      params: queryParams,
    });
  }

  update(
    id: number,
    storedProcedureUpdate: StoredProcedureUpdate,
  ): Observable<any> {
    const url = `${this.api}/update/${id}`;
    return this.http.put(url, JSON.stringify(storedProcedureUpdate), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  override findById(id: number): Observable<StoredProcedure> {
    const url = `${this.api}/find-by-id-async/${id}`;
    return this.http.get<StoredProcedure>(url, this.httpOptions);
  }

  getDefinicaoById(id: number): Observable<StoredProcedureView> {
    const url = `${this.api}/get-definicao-by-id/${id}`;
    return this.http.get<StoredProcedureView>(url, this.httpOptions);
  }

  override delete(id: number): Observable<any> {
    const url = `${this.api}/delete/${id}`;
    return this.http.delete(url, this.httpOptions);
  }

  recuperarFilteredOptions(
    storedProcedureFormControl: FormControl<
      string | StoredProcedureListagem | null
    >,
    storedProcedureOptions: StoredProcedureListagem[],
  ) {
    return storedProcedureFormControl.valueChanges.pipe(
      startWith(''),
      map((value) => {
        const name =
          typeof value === 'string' ? value : value?.noStoredProcedure;
        return name
          ? this._storedProcedureFilter(name as string, storedProcedureOptions)
          : storedProcedureOptions.slice();
      }),
    );
  }

  recuperarIdStoredProcedureSelecionada(
    storedProcedureFormControl: FormControl<
      string | StoredProcedureListagem | null
    >,
  ): number {
    let obj = storedProcedureFormControl.value;
    const idObj = typeof obj === 'string' ? obj : obj?.id;
    if (typeof idObj !== 'string') {
      return idObj;
    }
    return idObj.length === 0 ? null : 0;
  }

  private _storedProcedureFilter(
    name: string,
    storedProcedureOptions: StoredProcedureListagem[],
  ): StoredProcedureListagem[] {
    const filterValue = name.toLowerCase();
    return storedProcedureOptions.filter((option) =>
      option.noStoredProcedure.toLowerCase().includes(filterValue),
    );
  }
}
