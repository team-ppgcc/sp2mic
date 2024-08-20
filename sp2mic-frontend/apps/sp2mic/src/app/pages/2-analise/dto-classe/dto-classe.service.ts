import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { DtoClasse } from '../../../shared/models/dto-classe';
import { Observable } from 'rxjs';
import { UtilsService } from '../../../shared/util/utils.service';
import { GenericService } from '../../../shared/services/generic-service';
import { DtoClasseUpdateDto } from '../../../shared/models/dto-classe-update-dto';
import { ComboBoxDto } from '../../../shared/models/combo-box-dto';
import { FormControl } from '@angular/forms';
import { map, startWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DtoClasseService extends GenericService {
  public override api = `${environment.apiUrl}/analysis/dto-classes`;

  constructor(public override http: HttpClient) {
    super(http);
  }

  ping(): Observable<any> {
    const url = `${this.api}/ping`;
    return this.http.get(url, this.httpOptions);
  }

  findByIdProcedureForCombo(idProcedure: number): Observable<ComboBoxDto[]> {
    const url = `${this.api}/find-by-id-procedure-for-combo/${idProcedure}`;
    return this.http.get<ComboBoxDto[]>(url, this.httpOptions);
  }

  getAll(): Observable<DtoClasse[]> {
    const url = `${this.api}/find-all-async`;
    return this.http.get<DtoClasse[]>(url, this.httpOptions);
  }

  public findByFilter(filtrosClasse?: DtoClasse): Observable<DtoClasse[]> {
    const url = `${this.api}/find-by-filter-async`;
    const queryParams: HttpParams =
      UtilsService.buildQueryParams(filtrosClasse);
    return this.http.get<DtoClasse[]>(url, {
      headers: this.headers,
      params: queryParams,
    });
  }

  add(dto: DtoClasse): Observable<any> {
    const url = `${this.api}/add`;
    return this.http.post(url, JSON.stringify(dto), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  update(id: number, dto: DtoClasseUpdateDto): Observable<any> {
    const url = `${this.api}/update/${id}`;
    return this.http.put(url, JSON.stringify(dto), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  override findById(id: number): Observable<any> {
    const url = `${this.api}/find-by-id-async/${id}`;
    return this.http.get(url, this.httpOptions);
  }

  override delete(id: number): Observable<any> {
    const url = `${this.api}/delete/${id}`;
    return this.http.delete(url, this.httpOptions);
  }

  recuperarFilteredOptions(
    dtoClasseFormControl: FormControl<string | DtoClasse | null>,
    dtoClasseOptions: DtoClasse[],
  ) {
    return dtoClasseFormControl.valueChanges.pipe(
      startWith(''),
      map((value) => {
        const name = typeof value === 'string' ? value : value?.noDtoClasse;
        return name
          ? this._dtoClasseFilter(name as string, dtoClasseOptions)
          : dtoClasseOptions.slice();
      }),
    );
  }

  recuperarIdDtoClasseSelecionada(
    dtoClasseFormControl: FormControl<string | DtoClasse | null>,
  ): number {
    let obj = dtoClasseFormControl.value;
    const idObj = typeof obj === 'string' ? obj : obj?.id;
    if (typeof idObj !== 'string') {
      return idObj;
    }
    return idObj.length === 0 ? null : 0;
  }

  private _dtoClasseFilter(
    name: string,
    dtoClasseOptions: DtoClasse[],
  ): DtoClasse[] {
    const filterValue = name.toLowerCase();
    return dtoClasseOptions.filter((option) =>
      option.noDtoClasse.toLowerCase().includes(filterValue),
    );
  }
}
