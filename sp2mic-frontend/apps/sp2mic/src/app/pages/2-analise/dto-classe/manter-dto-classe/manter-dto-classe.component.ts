import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { DtoClasse } from '../../../../shared/models/dto-classe';
import { ActivatedRoute, Router } from '@angular/router';
import { MsgService } from '../../../../shared/services/msg.service';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
//import { isNullOrUndefined, Utils } from '../../../../shared/util/utils';
import { Utils } from '../../../../shared/util/utils';
//import { MicrosservicoService } from '../../microsservico/microsservico.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { UtilsEnum } from '../../../../shared/util/utils-enum';
//import { Microsservico } from '../../../../shared/models/microsservico';
import { DtoClasseService } from '../dto-classe.service';
import { MatTableDataSource } from '@angular/material/table';
import { AtributoDto } from '../../../../shared/models/atributo-dto';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { BlockDialogComponent } from '../../../../shared/services/dialog/block-dialog.component';
import { AtributoService } from '../../atributo/atributo.service';
import { UtilsHtml } from '../../../../shared/util/utils-html';
import { NgModel } from '@angular/forms';
import { AtributoUpdate } from '../../../../shared/models/atributo-update';
import { DtoClasseUpdateDto } from '../../../../shared/models/dto-classe-update-dto';
import { HttpErrorResponse } from '@angular/common/http';
import { ComboBoxDto } from '../../../../shared/models/combo-box-dto';
import { takeUntil } from 'rxjs/operators';
import { Unsub } from '../../../../shared/util/unsub.class';

