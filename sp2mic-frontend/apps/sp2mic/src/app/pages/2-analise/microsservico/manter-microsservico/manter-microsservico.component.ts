import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { Microsservico } from '../../../../shared/models/microsservico';
import { ActivatedRoute, Router } from '@angular/router';
import { MicrosservicoService } from '../microsservico.service';
import { MsgService } from '../../../../shared/services/msg.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { take } from 'rxjs';
import { Utils } from '../../../../shared/util/utils';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { TipoDadoEnum } from '../../../../shared/enums/tipo-dado-enum';
import { MicrosservicoUpdate } from '../../../../shared/models/microsservico-update';
import { MicrosservicoAdd } from '../../../../shared/models/microsservico-add';
import { UtilsHtml } from '../../../../shared/util/utils-html';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import isNil from 'lodash/isNil';
import { HttpErrorResponse } from '@angular/common/http';
import { EndpointListagem } from '../../../../shared/models/endpoint-listagem';
import { Unsub } from '../../../../shared/util/unsub.class';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manter-microsservico',
  templateUrl: './manter-microsservico.component.html',
  styleUrls: ['./manter-microsservico.component.scss'],
})
export class ManterMicrosservicoComponent extends Unsub implements OnInit {
  public customClass: string = 'custom-class';
  public nomeOperacaoJanela: string;
  public isInclusao: boolean;
  public microsservico: Microsservico = new Microsservico();
  public solicitouEdicao = false;
  public appBox: string = 'app-box';
  public DIALOG_WIDTH = '450px';
  public tiposDeDados = TipoDadoEnum;
  public valoresComboTipoDado = Object.values(this.tiposDeDados).filter(Number);
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dataSourceEndpointListagem: MatTableDataSource<EndpointListagem>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public displayedColumns: string[] = [
    'snAnalisado',
    'noTipoDadoRetorno',
    'noMetodoEndpoint',
    'noPath',
    'noStoredProcedure',
  ];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private microsservicoService: MicrosservicoService,
    private msgService: MsgService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super();
  }

  get labelSaveAction(): string {
    if (this.data.acao === 'Update') {
      return 'Save';
    }
    return this.isInclusao ? 'Insert' : 'Save';
  }

  get labelTitle(): string {
    if (this.isInclusao) {
      return 'Insert';
    } else {
      if (this.solicitouEdicao) {
        return 'Update';
      } else {
        return 'Detail';
      }
    }
  }

  get msgConfirmAddOrAlter(): string {
    return this.isInclusao
      ? 'Confirm the insert of Microservice?'
      : 'Confirm the update of Microservice?';
  }

  get tituloModal(): string {
    return this.isInclusao ? 'Confirm Insert' : 'Confirm Update';
  }

  rotuloBoolean(campo: boolean) {
    return campo ? 'Yes' : 'No';
  }

  load(id: number): void {
    const apiData = this.microsservicoService.findById(id);
    const apiObserver = {
      next: (obj: Microsservico) => {
        this.microsservico = obj;
        // this.microsservico.endpoints = _.orderBy(
        //   this.microsservico.endpoints,
        //   [(i: { noEndpoint: string }) => i.noEndpoint?.toLocaleLowerCase()],
        //   ['asc'],
        // );
        this.dataSourceEndpointListagem = new MatTableDataSource(
          this.microsservico.endpoints,
        );
        this.dataSourceEndpointListagem.paginator = this.paginator;
        this.dataSourceEndpointListagem.sort = this.sort;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao carregar Microsservico: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  ngOnInit(): void {
    if (this.data.modal) {
      //this.customClass = "custom-class-modal";
      this.customClass = 'custom-class';
      if (this.data.acao === 'Update') {
        this.appBox = 'app-box-modal';
        this.isInclusao = false;
        this.solicitouEdicao = true;
        this.load(this.data.idMicrosservico);
        this.nomeOperacaoJanela = 'Update';
      }
      if (this.data.acao === 'Insert') {
        this.appBox = 'app-box-modal';
        this.isInclusao = true;
        this.solicitouEdicao = true;
        this.nomeOperacaoJanela = 'Insert';
      }
    } else {
      this.activatedRoute.params
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((params) => {
          const idMicrosservico = Utils.readRouteParam(params, 'id');
          if (idMicrosservico) {
            this.isInclusao = false;
            this.load(idMicrosservico);
          } else {
            this.isInclusao = true;
            this.solicitouEdicao = true;
            this.microsservico = new Microsservico();
          }
        });
    }
  }

  salvar(): void {
    if (this.isInclusao) {
      this.inserir(this.microsservico);
    } else {
      this.atualizar(this.microsservico.id);
    }
  }

  close(): void {
    this.dialog.closeAll();
  }

  excluirMicrosservico(id: number) {
    const apiData = this.microsservicoService.delete(id);
    const apiObserver = {
      next: () => {
        this.router.navigate(['/analysis/microsservico']);
        this.msgService.success('Microsservico successfully deleted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir microsservico: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  habilitaEdicao(value: boolean) {
    this.solicitouEdicao = value;
    this.load(this.microsservico.id);
  }

  retornaMensagemDeErroCampoInvalido(campoForm: NgModel): string {
    return campoForm.hasError('required')
      ? 'Required field'
      : campoForm.hasError('pattern')
        ? 'Invalid format'
        : '';
  }

  abrirModalIncluirOuEditarMicrosservico() {
    console.log('this.isInclusao = ', this.isInclusao);
    const mensagem = this.msgConfirmAddOrAlter;
    const dialogData = new ConfirmDialogModel(this.tituloModal, mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: this.DIALOG_WIDTH,
      data: dialogData,
      disableClose: true,
    });
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((dialogResult) => {
        if (dialogResult) {
          this.salvar();
        }
      });
  }

  abrirModalDeletarMicrosservico() {
    const mensagem = `Do you really want to delete ${this.microsservico.noMicrosservico}?`;
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
          this.excluirMicrosservico(this.microsservico.id);
        }
      });
  }

  isInsertOrUpdateWithoutEndpoints() {
    return (
      isNil(this.microsservico) ||
      isNil(this.microsservico.endpoints) ||
      this.microsservico.endpoints?.length === 0
    );
  }

  private inserir(microsservico: Microsservico): void {
    let microsservicoAdd = new MicrosservicoAdd();
    microsservicoAdd.noMicrosservico = microsservico.noMicrosservico;
    microsservicoAdd.snProntoParaGerar = microsservico.snProntoParaGerar;
    const apiData = this.microsservicoService.post(microsservicoAdd);
    const apiObserver = {
      next: () => {
        if (this.data.modal) {
          this.close();
        } else {
          this.router.navigate(['/analysis/microsservico']);
        }
        this.msgService.success('Microsserviço successfully inserted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao inserir microsservico: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private atualizar(id: number): void {
    let microsservicoUpdate = new MicrosservicoUpdate();
    microsservicoUpdate.noMicrosservico = this.microsservico.noMicrosservico;
    microsservicoUpdate.snProntoParaGerar =
      this.microsservico.snProntoParaGerar;

    const apiData = this.microsservicoService.update(id, microsservicoUpdate);
    const apiObserver = {
      next: () => {
        if (this.data.modal) {
          this.close();
        } else {
          this.solicitouEdicao = false;
          this.habilitaEdicao(this.solicitouEdicao);
          this.router.navigate(['/analysis/microsservico']);
        }
        this.msgService.success('Microservice successfully updated.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao atualizar microsservico: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  retirarEspacosEmBranco() {
    if (this.microsservico.noMicrosservico) {
      this.microsservico.noMicrosservico =
        this.microsservico.noMicrosservico.replace(/\s/g, '');
    }
  }
}
