import {
  Component,
  EventEmitter,
  Output,
  ViewChildren,
  QueryList,
  ElementRef,
  AfterViewInit,
  OnInit,
  OnDestroy
} from '@angular/core';

@Component({
  selector: 'app-email-verification-dialog',
  templateUrl: './email-verification-dialog.component.html',
  styleUrls: ['./email-verification-dialog.component.scss']
})
export class EmailVerificationDialogComponent implements AfterViewInit, OnInit, OnDestroy {
  @Output() confirm = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();
  @Output() resend = new EventEmitter<void>();
  @Output() reset = new EventEmitter<void>();

  @ViewChildren('otpInput') otpInputs!: QueryList<ElementRef<HTMLInputElement>>;

  otp: string[] = ['', '', '', '', '', ''];
  timeLeft = 60;
  canResend = false;
  intervalId: any;

  maxEnabledIndex = 0; // Controls which inputs are enabled

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.focusInput(0);
    }, 100);
  }

  ngOnInit() {
    this.startTimer();
  }

  onInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    let val = input.value;

    // Only digits allowed
    val = val.replace(/\D/g, '');

    if (val.length > 1) {
      val = val.charAt(0); // Take only first digit if multiple pasted
    }

    this.otp[index] = val;

    if (val) {
      // Move to next input if current filled
      if (index < this.otp.length - 1) {
        this.maxEnabledIndex = Math.max(this.maxEnabledIndex, index + 1);
        this.focusInput(index + 1);
      }
    } else {
      // If current input cleared, reset maxEnabledIndex accordingly
      if (this.maxEnabledIndex > index) {
        this.maxEnabledIndex = index;
      }
    }

    this.updateInputsState();

    // If all inputs filled, submit
    if (this.otp.every(d => d !== '')) {
      this.onAutoSubmit();
    }
  }

  onBackspace(index: number, event: KeyboardEvent) {
    if (event.key === 'Backspace') {
      this.otp[index] = '';

      this.maxEnabledIndex = index; // Disable inputs after this
      this.updateInputsState();

      if (index > 0) {
        this.focusInput(index - 1);
      }
    }
  }

  private updateInputsState() {
    this.otpInputs.forEach((inputRef, i) => {
      const el = inputRef.nativeElement;
      el.disabled = i > this.maxEnabledIndex;
      if (el.disabled) {
        this.otp[i] = '';
      }
    });
  }

  private focusInput(index: number) {
    setTimeout(() => {
      this.otpInputs.toArray()[index]?.nativeElement.focus();
      this.otpInputs.toArray()[index]?.nativeElement.select();
    }, 0);
  }

  private onAutoSubmit() {
    const code = this.otp.join('');
    if (code.length === 6) {
      this.confirm.emit(code);
    }
  }

  onClose() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
    this.cancel.emit();
  }

  onResend() {
    this.resend.emit();
    this.clearOtp();
    this.startTimer();
  }

  onReset() {
    this.clearOtp();
    this.reset.emit();
  }

  private clearOtp() {
    this.otp = ['', '', '', '', '', ''];
    this.maxEnabledIndex = 0;

    this.updateInputsState();

    setTimeout(() => {
      this.focusInput(0);
    }, 100);
  }

  startTimer() {
    this.canResend = false;
    this.timeLeft = 60;

    if (this.intervalId) {
      clearInterval(this.intervalId);
    }

    this.intervalId = setInterval(() => {
      this.timeLeft--;
      if (this.timeLeft <= 0) {
        this.canResend = true;
        clearInterval(this.intervalId);
      }
    }, 1000);
  }

  ngOnDestroy() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }
}
