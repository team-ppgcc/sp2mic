import {
  HttpClient,
  HttpEvent,
  HttpEventType,
  HttpResponse,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { GeracaoDto } from '../../shared/models/geracao-dto';
import { GenericService } from '../../shared/services/generic-service';
import { MsgService } from '../../shared/services/msg.service';

@Injectable({
  providedIn: 'root',
})
export class GeracaoService extends GenericService {
  public override api = `${environment.apiUrl}/generation`;

  constructor(
    public override http: HttpClient,
    private msgService: MsgService,
  ) {
    super(http);
  }

  ping(): Observable<any> {
    const url = `${this.api}/ping`;
    return this.http.get(url, this.httpOptions);
  }

  gerarMicrosservicos(dto: GeracaoDto): Observable<HttpResponse<any>> {
    const url = `${this.api}/projetos`;
    return this.http.post(url, JSON.stringify(dto), {
      ...this.httpOptions,
      observe: 'response',
    });
  }

  download = (fileName: string) => {
    this.seviceDownload(fileName).subscribe((event) => {
      if (event.type === HttpEventType.Response) {
        if (event.ok) {
          console.log(`download ok fileName ${fileName}`);
          this.downloadFile(event, fileName);
        } else {
          this.msgService.error('Download error.');
        }
      }
    });
  };

  private downloadFile = (data: HttpResponse<Blob>, fileName: string) => {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = fileName;
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  };

  private seviceDownload(fileName: string): Observable<HttpEvent<Blob>> {
    const url = `${this.api}/download?fileName=${fileName}`;
    return this.http.get(url, {
      ...this.httpOptions,
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }
}
