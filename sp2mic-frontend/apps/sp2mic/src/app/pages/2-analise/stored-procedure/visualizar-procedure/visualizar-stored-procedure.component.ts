import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MsgService } from '../../../../shared/services/msg.service';
import { StoredProcedureService } from '../stored-procedure.service';
import { take } from 'rxjs';
import { Utils } from '../../../../shared/util/utils';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { StoredProcedureView } from '../../../../shared/models/stored-procedure-view';
import { takeUntil } from 'rxjs/operators';
import { Unsub } from '../../../../shared/util/unsub.class';

@Component({
  selector: 'app-visualizar-stored-procedure',
  templateUrl: './visualizar-stored-procedure.component.html',
  styleUrls: ['./visualizar-stored-procedure.component.scss'],
})
export class VisualizarStoredProcedureComponent extends Unsub implements OnInit {
  public storedProcedure: StoredProcedureView = new StoredProcedureView();
  public expandiu = true;

  constructor(
    private activatedRoute: ActivatedRoute,
    private storedProceduresService: StoredProcedureService,
    private msgService: MsgService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super();
  }

  ngOnInit(): void {
    if (this.data.modal) {
      this.recuperarStoredProcedure(this.data.idStoredProcedure);
      this.storedProcedure.txDefinicaoTratada = '\n' + this.storedProcedure.txDefinicaoTratada;
    } else {
      this.activatedRoute.params.pipe(takeUntil(this.unsubscribe$)).subscribe((params) => {
        const id = Utils.readRouteParam(params, 'id');
        if (id) {
          this.recuperarStoredProcedure(id);
        }
      });
    }
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  recuperarStoredProcedure(id: any) {
    const apiData = this.storedProceduresService.getDefinicaoById(id);
    const apiObserver = {
      next: (res: StoredProcedureView) => {
        this.storedProcedure = res;
        this.storedProcedure.txDefinicaoTratada = '\n' + this.storedProcedure.txDefinicaoTratada;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar Stored Procedure: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  close(): void {
    this.dialog.closeAll();
  }
}
