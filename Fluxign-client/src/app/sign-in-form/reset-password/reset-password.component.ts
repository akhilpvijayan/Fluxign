import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent {
  password: string = '';
  confirmPassword: string = '';
  message: string = '';
  error: string = '';
  token: string | null = null;

  showPassword: boolean = false;
  showConfirmPassword: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {
    this.token = this.route.snapshot.queryParamMap.get('token');
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  submit(form: any) {
    this.error = '';
    this.message = '';

    if (!this.isStrongPassword(this.password)) {
      this.error = 'Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.';
      return;
    }
    
    if (!this.token) {
      this.error = 'Invalid or missing token.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = "Passwords don't match.";
      return;
    }

    if (this.password.length < 8) {
      this.error = 'Password must be at least 8 characters long.';
      return;
    }

    this.authService.resetPassword(this.token, this.password).subscribe((res: any) => {
      if (res.isSuccess) {
        this.message = 'Password reset successful! Redirecting to login...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      } else {
        this.error = res.message || 'Failed to reset password.';
      }
    });
  }

  isStrongPassword(password: string): boolean {
    const strongPasswordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]).{8,}$/;
    return strongPasswordRegex.test(password);
  }
  
}
