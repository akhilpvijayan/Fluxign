import { AfterViewChecked, Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';

@Component({
  selector: 'app-password-confirmation-dialog',
  templateUrl: './password-confirmation-dialog.component.html',
  styleUrls: ['./password-confirmation-dialog.component.scss']
})
export class PasswordConfirmationDialogComponent implements OnInit, AfterViewChecked  {
  @ViewChild('passwordInput') passwordInput!: ElementRef<HTMLInputElement>;
  password: string = '';
  wrongPassword: boolean = false;
  private focusSet = false;
  @Output() confirm = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();

  ngOnInit(): void {
  }

  ngAfterViewChecked() {
    if (!this.focusSet && this.passwordInput) {
      this.passwordInput.nativeElement.focus();
      this.focusSet = true; // prevent focusing repeatedly
    }
  }

  onConfirm() {
    if (!this.password) return;
    this.confirm.emit(this.password);
  }

  onCancel() {
    this.cancel.emit();
  }

  setWrongPassword(state: boolean) {
    this.wrongPassword = state;
  }

  reset() {
    this.password = '';
    this.wrongPassword = false;
  }
}