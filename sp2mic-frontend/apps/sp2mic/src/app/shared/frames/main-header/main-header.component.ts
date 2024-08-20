import { AfterViewChecked, Component } from '@angular/core';
import { LayoutService } from '../../services/layout.service';
import { MainMenuComponent } from './main-menu.componet';

@Component({
  selector: 'app-main-header',
  templateUrl: './main-header.component.html',
  styleUrls: ['./main-header.component.scss'],
})
export class MainHeaderComponent implements AfterViewChecked {
  overlayHeader = false;
  mainLogoSrc = '';
  isPhone = false;

  constructor(
    public mainMenu: MainMenuComponent,
    public layoutService: LayoutService,
  ) {}

  ngAfterViewChecked(): void {
    setTimeout(() => {
      this.overlayHeader = this.layoutService.overlayHeader;
      this.mainLogoSrc = this.layoutService.mainLogoSrc;
      this.isPhone = this.layoutService.isPhone;
    });
  }

  toggle() {
    this.mainMenu.toggle();
  }
}
