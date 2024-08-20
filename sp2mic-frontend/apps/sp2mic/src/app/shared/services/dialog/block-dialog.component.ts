import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-block-dialog',
  templateUrl: './block-dialog.component.html',
  styleUrls: ['./block-dialog.component.scss'],
})
export class BlockDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
}
