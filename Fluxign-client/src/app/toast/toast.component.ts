import { Component, OnInit } from '@angular/core';
import { ToastMessage } from '../models/toast';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss'],
})
export class ToastComponent implements OnInit {
  toast: ToastMessage | null = null;
  visible = false;
  timeout: any;

  constructor(private toastService: ToastService) {}

  ngOnInit(): void {
    this.toastService.toast$.subscribe((toast: any) => {
      this.toast = toast;
      this.visible = true;

      clearTimeout(this.timeout);
      this.timeout = setTimeout(() => {
        this.visible = false;
        setTimeout(() => (this.toast = null), 300); // wait for animation
      }, 4000);
    });
  }

  icon(type: string): string {
    switch (type) {
      case 'success': return '✅';
      case 'error': return '❌';
      case 'warning': return '⚠️';
      default: return 'ℹ️';
    }
  }

  bgGlassColor(type: string): string {
    switch (type) {
      case 'success':
        return 'border-green-400 text-white shadow-green-500/30';
      case 'error':
        return 'border-red-400 text-white shadow-red-500/30';
      case 'warning':
        return 'border-yellow-400 text-black shadow-yellow-400/30';
      case 'info':
        return 'border-blue-400 text-white shadow-blue-500/30';
      default:
        return 'border-gray-400 text-white shadow-gray-500/30';
    }
  }
  
  close() {
    this.visible = false;
    setTimeout(() => (this.toast = null), 300);
  }
}
