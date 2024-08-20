import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DtoClasse } from '../../../../shared/models/dto-classe';

@Component({
  selector: 'app-visualizar-dto-classe',
  templateUrl: './visualizar-dto-classe.component.html',
  styleUrls: ['./visualizar-dto-classe.component.scss'],
})
export class VisualizarDtoClasseComponent implements OnInit {
  public dtoClasse: DtoClasse;
  public expandiu = true;

  constructor(
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}

  ngOnInit(): void {
    if (this.data.modal) {
      this.dtoClasse = this.data.dtoClasse;
      this.dtoClasse.txDtoClasse = '\n' + this.dtoClasse.txDtoClasse;
    }
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  close(): void {
    this.dialog.closeAll();
  }
}
