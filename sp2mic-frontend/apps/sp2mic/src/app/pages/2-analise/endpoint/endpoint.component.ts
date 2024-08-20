import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { UtilsHtml } from '../../../shared/util/utils-html';
import { MsgService } from '../../../shared/services/msg.service';
import { EndpointService } from './endpoint.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { UtilsEnum } from '../../../shared/util/utils-enum';
import { MatSort } from '@angular/material/sort';
import { takeUntil } from 'rxjs/operators';
import * as _ from 'lodash';
import { NgForm } from '@angular/forms';
import { StoredProcedureService } from '../stored-procedure/stored-procedure.service';
import { Microsservico } from '../../../shared/models/microsservico';
import { MicrosservicoService } from '../microsservico/microsservico.service';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import { EndpointListagem } from '../../../shared/models/endpoint-listagem';
import { StoredProcedureListagem } from '../../../shared/models/stored-procedure-listagem';
import { EndpointFilter } from '../../../shared/models/endpoint-filter';
import { HttpErrorResponse } from '@angular/common/http';
import { ComboBoxDto } from '../../../shared/models/combo-box-dto';
import { PaginatorIntl } from '../../../shared/services/paginator-intl.service';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-endpoint',
  templateUrl: './endpoint.component.html',
  styleUrls: ['./endpoint.component.scss'],
  providers: [{ provide: MatPaginatorIntl, useClass: PaginatorIntl }],
})
export class EndpointComponent extends Unsub implements OnInit {
  public endpointFilter: EndpointFilter;
  public endpoints: EndpointListagem[] = [];
  public pesquisou = false;
  public expandiu = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  // public displayedColumns: string[] = ['snAnalisado', 'noStoredProcedure', 'noMicrosservico', 'noMetodoEndpoint', 'noPath', 'noTipoDadoRetorno',
  // 'noVariavelRetornada', 'snRetornoLista', 'noTipoSqlDml', 'acao'];
  public displayedColumns: string[] = [
    'snAnalisado',
    'noStoredProcedure',
    'noMicrosservico',
    'noMetodoEndpoint',
    'noPath',
    'noTipoDadoRetorno',
    'noTipoSqlDml',
    'tabelasAssociadas',
    'acao',
  ];
  public dataSourceEndpoints: MatTableDataSource<EndpointListagem>;
  //public pageEvent: PageEvent;
  public valoresComboProcedure: StoredProcedureListagem[] = [];
  public valoresComboMicrosservico: Microsservico[] = [];
  public valoresComboTipoDado: ComboBoxDto[];
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;

  constructor(
    private router: Router, //private route: ActivatedRoute,
    private endpointService: EndpointService, //private dtoClasseService: DtoClasseService,
    private msgService: MsgService, //private util: UtilsService,
    private dialog: MatDialog,
    private procedureService: StoredProcedureService,
    private microsservicoService: MicrosservicoService,
  ) {
    super();
    this.dataSourceEndpoints = new MatTableDataSource(this.endpoints);
  }

  ngOnInit(): void {
    //this.dialogRef = Utils.abrirBloqueioTela('Wait...', this.msgService);
    this.recuperarProcedures();
    this.recuperarMicrosservicos();
    this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
    this.pesquisou = false;
    //this.expandiu = true;
    this.endpointFilter = new EndpointFilter();
    this.endpointFilter.idStoredProcedure = history.state.idStoredProcedure;
    this.buscar();
  }

