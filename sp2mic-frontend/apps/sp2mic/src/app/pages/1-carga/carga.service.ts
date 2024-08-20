import { HttpClient, HttpResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { CargaDto } from '../../shared/models/carga-dto';
import { Injectable } from '@angular/core';
import { GenericService } from '../../shared/services/generic-service';
import { ParDto } from '../../shared/models/par-dto';

@Injectable({
  providedIn: 'root',
})
export class CargaService extends GenericService {
  public override api = `${environment.apiUrl}/load`;

  constructor(public override http: HttpClient) {
    super(http);
  }

  ping(): Observable<any> {
    const url = `${this.api}/ping`;
    return this.http.get(url, this.httpOptions);
  }

  public listarSupportedDatabase(): Observable<ParDto[]> {
    const url = this.api + '/supported-database';
    return this.http.get<ParDto[]>(url, this.httpOptions);
  }

  public listarNomesProcedures(
    dto: CargaDto,
  ): Observable<HttpResponse<ParDto>> {
    const url = this.api + '/procedures-names';
    return this.http.post<ParDto>(url, JSON.stringify(dto), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  public carregarProceduresSelecionadas(
    dto: CargaDto,
  ): Observable<HttpResponse<any>> {
    const url = this.api + '/load-procedures';
    return this.http.post(url, JSON.stringify(dto), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  /*public carregarProcedureSelecionada(dto: any, nomes: any) { //: Observable<HttpResponse<any>> {
   const url = this.api + '/CarregarProceduresSelecionadas';
   let obj = {dadosConexao: dto, nomesProcedures: nomes};
   //return this.http.post(url, JSON.stringify(obj), {...this.httpOptions, observe: 'response'});
   return this.instance.post(url, obj).then((res: any) => {
   return res;
   }).catch((err: any) => {
   return err;
   })
   }*/

  uploadFile(formData: FormData): Observable<any> {
    const url = `${this.api}/include-procedure-file`;
    //const formData: FormData = new FormData();
    ///formData.append('arquivo', arquivoSpInclusao);
    return this.http.post<any>(url, formData);
  }
}
