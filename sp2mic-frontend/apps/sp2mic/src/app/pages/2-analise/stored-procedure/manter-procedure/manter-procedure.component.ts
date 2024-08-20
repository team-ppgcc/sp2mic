import { Component, OnInit, ViewChild } from '@angular/core';
import { StoredProcedure } from '../../../../shared/models/stored-procedure';
import { ActivatedRoute, Router } from '@angular/router';
import { StoredProcedureService } from '../stored-procedure.service';
import { MsgService } from '../../../../shared/services/msg.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { take } from 'rxjs';
import { Utils } from '../../../../shared/util/utils';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { UtilsEnum } from '../../../../shared/util/utils-enum';
import { TipoDadoEnum } from '../../../../shared/enums/tipo-dado-enum';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { BlockDialogComponent } from '../../../../shared/services/dialog/block-dialog.component';
import { UtilsHtml } from '../../../../shared/util/utils-html';
import { StoredProcedureUpdate } from '../../../../shared/models/stored-procedure-update';
import { MatSort } from '@angular/material/sort';
import { EndpointListagem } from '../../../../shared/models/endpoint-listagem';
import { ComboBoxDto } from '../../../../shared/models/combo-box-dto';
import { DtoClasseService } from '../../dto-classe/dto-classe.service';
import { HttpErrorResponse } from '@angular/common/http';
import { VisualizarEndpointComponent } from '../../endpoint/visualizar-endpoint/visualizar-endpoint.component';
import { EndpointService } from '../../endpoint/endpoint.service';
import { EndpointFilter } from '../../../../shared/models/endpoint-filter';
import { Unsub } from '../../../../shared/util/unsub.class';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manter-procedure',
  templateUrl: './manter-procedure.component.html',
  styleUrls: ['./manter-procedure.component.scss'],
})
export class ManterProcedureComponent extends Unsub implements OnInit {
  public storedProcedure: StoredProcedure = new StoredProcedure();
  public solicitouEdicao = false;
  public DIALOG_WIDTH = '450px';
  public valoresComboTipoDado: ComboBoxDto[];
  public valoresComboDtoClassName: ComboBoxDto[];
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dataSourceEndpoints: MatTableDataSource<EndpointListagem>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;
  public displayedColumns: string[] = [
    'snAnalisado',
    'noTipoSqlDml',
    'noTipoDadoRetorno',
    'noMicrosservico',
    'noMetodoEndpoint',
    'noPath',
    'tabelasAssociadas',
    'acao'
  ];
  protected readonly TipoDadoEnum = TipoDadoEnum;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private storedProcedureService: StoredProcedureService,
    private endpointService: EndpointService,
    private dtoClasseService: DtoClasseService,
    private msgService: MsgService,
    private dialog: MatDialog,
  ) {
    super();
  }

  get labelTitle(): string {
    if (this.solicitouEdicao) {
      return 'Update';
    } else {
      return 'Detail';
    }
  }

  ngOnInit(): void {
    this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
    this.activatedRoute.params.pipe(takeUntil(this.unsubscribe$)).subscribe((params) => {
      const idStoredProcedure = Utils.readRouteParam(params, 'id');
      if (idStoredProcedure) {
        this.load(idStoredProcedure);
        // TODO unir nome das classes no combo do tipo de retrono
        //this.carregarComboDataTypeReturned();
      }
    });
  }

  load(id: number): void {
    const apiData = this.storedProcedureService.findById(id);
    const apiObserver = {
      next: (storedProcedure: StoredProcedure) => {
        this.storedProcedure = storedProcedure;
        this.recuperarClassesDaProcedure();
        this.dataSourceEndpoints = new MatTableDataSource(
          this.storedProcedure.endpoints,
        );
        this.dataSourceEndpoints.paginator = this.paginator;
        this.dataSourceEndpoints.sort = this.sort;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar Stored Procedures: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  recuperarClassesDaProcedure() {
    const apiData = this.dtoClasseService.findByIdProcedureForCombo(
      this.storedProcedure.id,
    );
    const apiObserver = {
      next: (res: ComboBoxDto[]) => {
        this.valoresComboDtoClassName = res;
      },
      error: (erro: HttpErrorResponse) => {
        console.error(
          'Erro ao recuperar classes dessa procedures: ',
          erro.message,
        );
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  excluirStoredProcedure(id: number) {
    const apiData = this.storedProcedureService.delete(id);

    const apiObserver = {
      next: () => {
        this.router.navigate(['/analysis/stored-procedure']);
        this.msgService.success('Stored Procedure successfully deleted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir Stored Procedure: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  habilitaEdicao(value: boolean) {
    this.solicitouEdicao = value;
    this.load(this.storedProcedure.id);
  }

  // retornaMensagemDeErroCampoInvalido(campoForm: NgModel): string {
  //   return campoForm.hasError('required')
  //     ? 'Required field'
  //     : campoForm.hasError('pattern')
  //       ? 'Invalid format'
  //       : '';
  // }

  abrirModalAlterarStoredProcedure() {
    const dialogData = new ConfirmDialogModel(
      'Confirm Update',
      'Confirm Update of the Stored Procedure',
    );
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: this.DIALOG_WIDTH,
      data: dialogData,
      disableClose: true,
    });
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$)).subscribe((dialogResult) => {
        if (dialogResult) {
          this.atualizar(this.storedProcedure.id);
        }
      });
  }

  abrirModalDeletarStoredProcedure() {
    const mensagem = `Do you really want to delete ${this.storedProcedure.noStoredProcedure}?`;
    const dialogData = new ConfirmDialogModel('Confirm Delete', mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: this.DIALOG_WIDTH,
      data: dialogData,
      disableClose: true,
    });
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$)).subscribe((dialogResult) => {
        if (dialogResult) {
          this.excluirStoredProcedure(this.storedProcedure.id);
        }
      });
  }

  rotuloBoolean(campo: boolean) {
    return campo ? 'Yes' : 'No';
  }

  viewProcedure() {
    this.router.navigate([
      '/analysis/stored-procedure/view/' + this.storedProcedure.id,
    ]);
  }

  setNoListReturned() {
    if (this.storedProcedure.coTipoDadoRetorno == TipoDadoEnum.VOID) {
      this.storedProcedure.snRetornoLista = false;
    }
  }

  private atualizar(id: number): void {
    let storedProcedureUpdate = new StoredProcedureUpdate();
    storedProcedureUpdate.noStoredProcedure =
      this.storedProcedure.noStoredProcedure;
    storedProcedureUpdate.noSchema = this.storedProcedure.noSchema;
    storedProcedureUpdate.coTipoDadoRetorno =
      this.storedProcedure.coTipoDadoRetorno;
    storedProcedureUpdate.snRetornoLista = this.storedProcedure.snRetornoLista;
    //storedProcedureUpdate.snAnalisada = this.storedProcedure.snAnalisada;
    storedProcedureUpdate.snAnalisada = true;
    storedProcedureUpdate.idDtoClasse = this.storedProcedure.idDtoClasse;
    const apiData = this.storedProcedureService.update(
      id,
      storedProcedureUpdate,
    );
    const apiObserver = {
      next: () => {
        this.solicitouEdicao = false;
        this.habilitaEdicao(this.solicitouEdicao);
        this.msgService.success('Stored Procedure successfully updated.');
        this.router.navigate(['/analysis/stored-procedure']);
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao atualizar storedProcedure: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  retirarEspacosEmBrancoNoSchema() {
    if (this.storedProcedure.noSchema) {
      this.storedProcedure.noSchema = this.storedProcedure.noSchema.replace(
        /\s/g,
        '',
      );
    }
  }
  retirarEspacosEmBrancoNoStoredProcedure() {
    if (this.storedProcedure.noStoredProcedure) {
      this.storedProcedure.noStoredProcedure =
        this.storedProcedure.noStoredProcedure.replace(/\s/g, '');
    }
  }

  abrirModalVisualizarEndpoint(id: number): void {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '50%';
    dialogConfig.disableClose = false;
    dialogConfig.data = {
      modal: true,
      idEndpoint: id,
    };
    this.dialog.open(VisualizarEndpointComponent, dialogConfig);
  }

  confirmDialogDeleteEndpoint(id: number, noMetodoEndpoint: string): void {
    const mensagem = `Do you really want to delete ${noMetodoEndpoint}?`;
    const dialogData = new ConfirmDialogModel('Confirm Delete', mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData,
    });
    dialogRef.afterClosed().pipe(takeUntil(this.unsubscribe$)).subscribe((dialogResult) => {
      if (dialogResult) {
        this.excluirEndpoint(id);
      }
    });
  }

  private excluirEndpoint(id: number) {
    const apiObserver = {
      next: () => {
        this.msgService.success('Endpoint successfully deleted.');
        this.load(this.storedProcedure.id);
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir Endpoint: ', erro);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.endpointService.delete(id).pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private recuperarEndpoint() {
    let filtrosEndpoint = new EndpointFilter();
    filtrosEndpoint.idStoredProcedure = this.storedProcedure.id;
    const apiData = this.endpointService.findByFilter(filtrosEndpoint);
    const apiObserver = {
      next: (endpoints: EndpointListagem[]) => {
        this.storedProcedure.endpoints = endpoints;
        this.dataSourceEndpoints = new MatTableDataSource(
            this.storedProcedure.endpoints,
        );
        this.dataSourceEndpoints.paginator = this.paginator;
        this.dataSourceEndpoints.sort = this.sort;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar Stored Procedures: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }
}
