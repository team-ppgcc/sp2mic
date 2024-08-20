import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { MaterialModule } from '../../../material.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { VisualizarEndpointComponent } from './visualizar-endpoint/visualizar-endpoint.component';
import { ManterEndpointComponent } from './manter-endpoint/manter-endpoint.component';
import { EndpointComponent } from './endpoint.component';

@NgModule({
  declarations: [
    EndpointComponent,
    ManterEndpointComponent,
    VisualizarEndpointComponent,
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    DragDropModule,
    HttpClientModule,

    RouterModule.forChild([
      { path: '', component: EndpointComponent },
      { path: 'edit', component: ManterEndpointComponent },
      {
        path: 'edit/:id/:origem',
        component: ManterEndpointComponent,
      },
      {
        path: 'view/:id',
        component: VisualizarEndpointComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class EndpointModule {}
