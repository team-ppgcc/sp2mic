import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    loadChildren: () =>
      import('./pages/home/home.module').then((m) => m.HomeModule),
  },
  {
    path: 'load',
    loadChildren: () =>
      import('./pages/1-carga/carga.module').then((m) => m.CargaModule),
  },
  {
    path: 'analysis/dto-classe',
    loadChildren: () =>
      import('./pages/2-analise/dto-classe/dto-classe.module').then(
        (m) => m.DtoClasseModule,
      ),
  },
  {
    path: 'analysis/endpoint',
    loadChildren: () =>
      import('./pages/2-analise/endpoint/endpoint.module').then(
        (m) => m.EndpointModule,
      ),
  },
  {
    path: 'analysis/microsservico',
    loadChildren: () =>
      import('./pages/2-analise/microsservico/microsservico.module').then(
        (m) => m.MicrosservicoModule,
      ),
  },
  {
    path: 'analysis/stored-procedure',
    loadChildren: () =>
      import('./pages/2-analise/stored-procedure/stored-procedure.module').then(
        (m) => m.StoredProcedureModule,
      ),
  },
  {
    path: 'generation/microsservicos',
    loadChildren: () =>
      import('./pages/3-geracao/geracao.module').then((m) => m.GeracaoModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
