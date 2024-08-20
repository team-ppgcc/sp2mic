import { Component, Inject, OnInit } from '@angular/core';
import { Variavel } from '../../../../shared/models/variavel';
import { take, takeUntil } from 'rxjs/operators';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { UtilsEnum } from '../../../../shared/util/utils-enum';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { ActivatedRoute, Router } from '@angular/router';
import { VariavelService } from '../variavel.service';
import { MsgService } from '../../../../shared/services/msg.service';
import { FormControl, Validators } from '@angular/forms';
import { Utils } from '../../../../shared/util/utils';
import { StoredProcedureService } from '../../stored-procedure/stored-procedure.service';
import { HttpErrorResponse } from '@angular/common/http';
import { StoredProcedureListagem } from '../../../../shared/models/stored-procedure-listagem';
import { ComboBoxDto } from '../../../../shared/models/combo-box-dto';
import { Unsub } from '../../../../shared/util/unsub.class';

@Component({
  selector: 'app-manter-variavel',
  templateUrl: './manter-variavel.component.html',
  styleUrls: ['./manter-variavel.component.scss'],
})
export class ManterVariavelComponent extends Unsub implements OnInit {
  public customClass: string = 'custom-class';
  public nomeOperacaoJanela: string;
  public isInclusao: boolean;
  public variavel: Variavel = new Variavel();
  public solicitouEdicao = false;
  public salvou: boolean;
  public appBox: string = 'app-box';
  public DIALOG_WIDTH = '450px';
  public noVariavelFormControl = new FormControl('', [
    Validators.required,
    Validators.pattern('^[a-zA-Z0-9À-ú ]+$'),
  ]);
  public coTipoDadolFormControl = new FormControl();
  public idStoredProcedureFormControl = new FormControl();
  public valoresComboTipoDado: ComboBoxDto[];
  public valoresComboStoredProcedure: StoredProcedureListagem[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private variavelService: VariavelService,
    private storedProcedureService: StoredProcedureService,
    private msgService: MsgService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super();
  }

  get labelSaveAction(): string {
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
      ? 'Confirm Insert of the Variable?'
      : 'Confirm Update of the Variable?';
  }

  get tituloModal(): string {
    return this.isInclusao ? 'Confirm Insert' : 'Confirm Update';
  }

  ngOnInit(): void {
    this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
    if (this.data.modal) {
      this.customClass = 'custom-class-modal';
    }
    this.carregarComboStoredProcedures();

    if (this.data.acao === 'Update') {
      this.appBox = 'app-box-modal';
      this.isInclusao = false;
      this.solicitouEdicao = true;
      this.load(this.data.idVariavel);
      this.nomeOperacaoJanela = 'Update';
    }

    if (this.data.acao === 'Insert') {
      this.appBox = 'app-box-modal';
      this.isInclusao = true;
      this.solicitouEdicao = false;
      this.nomeOperacaoJanela = 'Include';
      this.variavel.coTipoDado = this.data.coTipoDado;
      this.variavel.idStoredProcedure = this.data.idStoredProcedure;
      this.coTipoDadolFormControl.disable();
      this.idStoredProcedureFormControl.disable();
    }

    this.route.params.pipe(takeUntil(this.unsubscribe$)).subscribe((params) => {
      const idVariavel = Utils.readRouteParam(params, 'id');
      if (idVariavel) {
        this.isInclusao = false;
        this.load(idVariavel);
        this.noVariavelFormControl.disable();
        this.coTipoDadolFormControl.disable();
        this.idStoredProcedureFormControl.disable();
      } else {
        this.isInclusao = true;
        this.variavel = new Variavel();
      }
    });
  }

  carregarComboStoredProcedures() {
    const apiData = this.storedProcedureService.findByFilter(null);
    const apiObserver = {
      next: (obj: StoredProcedureListagem[]) => {
        this.valoresComboStoredProcedure = obj;
        if (this.data.modal && this.data.acao === 'Insert') {
          this.variavel.coTipoDado = this.data.coTipoDado;
          this.variavel.idStoredProcedure = this.data.idStoredProcedure;
        }
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao carregar StoredProcedures: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  load(id: number): void {
    const apiData = this.variavelService.findById(id);
    const apiObserver = {
      next: (res: Variavel) => {
        this.variavel = res;
        this.noVariavelFormControl.setValue(this.variavel.noVariavel);
        this.coTipoDadolFormControl.setValue(this.variavel.coTipoDado);
        this.idStoredProcedureFormControl.setValue(
          this.variavel.idStoredProcedure,
        );
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao carregar Variável: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  salvar(): void {
    if (this.isInclusao) {
      this.inserir(this.variavel);
    } else {
      this.atualizar(this.variavel.id);
    }
  }

  close(): void {
    this.dialog.closeAll();
  }

  excluirVariavel(id: number) {
    const apiData = this.variavelService.delete(id);
    const apiObserver = {
      next: () => {
        this.router.navigate(['/analysis/variavel']);
        this.msgService.success('Variable successfully deleted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir variavel: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  habilitaEdicao(value: boolean) {
    this.carregarComboStoredProcedures();
    this.solicitouEdicao = value;

    if (this.solicitouEdicao) {
      this.noVariavelFormControl.enable();
      this.coTipoDadolFormControl.enable();
      this.idStoredProcedureFormControl.enable();
      this.salvou = false;
    } else {
      this.load(this.variavel.id);
      this.noVariavelFormControl.disable();
      this.coTipoDadolFormControl.disable();
      this.idStoredProcedureFormControl.disable();
    }
  }

  retornaMensagemDeErronoVariavel(): string {
    return this.noVariavelFormControl.hasError('required')
      ? 'Required field'
      : this.noVariavelFormControl.hasError('pattern')
        ? 'Invalid format'
        : '';
  }

  abrirModalIncluirOuEditarVariavel() {
    const mensagem = this.msgConfirmAddOrAlter;
    const dialogData = new ConfirmDialogModel(this.tituloModal, mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: this.DIALOG_WIDTH,
      data: dialogData,
      disableClose: true,
    });
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$)).subscribe((dialogResult) => {
        if (dialogResult) {
          this.salvar();
        }
      });
  }

  abrirModalDeletarVariavel() {
    const mensagem = `Do you really want to delete ${this.variavel.noVariavel}}?`;
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
          this.excluirVariavel(this.variavel.id);
        }
      });
  }

  rotuloTipoDado(tipo: any): string {
    return UtilsEnum.retornaRotuloTipoDado(tipo);
  }

  private inserir(variavel: Variavel): void {
    const apiData = this.variavelService.post(variavel);
    const apiObserver = {
      next: () => {
        if (this.data.modal) {
          this.close();
        } else {
          this.router.navigate(['/analysis/variavel']);
        }
        this.msgService.success('Variable added successfully.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao inserir variável: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private atualizar(id: number): void {
    const variavelAlterada = JSON.parse(JSON.stringify(this.variavel));
    const apiData = this.variavelService.put(id, variavelAlterada);
    const apiObserver = {
      next: () => {
        if (this.data.modal) {
          this.close();
        } else {
          this.solicitouEdicao = false;
          this.salvou = true;
          this.habilitaEdicao(this.solicitouEdicao);
          this.router.navigate(['/analysis/variavel']);
        }
        this.msgService.success('Variable updated successfully.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao atualizar variável: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }
}
