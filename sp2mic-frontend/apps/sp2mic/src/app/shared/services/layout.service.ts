import { Injectable } from '@angular/core';

const DEVICES = [
  { type: 'phone', min: 0, max: 767 },
  { type: 'tablet', min: 768, max: 991 },
  { type: 'desktop', min: 992, max: Number.MAX_VALUE },
];

@Injectable({
  providedIn: 'root',
})
export class LayoutService {
  width = 0;
  height = 0;
  scrollTop = 0;
  scrollLeft = 0;
  headerHeight = 0;

  logo = {
    original: 'assets/img/template/sp2mic-logo.png',
    inline: 'assets/img/template/sp2mic-logo.png',
  };

  // https://www.brandcrowd.com/maker/logo/digital-tech-cloud-149128#popup-colorpalette
  constructor() {
    //Construtor vazio Intencional. Retirar Comentário somente caso seja inicializado.
  }

  get deviceType(): string {
    const dev = DEVICES.filter(
      (d) => this.width >= d.min && this.width <= d.max,
    );
    return dev[0] ? dev[0].type : 'unknown-device';
  }

  get isPhone() {
    return this.deviceType === 'phone';
  }

  get overlayHeader(): boolean {
    return this.scrollTop > this.headerHeight;
  }

  get mainLogoSrc(): string {
    return this.isPhone || this.overlayHeader
      ? this.logo.inline
      : this.logo.original;
  }
}
