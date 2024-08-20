import { Component, Inject, OnInit } from '@angular/core';
import { NgForm, NgModel } from '@angular/forms';
import { GeracaoDto } from '../../../shared/models/geracao-dto';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
import { MsgService } from '../../../shared/services/msg.service';
import { GeracaoService } from '../geracao.service';
import {
  ConfirmDialogModel,
  MaterialConfirmDialogComponent,
} from '../../../shared/components/material-confirm-dialog/material-confirm-dialog.component';
import { take } from 'rxjs';
import { BlockDialogComponent } from '../../../shared/services/dialog/block-dialog.component';
import { environment } from '../../../../environments/environment';
import { takeUntil } from 'rxjs/operators';
import { Unsub } from '../../../shared/util/unsub.class';

@Component({
  selector: 'app-gerar-microsservicos',
  templateUrl: './gerar-microsservicos.component.html',
  styleUrls: ['./gerar-microsservicos.component.scss'],
})
export class GerarMicrosservicosComponent extends Unsub implements OnInit {
  servidorDisponivel = false;
  public geracaoDto: GeracaoDto = new GeracaoDto();
  public dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>;
  public expandiu1 = true;
  public expandiu2 = true;
  public expandiu3 = true;
  public DIALOG_WIDTH = '450px';
  private fileName = 'generated-microservices';

  constructor(
    private service: GeracaoService,
    private msgService: MsgService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super();
  }

  ngOnInit(): void {
    const apiData = this.service.ping();
    const apiObserver = {
      next: () => {
        this.servidorDisponivel = true;
        this.geracaoDto.diretorioDestinoClasses = this.fileName;
        if (!environment.production) {
          this.geracaoDto.databaseName = 'dbp_54808_sig2000';
          this.geracaoDto.databaseUserName = 'sp2mic';
          this.geracaoDto.databasePassword = 'admin';
        }
        this.geracaoDto.springBootVersion = '3.2.6';
        this.geracaoDto.projectMetadataJavaVersion = '22';
        this.geracaoDto.projectMetadataGroupId = 'br.uece.ppgcc';
        this.geracaoDto.projectMetadataArtifactId = 'sp2mic';
        this.geracaoDto.projectMetadataPackageName = `${this.geracaoDto.projectMetadataGroupId}.${this.geracaoDto.projectMetadataArtifactId}`;
        this.geracaoDto.orchestratorPort = '8081';
        this.geracaoDto.gatewayHost = 'localhost';
        this.geracaoDto.gatewayPort = '8090';
        this.geracaoDto.consulHost = 'localhost';
        this.geracaoDto.consulPort = '8500';
        this.geracaoDto.databaseHost = 'localhost';
        this.geracaoDto.databasePort = '1433';
      },
      error: () => {
        this.servidorDisponivel = false;
        this.msgService.error('Service unavailable.');
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  ping() {
    const apiData = this.service.ping();
    const apiObserver = {
      next: () => {
        this.servidorDisponivel = true;
      },
      error: () => {
        this.servidorDisponivel = false;
        this.msgService.error('Service unavailable.');
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  atualizarPackageName() {
    this.geracaoDto.projectMetadataPackageName = `${this.geracaoDto.projectMetadataGroupId}.${this.geracaoDto.projectMetadataArtifactId}`;
  }

  gerar = () => {
    const apiData = this.service.gerarMicrosservicos(this.geracaoDto);
    const apiObserver = {
      next: () => {
        this.msgService.success(
          'Microservices Projects Successfully generated.',
        );
        this.service.download(this.fileName);
      },
      error: (erro: any) => {
        const mensagem =
          'Erro ao gerar microsserviços: ' +
          erro.message +
          ' erro: ' +
          JSON.stringify(erro);
        console.log(mensagem);
        this.msgService.showErrorMessageBackend(erro);
      },
    };
    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  };

  limparFormulario(formGeracao: NgForm) {
    formGeracao.reset();
  }

  aposExpandir1() {
    this.expandiu1 = true;
  }

  aposExpandir2() {
    this.expandiu2 = true;
  }

  aposExpandir3() {
    this.expandiu3 = true;
  }

  abrirModalConfirmcao() {
    const mensagem = 'Do you really want to generate Microservices?';
    const dialogData = new ConfirmDialogModel('Confirm Generation', mensagem);
    const dialogRef = this.dialog.open(MaterialConfirmDialogComponent, {
      maxWidth: this.DIALOG_WIDTH,
      data: dialogData,
      disableClose: true,
    });
    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.unsubscribe$)).subscribe((dialogResult) => {
        if (dialogResult) {
          this.gerar();
        }
      });
  }

  retornaMensagemDeErroCampoInvalido(campoForm: NgModel): string {
    return campoForm.hasError('required')
      ? 'Required field'
      : campoForm.hasError('pattern')
        ? 'Accepted versions: 17 or 20'
        : '';
  }
}
