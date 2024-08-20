import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-material-confirm-dialog',
  templateUrl: './material-confirm-dialog.component.html',
  styleUrls: ['./material-confirm-dialog.component.scss'],
})
export class MaterialConfirmDialogComponent implements OnInit {
  titulo: string;
  mensagem: string;

  constructor(
    public dialogRef: MatDialogRef<MaterialConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogModel,
  ) {
    this.titulo = data.titulo;
    this.mensagem = data.mensagem;
  }

  ngOnInit(): void {}

  confirmar(): void {
    // Confirma com retorno true
    this.dialogRef.close(true);
  }

  cancelar(): void {
    // Cancela com retorno false
    this.dialogRef.close(false);
  }
}

export class ConfirmDialogModel {
  constructor(
    public titulo: string,
    public mensagem: string,
  ) {}
}
