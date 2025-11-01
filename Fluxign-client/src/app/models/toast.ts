export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastMessage {
  title: string;
  message: string;
  type: ToastType;
}

