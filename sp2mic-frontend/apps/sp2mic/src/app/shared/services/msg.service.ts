import { Subscription } from 'rxjs';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import {
  MatSnackBar,
  MatSnackBarHorizontalPosition,
  MatSnackBarRef,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';
//import { ActionSnackBarComponent } from './action-snackbar/action-snackbar';
import { HttpErrorResponse } from '@angular/common/http';
import { HTTP_STATUS } from './http-status';
import { BlockDialogComponent } from './dialog/block-dialog.component';
import { isNullOrUndefined } from '../util/utils';
import { ActionSnackBarComponent } from './action-snackbar/action-snackbar';
import { Overlay } from '@angular/cdk/overlay';
import { GenericDialogComponent } from '../components/generic-dialog/generic-dialog.component';

//import { ActionSnackBarComponent } from './action-snackbar/action-snackbar';

/**
 * Encapsula uma ação e uma função a ser executada ao clicar no snackbar.
 */
export class AcaoSnackBar<T> {
  /**
   * Construtor da dto-classe.
   * @param nome o nome a exibir como botão para o snackbar.
   * @param funcao uma função associada ao nome. Executada, caso exista,
   * com os parâmetros vistos em @method message em MsgService.
   */
  constructor(
    public nome: string,
    public funcao?: (
      nome: string,
      t: T,
      data: any,
      snackBarRef: MatSnackBarRef<ActionSnackBarComponent<T>>,
    ) => void,
  ) {}
}

// @Component({
//   selector: 'app-dialog',
//   templateUrl: './dialog/block-dialog.component.html',
//   styleUrls: ['./dialog/block-dialog.component.scss']
// })
// export class BlockDialogComponent {
//
//   constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
//   }
// }

@Injectable({ providedIn: 'root' })
export class MsgService {
  constructor(
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private overlay: Overlay,
  ) {}

  /**
   * Cria uma mensagem com uma lista de ações a serem executadas.
   * Em caso de não ser provida uma lista de ações, é criada uma padrão com a ação de fechar.
   * @param msg a mensagem, ou lista de mensagens, a exibir.
   * @param acoes as ações a serem disparadas pelo usuário.
   * @param segundos quantos segundos a tela deve se manter antes de desaparecer.
   * 5 se não for definido ou se o valor for menor que zero.
   * @param titulo o título a exibir.
   * @param subtitulo o subtítulo a exibir.
   * @param panelClass a classe base para alterar a cor da borda e seleção do logo de exibição.
   * @param hPosition a posição no eixo horizontal como MatSnackBarHorizontalPosition.
   * @param vPosition a posição no eixo vertical como MatSnackBarVerticalPosition.
   * @param t uma instância de T que pode ser passada para execução da função associada a uma das ações passadas.
   */
  message<T>(
    msg: string | string[],
    acoes: AcaoSnackBar<T>[] = [new AcaoSnackBar('Close')],
    segundos = 8,
    titulo = 'Information',
    subtitulo = 'Operation Details',
    panelClass = 'informacao',
    hPosition = 'right',
    vPosition = 'top',
    t?: T,
  ): MatSnackBarRef<ActionSnackBarComponent<T>> {
    return this.snackBar.openFromComponent(ActionSnackBarComponent, {
      duration: segundos > 0 ? segundos * 1000 : 5000,
      verticalPosition: vPosition as MatSnackBarVerticalPosition,
      horizontalPosition: hPosition as MatSnackBarHorizontalPosition,
      panelClass,
      data: {
        textos: msg instanceof Array ? msg : [msg],
        titulo,
        subtitulo,
        t,
        acoes,
        panelClass,
      },
    });
  }

  /**
   * Cria uma mensagem de erro.
   * Vide @method message para maiores detalhes.
   */
  error<T>(
    msg: string | string[],
    acoes: AcaoSnackBar<T>[] = [new AcaoSnackBar('Close')],
    segundos = 8,
    titulo = 'Error',
    subtitulo = 'Operation Details',
    panelClass = 'erro',
    hPosition = 'right',
    vPosition = 'top',
    t?: T,
  ): MatSnackBarRef<ActionSnackBarComponent<T>> {
    return this.message(
      msg,
      acoes,
      segundos,
      titulo,
      subtitulo,
      panelClass,
      hPosition,
      vPosition,
      t,
    );
  }

  /**
   * Cria uma mensagem de sucesso.
   * Vide @method message para maiores detalhes.
   */
  success<T>(
    msg: string | string[],
    acoes: AcaoSnackBar<T>[] = [new AcaoSnackBar('Close')],
    segundos = 8,
    titulo = 'Success',
    subtitulo = 'Operation Details',
    panelClass = 'sucesso',
    hPosition = 'right',
    vPosition = 'top',
    t?: T,
  ): MatSnackBarRef<ActionSnackBarComponent<T>> {
    return this.message(
      msg,
      acoes,
      segundos,
      titulo,
      subtitulo,
      panelClass,
      hPosition,
      vPosition,
      t,
    );
  }

  /**
   * Cria uma mensagem de informação.
   * Vide @method message para maiores detalhes.
   */
  info<T>(
    msg: string | string[],
    acoes: AcaoSnackBar<T>[] = [new AcaoSnackBar('Close')],
    segundos = 8,
    titulo = 'Information',
    subtitulo = 'Operation Details',
    panelClass = 'informacao',
    hPosition = 'right',
    vPosition = 'top',
    t?: T,
  ): MatSnackBarRef<ActionSnackBarComponent<T>> {
    return this.message(
      msg,
      acoes,
      segundos,
      titulo,
      subtitulo,
      panelClass,
      hPosition,
      vPosition,
      t,
    );
  }

  /**
   * Cria uma mensagem de aviso.
   * Vide @method message para maiores detalhes.
   */
  warn<T>(
    msg: string | string[],
    acoes: AcaoSnackBar<T>[] = [new AcaoSnackBar('Close')],
    segundos = 8,
    titulo = 'Warning',
    subtitulo = 'Operation Details',
    panelClass = 'aviso',
    hPosition = 'right',
    vPosition = 'top',
    t?: T,
  ): MatSnackBarRef<ActionSnackBarComponent<T>> {
    return this.message(
      msg,
      acoes,
      segundos,
      titulo,
      subtitulo,
      panelClass,
      hPosition,
      vPosition,
      t,
    );
  }

  displayAlertDialog(options?: any) {
    let global_options = {
      autoFocus: false,
      panelClass: 'aviso',
      scrollStrategy: this.overlay.scrollStrategies.noop(),
    };
    let dialog_config = { ...global_options, ...options };
    return this.dialog.open(GenericDialogComponent, dialog_config);
  }

  unsubscribeAll(subs: Subscription[]) {
    let sub_count = subs.length;
    for (let i = 0; i < sub_count; i++) {
      let current_sub = subs[i];
      if (!!current_sub) {
        current_sub.unsubscribe();
      }
    }
  }

  scrollToElement(element_ref: any, offset = 10) {
    setTimeout(() => {
      let is_selector = typeof element_ref == 'string';
      let element = is_selector
        ? document.querySelector(element_ref)
        : element_ref;
      let scroll_extent =
        element.getBoundingClientRect().top + window.pageYOffset - offset;
      window.scrollTo(0, scroll_extent);
    }, 200);
  }

  /**
   * Exibe a mensagem de erro com texto vindo do backend.
   * @param errors o objeto do tipo HttpErrorResponse.
   */

  /* showErrorMessageBackend(status: number, msg: string | string[]): void {
   if (status === 0 || status === undefined) {
   this.error('Server error.');
   } else {
   if (status === HTTP_STATUS.SERVICE_UNAVAILABLE_503 || status === HTTP_STATUS.PERMISSION_DENIED_550
   || status === HTTP_STATUS.GATEWAY_TIMEOUT_504 || status === HTTP_STATUS.INTERNAL_SERVER_ERROR_500) {
   this.error('Service unavailable.');
   } else {
   this.error(msg);
   }
   }
   }*/

  /**
   * Exibe a mensagem de erro com texto vindo do backend.
   * @param errors o objeto do tipo HttpErrorResponse.
   */
  showErrorMessageBackend(errors: HttpErrorResponse): void {
    switch (errors.status) {
      case 0 || undefined:
        this.error('Service unavailable.');
        return;
      case HTTP_STATUS.SERVICE_UNAVAILABLE_503:
        this.error('Service unavailable.');
        return;
      case HTTP_STATUS.PERMISSION_DENIED_550:
        this.error('Permission denied.');
        return;
      case HTTP_STATUS.GATEWAY_TIMEOUT_504:
        this.error('Gateway timeout.');
        return;
      case HTTP_STATUS.INTERNAL_SERVER_ERROR_500:
        this.error('Internal server error.');
        return;
      default:
        this.error('Internal server error.');
    }

    if (!isNullOrUndefined(errors.error.errors)) {
      this.error(errors.error.errors);
    } else {
      if (!isNullOrUndefined(errors.error.message)) {
        this.error(errors.error.message);
      } else {
        this.error(errors.statusText);
      }
    }
  }

  /**
   * Cria um dialog que bloqueia a tela com um spinner e uma lista de mensagens para o usuário.
   * @param msg uma mensagem ou lista de mensagens a exibir. Vazio por padrão.
   * @param largura a largura da tela do dialog. 400px o padrão.
   * @param altura a altura da tela do dialog. 200ps o padrão.
   */
  openDialogDisableClose(
    msg: string | string[] = '',
    largura = 400,
    altura = 180,
  ): MatDialogRef<BlockDialogComponent, any> {
    return this.dialog.open(BlockDialogComponent, {
      //panelClass: 'custom-modalbox',
      width: (largura > 0 ? largura : 400) + 'px',
      height: (altura > 0 ? altura : 170) + 'px',
      disableClose: true,
      hasBackdrop: true,
      data: msg instanceof Array ? msg : [msg],
    });
  }
}
