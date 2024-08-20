import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MsgService } from '../../shared/services/msg.service';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ParDto } from '../../shared/models/par-dto';
import { CargaDto } from '../../shared/models/carga-dto';
import { UtilsHtml } from '../../shared/util/utils-html';
import { CargaService } from './carga.service';
import { BlockDialogComponent } from '../../shared/services/dialog/block-dialog.component';
import { MatSort } from '@angular/material/sort';
import { TipoBancoDeDados } from '../../shared/enums/tipo-banco-de-dados';
import { takeUntil } from 'rxjs/operators';
import { Unsub } from '../../shared/util/unsub.class';

@Component({
  selector: 'app-carga',
  templateUrl: './carga.component.html',
  styleUrls: ['./carga.component.scss'],
})
export class CargaComponent extends Unsub implements OnInit {
  servidorDisponivel = false;
  public TipoBancoDeDados = TipoBancoDeDados;
  public cargaDtoConexao = new CargaDto();
  public resultadoDataSourceConexao: ParDto[] = [];
  public displayedColumnsConexao = ['snCarregada', 'nome'];
  public dataSourceConexao: MatTableDataSource<ParDto>;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  public selecionarTodosConexao = false;
  public pesquisouConexao = false;
  public expandiuConexao = true;

  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;

  public noStoredProcedure: string;
  public supportedDatabases: ParDto[];
  public nomeTipoBancoDeDados: string;

  constructor(
    private msgService: MsgService,
    private router: Router,
    private service: CargaService,
  ) {
    super();
    this.dataSourceConexao = new MatTableDataSource([]);
  }

  ngOnInit(): void {
    this.pesquisouConexao = false;
    this.expandiuConexao = true;
    this.selecionarTodosConexao = false;
    this.displayedColumnsConexao.unshift('checado');
    const apiData = this.service.ping();
    const apiObserver = {
      next: () => {
        this.servidorDisponivel = true;
        this.carregarSupportedDatabase();
      },
      error: () => {
        this.servidorDisponivel = false;
        this.msgService.error('Service unavailable.');
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  pesquisarConexao() {
    this.selecionarTodosConexao = false;
    this.resultadoDataSourceConexao = [];
    this.dataSourceConexao = new MatTableDataSource([]);
    const apiData = this.service.listarNomesProcedures(this.cargaDtoConexao);
    const apiObserver = {
      next: (res: any) => {
        this.resultadoDataSourceConexao = res.body;
        this.dataSourceConexao = new MatTableDataSource(
          this.resultadoDataSourceConexao,
        );
        this.dataSourceConexao.paginator = this.paginator;
        this.dataSourceConexao.sort = this.sort;
      },
      error: (erro: HttpErrorResponse) => {
        this.pesquisouConexao = false;
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
    this.pesquisouConexao = true;
  }

  limparFormularioConexao(form: NgForm) {
    this.pesquisouConexao = false;
    //this.expandiuConexao = true;
    form.reset();
    this.resultadoDataSourceConexao = [];
    this.dataSourceConexao = new MatTableDataSource([]);
  }

  selecionarTodasProceduresConexao(): void {
    // Caso o checkbox selecionar todas esteja marcado e seja clicado no mesmo, os itens da lista também são marcados
    if (this.selecionarTodosConexao) {
      this.resultadoDataSourceConexao.forEach((resultados) => {
        return (resultados.checado = true);
      });
      // Caso o checkbox selecionar todas esteja desmarcado e seja clicado no mesmo, os itens da lista também são desmarcados
    } else {
      this.resultadoDataSourceConexao.forEach((resultados) => {
        return (resultados.checado = false);
      });
    }
  }

  carregarProceduresConexao() {
    this.cargaDtoConexao.nomesProcedures =
      this.resultadoDataSourceConexao.filter((resultados) => {
        return resultados.checado;
      });

    if (this.cargaDtoConexao.nomesProcedures.length === 0) {
      this.msgService.info('Select at least one Stored Procedure.');
    } else {
      const apiObserver = {
        next: () => {
          this.msgService.success(
            'Stored Procedure(s) successfully loaded and processed.',
          );
          this.router.navigate(['/analysis/stored-procedure']);
        },
        error: (erro: HttpErrorResponse) => {
          console.error(
            'Erro ao carregar e processar Stored Procedure(s): ',
            erro.message,
          );
          console.error('erro:', JSON.stringify(erro));
          this.msgService.showErrorMessageBackend(erro);
        },
      };
      this.service
        .carregarProceduresSelecionadas(this.cargaDtoConexao)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(apiObserver);
    }
  }

  aposExpandirConexao() {
    this.expandiuConexao = true;
  }

  rotuloBoolean(campo: boolean) {
    return campo ? 'Yes' : 'No ';
  }

  setNomeTipoBancoDeDados() {
    this.nomeTipoBancoDeDados =
      ': ' +
      this.supportedDatabases.filter(
        (sd) => sd.id == this.cargaDtoConexao.tipoBancoDeDados,
      )[0].nome;
    if (this.cargaDtoConexao.tipoBancoDeDados === TipoBancoDeDados.SQLSERVER) {
      this.router.navigate(['/database-connection']);
    } else {
      this.router.navigate(['/file-uploaded']);
    }
  }

  private carregarSupportedDatabase(): void {
    const apiObserver = {
      next: (supportedDatabases: ParDto[]) => {
        this.supportedDatabases = supportedDatabases;
      },
      error: (erro: HttpErrorResponse) => {
        console.error('Erro ao carregar supported database: ', erro.message);
        console.error('erro:', JSON.stringify(erro));
        this.msgService.error('Error retrieving supported database');
      },
    };
    this.service
      .listarSupportedDatabase()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(apiObserver);
  }
}
