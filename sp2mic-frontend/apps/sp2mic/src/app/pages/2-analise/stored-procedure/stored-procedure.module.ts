import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { MaterialModule } from '../../../material.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { VisualizarStoredProcedureComponent } from './visualizar-procedure/visualizar-stored-procedure.component';
import { ManterProcedureComponent } from './manter-procedure/manter-procedure.component';
import { StoredProcedureComponent } from './stored-procedure.component';

@NgModule({
  declarations: [
    StoredProcedureComponent,
    ManterProcedureComponent,
    VisualizarStoredProcedureComponent,
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
      { path: '', component: StoredProcedureComponent },
      { path: 'edit', component: ManterProcedureComponent },
      {
        path: 'edit/:id',
        component: ManterProcedureComponent,
      },
      {
        path: 'view/:id',
        component: VisualizarStoredProcedureComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class StoredProcedureModule {}
