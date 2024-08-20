import { Component, Inject, OnInit } from '@angular/core';
import { Endpoint } from '../../../../shared/models/endpoint';
import { ActivatedRoute, Router } from '@angular/router';
import { EndpointService } from '../endpoint.service';
import { MsgService } from '../../../../shared/services/msg.service';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogConfig,
} from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { Utils } from '../../../../shared/util/utils';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { UtilsEnum } from '../../../../shared/util/utils-enum';
import { TipoDadoEnum } from '../../../../shared/enums/tipo-dado-enum';
import { ManterMicrosservicoComponent } from '../../microsservico/manter-microsservico/manter-microsservico.component';
import { MicrosservicoService } from '../../microsservico/microsservico.service';
import { FormControl, NgForm } from '@angular/forms';
import { EndpointUpdate } from '../../../../shared/models/endpoint-update';
import { HttpErrorResponse } from '@angular/common/http';
import { ComboBoxDto } from '../../../../shared/models/combo-box-dto';
import { DtoClasseService } from '../../dto-classe/dto-classe.service';
import { Microsservico } from '../../../../shared/models/microsservico';
import { Unsub } from '../../../../shared/util/unsub.class';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manter-endpoint',
  templateUrl: './manter-endpoint.component.html',
  styleUrls: ['./manter-endpoint.component.scss'],
})
export class ManterEndpointComponent extends Unsub implements OnInit {
  microserviceFormControl = new FormControl<string | Microsservico>('');
  microserviceOptions: Microsservico[] = [];
  microserviceFilteredOptions: Observable<Microsservico[]>;
  public endpoint: Endpoint = new Endpoint();
  public solicitouEdicao = false;
  public DIALOG_WIDTH = '450px';
  //public valoresComboMicrosservicos: ComboBoxDto[];
  public valoresComboTipoDado: ComboBoxDto[];
  public valoresComboDtoClassName: ComboBoxDto[];
  public origemNavegacao = 'sp';
  public retornarToolTip = 'Return to stored procedure edition';
  public nomeVariavelRetorndaFormControl = new FormControl('');

  //public idDtoClasseNavigationFormControl = new FormControl();
  protected readonly TipoDadoEnum = TipoDadoEnum;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private router: Router,
    private dialog: MatDialog,
    private activatedRoute: ActivatedRoute,
    private msgService: MsgService,
    private endpointService: EndpointService,
    private microsservicoService: MicrosservicoService,
    private dtoClasseService: DtoClasseService,
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

  microserviceDisplayFn(obj: Microsservico): string {
    return obj && obj.noMicrosservico ? obj.noMicrosservico : '';
  }

