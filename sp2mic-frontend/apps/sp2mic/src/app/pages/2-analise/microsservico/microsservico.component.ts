import { Component, OnInit, ViewChild } from '@angular/core';
import {
  MatPaginator,
  MatPaginatorIntl,
  PageEvent,
} from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { UtilsHtml } from '../../../shared/util/utils-html';
import { MsgService } from '../../../shared/services/msg.service';
import { MicrosservicoService } from './microsservico.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { Microsservico } from '../../../shared/models/microsservico';
import { MatSort } from '@angular/material/sort';
import { take, takeUntil } from 'rxjs/operators';
import { FormControl, NgForm } from '@angular/forms';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import isNil from 'lodash/isNil';
import { Observable } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { PaginatorIntl } from '../../../shared/services/paginator-intl.service';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-microsservico',
  templateUrl: './microsservico.component.html',
  styleUrls: ['./microsservico.component.scss'],
  providers: [{ provide: MatPaginatorIntl, useClass: PaginatorIntl }],
})
export class MicrosservicoComponent extends Unsub implements OnInit {
  servidorDisponivel = false;
  microserviceFormControl = new FormControl<string | Microsservico>('');
  microserviceOptions: Microsservico[] = [];
  microserviceFilteredOptions: Observable<Microsservico[]>;

  public customClass: string = 'custom-class';
  public microsservicoFilter: Microsservico;
  public microsservicos: Microsservico[] = [];
  public pesquisou = false;
  public expandiu = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public displayedColumns: string[] = [
    'snProntoParaGerar',
    'noMicrosservico',
    'qtdEndpoints',
    'acao',
  ];
  public dataSource: MatTableDataSource<Microsservico>;
  public pageEvent: PageEvent;
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;

  constructor(
    private router: Router,
    private microsservicoService: MicrosservicoService,
    private msgService: MsgService,
    private dialog: MatDialog,
  ) {
    super();
    this.dataSource = new MatTableDataSource(this.microsservicos);
  }

  ngOnInit(): void {
    this.microsservicoFilter = new Microsservico();
    const apiData = this.microsservicoService.ping();
    const apiObserver = {
      next: () => {
        this.servidorDisponivel = true;
        this.microserviceFormControl.enable();
        this.recuperarMicrosservicos();
        this.pesquisou = false;
        //this.expandiu = true;
        this.buscar();
      },
      error: () => {
        this.servidorDisponivel = false;
        this.microserviceFormControl.disable();
        this.msgService.error('Service unavailable.');
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  recuperarMicrosservicos() {
    const apiData = this.microsservicoService.findByFilter(null);
    const apiObserver = {
      next: (res: Microsservico[]) => {
        this.microserviceOptions = res;
        this.microserviceFilteredOptions =
          this.microsservicoService.recuperarFilteredOptions(
            this.microserviceFormControl,
            this.microserviceOptions,
          );
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar microsservicos: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  guardarResultado(microsservicosRetornados: Microsservico[]) {
    this.microsservicos = microsservicosRetornados;
    this.dataSource = new MatTableDataSource(this.microsservicos);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  buscar() {
    let microsservicoFiltro = new Microsservico();
    microsservicoFiltro.id =
      this.microsservicoService.recuperarIdMicroserviceSelecionado(
        this.microserviceFormControl,
      );
    microsservicoFiltro.snProntoParaGerar = isNil(
      this.microsservicoFilter.snProntoParaGerar,
    )
      ? null
      : this.microsservicoFilter.snProntoParaGerar;
    const apiData = this.microsservicoService.findByFilter(microsservicoFiltro);
    const apiObserver = {
      next: (microsservicosRetornados: Microsservico[]) => {
        this.guardarResultado(microsservicosRetornados);
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar microsservicos: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
    this.pesquisou = true;
    //this.expandiu = false; TODO para ficar sempre aberto o filtro
  }

  microserviceDisplayFn(obj: Microsservico): string {
    return obj && obj.noMicrosservico ? obj.noMicrosservico : '';
  }

  inserirMicrosservico(): void {
    this.router.navigate(['/analysis/microsservico/edit']);
  }

  limpar(form: NgForm) {
    this.pesquisou = false;
    //this.expandiu = true;
    form.reset();
    this.microserviceFormControl.reset();
    this.microsservicoFilter = new Microsservico();
    this.microsservicos = [];
    this.dataSource = new MatTableDataSource(this.microsservicos);
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  excluir(id: number) {
    const apiObserver = {
      next: () => {
        this.msgService.success('Microsservico successfully deleted.');
        this.buscar();
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir microsservico: ', erro);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.microsservicoService.delete(id).pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  confirmDialog(id: number, noMicrosservico: string): void {
    const mensagem = `Do you really want to delete ${noMicrosservico}?`;
    const dialogData = new ConfirmDialogModel('Confirm Delete', mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData,
    });
    dialogRef.afterClosed().pipe(takeUntil(this.unsubscribe$)).subscribe((dialogResult) => {
      if (dialogResult) {
        this.excluir(id);
      }
    });
  }

  rotuloBoolean(campo: boolean) {
    return campo ? 'Yes' : 'No';
  }
}
