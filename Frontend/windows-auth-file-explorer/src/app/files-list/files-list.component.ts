import { Component, OnInit } from '@angular/core';
import { FileModel } from '../models/file.model';
import { FileService } from '../services/file.service';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-files-list',
  templateUrl: './files-list.component.html',
  styleUrls: ['./files-list.component.css']
})
export class FilesListComponent implements OnInit {
  files: FileModel[] = [];
  selectedFile?: FileModel;

  constructor(private fileService: FileService, public toastService: ToastService) { }

  ngOnInit(): void {
    this.getFiles();
  }

  getFiles(): void {
    this.fileService.getFiles()
      .subscribe(files => {
        this.files = files;
        this.toastService.show(`Список файлов обновлён`);
      });
  }

  receiveMessage() {
    this.getFiles();
  }
}