  ngOnInit(): void {
    this.endpoint.snAnalisado = true;
    this.microserviceFormControl.disable();
    this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
    this.recuperarMicrosservicos();
    //this.carregarComboMicrosservicos('Detail');
    this.activatedRoute.params
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params) => {
        const idEndpoint = Utils.readRouteParam(params, 'id');
        this.origemNavegacao = Utils.readRouteParam(params, 'origem');
        if (this.origemNavegacao === 'ms') {
          this.retornarToolTip = 'Return to microservice edition';
        }
        if (idEndpoint) {
          this.load(idEndpoint);
          // TODO unir nome das classes no combo do tipo de retrono
          //this.carregarComboDataTypeReturned();
        }
      });
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

  load(id: number): void {
    const apiData = this.endpointService.findById(id);
    const apiObserver = {
      next: (objEndpoint: Endpoint) => {
        this.endpoint = objEndpoint;
        this.microserviceFormControl.setValue(this.montarMicrosservico());
        this.recuperarClassesDaProcedure();
        if (this.endpoint.idVariavelRetornadaNavigation) {
          this.nomeVariavelRetorndaFormControl.setValue(
            this.endpoint.idVariavelRetornadaNavigation.noVariavel,
          );
        } else {
          this.nomeVariavelRetorndaFormControl.setValue('');
        }
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao carregar Endpoint: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  recuperarClassesDaProcedure() {
    const apiData = this.dtoClasseService.findByIdProcedureForCombo(
      this.endpoint.idStoredProcedure,
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

  excluirEndpoint(id: number): void {
    const apiData = this.endpointService.delete(id);
    const apiObserver = {
      next: () => {
        this.retornar();
        this.msgService.success('Endpoint successfully deleted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir endpoint: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  // abrirModalEditarDtoClasse() {
  //   const dialogConfig = new MatDialogConfig();
  //   dialogConfig.autoFocus = true;
  //   dialogConfig.width = '50%';
  //   dialogConfig.disableClose = false;
  //   dialogConfig.data = {
  //     modal: true,
  //     acao: 'Update',
  //     idDtoClasse: this.endpoint.idDtoClasse,
  //     idMicrosservico: this.endpoint.idMicrosservico
  //   };
  //   console.log("this.endpoint.idMicrosservico", this.endpoint.idMicrosservico);
  //   const dialogRef = this.dialog.open(ManterDtoClasseComponent, dialogConfig);
  //   dialogRef.afterClosed().pipe(takeUntil(this.unsubscribe$)).subscribe(() => {
  //     this.carregarComboClasses('Update');
  //   });
  // }

  // abrirModalEditarVariavel() {
  //   const dialogConfig = new MatDialogConfig();
  //   dialogConfig.autoFocus = true;
  //   dialogConfig.width = '50%';
  //   dialogConfig.disableClose = false;
  //   dialogConfig.data = {
  //     modal: true,
  //     acao: 'Update',
  //     idVariavel: this.endpoint.idVariavelRetornada
  //   };
  //   const dialogRef = this.dialog.open(ManterVariavelComponent, dialogConfig);
  //   dialogRef.afterClosed().pipe(takeUntil(this.unsubscribe$)).subscribe(() => {
  //     this.carregarComboVariaveis('Update');
  //   });
  // }
  limparValor() {
    this.microserviceFormControl.setValue(undefined);
  }

  habilitaEdicao(value: boolean) {
    this.solicitouEdicao = value;
    if (this.solicitouEdicao) {
      this.microserviceFormControl.enable();
    }
    if (!this.solicitouEdicao) {
      this.microserviceFormControl.disable();
    }
    this.load(this.endpoint.id);
  }

  abrirModalCriarEditarMicrosservico(insertOrUpdate: string) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '30%';
    dialogConfig.disableClose = false;
    dialogConfig.data = {
      modal: true, //acao: this.endpoint.idMicrosservico === undefined ? 'Insert' : 'Update',
      acao: insertOrUpdate, //this.endpoint.idMicrosservico === undefined ? 'Insert' : 'Update',
      idMicrosservico: this.endpoint.idMicrosservico,
    };
    const dialogRef = this.dialog.open(
      ManterMicrosservicoComponent,
      dialogConfig,
    );
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        //this.carregarComboMicrosservicos(this.data.acao);
        this.recuperarMicrosservicos();
      });
  }

  abrirModalEditarEndpoint() {
    const dialogData = new ConfirmDialogModel(
      'Confirm Update',
      'Confirm Update of the Endpoint',
    );
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
          this.atualizar(this.endpoint.id);
        }
      });
  }

  abrirModalDeletarEndpoint(): void {
    const mensagem = `Do you really want to delete ${this.endpoint.noMetodoEndpoint}?`;
    const dialogData = new ConfirmDialogModel('Confirm Delete', mensagem);
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
          this.excluirEndpoint(this.endpoint.id);
        }
      });
  }

  // retornaMensagemDeErroCampoInvalido(campoForm: NgModel): string {
  //   return campoForm.hasError('required')
  //     ? 'Required field'
  //     : campoForm.hasError('pattern')
  //       ? 'Invalid format'
  //       : '';
  // }

  retornar() {
    let url = '/analysis/stored-procedure/edit/';
    let id = this.endpoint.idStoredProcedure;

    if (this.origemNavegacao === 'ms') {
      url = '/analysis/microsservico/edit/';
      id = this.endpoint.idMicrosservico;
    }
    this.router.navigate([url, id]);
  }

  marcarRetornoListaComoNao() {
    this.endpoint.snRetornoLista = false;
  }

  formInvalido(endpointForm: NgForm): boolean {
    return (
      endpointForm.invalid ||
      this.microsservicoService.recuperarIdMicroserviceSelecionado(
        this.microserviceFormControl,
      ) === undefined
    );
  }

  private atualizar(id: number): void {
    let endpointUpdate = new EndpointUpdate();
    endpointUpdate.noMetodoEndpoint = this.endpoint.noMetodoEndpoint;
    endpointUpdate.txEndpointTratado = this.endpoint.txEndpointTratado.trim();
    endpointUpdate.noPath = this.endpoint.noPath;
    endpointUpdate.coTipoDadoRetorno = this.endpoint.coTipoDadoRetorno;
    endpointUpdate.snRetornoLista = this.endpoint.snRetornoLista;
    //endpointUpdate.snAnalisado = this.endpoint.snAnalisado;
    endpointUpdate.snAnalisado = true;
    endpointUpdate.idMicrosservico =
      this.microsservicoService.recuperarIdMicroserviceSelecionado(
        this.microserviceFormControl,
      );
    if (endpointUpdate.coTipoDadoRetorno == TipoDadoEnum.DTO) {
      endpointUpdate.idDtoClasse = this.endpoint.idDtoClasse;
      endpointUpdate.idDtoClasseNavigation =
        this.endpoint.idDtoClasseNavigation;
    } else {
      endpointUpdate.idDtoClasse = null;
      endpointUpdate.idDtoClasseNavigation = null;
    }
    const apiData = this.endpointService.update(id, endpointUpdate);
    const apiObserver = {
      next: () => {
        this.solicitouEdicao = false;
        this.habilitaEdicao(this.solicitouEdicao);
        this.msgService.success('Endpoint successfully updated.');
        this.retornar();
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao atualizar endpoint: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private montarMicrosservico() {
    let microsservico = new Microsservico();
    microsservico.id = this.endpoint.idMicrosservico;
    microsservico.noMicrosservico = this.endpoint.noMicrosservico;
    return microsservico;
  }

  colocarBarra() {
    if (this.endpoint.noPath) {
      this.endpoint.noPath = this.endpoint.noPath.replace(/\s/g, '');
      if (this.endpoint.noPath.charAt(0) != '/') {
        this.endpoint.noPath = '/' + this.endpoint.noPath;
      }
    }
  }

  retirarEspacosEmBranco() {
    if (this.endpoint.noMetodoEndpoint) {
      this.endpoint.noMetodoEndpoint = this.endpoint.noMetodoEndpoint.replace(
        /\s/g,
        '',
      );
    }
  }
}
