import { Component, OnInit, ViewChild } from '@angular/core';
import {
  MatPaginator,
  MatPaginatorIntl,
  PageEvent,
} from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import {
  MatDialog,
  MatDialogConfig,
  MatDialogRef,
} from '@angular/material/dialog';
import { UtilsHtml } from '../../../shared/util/utils-html';
import { MsgService } from '../../../shared/services/msg.service';
import { StoredProcedureService } from './stored-procedure.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { MatSort } from '@angular/material/sort';
import { take, takeUntil } from 'rxjs/operators';
import { FormControl, NgForm } from '@angular/forms';
import { UtilsEnum } from '../../../shared/util/utils-enum';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import { VisualizarStoredProcedureComponent } from './visualizar-procedure/visualizar-stored-procedure.component';
import { Observable } from 'rxjs';
import { StoredProcedureListagem } from '../../../shared/models/stored-procedure-listagem';
import { StoredProcedureFilter } from '../../../shared/models/stored-procedure-filter';
import { ComboBoxDto } from '../../../shared/models/combo-box-dto';
import { HttpErrorResponse } from '@angular/common/http';
import { PaginatorIntl } from '../../../shared/services/paginator-intl.service';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-stored-procedure',
  templateUrl: './stored-procedure.component.html',
  styleUrls: ['./stored-procedure.component.scss'],
  providers: [{ provide: MatPaginatorIntl, useClass: PaginatorIntl }],
})
export class StoredProcedureComponent extends Unsub implements OnInit {
  servidorDisponivel = false;
  storedProcedureFormControl = new FormControl<
    string | StoredProcedureListagem
  >('');
  storedProcedureOptions: StoredProcedureListagem[] = [];
  storedProcedureFilteredOptions: Observable<StoredProcedureListagem[]>;
  public spFilter: StoredProcedureFilter = new StoredProcedureFilter();
  public storedProcedures: StoredProcedureListagem[] = [];
  public pesquisou = false;
  public expandiu = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public displayedColumns: string[] = [
    'noSucessoParser',
    'snAnalisada',
    'noSchema',
    'noStoredProcedure',
    'noTipoDadoRetorno',
    'qtdEndpoints',
    'tabelasAssociadas',
    'acao',
  ];
  public dataSourceStoredProcedures: MatTableDataSource<StoredProcedureListagem>;
  public pageEvent: PageEvent;
  public valoresComboTipoDado: ComboBoxDto[];
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;

  constructor(
    private router: Router,
    private storedProcedureService: StoredProcedureService,
    private msgService: MsgService,
    private dialog: MatDialog,
  ) {
    super();
    this.dataSourceStoredProcedures = new MatTableDataSource(
      this.storedProcedures,
    );
  }

  storedProcedureDisplayFn(sp: StoredProcedureListagem): string {
    return sp && sp.noStoredProcedure ? sp.noStoredProcedure : '';
  }

  //
  // private _storedProcedureFilter(name: string): StoredProcedureListagem[] {
  //   const filterValue = name.toLowerCase();
  //   return this.storedProcedureOptions.filter((option) =>
  //     option.noStoredProcedure.toLowerCase().includes(filterValue),
  //   );
  // }

  ngOnInit(): void {
    this.spFilter = new StoredProcedureFilter();
    const apiData = this.storedProcedureService.ping();
    const apiObserver = {
      next: () => {
        this.servidorDisponivel = true;
        this.storedProcedureFormControl.enable();
        this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
        this.recuperarStoredProcedures();
        this.pesquisou = false;
        //this.expandiu = true;
        this.buscar();
      },
      error: () => {
        this.servidorDisponivel = false;
        this.storedProcedureFormControl.disable();
        this.msgService.error('Service unavailable.');
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  recuperarStoredProcedures() {
    const apiData = this.storedProcedureService.findByFilter(null);
    const apiObserver = {
      next: (res: StoredProcedureListagem[]) => {
        this.storedProcedureOptions = res;
        this.storedProcedureFilteredOptions =
          this.storedProcedureService.recuperarFilteredOptions(
            this.storedProcedureFormControl,
            this.storedProcedureOptions,
          );
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar stored procedures: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  guardarResultado(storedProceduresRetornadas: StoredProcedureListagem[]) {
    this.storedProcedures = storedProceduresRetornadas;
    this.dataSourceStoredProcedures = new MatTableDataSource(
      this.storedProcedures,
    );
    this.dataSourceStoredProcedures.paginator = this.paginator;
    this.dataSourceStoredProcedures.sort = this.sort;
  }

  buscar() {
    this.spFilter.id =
      this.storedProcedureService.recuperarIdStoredProcedureSelecionada(
        this.storedProcedureFormControl,
      );

    const apiData = this.storedProcedureService.findByFilter(this.spFilter);
    const apiObserver = {
      next: (storedProceduresRetornados: StoredProcedureListagem[]) => {
        this.guardarResultado(storedProceduresRetornados);
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar Stored Procedures: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
    this.pesquisou = true;
    //this.expandiu = false; TODO para ficar sempre aberto o filtro
  }

  inserirStoredProcedure(): void {
    this.router.navigate(['/send-procedure-file']);
  }

  limpar(form: NgForm) {
    this.pesquisou = false;
    //this.expandiu = true;
    form.reset();
    this.storedProcedureFormControl.reset();
    this.spFilter = new StoredProcedureFilter();
    this.storedProcedures = [];
    this.dataSourceStoredProcedures = new MatTableDataSource(
      this.storedProcedures,
    );
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  excluir(id: number) {
    const apiObserver = {
      next: () => {
        this.msgService.success('Stored Procedure successfully deleted.');
        this.recuperarStoredProcedures();
        this.buscar();
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir Stored Procedure: ', erro);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.storedProcedureService.delete(id).pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  confirmDialog(id: number, noStoredProcedure: string): void {
    const mensagem = `Do you really want to delete ${noStoredProcedure}?`;
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

  abrirModalVisualizarProcedure(id: number) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '50%';
    dialogConfig.disableClose = false;
    dialogConfig.data = {
      modal: true,
      idStoredProcedure: id,
    };
    this.dialog.open(VisualizarStoredProcedureComponent, dialogConfig);
  }
}
