import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { Otpenum } from 'src/app/common/enum/otpenum';
import { AuthService } from 'src/app/services/auth.service';
import { OtpService } from 'src/app/services/otp.service';
import { ToastService } from 'src/app/services/toast.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  @ViewChild('nameInput') nameInput!: ElementRef<HTMLInputElement>;
  @ViewChild('newEidInputField') newEidInputElement!: ElementRef<HTMLInputElement>;

  isEditingName = false;
  user: any;
  currentLang = 'en';
  showPasswordDialog = false;
  showEmiratesId = false;
  showOtpDialog = false;
  selectedFile: any;
  isLoading = true;
  isEditingPhone: boolean = false;
  isEditingEmail: boolean = false;
  showOldEid = false;
  newEmiratesId: string = '';
  isEditingIdModalOpen: boolean = false;
  isPasswordVerified: boolean = false;
  pendingEidUpdate: string | null = null;

  constructor(private router: Router, private userService: UserService, private toast: ToastService, private otpService: OtpService, private authService: AuthService) {
    this.currentLang = localStorage.getItem('lang') || 'en';
  }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.userService.getProfile().subscribe({
      next: (res: any) => {
        setTimeout(() => {
          this.user = res.data;
          this.isLoading = false;
        }, 500);
      },
      error: (err: any) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  updateProfile(): Observable<boolean> {
    const formData = new FormData();
  
    formData.append('Id', this.user.id);
    formData.append('FirstName', this.user.firstName);
    formData.append('LastName', this.user.lastName);
    formData.append('UserEmail', this.user.userEmail);
    formData.append('UserPhone', this.user.userPhone);
    formData.append('EmiratesId', this.user.emiratesId);
  
    if (this.user.avatarImage) {
      formData.append('AvatarImage', this.selectedFile ?? this.user.avatarImage.replace(/^data:image\/(png|jpeg|jpg);base64,/, ''));
    }
  
    // Return the observable
    return this.userService.updateProfile(this.user).pipe(
      tap((res: any) => {
        if (res.isSuccess) {
          this.toast.success(res.message, "");
        } else {
          this.toast.error("Failed to update.", res.message);
        }
      }),
      map((res: any) => res.isSuccess),
      catchError(() => {
        this.toast.error("Failed to update.", "Server error");
        return of(false);
      })
    );
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    this.selectedFile = file;
    const reader = new FileReader();

    reader.onload = () => {
      this.user.avatarImage = reader.result as string;
      this.updateProfile().subscribe(success => {
        if (success) {
          console.log('Update successful');
        } else {
          console.log('Update failed');
        }
      });
    };

    reader.readAsDataURL(file);
  }

  toggleEditing(key: string) {
    if (key == 'Name') {
      this.isEditingName = true;

      // setTimeout(() => {
      //   this.nameInput?.nativeElement.focus();
      //   this.nameInput?.nativeElement.select();
      // }, 0);
    }
    else if (key == 'Phone') {
      this.isEditingPhone = true;
    }
    else if (key == 'Email') {
      this.isEditingEmail = true;
    }
  }

  stopEditing(key: string) {
    if (key == 'Name') {
      if (this.user.firstName != null && this.user.lastName != null) {
        this.isEditingName = false;
        this.updateProfile().subscribe(success => {
          if (success) {
            console.log('Update successful');
          } else {
            console.log('Update failed');
          }
        });
      }
    }
    else if (key == 'Phone') {
      if (this.user.userPhone &&
        /^[0-9]{9}$/.test(this.user.userPhone) && this.user.userPhone != null
      ) {
        this.isEditingPhone = false;
        this.updateProfile().subscribe(success => {
          if (success) {
            console.log('Update successful');
          } else {
            console.log('Update failed');
          }
        });
      }
    }
    else if (key == 'Email') {
      this.isEditingEmail = false;
      this.updateProfile().subscribe(success => {
        if (success) {
          console.log('Update successful');
        } else {
          console.log('Update failed');
        }
      });
    }
    else if (key == 'Eid') {
      if(!this.isPasswordVerified){
        this.pendingEidUpdate = this.newEmiratesId;
        this.openViewIdPopup();
      }
      else{
        this.updateEid(this.newEmiratesId);
      }
    }
  }

  updateEid(newEid: string) {
    const oldId = this.user.emiratesId;
    this.user.emiratesId = newEid;
    this.updateProfile().subscribe(isSuccess => {
      if (!isSuccess) {
        this.user.emiratesId = oldId;
      }
      else{
        this.isEditingIdModalOpen = false;
      }
    });
  }

  onVerifyEmail() {
    this.showOtpDialog = true;
    this.sendOtpToEmail();
  }

  get maskedEmiratesId(): string {
    const id = this.user?.emiratesId || '';
    return id.replace(/^(\d{3})-(\d{4})-(\d{7})-(\d{1})$/, (_: any, g1: any, g2: any, g3: any, g4: any) => {
      return `***-****-*****${g3.slice(-2)}-${g4}`;
    });
  }

  openViewIdPopup() {
    this.showPasswordDialog = true;
  }

  closePasswordDialog() {
    this.showPasswordDialog = false;
  }

  handlePasswordConfirm(password: string) {

    this.authService.confirmPassword(password).subscribe((res: any) => {
      if (res.isSuccess) {
        this.showEmiratesId = true;
        this.closePasswordDialog();
        this.toast.success("Success", res.message);
        this.isPasswordVerified = true;

        if (this.pendingEidUpdate) {
          this.updateEid(this.pendingEidUpdate);
          this.pendingEidUpdate = null; // clear pending update
        }
      } else {
        const dialogRef = document.querySelector('app-confirm-password-dialog') as any;
        dialogRef?.componentInstance?.setWrongPassword(true);
        this.toast.error('Password Verification Failed', res.message);
      }
    })
  }

  handlePasswordCancel(): void {
    this.showPasswordDialog = false;
  }

  handleOtpConfirm(enteredOtp: string) {
    if (enteredOtp != null) {
      this.otpService.validateOtp(enteredOtp, Otpenum.EMAIL_VERIFICATION).subscribe((res: any) => {
        if (res.isSuccess) {
          this.toast.success("Success", res.message);
          this.showOtpDialog = false;
          this.loadUserProfile();
        }
        else {
          this.toast.error('OTP Validation Failed', res.message);
        }
      })
    }
  }

  handleOtpCancel() {
    this.showOtpDialog = false;
  }

  sendOtpToEmail() {
    this.otpService.requestOtp(Otpenum.EMAIL_VERIFICATION).subscribe((res: any) => {
      if (res.isSuccess) {
      }
    });
  }

  openEditEmiratesIdModal() {
    this.isEditingIdModalOpen = true;
    this.newEmiratesId = '';
    this.showOldEid = false;
    this.focusNewEidInput();
  }

  focusNewEidInput() {
    setTimeout(() => {
      this.newEidInputElement?.nativeElement.focus();
    }, 0);
  }

  closeEditEmiratesIdModal() {
    this.isEditingIdModalOpen = false;
  }

  onEmiratesIdInput(value: string) {
    const formatted = this.formatEmiratesId(value);
    this.newEmiratesId = formatted;
  }

  formatEmiratesId(value: string): string {
    const digits = value.replace(/\D/g, '').substring(0, 15);

    const parts = [
      digits.substring(0, 3),
      digits.substring(3, 7),
      digits.substring(7, 14),
      digits.substring(14, 15)
    ];

    return parts.filter(part => part.length > 0).join('-');
  }

  onKeyPress(event: KeyboardEvent) {
    const allowedKeys = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab'];

    if (allowedKeys.indexOf(event.key) !== -1) {
      return;
    }

    if (!/^[0-9]$/.test(event.key)) {
      event.preventDefault();
    }
  }
}
