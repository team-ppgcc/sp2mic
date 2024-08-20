export class Utils {
  static isEmptyObject(o: object): boolean {
    return Object.keys(o).length === 0;
  }

  static readRouteParam(params, paramName) {
    return Utils.isEmptyObject(params) ? undefined : params[paramName];
  }

  static isSomeValueDefined(someValue: any): boolean {
    return !!someValue;
  }

  // tslint:enable:no-null-keyword

  // static abrirBloqueioTela(texto: string, msgService: MsgService) {
  //   return msgService.openDialogDisableClose(texto);
  // }

  // static fecharBloqueioTela(
  //   dialogRef: MatDialogRef<BlockDialogComponent, BlockDialogComponent>
  // ) {
  //   if (dialogRef && dialogRef.getState() !== MatDialogState.CLOSED) {
  //     dialogRef.close();
  //   }
  // }
}

export const isNullOrUndefined = <T>(
  obj: T | null | undefined,
): obj is null | undefined => {
  return typeof obj === 'undefined' || obj === null;
};
