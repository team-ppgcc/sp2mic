import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ErrorInterceptor implements HttpInterceptor {
  constructor() {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((err: HttpErrorResponse) => {
        let mensagem = err.error;

        if (err.status === 401) {
          mensagem = err?.error?.message;
          if (mensagem === null || mensagem === undefined || mensagem === '') {
            mensagem = 'Usuário não autorizado.';
          }
        } else {
          if (err.status === 403) {
            mensagem = 'Acesso Negado.';
          }
        }
        return throwError({ ...err, error: mensagem });
      }),
    );
  }
}
