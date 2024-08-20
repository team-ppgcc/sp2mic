import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { MaterialModule } from '../../../material.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { VariavelComponent } from './variavel.component';
import { ManterVariavelComponent } from './manter-variavel/manter-variavel.component';

@NgModule({
  declarations: [VariavelComponent, ManterVariavelComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    DragDropModule,
    HttpClientModule,

    RouterModule.forChild([
      { path: '', component: VariavelComponent },
      {
        path: 'edit',
        component: ManterVariavelComponent,
      },
      {
        path: 'edit/:id',
        component: ManterVariavelComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class VariavelModule {}
