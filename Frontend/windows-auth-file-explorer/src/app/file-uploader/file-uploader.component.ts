import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FileService } from '../services/file.service';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.css']
})
export class FileUploaderComponent implements OnInit {

  isButtonDisabled: boolean = true;
  selectedFile: File | null = null;
  @Output() messageEvent = new EventEmitter<void>();

  constructor(private fileService: FileService, public toastService: ToastService) { }

  ngOnInit(): void {
  }

  onFileSelected(e: Event) {
    if (e !== null) {
      let target = (<HTMLInputElement>e.target);
      if (target?.files !== null && target.files.length > 0) {
        let file = target.files[0];
        this.fileService.checkFileHash(file)
          .then(result =>
            result.subscribe(r => {
              this.isButtonDisabled = r;
              this.selectedFile = file;
              if (r) {
                this.toastService.show('Данный файл уже существует!',
                  { classname: 'bg-danger text-light' });
              } else {
                this.toastService.show('Выбран уникальный файл!',
                  { classname: 'bg-success text-light' });
              }
            })
          );
      }
    }
  }

  uploadFile() {
    if (this.selectedFile !== null) {
      this.fileService.uploadFile(this.selectedFile)
        .subscribe(_ => {
          this.messageEvent.emit(); 
          this.toastService.show('Файл загружен!',
            { classname: 'bg-success text-light' });
        });
      this.isButtonDisabled = true;
    }
  }
}
