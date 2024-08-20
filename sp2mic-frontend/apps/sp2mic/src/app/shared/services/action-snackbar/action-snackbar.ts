import { Component, Inject, OnDestroy } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarRef } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';
import { AcaoSnackBar } from '../msg.service';

/**
 * Janela para exibição de mensagens com temporizador e ação padrão de fechar.
 */
@Component({
  selector: 'app-action-snackbar',
  templateUrl: './action-snackbar.html',
  styleUrls: ['./action-snackbar.scss'],
})
export class ActionSnackBarComponent<T> implements OnDestroy {
  // valor em % para preenchimento do círculo
  value = 100;
  // subscritor para limpeza da subscription de intervalos
  subs = new Subscription();

  constructor(
    public snackBarRef: MatSnackBarRef<ActionSnackBarComponent<T>>,
    @Inject(MAT_SNACK_BAR_DEFAULT_OPTIONS) public options: any,
    @Inject(MAT_SNACK_BAR_DATA) public data: any,
  ) {}

  /**
   * Retorna a classe CSS para alteração de cor.
   */
  getColor(): string {
    return this.data.panelClass !== ''
      ? this.data.panelClass + '-ico'
      : 'informacao-ico';
  }

  getBackground(): string {
    return this.data.panelClass !== '' ? this.data.panelClass : 'informacao';
  }

  /**
   * Retorna o icone do material icons de acordo com a classe CSS.
   */
  getIcon(): string {
    const className = this.data.panelClass;
    return className === 'erro'
      ? 'error'
      : className === 'aviso'
        ? 'warning'
        : className === 'informacao'
          ? 'info'
          : className === 'sucesso'
            ? 'check_circle'
            : 'announcement';
  }

  /**
   * Libera o subscritor.
   */
  ngOnDestroy(): void {
    if (this.subs) {
      this.subs.unsubscribe();
    }
  }

  /**
   * Nome da acção a executar.
   * @param nome o rótulo a exibir na tela e mapear com a ação.
   */
  executarPeloNomeDaAcao(nome: string) {
    this.snackBarRef.instance.data.action = nome;
    const acoes = this.data.acoes as AcaoSnackBar<T>[];
    if (acoes) {
      const acao = acoes.filter((a: AcaoSnackBar<T>) => {
        return a.nome === nome;
      })[0];
      if (acao && acao.funcao) {
        acao.funcao(nome, this.data.t, this.data, this.snackBarRef);
      }
    }
    this.snackBarRef.dismissWithAction();
  }

  /**
   * Fecha o snackbar.
   */
  fechar() {
    this.snackBarRef.dismiss();
  }
}
