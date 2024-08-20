import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-material-confirm-dialog-database-schema',
  templateUrl: './material-confirm-dialog-database-schema.html',
  styleUrls: ['./material-confirm-dialog.component.scss']
})
export class MaterialConfirmDialogDatabaseSchemaComponent {
  mensagem: string;
  databaseSchema: string;

  constructor(
    public dialogRef: MatDialogRef<MaterialConfirmDialogDatabaseSchemaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogDatabaseSchemaModel) {
    this.databaseSchema = 'dbo';
  }

  confirmar(): string {
    // Confirma com retorno true
    this.dialogRef.close({ databaseSchema: this.databaseSchema });
    return this.databaseSchema;
  }

  cancelar(): void {
    // Cancela com retorno false
    this.dialogRef.close(false);
  }

  retirarEspacosEmBrancoNoSchema() {
    if (this.databaseSchema) {
      this.databaseSchema =  this.databaseSchema.replace(/\s/g, '');
    }
  }
}

export class ConfirmDialogDatabaseSchemaModel {

  constructor(
    public databaseSchema: string,
  ) { }

}