  recuperarProcedures() {
    const apiData = this.procedureService.findByFilter(null);
    const apiObserver = {
      next: (res: StoredProcedureListagem[]) => {
        this.valoresComboProcedure = res;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar stored procedures: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  guardarResultado(endpointsRetornados: EndpointListagem[]) {
    this.endpoints = endpointsRetornados;
    this.endpoints = _.orderBy(
      this.endpoints,
      [(i) => i.noMetodoEndpoint?.toLocaleLowerCase()],
      ['asc'],
    );
    this.dataSourceEndpoints = new MatTableDataSource(this.endpoints);
    this.dataSourceEndpoints.paginator = this.paginator;
    this.dataSourceEndpoints.sort = this.sort;
    //this.configurarNomeTipoDado();
  }

  buscar() {
    const apiData = this.endpointService.findByFilter(this.endpointFilter);
    const apiObserver = {
      next: (endpointsRetornados: EndpointListagem[]) => {
        this.guardarResultado(endpointsRetornados);
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar endpoints: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
    this.pesquisou = true;
    //this.expandiu = false; TODO para ficar sempre aberto o filtro
  }

  limpar(form: NgForm) {
    this.pesquisou = false;
    //this.expandiu = true;
    form.reset();
    this.endpointFilter = new EndpointFilter();
    this.endpoints = [];
    this.dataSourceEndpoints = new MatTableDataSource(this.endpoints);
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  // configurarNomeTipoDado(): void {
  //   this.endpoints.forEach((endpoint) => {
  //     endpoint.noTipoDadoRetorno = this.rotuloTipoDado(
  //       endpoint.coTipoDadoRetorno,
  //     );
  //   });
  // }

  // rotuloTipoDado(tipo: any): string {
  //   return UtilsEnum.retornaRotuloTipoDado(tipo);
  // }

  // rotuloTipoSqlDml(tipo: any): string {
  //   return UtilsEnum.retornaRotuloTipoSqlDml(tipo);
  // }

  excluir(id: number) {
    const apiObserver = {
      next: () => {
        this.msgService.success('Endpoint successfully deleted.');
        this.buscar();
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir endpoint: ', erro);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.endpointService
      .delete(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(apiObserver);
  }

  confirmDialog(id: number, noMetodoEndpoint: string): void {
    const mensagem = `Do you really want to delete ${noMetodoEndpoint}?`;
    const dialogData = new ConfirmDialogModel('Confirm Delete', mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData,
    });
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((dialogResult) => {
        if (dialogResult) {
          this.excluir(id);
        }
      });
  }

  inserirEndpoint(): void {
    this.router.navigate(['/analysis/endpoint/edit']);
  }

  rotuloBoolean(campo: boolean) {
    return campo ? 'Yes' : 'No';
  }

  avisoDialog(): void {
    const message = `Functionality not yet implemented..`;
    const dialogData = new ConfirmDialogModel('Warning', message);
    this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData,
    });
  }

  // getRetornoVariavel(ep: Endpoint): string {
  //   if (ep.idVariavelRetornadaNavigation === undefined) {
  //     return '-';
  //   }
  //   return ep.idVariavelRetornadaNavigation.noVariavel;
  // }

  // getRetorno(ep: Endpoint): string {
  //   if (
  //     ep.coTipoDadoRetorno === TipoDadoEnum.DTO &&
  //     ep.idDtoClasseNavigation !== undefined
  //   ) {
  //     if (ep.idDtoClasseNavigation.noDtoClasse === 'NomeAindaNaoDefinido') {
  //       return '';
  //     }
  //     return ep.idDtoClasseNavigation.noDtoClasse;
  //   }
  //   return this.rotuloTipoDado(ep.coTipoDadoRetorno);
  // }

  retornar() {
    if (history.state.origem === 'Search') {
      this.router.navigate(['/analysis/stored-procedure']);
    } else if (history.state.origem === 'Detail') {
      this.router.navigate([
        '/analysis/stored-procedure/edit/' +
          this.endpointFilter.idStoredProcedure,
      ]);
    } else {
      this.router.navigate(['/home']);
    }
  }

  private recuperarMicrosservicos() {
    const apiData = this.microsservicoService.findByFilter(new Microsservico());
    const apiObserver = {
      next: (res: Microsservico[]) => {
        this.valoresComboMicrosservico = res;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar microsservicos: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }
}
