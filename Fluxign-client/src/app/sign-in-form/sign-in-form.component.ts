import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-sign-in-form',
  templateUrl: './sign-in-form.component.html',
  styleUrls: ['./sign-in-form.component.scss']
})
export class SignInFormComponent implements OnInit {

  isEmailValid = true;
  isPasswordValid = true;
  signInForm!: FormGroup;
  isFormValid = true;

  email = '';
  password = '';
  showPassword: boolean = false;
  showForgotPassword = false;

  constructor(private fb: FormBuilder, private router: Router, private loginService: LoginService, private toast: ToastService) {}

  ngOnInit(): void {
    this.signInForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      remember: [false]
    });

    // Update submit button state on value changes
    this.signInForm.statusChanges.subscribe(status => {
      this.isFormValid = status !== 'VALID';
    });
  }

  validate(): void {
    if(this.signInForm.value.email != null){
      this.isEmailValid = this.signInForm.value.email.trim() !== '' && this.validateEmail(this.signInForm.value.email);
    }
    if(this.signInForm.value.password != null){
      this.isPasswordValid = this.signInForm.value.password.trim() !== '';
    }

    this.isFormValid = this.isEmailValid && this.isPasswordValid;
  }

  validateEmail(email: string): boolean {
    // Basic email pattern check
    const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return pattern.test(email);
  }

   onSubmit() {
    if (this.signInForm.valid) {
     this.loginService.Login(this.signInForm.value).subscribe((res: any)=>{
      if(res.isSuccess && res?.data?.token){
        localStorage.setItem('authToken', res?.data?.token);
        localStorage.setItem('refreshToken', res?.data?.refreshToken);
        this.router.navigateByUrl("/dashboard");
        this.toast.success(res.message, "");
      }
      else{
        this.toast.error('Login Failed', res.message);
      }
     })
    } else {
      this.signInForm.markAllAsTouched();
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  get isDisabled() {
    return this.signInForm.get('password')?.invalid || this.signInForm.get('email')?.invalid || !this.isEmailValid;
  }
  hovering = false;

  openForgotPassword() {
    this.showForgotPassword = true;
  }
}
