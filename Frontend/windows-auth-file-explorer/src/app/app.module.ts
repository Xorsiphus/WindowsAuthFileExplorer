import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FilesListComponent } from './files-list/files-list.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FileElementComponent } from './file-element/file-element.component';
import { FileUploaderComponent } from './file-uploader/file-uploader.component';
import { ToastContainerComponent } from './toast-container/toast-container.component';

@NgModule({
  declarations: [
    AppComponent,
    FilesListComponent,
    FileElementComponent,
    FileUploaderComponent,
    ToastContainerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
