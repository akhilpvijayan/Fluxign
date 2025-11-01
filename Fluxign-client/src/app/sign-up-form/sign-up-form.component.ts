import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SignUpService } from '../services/sign-up.service';

@Component({
  selector: 'app-sign-up-form',
  templateUrl: './sign-up-form.component.html',
  styleUrls: ['./sign-up-form.component.scss']
})
export class SignUpFormComponent implements OnInit {
  signupForm!: FormGroup;
  showPassword: boolean = false;
  showConfirmPassword: boolean = false;

  constructor(private fb: FormBuilder, private signupservice: SignUpService) { }

  ngOnInit(): void {
    this.signupForm = this.fb.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      userEmail: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)]],
      userPhone: ['', [Validators.required, Validators.minLength(9), Validators.maxLength(9)]],
      emiratesId: ['', [Validators.required, Validators.pattern(/^\d{3}-\d{4}-\d{7}-\d{1}$/)]],
      confirmPassword: ['', Validators.required]
    },
      { validator: this.passwordMatchValidator });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  isInvalid(controlName: string): boolean {
    const control = this.signupForm.get(controlName);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  passwordMatchValidator(group: FormGroup) {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  get passwordMismatch(): boolean {
    return Boolean(
      this.signupForm.hasError('passwordMismatch') &&
      this.signupForm.get('confirmPassword')?.touched
    );
  }

  onSubmit(): void {
    if (this.signupForm.valid) {
      this.signupForm.value.role = "User";
      const formData = this.signupForm.value;
      this.signupservice.createUser(formData).subscribe((res: any) => {
        if (res) {

        }
      })
    } else {
      this.signupForm.markAllAsTouched();
    }
  }

  formatEmiratesId(): void {
    const control = this.signupForm.get('emiratesId');
    if (!control) return;

    let digits = control.value.replace(/\D/g, '');

    if (digits.length > 3) digits = digits.slice(0, 3) + '-' + digits.slice(3);
    if (digits.length > 8) digits = digits.slice(0, 8) + '-' + digits.slice(8);
    if (digits.length > 16) digits = digits.slice(0, 16) + '-' + digits.slice(16);
    if (digits.length > 18) digits = digits.slice(0, 18);

    control.setValue(digits, { emitEvent: false });
  }

  onMobileInput(event: Event) {
    const input = event.target as HTMLInputElement;
    let digitsOnly = input.value.replace(/\D/g, '');

    if (digitsOnly.length > 9) {
      digitsOnly = digitsOnly.slice(0, 9);
    }

    input.value = digitsOnly;
    this.signupForm.get('userPhone')?.setValue(digitsOnly, { emitEvent: false });
  }
}

