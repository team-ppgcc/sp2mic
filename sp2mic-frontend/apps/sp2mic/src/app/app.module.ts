import { LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { MaterialModule } from './material.module';
import { HomeModule } from './pages/home/home.module';
import { DtoClasseModule } from './pages/2-analise/dto-classe/dto-classe.module';
import { EndpointModule } from './pages/2-analise/endpoint/endpoint.module';
import { MicrosservicoModule } from './pages/2-analise/microsservico/microsservico.module';
import { StoredProcedureModule } from './pages/2-analise/stored-procedure/stored-procedure.module';
import { GeracaoModule } from './pages/3-geracao/geracao.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { MatMenuModule } from '@angular/material/menu';
import { MatLineModule } from '@angular/material/core';
import { VariavelModule } from './pages/2-analise/variavel/variavel.module';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoadingInterceptor } from './shared/interceptor/loading.interceptor';
import { NgOptimizedImage } from '@angular/common';
import { CargaModule } from './pages/1-carga/carga.module';

@NgModule({
  declarations: [
    AppComponent,
    //CountDownComponent,
    //ActionSnackBarComponent
  ],
  imports: [
    MaterialModule,
    SharedModule,
    HomeModule,
    CargaModule,
    DtoClasseModule,
    EndpointModule,
    MicrosservicoModule,
    StoredProcedureModule,
    VariavelModule,
    GeracaoModule,
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    NgxSpinnerModule,
    ReactiveFormsModule,
    FormsModule,
    MatMenuModule,
    MatLineModule,
    NgOptimizedImage,
  ],
  providers: [
    {
      provide: LOCALE_ID,
      useValue: 'pt-BR',
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
