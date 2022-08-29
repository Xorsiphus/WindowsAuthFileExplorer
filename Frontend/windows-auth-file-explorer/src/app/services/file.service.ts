import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as CryptoJS from 'crypto-js';


import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { ToastService } from './toast.service';
import { FileModel } from '../models/file.model';


@Injectable({ providedIn: 'root' })
export class FileService {

    private filesUrl = 'https://localhost:7188/FileManagement/';

    httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    constructor(private http: HttpClient, public toastService: ToastService) { }

    getFiles(): Observable<FileModel[]> {
        const link = this.filesUrl + 'GetUserFiles';
        const httpOptions = { withCredentials: true };
        return this.http.get<FileModel[]>(link, httpOptions)
            .pipe(
                tap(_ => this.log('fetched files')),
                catchError(this.handleError<FileModel[]>('getFiles', []))
            );
    }

    getFileLink(fileName: string) {
        return this.filesUrl + 'GetFile/' + fileName;
    }

    deleteFile(fileName: string) {
        const link = this.filesUrl + 'DeleteFile/' + fileName;
        const httpOptions = { withCredentials: true };

        return this.http.delete(link, httpOptions)
            .pipe(
                tap(_ => this.log('deleted file' + fileName)),
                catchError(this.handleError<string>('getFiles', fileName))
            );
    }

    uploadFile(file: File) {
        const link = this.filesUrl + 'AddFile';

        const formData = new FormData();
        formData.append('uploadedFile', file);
        const httpOptions = { withCredentials: true };

        return this.http.post<number>(link, formData, httpOptions)
            .pipe(
                tap(_ => this.log('uploaded file' + file.name)),
                catchError(this.handleError<File>('getFiles', file))
            );
    }

    async checkFileHash(file: File) {
        const link = this.filesUrl + 'GetFileExistence/' + file.name;

        const fileHash = await this.encodeFileToMD5(file);
        const httpOptions = { withCredentials: true, params: { fileHash: fileHash } };

        return this.http.get<boolean>(link, httpOptions)
            .pipe(
                tap(_ => this.log('file "' + file.name + '" hash "' + fileHash + '" has checked')),
                catchError(this.handleError<boolean>('getFiles', false))
            );
    }

    private encodeFileToMD5(file: File): Promise<string> {
        const reader = new FileReader();
        reader.readAsBinaryString(file);
        return new Promise((resolve, reject) => {
            reader.onerror = () => {
                reader.abort();
                reject(new DOMException("Problem parsing input file."));
            };

            reader.onload = (e) => {
                resolve(CryptoJS
                    .MD5(CryptoJS.enc.Latin1.parse(e.target!.result as string))
                    .toString()
                    .replace('-', '')
                    .toLocaleLowerCase()
                );
            };
        });
    }

    private handleError<T>(operation = 'operation', result?: T) {
        return (error: any): Observable<T> => {
            console.error(error);
            this.log(`${operation} failed: ${error.message}`);
            this.toastService.show(`Ошбика выполнеия запроса: ${error.status} - ${error.statusText}`,
                  { classname: 'bg-danger text-light' });
            return of(result as T);
        };
    }

    private log(message: string) {
        console.log(`${message}`);
    }
}