import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { MaterialModule } from '../../material.module';
import { GerarMicrosservicosComponent } from './gerar-microsservicos/gerar-microsservicos.component';

@NgModule({
  declarations: [GerarMicrosservicosComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    DragDropModule,
    HttpClientModule,

    RouterModule.forChild([
      {
        path: '',
        component: GerarMicrosservicosComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class GeracaoModule {}
