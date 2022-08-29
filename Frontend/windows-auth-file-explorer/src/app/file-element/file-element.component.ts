import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileModel } from '../models/file.model';
import { FileService } from '../services/file.service';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-file-element',
  templateUrl: './file-element.component.html',
  styleUrls: ['./file-element.component.css']
})
export class FileElementComponent implements OnInit {
  @Output() messageEvent = new EventEmitter<void>();
  @Input() file?: FileModel;

  constructor(public fileService: FileService, public toastService: ToastService) { }

  ngOnInit(): void {
  }

  deleteFile(fileName: string): void {
    this.fileService.deleteFile(fileName)
      .subscribe(_ => {
        this.file = undefined;
        this.toastService.show(`Файл "${fileName}" удалён!`,
          { classname: 'bg-success text-light' });
        this.messageEvent.emit();
      });
  }
}
