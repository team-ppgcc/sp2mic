import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { MaterialModule } from '../../../material.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MicrosservicoComponent } from './microsservico.component';
import { ManterMicrosservicoComponent } from './manter-microsservico/manter-microsservico.component';

@NgModule({
  declarations: [MicrosservicoComponent, ManterMicrosservicoComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    DragDropModule,
    HttpClientModule,

    RouterModule.forChild([
      { path: '', component: MicrosservicoComponent },
      {
        path: 'edit',
        component: ManterMicrosservicoComponent,
      },
      {
        path: 'edit/:id',
        component: ManterMicrosservicoComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class MicrosservicoModule {}
