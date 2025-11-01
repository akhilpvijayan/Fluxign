import { Component, EventEmitter, Output } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { ToastService } from 'src/app/services/toast.service';

@Component({
  selector: 'app-forgot-password-modal',
  templateUrl: './forgot-password-modal.component.html',
  styleUrls: ['./forgot-password-modal.component.scss']
})
export class ForgotPasswordModalComponent {
  @Output() close = new EventEmitter<void>();

  email: string = '';
  isSubmitting: boolean = false;
  isEmailValid: boolean = false;
  emailTouched: boolean = false;

  // Error messages
  error: string = '';
  validationError: string = '';
  message: string = '';

  constructor(
    private userService: UserService,
    private toastService: ToastService
  ) {}

  // Live email format validation
  onEmailChange() {
    this.emailTouched = true;
    this.validateForm();
  }

  validateEmail(email: string): boolean {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
  }

  validateForm() {
    if (!this.email) {
      this.isEmailValid = false;
      this.validationError = 'Email is required.';
    } else if (!this.validateEmail(this.email)) {
      this.isEmailValid = false;
      this.validationError = 'Please enter a valid email address.';
    } else {
      this.isEmailValid = true;
      this.validationError = '';
    }
  }

  submit() {
    this.emailTouched = true;
    this.validateForm();

    if (!this.isEmailValid) return;

    this.isSubmitting = true;
    this.error = '';
    this.message = '';

    this.userService.requestPasswordReset(this.email).subscribe({
      next: res => {
        this.isSubmitting = false;
        if (res.isSuccess) {
          this.message = res.message || 'Password reset link sent.';
          this.toastService.success(this.message, '');
        } else {
          this.error = res.message || 'Something went wrong.';
          this.toastService.error('Failed', this.error);
        }
      },
      error: err => {
        this.isSubmitting = false;
        this.error = err?.error?.message || 'Server error occurred.';
        this.toastService.error('Failed', this.error);
      }
    });
  }

  onCancel() {
    this.close.emit();
  }
}
