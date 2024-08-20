import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { AtributoDto } from '../../../shared/models/atributo-dto';
import { Observable } from 'rxjs';
import { GenericService } from '../../../shared/services/generic-service';
import { AtributoUpdate } from '../../../shared/models/atributo-update';

@Injectable({
  providedIn: 'root',
})
export class AtributoService extends GenericService {
  public override api = `${environment.apiUrl}/analysis/atributos`;

  constructor(public override http: HttpClient) {
    super(http);
  }

  update(
    id: number,
    dto: AtributoUpdate,
  ): Observable<HttpResponse<AtributoDto>> {
    const url = `${this.api}/update/${id}`;
    return this.http.put<AtributoDto>(url, JSON.stringify(dto), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  override delete(id: number): Observable<any> {
    const url = `${this.api}/delete/${id}`;
    return this.http.delete(url, this.httpOptions);
  }

  getByIdClasse(idDtoClasse: number): Observable<AtributoDto[]> {
    const url = `${this.api}/atributos-by-id-dtoclasse/${idDtoClasse}`;
    return this.http.get<AtributoDto[]>(url, this.httpOptions);
  }
}
