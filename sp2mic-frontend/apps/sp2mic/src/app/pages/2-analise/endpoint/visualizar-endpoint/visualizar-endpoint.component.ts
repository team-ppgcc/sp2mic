import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Endpoint } from '../../../../shared/models/endpoint';
import { EndpointService } from '../endpoint.service';
import { MsgService } from '../../../../shared/services/msg.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { Unsub } from '../../../../shared/util/unsub.class';

@Component({
  selector: 'app-visualizar-endpoint',
  templateUrl: './visualizar-endpoint.component.html',
  styleUrls: ['./visualizar-endpoint.component.scss'],
})
export class VisualizarEndpointComponent extends Unsub implements OnInit {
  public endpoint: Endpoint;
  public expandiu = true;

  constructor(
    private activatedRoute: ActivatedRoute,
    private endpointService: EndpointService,
    private msgService: MsgService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super();
  }

  ngOnInit(): void {
    if (this.data.modal) {
      this.recuperarEndpoint(this.data.idEndpoint);
      this.endpoint.txEndpointTratado = '\n' + this.endpoint.txEndpointTratado;
    } else {
      this.activatedRoute.paramMap.pipe(takeUntil(this.unsubscribe$)).subscribe((param) => {
        let id = param.get('id');
        if (id) {
          this.recuperarEndpoint(id);
        }
      });
    }
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  recuperarEndpoint(id: any) {
    const apiData = this.endpointService.findById(id);
    const apiObserver = {
      next: (res: Endpoint) => {
        this.endpoint = res;
        this.endpoint.txEndpointTratado = '\n' + this.endpoint.txEndpointTratado;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar endpoint: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  close(): void {
    this.dialog.closeAll();
  }
}
