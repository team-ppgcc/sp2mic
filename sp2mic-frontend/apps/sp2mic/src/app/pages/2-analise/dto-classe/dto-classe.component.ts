import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UtilsHtml } from '../../../shared/util/utils-html';
import { MsgService } from '../../../shared/services/msg.service';
//import { MicrosservicoService } from '../microsservico/microsservico.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
//import { Microsservico } from '../../../shared/models/microsservico';
import { DtoClasse } from '../../../shared/models/dto-classe';
import { MatSort } from '@angular/material/sort';
import { take, takeUntil } from 'rxjs/operators';
import { FormControl, NgForm } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { DtoClasseService } from './dto-classe.service';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import { StoredProcedureService } from '../stored-procedure/stored-procedure.service';
import { Observable } from 'rxjs';
import { StoredProcedureListagem } from '../../../shared/models/stored-procedure-listagem';
import { PaginatorIntl } from '../../../shared/services/paginator-intl.service';
//import { VisualizarEndpointComponent } from '../endpoint/visualizar-endpoint/visualizar-endpoint.component';
import { VisualizarDtoClasseComponent } from './visualizar-dto-classe/visualizar-dto-classe.component';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-dto-classe',
  templateUrl: './dto-classe.component.html',
  styleUrls: ['./dto-classe.component.scss'],
  providers: [{ provide: MatPaginatorIntl, useClass: PaginatorIntl }],
})
export class DtoClasseComponent extends Unsub implements OnInit {
  servidorDisponivel = false;
  dtoClasseFormControl = new FormControl<string | DtoClasse>('');
  dtoClasseOptions: DtoClasse[] = [];
  dtoClasseFilteredOptions: Observable<DtoClasse[]>;

  // microserviceFormControl = new FormControl<string | Microsservico>('');
  // microserviceOptions: Microsservico[] = [];
  // microserviceFilteredOptions: Observable<Microsservico[]>;

  storedProcedureFormControl = new FormControl<
    string | StoredProcedureListagem
  >('');
  storedProcedureOptions: StoredProcedureListagem[] = [];
  storedProcedureFilteredOptions: Observable<StoredProcedureListagem[]>;
  public dtoClasse: DtoClasse;
  public dtoClasses: DtoClasse[] = [];
  public pesquisou = false;
  public expandiu = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public displayedColumns: string[] = [
    'noDtoClasse',
    //'noMicrosservico',
    'noStoredProcedure',
    'acao',
  ];
  public dataSource: MatTableDataSource<DtoClasse>;
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;

  constructor(
    private dtoClasseService: DtoClasseService,
    //private microsservicoService: MicrosservicoService,
    private storedProcedureService: StoredProcedureService,
    private msgService: MsgService,
    private dialog: MatDialog,
  ) {
    super();
  }

  dtoClasseDisplayFn(obj: DtoClasse): string {
    return obj && obj.noDtoClasse ? obj.noDtoClasse : '';
  }

  // microserviceDisplayFn(obj: Microsservico): string {
  //   return obj && obj.noMicrosservico ? obj.noMicrosservico : '';
  // }

  storedProcedureDisplayFn(sp: StoredProcedureListagem): string {
    return sp && sp.noStoredProcedure ? sp.noStoredProcedure : '';
  }

  ngOnInit(): void {
    this.dtoClasse = new DtoClasse();
    const apiData = this.dtoClasseService.ping();
    const apiObserver = {
      next: () => {
        this.servidorDisponivel = true;
        this.storedProcedureFormControl.enable();
        this.dtoClasseFormControl.enable();
        //this.microserviceFormControl.enable();
        this.recuperarDtoClasses();
        //this.recuperarMicrosservicos();
        this.recuperarStoredProcedures();
        this.pesquisou = false;
        //this.expandiu = true;
        this.buscar();
      },
      error: () => {
        this.servidorDisponivel = false;
        this.msgService.error('Service unavailable.');
        this.storedProcedureFormControl.disable();
        this.dtoClasseFormControl.disable();
        //this.microserviceFormControl.disable();
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  recuperarDtoClasses() {
    const apiData = this.dtoClasseService.findByFilter(null);
    const apiObserver = {
      next: (res: DtoClasse[]) => {
        this.dtoClasseOptions = res;
        this.dtoClasseFilteredOptions =
          this.dtoClasseService.recuperarFilteredOptions(
            this.dtoClasseFormControl,
            this.dtoClasseOptions,
          );
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar DtoClasse: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  // recuperarMicrosservicos() {
  //   const apiData = this.microsservicoService.findByFilter(null);
  //   const apiObserver = {
  //     next: (res: Microsservico[]) => {
  //       this.microserviceOptions = res;
  //       this.microserviceFilteredOptions =
  //         this.microsservicoService.recuperarFilteredOptions(
  //           this.microserviceFormControl,
  //           this.microserviceOptions,
  //         );
  //     },
  //     error: (erro: HttpErrorResponse) => {
  //       console.error('Erro ao recuperar microsservicos: ', erro.message);
  //       console.error('erro:', JSON.stringify(erro));
  //       this.msgService.showErrorMessageBackend(erro);
  //     },
  //   };
  //   apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  // }

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

  buscar() {
    this.dtoClasse.id = this.dtoClasseService.recuperarIdDtoClasseSelecionada(
      this.dtoClasseFormControl,
    );
    // this.dtoClasse.idMicrosservico =
    //   this.microsservicoService.recuperarIdMicroserviceSelecionado(
    //     this.microserviceFormControl,
    //   );
    this.dtoClasse.idStoredProcedure =
      this.storedProcedureService.recuperarIdStoredProcedureSelecionada(
        this.storedProcedureFormControl,
      );

    const apiData = this.dtoClasseService.findByFilter(this.dtoClasse);
    const apiObserver = {
      next: (classesRetornados: DtoClasse[]) => {
        this.dtoClasses = classesRetornados;
        this.dataSource = new MatTableDataSource(this.dtoClasses);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar classes: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
    this.pesquisou = true;
    //this.expandiu = false; TODO para ficar sempre aberto o filtro
  }

  limpar(form: NgForm) {
    this.dtoClasseFormControl.reset();
    //this.microserviceFormControl.reset();
    this.storedProcedureFormControl.reset();
    this.pesquisou = false;
    //this.expandiu = true;
    form.reset();
    this.dtoClasse = new DtoClasse();
    this.dtoClasses = [];
    this.dataSource = new MatTableDataSource(this.dtoClasses);
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  excluir(id: number) {
    const apiObserver = {
      next: () => {
        this.msgService.success('DTO Class successfully deleted.');
        this.buscar();
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir dto-classe: ', erro);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.dtoClasseService.delete(id).pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  confirmDialog(id: number, noDtoClasse: string): void {
    const mensagem = `Do you really want to delete ${noDtoClasse}?`;
    const dialogData = new ConfirmDialogModel('Delete DTO Class', mensagem);
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

  abrirModalVisualizarDtoClasse(classe: DtoClasse) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '30%';
    dialogConfig.disableClose = false;
    dialogConfig.data = {
      modal: true,
      dtoClasse: classe,
    };
    this.dialog.open(VisualizarDtoClasseComponent, dialogConfig);
  }
}
