import { CdkTableModule } from '@angular/cdk/table';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UiSwitchModule } from 'ngx-ui-switch';
import { IconComponent } from './components/icon/icon.component';
import { MaterialConfirmDialogComponent } from './components/material-confirm-dialog/material-confirm-dialog.component';
import { BtnDirective } from './directives/btn.directive';
import { DisableControlDirective } from './directives/disable-control.directive';
import { BreadcrumbComponent } from './frames/breadcrumb/breadcrumb.component';
import { CrumbComponent } from './frames/breadcrumb/crumb/crumb.component';
import { MainContentComponent } from './frames/main-content/main-content.component';
import { MainFooterComponent } from './frames/main-footer/main-footer.component';
import { MainFrameComponent } from './frames/main-frame/main-frame.component';
import { MainHeaderComponent } from './frames/main-header/main-header.component';
import { MaterialModule } from '../material.module';
import { ClasseAutocompleteComponent } from './components/classe-autocomplete/classe-autocomplete.component';
import { BlockDialogComponent } from './services/dialog/block-dialog.component';
import { ActionSnackBarComponent } from './services/action-snackbar/action-snackbar';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { MaterialConfirmDialogDatabaseSchemaComponent } from './components/material-confirm-dialog/material-confirm-dialog-database-schema';
import { FileUploaderComponent } from '../pages/1-carga/file-uploaded/file-uploader/file-uploader.component';
import { DragAndDropDirective } from './services/drag-and-drop-directive/drag-and-drop.directive';
import { MatRippleModule } from '@angular/material/core';
import { GenericDialogComponent } from './components/generic-dialog/generic-dialog.component';

@NgModule({
  declarations: [
    BtnDirective,
    DisableControlDirective,
    // HasErrorDirective,
    IconComponent,
    BreadcrumbComponent,
    CrumbComponent,
    MainHeaderComponent,
    MainFooterComponent,
    MainFrameComponent,
    MainContentComponent,
    MaterialConfirmDialogComponent,
    MaterialConfirmDialogDatabaseSchemaComponent,
    GenericDialogComponent,
    FileUploaderComponent,
    DragAndDropDirective,
    ClasseAutocompleteComponent,
    ActionSnackBarComponent,
    BlockDialogComponent,
    SpinnerComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    CdkTableModule,
    FormsModule,
    MaterialModule,
    ReactiveFormsModule,
    UiSwitchModule.forRoot({
      color: 'rgb(255, 255, 255)',
      switchColor: '#303f9f',
      checkedLabel: 'Cancel',
      uncheckedLabel: 'Edit',
    }),
    MatRippleModule,
  ],
  exports: [
    //MaterialModule, // Esse export não dá problema?
    BtnDirective,
    DisableControlDirective,
    CdkTableModule,

    UiSwitchModule,
    IconComponent,
    BreadcrumbComponent,
    CrumbComponent,
    MainHeaderComponent,
    MainFooterComponent,
    MainFrameComponent,
    MainContentComponent,
    MaterialConfirmDialogComponent,
    MaterialConfirmDialogDatabaseSchemaComponent,
    GenericDialogComponent,
    ClasseAutocompleteComponent,
    ActionSnackBarComponent,
    BlockDialogComponent,
    SpinnerComponent,
    FileUploaderComponent,
  ],
})
export class SharedModule {}
