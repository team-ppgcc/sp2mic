import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import isNil from 'lodash/isNil';
import isPlainObject from 'lodash/isPlainObject';

@Injectable({
  providedIn: 'root',
})
export class UtilsService {
  constructor() {}

  public static buildQueryParams(source: Object): HttpParams {
    let target: HttpParams = new HttpParams();
    if (isNil(source)) {
      return null;
    }
    Object.keys(source).forEach((key: string) => {
      let value: any = source[key];
      if (isNil(value)) {
        return;
      }
      if (isPlainObject(value)) {
        value = JSON.stringify(value);
      } else {
        value = value.toString();
      }
      target = target.append(key, value);
    });
    return target;
  }
}
