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
import { VariavelService } from './variavel.service';
import { DtoClasseService } from '../dto-classe/dto-classe.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { UtilsEnum } from '../../../shared/util/utils-enum';
import { DtoClasse } from '../../../shared/models/dto-classe';
import { Variavel, VariavelPaginado } from '../../../shared/models/variavel';
import { MatSort, Sort } from '@angular/material/sort';
import { take, takeUntil } from 'rxjs/operators';
import * as _ from 'lodash';
import { NgForm } from '@angular/forms';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import { ComboBoxDto } from '../../../shared/models/combo-box-dto';
import { PaginatorIntl } from '../../../shared/services/paginator-intl.service';
import { VariavelPageDto } from '../../../shared/models/variavel-page-dto';
import { HttpErrorResponse } from '@angular/common/http';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-variavel',
  templateUrl: './variavel.component.html',
  styleUrls: ['./variavel.component.scss'],
  providers: [{ provide: MatPaginatorIntl, useClass: PaginatorIntl }],
})
export class VariavelComponent extends Unsub implements OnInit {
  public variavel: VariavelPaginado = new VariavelPaginado();
  public variaveis: Variavel[] = [];
  public pesquisou = false;
  public expandiu = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  public displayedColumns: string[] = ['noVariavel', 'acao'];
  public dataSource: MatTableDataSource<Variavel>;
  public pageEvent: PageEvent;
  public valoresComboClasse: DtoClasse[] = [];
  public valoresComboTipoDado: ComboBoxDto[];
  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;
  public tamanhoPagina: number;
  public totalVariaveisPagina: number;
  public totalVariaveis: number;
  public paginaBusca: number;

  constructor(
    private router: Router,
    private variavelService: VariavelService,
    private dtoClasseService: DtoClasseService,
    private msgService: MsgService,
    private dialog: MatDialog,
  ) {
    super();
    this.dataSource = new MatTableDataSource(this.variaveis);
  }

  ngOnInit(): void {
    this.valoresComboTipoDado = UtilsEnum.recuperarValoresComboTipoDado();
    this.recuperarClasses();
    this.pesquisou = false;
    //this.expandiu = true;
    this.variavel = new VariavelPaginado();
    this.buscar();
  }

  recuperarClasses() {
    const apiData = this.dtoClasseService.getAll();
    const apiObserver = {
      next: (res: DtoClasse[]) => {
        this.valoresComboClasse = res;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao recuperar classes: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  guardarResultado(resultado: VariavelPageDto) {
    this.variaveis = resultado.content;
    this.totalVariaveisPagina = resultado.content.length;
    this.totalVariaveis = resultado.totalElements;
    this.variaveis = _.orderBy(
      this.variaveis,
      [(i) => i.noVariavel?.toLocaleLowerCase()],
      ['asc'],
    );
    this.dataSource = new MatTableDataSource(this.variaveis);
    this.dataSource.sort = this.sort;
    this.paginator.pageIndex = resultado.pageable.pageNumber;
    this.tamanhoPagina = resultado.pageable.pageSize;
  }

  buscar() {
    this.variavel.linesPerPage = this.tamanhoPagina;
    this.variavel.page = this.paginaBusca;
    const apiData = this.variavelService
      .getByFilters(this.variavel)
      .pipe(take(1));
    const apiObserver = {
      next: (variaveisRetornados: VariavelPageDto) => {
        this.guardarResultado(variaveisRetornados);
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao pesquisar variaveis: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
    this.pesquisou = true;
    //this.expandiu = false; TODO para ficar sempre aberto o filtro
  }

  inserirVariavel(): void {
    this.router.navigate(['/analysis/variavel/edit']);
  }

  limpar(form: NgForm) {
    this.pesquisou = false;
    ///this.expandiu = true;
    form.reset();
    this.variavel = new VariavelPaginado();
    this.variaveis = [];
    this.dataSource = new MatTableDataSource(this.variaveis);
    this.totalVariaveis = 0;
    this.totalVariaveisPagina = 0;
  }

  aoMudarPaginacao(event: PageEvent) {
    this.tamanhoPagina = event.pageSize;
    this.paginaBusca = this.paginator.pageIndex;
    this.buscar();
  }

  aposExpandir() {
    //this.expandiu = true;
  }

  excluir(id: number) {
    const apiObserver = {
      next: () => {
        this.msgService.success('Variable successfully deleted.');
        this.buscar();
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao excluir manter-variavel: ', erro.message);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.variavelService.delete(id).pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  confirmDialog(id: number, noVariavel: string): void {
    const mensagem = `Do you really want to delete ${noVariavel}?`;
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

  ordenarDados(sort: Sort) {
    if (this.variavel.direction === '') {
      this.variavel.direction = 'ASC';
    }
    this.variavel.direction = sort.direction.toUpperCase();
    this.variavel.orderBy = sort.active;
    this.buscar();
  }
}
