import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm } from '@angular/forms';
import { MsgService } from '../../../shared/services/msg.service';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { CargaService } from '../carga.service';
import { ParDto } from '../../../shared/models/par-dto';
import { MatTableDataSource } from '@angular/material/table';
import { HttpErrorResponse } from '@angular/common/http';
import { CargaDto } from '../../../shared/models/carga-dto';
import { MatPaginator } from '@angular/material/paginator';
import { UtilsHtml } from '../../../shared/util/utils-html';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import { MatSort } from '@angular/material/sort';
import { Observable } from 'rxjs';
import { map, startWith, takeUntil } from 'rxjs/operators';
import { TipoBancoDeDados } from '../../../shared/enums/tipo-banco-de-dados';
import {
  ConfirmDialogDatabaseSchemaModel,
  MaterialConfirmDialogDatabaseSchemaComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog-database-schema';
import { environment } from '../../../../environments/environment';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-file-uploaded',
  templateUrl: './file-uploaded.component.html',
  styleUrls: ['./file-uploaded.component.scss'],
})
export class FileUploadedComponent extends Unsub implements OnInit {
  storedProcedureFormControl = new FormControl<string | ParDto>('');
  storedProcedureOptions: ParDto[] = [];
  storedProcedureFilteredOptions: Observable<ParDto[]>;

  public arquivoEnviado: boolean;
  public sendProcedureFile: boolean;
  public cargaDtoArquivo = new CargaDto();
  public resultadoDataSourceArquivo: ParDto[] = [];
  public displayedColumnsArquivo = ['snCarregada', 'nome'];

  public dataSourceArquivo: MatTableDataSource<ParDto>;
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('arquivoSP', { static: true }) arquivoSP: ElementRef;

  public selecionarTodosArquivo = false;
  public pesquisouArquivo = false;
  public expandiuArquivo = true;

  public tamanhoPaginaPadrao = UtilsHtml.tamanhoPaginaPadrao;
  public opcoesTamanhoPagina = UtilsHtml.opcoesTamanhoPagina;

  public noStoredProcedure: string;
  public formUploadArquivo: FormGroup;
  public DIALOG_WIDTH = '290px';
  public DIALOG_HEIGHT = '245px';

  constructor(
    private formBuilder: FormBuilder,
    private msgService: MsgService,
    private router: Router,
    private service: CargaService,
    private dialog: MatDialog,
  ) {
    super();
    this.dataSourceArquivo = new MatTableDataSource([]);
    const nav = this.router.getCurrentNavigation();
    if (nav.extras.state) {
      this.arquivoEnviado = nav.extras.state['enviado'];
    }
  }

  storedProcedureDisplayFn(sp: ParDto): string {
    return sp && sp.nome ? sp.nome : '';
  }

  ngOnInit(): void {
    this.cargaDtoArquivo.tipoBancoDeDados = TipoBancoDeDados.SQLSERVER_FILE;
    if (!environment.production) {
      this.cargaDtoArquivo.schema = 'dbo';
    }
    this.formUploadArquivo = this.formBuilder.group({
      arquivoFormControl: new FormControl(),
    });
    this.pesquisouArquivo = false;
    this.expandiuArquivo = true;
    this.selecionarTodosArquivo = false;
    this.displayedColumnsArquivo.unshift('checado');
    this.storedProcedureFilteredOptions =
      this.storedProcedureFormControl.valueChanges.pipe(
        startWith(''),
        map((value) => {
          const name = typeof value === 'string' ? value : value?.nome;
          return name
            ? this._storedProcedureFilter(name as string)
            : this.storedProcedureOptions.slice();
        }),
      );
    this.pesquisarArquivo();
  }

  recuperarNomeSpSelecionada(): string {
    let obj = this.storedProcedureFormControl.value;
    return typeof obj === 'string' ? obj : obj?.nome;
  }

  pesquisarArquivo() {
    this.pesquisouArquivo = true;
    this.expandiuArquivo = false;
    this.selecionarTodosArquivo = false;
    this.resultadoDataSourceArquivo = [];
    this.dataSourceArquivo = new MatTableDataSource([]);

    const apiObserver = {
      next: (res: any) => {
        this.resultadoDataSourceArquivo = res.body;
        this.dataSourceArquivo = new MatTableDataSource(
          this.resultadoDataSourceArquivo,
        );
        this.dataSourceArquivo.paginator = this.paginator;
        this.dataSourceArquivo.sort = this.sort;
      },
      error: (erro: HttpErrorResponse) => {
        this.pesquisouArquivo = false;
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    this.service
      .listarNomesProcedures(this.cargaDtoArquivo)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(apiObserver);
  }

  limparFormularioArquivo(form: NgForm) {
    this.pesquisouArquivo = false;
    this.expandiuArquivo = true;
    form.reset();
    this.storedProcedureFormControl.reset();
    this.resultadoDataSourceArquivo = [];
    this.dataSourceArquivo = new MatTableDataSource([]);
    this.cargaDtoArquivo.sendProcedureFile = false;
  }

  selecionarTodasProceduresArquivo(): void {
    // Caso o checkbox selecionar todas esteja marcado e seja clicado no mesmo, os itens da lista também são marcados
    if (this.selecionarTodosArquivo) {
      this.resultadoDataSourceArquivo.forEach((resultadosArquivo) => {
        return (resultadosArquivo.checado = true);
      });
      // Caso o checkbox selecionar todas esteja desmarcado e seja clicado no mesmo, os itens da lista também são desmarcados
    } else {
      this.resultadoDataSourceArquivo.forEach((resultadosArquivo) => {
        return (resultadosArquivo.checado = false);
      });
    }
  }

  abrirModalSchema() {
    const dialogData = new ConfirmDialogDatabaseSchemaModel('');
    const dialogRef = this.dialog.open(
      MaterialConfirmDialogDatabaseSchemaComponent,
      {
        width: this.DIALOG_WIDTH,
        minWidth: this.DIALOG_WIDTH,
        maxWidth: this.DIALOG_WIDTH,
        height: this.DIALOG_HEIGHT,
        data: dialogData,
        disableClose: true,
      },
    );
    this.cargaDtoArquivo.schema = null;
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((dialogResult) => {
        if (dialogResult) {
          if (dialogResult.databaseSchema) {
            this.cargaDtoArquivo.schema = dialogResult.databaseSchema;
          }
          this.carregarProceduresArquivo();
        }
      });
  }

  carregarProceduresArquivo() {
    this.cargaDtoArquivo.nomesProcedures =
      this.resultadoDataSourceArquivo.filter((resultadosArquivo) => {
        return resultadosArquivo.checado;
      });
    if (this.cargaDtoArquivo.nomesProcedures.length === 0) {
      this.msgService.info('Select at least one Stored Procedure.');
    } else {
      const apiObserverCarregarProceduresArquivo = {
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
        .carregarProceduresSelecionadas(this.cargaDtoArquivo)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(apiObserverCarregarProceduresArquivo);
    }
  }

  incluirArquivo() {
    this.router.navigate(['/send-procedure-file']);
  }

  aposExpandirArquivo() {
    this.expandiuArquivo = true;
  }

  rotuloBoolean(campo: boolean) {
    return campo ? 'Yes' : 'No';
  }

  private _storedProcedureFilter(name: string): ParDto[] {
    const filterValue = name.toLowerCase();
    return this.storedProcedureOptions.filter((option) =>
      option.nome.toLowerCase().includes(filterValue),
    );
  }
}
