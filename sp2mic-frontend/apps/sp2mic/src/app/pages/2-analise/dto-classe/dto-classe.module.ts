import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { MaterialModule } from '../../../material.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { DtoClasseComponent } from './dto-classe.component';
import { ManterDtoClasseComponent } from './manter-dto-classe/manter-dto-classe.component';
import { VisualizarEndpointComponent } from '../endpoint/visualizar-endpoint/visualizar-endpoint.component';
import { VisualizarDtoClasseComponent } from './visualizar-dto-classe/visualizar-dto-classe.component';

@NgModule({
  declarations: [DtoClasseComponent, ManterDtoClasseComponent, VisualizarDtoClasseComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    DragDropModule,
    HttpClientModule,

    RouterModule.forChild([
      { path: '', component: DtoClasseComponent },
      {
        path: 'edit',
        component: ManterDtoClasseComponent,
      },
      {
        path: 'edit/:id',
        component: ManterDtoClasseComponent,
      },
      {
        path: 'view/:id',
        component: VisualizarDtoClasseComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DtoClasseModule {}