@Component({
  selector: 'app-manter-dto-classe',
  templateUrl: './manter-dto-classe.component.html',
  styleUrls: ['./manter-dto-classe.component.scss'],
})
export class ManterDtoClasseComponent extends Unsub implements OnInit {
  public dtoClasse: DtoClasse = new DtoClasse();
  public atributos: AtributoDto[] = [];
  public solicitouEdicao = false;
  public DIALOG_WIDTH = '450px';
  public displayedColumns: string[] = ['noTipoDado', 'noAtributo', 'acoes'];
  public valoresComboTipoDado: ComboBoxDto[];
  //public valoresComboMicrosservico: Microsservico[] = [];
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dataSource: MatTableDataSource<AtributoDto>;
  public semAtributos = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;
  public nomeOperacaoJanela: string;
  public isInclusao: boolean;
  public salvou: boolean;
  public appBox: string = 'app-box';
  public buscouAtributos: boolean = false;
  public clicouEditar: boolean[] = [];
  public teveAlteracao: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private dtoClasseService: DtoClasseService, //private microsservicoService: MicrosservicoService,
    private msgService: MsgService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private atributoService: AtributoService,
  ) {
    super();
  }

  get labelSaveAction(): string {
    return this.data.acao === 'Insert' ? 'Insert' : 'Save';
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
      ? 'Confirm Insert of the DTO Class?'
      : 'Confirm the update of DTO Class ?';
  }

  get tituloModal(): string {
    return this.isInclusao ? 'Confirm Insert' : 'Confirm Update';
  }

  ngOnInit(): void {
    this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
    //this.carregarComboMicrosservicos();
    this.route.params.pipe(takeUntil(this.unsubscribe$)).subscribe((params) => {
      const idDtoClasse = Utils.readRouteParam(params, 'id');
      if (idDtoClasse) {
        this.isInclusao = false;
        this.dtoClasse.id = idDtoClasse;
        this.load(idDtoClasse);
      } else {
        this.isInclusao = true;
        this.dtoClasse = new DtoClasse();
      }
    });

    if (this.data.acao === 'Update') {
      this.appBox = 'app-box-modal';
      this.isInclusao = false;
      this.solicitouEdicao = true;
      this.load(this.data.idDtoClasse);
      this.dtoClasse.id = this.data.idDtoClasse;
      this.nomeOperacaoJanela = 'Update';
      //this.dtoClasse.idMicrosservico = this.data.idMicrosservico;
    }

    if (this.data.acao === 'Insert') {
      this.appBox = 'app-box-modal';
      this.isInclusao = true;
      this.solicitouEdicao = false;
      this.nomeOperacaoJanela = 'Insert';
      //this.dtoClasse.idMicrosservico = this.data.idMicrosservico;
    }
  }

  // carregarComboMicrosservicos() {
  //   const apiData = this.microsservicoService.findByFilter(new Microsservico());
  //   const apiObserver = {
  //     next: (obj: Microsservico[]) => {
  //       this.valoresComboMicrosservico = obj;
  //     },
  //     error: (erro: HttpErrorResponse) => {
  //       console.error('Erro ao carregar Microsservicos: ', erro.message);
  //       console.error('erro:', JSON.stringify(erro));
  //       this.msgService.showErrorMessageBackend(erro);
  //     },
  //   };
  //   apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  // }

  load(id: number): void {
    const apiData = this.dtoClasseService.findById(id);
    const apiObserver = {
      next: (dtoClasse: DtoClasse) => {
        this.dtoClasse = dtoClasse;
        const apiDataAtr = this.atributoService.getByIdClasse(id);
        const apiObserverAtr = {
          next: (atrs: AtributoDto[]) => {
            this.atributos = atrs;
            this.semAtributos = this.atributos.length === 0;
            this.dataSource = new MatTableDataSource(this.atributos);
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
          },
          error: (erro: HttpErrorResponse) => {
            console.error('Erro ao carregar atributos: ', erro.message);
            console.error('erro:', JSON.stringify(erro));
            this.msgService.showErrorMessageBackend(erro);
          },
        };
        apiDataAtr.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserverAtr);
        this.teveAlteracao = true;
        this.buscouAtributos = true;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao carregar Classe: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  salvar(): void {
    if (this.data.acao === 'Insert') {
      this.inserir(this.dtoClasse);
    } else if (this.data.acao === 'Update') {
      this.atualizar(this.dtoClasse.id);
    } else {
      if (this.isInclusao) {
        this.inserir(this.dtoClasse);
      } else {
        this.atualizar(this.dtoClasse.id);
      }
    }
  }

  close(): void {
    this.dialog.closeAll();
  }

  excluirClasse(id: number) {
    const apiData = this.dtoClasseService.delete(id);
    const apiObserver = {
      next: () => {
        this.router.navigate(['/analysis/dto-classe']);
        this.msgService.success('Classe successfully deleted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir Classe: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  habilitaEdicao(value: boolean) {
    //this.carregarComboMicrosservicos();
    this.solicitouEdicao = value;
    this.load(this.dtoClasse.id);
  }

  retornaMensagemDeErroCampoInvalido(campoForm: NgModel): string {
    return campoForm.hasError('required')
      ? 'Required field'
      : campoForm.hasError('pattern')
        ? 'Invalid format'
        : '';
  }

  abrirModalIncluirOuAlterarClasse() {
    this.isInclusao = this.data.acao === 'Insert';
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

  abrirModalDeletarClasse(): void {
    const mensagem = `Do you really want to delete ${this.dtoClasse.noDtoClasse}?`;
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
          this.excluirClasse(this.dtoClasse.id);
        }
      });
  }

  rotuloTipoDado(tipo: any): string {
    return UtilsEnum.retornaRotuloTipoDado(tipo);
  }

  salvarAtributo(i: number): void {
    const mensagem = `Confirm the update of Atributo: \"${this.atributos[i].noAtributo}\"?`;
    const tituloModal = 'Confirm Update';
    const dialogData = new ConfirmDialogModel(tituloModal, mensagem);
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
          this.atualizarAtributo(i);
        }
      });
  }

  excluirAtributo(i: number): void {
    const mensagem = `Do you really want to delete ${this.atributos[i].noAtributo}?`;
    const tituloModal = 'Confirm Delete';
    const dialogData = new ConfirmDialogModel(tituloModal, mensagem);
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
          const apiData = this.atributoService.delete(this.atributos[i].id);
          const apiObserver = {
            next: () => {
              this.buscarAtributosDaDtoClasse();
              this.dataSource = new MatTableDataSource(this.atributos);
              this.dataSource.paginator = this.paginator;
              this.dataSource.sort = this.sort;
              this.teveAlteracao = true;
              this.buscouAtributos = true;
              this.clicouEditar[i] = false;
              this.msgService.success('Attribute successfully deleted.');
            },
            error: (erro: HttpErrorResponse) => {
              console.error('Erro ao excluir AtributoDto:: ', erro.message);
              console.error('erro:', JSON.stringify(erro));
              this.msgService.showErrorMessageBackend(erro);
            },
          };
          apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
        }
      });
  }

  editarAtributo(i: number): void {
    // this.valoresComboTipoDado = UtilsEnum.ordenarNumerosPorNomeDaFuncao(
    //   this.valoresComboTipoDado as number[],
    //   UtilsEnum.retornaRotuloTipoDado,
    // );
    this.clicouEditar[i] = true;
  }

  desfazer(i: number): void {
    this.atributos[i].coTipoDado = undefined;
    this.clicouEditar[i] = false;
  }

  private inserir(classe: DtoClasse): void {
    const apiData = this.dtoClasseService.add(classe);
    const apiObserver = {
      next: () => {
        if (this.data.modal) {
          this.close();
        } else {
          this.router.navigate(['/analysis/dto-classe']);
        }
        this.msgService.success('Class successfully inserted.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao inserir Classe: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private atualizar(id: number): void {
    let classeUpdate = new DtoClasseUpdateDto();
    classeUpdate.id = this.dtoClasse.id;
    classeUpdate.noDtoClasse = this.dtoClasse.noDtoClasse;
    // if (!isNullOrUndefined(this.dtoClasse.idMicrosservico)) {
    //   classeUpdate.idMicrosservico = this.dtoClasse.idMicrosservico;
    // }
    const apiObserver = {
      next: () => {
        if (this.data.modal) {
          this.close();
        } else {
          this.solicitouEdicao = false;
          this.salvou = true;
          this.habilitaEdicao(this.solicitouEdicao);
          this.router.navigate(['/analysis/dto-classe']);
        }
        this.msgService.success('Class successfully updated.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao atualizar Classe: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.dtoClasseService
      .update(id, classeUpdate)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(apiObserver);
  }

  private atualizarAtributo(i: number): void {
    let atributoUpdate = new AtributoUpdate();
    atributoUpdate.noAtributo = this.atributos[i].noAtributo;
    atributoUpdate.coTipoDado = this.atributos[i].coTipoDado;
    atributoUpdate.idDtoClasse = this.atributos[i].idDtoClasse;
    const apiData = this.atributoService.update(
      this.atributos[i].id,
      atributoUpdate,
    );
    const apiObserver = {
      next: () => {
        this.atributos[i].noTipoDado = this.rotuloTipoDado(
          this.atributos[i].coTipoDado,
        );
        this.clicouEditar[i] = false;
        this.teveAlteracao = true;
        this.buscouAtributos = true;
        this.msgService.success('Attribute successfully updated.');
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao atualizar atributo: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private buscarAtributosDaDtoClasse() {
    const apiData = this.atributoService.getByIdClasse(this.data.idDtoClasse);
    const apiObserver = {
      next: (atributos: AtributoDto[]) => {
        this.atributos = atributos;
        this.dataSource = new MatTableDataSource(this.atributos);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.teveAlteracao = true;
        this.buscouAtributos = true;
        this.inicializarClicouEditar();
      },
      error: (erro: HttpErrorResponse) => {
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  private inicializarClicouEditar() {
    for (let i = 0; i < this.atributos.length; i++) {
      this.clicouEditar[i] = false;
    }
  }

  retirarEspacosEmBranco() {
    if (this.dtoClasse.noDtoClasse) {
      this.dtoClasse.noDtoClasse = this.dtoClasse.noDtoClasse.replace(
        /\s/g,
        '',
      );
    }
  }
}
