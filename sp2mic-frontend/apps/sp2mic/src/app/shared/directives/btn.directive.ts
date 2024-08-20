import {
  AfterViewInit,
  Directive,
  ElementRef,
  Input,
  OnDestroy,
} from '@angular/core';

@Directive({
  selector: '[appBtn]',
})
export class BtnDirective implements AfterViewInit, OnDestroy {
  @Input() iconLib = 'fa';
  @Input() icon: string | undefined;
  @Input() iconPosition: 'left' | 'right' = 'left';
  @Input() label: string | undefined;
  @Input() type: 'submit' | 'reset' | 'button' = 'button';

  constructor(public elementRef: ElementRef) {}

  ngAfterViewInit() {
    const element = this.elementRef.nativeElement;

    // Add btn class
    if (!element.classList.contains('app-btn')) {
      element.classList.add('app-btn');
    }

    // Set position class
    const rightClass = 'app-btn-icon-right';
    const positionClass = this.iconPosition === 'right' ? rightClass : '';

    if (
      positionClass === rightClass &&
      !element.classList.contains(rightClass)
    ) {
      element.classList.add(rightClass);
    }

    // Set the default button type
    element.setAttribute('type', this.type);

    // Create icon element
    if (this.icon) {
      const iconContainer = document.createElement('span');
      iconContainer.setAttribute('aria-hidden', 'true');
      iconContainer.className = `app-btn-icon ${this.iconLib} ${this.icon}`;
      element.appendChild(iconContainer);
    }

    // Create Label Element
    if (this.label) {
      const labelContainer = document.createElement('span');
      labelContainer.className = 'app-btn-label';
      labelContainer.appendChild(document.createTextNode(this.label));
      element.appendChild(labelContainer);
    }
  }

  ngOnDestroy(): void {
    const element = this.elementRef.nativeElement;
    while (element.hasChildNodes()) {
      element.removeChild(element.lastChild);
    }
  }
}
