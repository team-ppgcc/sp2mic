// import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
// import {MAT_SNACK_BAR_DATA, MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarRef} from '@angular/material/snack-bar';
// import {interval, Subscription} from 'rxjs';
// import {take} from 'rxjs/operators';
//
// const CEM = 100;
// const ZERO = 0;
// const TICK = 1000;
//
// /**
//  * Janela para exibição de mensagens com temporizador e ação padrão de fechar.
//  */
// @Component({
//   selector: 'app-countdown-snackbar',
//   templateUrl: './countdown-snackbar.html',
//   styleUrls: ['./countdown-snackbar.scss'],
// })
// export class CountDownComponent implements OnInit, OnDestroy {
//
//   // valor em % para preenchimento do círculo
//   value = 100;
//   // subscritor para limpeza da subscription de intervalos
//   subs = new Subscription();
//
//   constructor(
//     public snackBarRef: MatSnackBarRef<CountDownComponent>,
//     @Inject(MAT_SNACK_BAR_DEFAULT_OPTIONS) public options: any,
//     @Inject(MAT_SNACK_BAR_DATA) public data: any,
//   ) {
//   }
//
//   ngOnInit(): void {
//     //let s: any;
//
//     const apiData = interval(TICK).pipe(take(5));
//
//     const apiObserver = {
//       next: (v: any) => {
//         // começa em 0, precisa incrementar de 1
//         v = v + 1;
//         // valor para o percentual de preenchimento do circulo
//         this.value = CEM - (v * 20) <= 0 ? 0 : CEM - (v * 20);
//         // se zerar, finalize a janela
//         if (this.value <= ZERO) {
//           setInterval(
//             () => {
//               this.fechar("undefined");
//             }, 100
//           );
//         }
//       },
//       error: (erro) => {
//         console.error('Erro do temporizador: ' + JSON.stringify(errors));
//       }
//     }
//
//     let s = apiData.subscribe(apiObserver);
//
//     /*
//     // receba os números produzidos, 5 vezes, uma a cada segundo
//     const s = interval(TICK).pipe(take(5)).subscribe({
//       v  =>    {
//       // começa em 0, precisa incrementar de 1
//       v = v + 1;
//       // valor para o percentual de preenchimento do circulo
//       this.value = CEM - (v * 20) <= 0 ? 0 : CEM - (v * 20);
//       // se zerar, finalize a janela
//       if (this.value <= ZERO) {
//         setInterval(
//           () => {
//             this.fechar("undefined");
//           }, 100
//         );
//       }
//     }
//   ,
//     erro => {
//       console.error('Erro do temporizador: ' + JSON.stringify(erro));
//     }
//   });
//     */
//     this.subs.add(s);
//   }
//
//   /**
//    * Libera o subscritor.
//    */
//   ngOnDestroy(): void {
//     if (this.subs) {
//       this.subs.unsubscribe();
//     }
//   }
//
//   /**
//    * Fecha o snackbar.
//    */
//   fechar(msg: string) {
//     if (msg !== "undefined") {
//       this.snackBarRef.instance.data.action = msg;
//       this.snackBarRef.dismissWithAction();
//     } else {
//       this.snackBarRef.dismiss();
//     }
//   }
//
// }
