import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { FilesListComponent } from '../app/files-list/files-list.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit {
  @ViewChild(FilesListComponent) child: FilesListComponent | null = null;
  title = 'Windows auth file explorer';

  receiveMessage() {
    this.child?.getFiles();
  }

  ngAfterViewInit() {
  }
}
