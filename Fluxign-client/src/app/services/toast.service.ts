import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ToastMessage, ToastType } from '../models/toast';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toastSubject = new Subject<ToastMessage>();
  toast$ = this.toastSubject.asObservable();

  show(type: ToastType, title: string, message: string) {
    this.toastSubject.next({ type, title, message });
  }

  success(title: string, message: string) {
    this.show('success', title, message);
  }

  error(title: string, message: string) {
    this.show('error', title, message);
  }

  warning(title: string, message: string) {
    this.show('warning', title, message);
  }

  info(title: string, message: string) {
    this.show('info', title, message);
  }
}
