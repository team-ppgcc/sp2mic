import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { SharedModule } from '../../shared/shared.module';
import { MaterialModule } from '../../material.module';
import { HttpClientModule } from '@angular/common/http';
import { FileUploadedComponent } from './file-uploaded/file-uploaded.component';
import { SendProcedureFileComponent } from './file-uploaded/send-procedure-file/send-procedure-file.component';
import { DatabaseConnectionComponent } from './database-connection/database-connection.component';
import { CargaComponent } from './carga.component';

@NgModule({
  declarations: [
    CargaComponent,
    DatabaseConnectionComponent,
    FileUploadedComponent,
    SendProcedureFileComponent,
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
      {
        path: '',
        component: CargaComponent,
      },
      {
        path: 'file-uploaded',
        component: FileUploadedComponent,
      },
      {
        path: 'send-procedure-file',
        component: SendProcedureFileComponent,
      },
      {
        path: 'database-connection',
        component: DatabaseConnectionComponent,
      },
    ]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CargaModule {}
