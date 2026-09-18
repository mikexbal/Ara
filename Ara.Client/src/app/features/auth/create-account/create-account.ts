import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { ImageService } from '../../../core/services/image.service';
import { PASSWORD_REQUIREMENTS, isPasswordValid, passwordComplexityValidator } from '../../../core/validation/password-policy';

interface PasswordStrength {
  filledBars: 0 | 1 | 2 | 3;
  label: string;
}

function computePasswordStrength(password: string): PasswordStrength {
  if (!password) {
    return { filledBars: 0, label: '' };
  }
  if (!isPasswordValid(password)) {
    return { filledBars: 1, label: 'Weak' };
  }
  return password.length >= 16 ? { filledBars: 3, label: 'Strong' } : { filledBars: 2, label: 'Good' };
}

const RESEND_COOLDOWN_SECONDS = 8;

@Component({
  selector: 'app-create-account',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-account.html'
})
export class CreateAccount {
  protected readonly passwordRequirements = PASSWORD_REQUIREMENTS;

  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly imageService = inject(ImageService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly signUpImageUrl = signal<string | null>(null);
  protected readonly showPassword = signal(false);
  protected readonly submitted = signal(false);
  protected readonly submitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(25)]],
    lastName: ['', [Validators.required, Validators.maxLength(25)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, passwordComplexityValidator]],
    receiveGuides: [false]
  });

  private readonly passwordValue = toSignal(this.form.controls.password.valueChanges, {
    initialValue: this.form.controls.password.value
  });

  protected readonly passwordStrength = computed<PasswordStrength>(() =>
    computePasswordStrength(this.passwordValue())
  );

  // --- Email verification modal ---
  protected readonly showVerifyModal = signal(false);
  protected readonly pendingEmail = signal('');
  protected readonly verifyCode = signal('');
  protected readonly verifying = signal(false);
  protected readonly verifyError = signal<string | null>(null);
  protected readonly resending = signal(false);
  protected readonly resendNotice = signal<string | null>(null);
  protected readonly secondsRemaining = signal(0);
  protected readonly resendCooldown = signal(0);
  protected readonly codeExpired = computed(() => this.secondsRemaining() <= 0);
  protected readonly canResend = computed(() => this.resendCooldown() <= 0 && !this.resending());

  private countdownTimer?: ReturnType<typeof setInterval>;

  constructor() {
    this.destroyRef.onDestroy(() => this.stopCountdown());

    this.imageService.getSignUpImages().subscribe({
      next: (images) => this.signUpImageUrl.set(images[0] ?? null),
      // No image uploaded yet — the panel keeps its placeholder background.
      error: () => this.signUpImageUrl.set(null)
    });
  }

  togglePassword(): void {
    this.showPassword.update((value) => !value);
  }

  submit(): void {
    this.submitted.set(true);
    this.errorMessage.set(null);
    if (this.form.invalid || this.submitting()) {
      return;
    }

    const { firstName, lastName, email, password } = this.form.getRawValue();
    this.submitting.set(true);
    this.authService.register({ firstName, lastName, email, password }).subscribe({
      next: (pending) => {
        this.submitting.set(false);
        this.openVerifyModal(pending.email, pending.expiresInSeconds);
      },
      error: (error: unknown) => {
        this.submitting.set(false);
        this.errorMessage.set(this.extractErrorMessage(error));
      }
    });
  }

  onCodeInput(value: string): void {
    this.verifyCode.set(value.replace(/\D/g, '').slice(0, 6));
  }

  submitVerification(): void {
    if (this.verifying() || this.codeExpired() || this.verifyCode().length !== 6) {
      return;
    }

    this.verifying.set(true);
    this.verifyError.set(null);
    this.authService.verifyEmail({ email: this.pendingEmail(), code: this.verifyCode() }).subscribe({
      next: () => {
        this.verifying.set(false);
        this.stopCountdown();
        this.showVerifyModal.set(false);
        this.router.navigateByUrl('/');
      },
      error: (error: unknown) => {
        this.verifying.set(false);
        this.verifyError.set(this.extractErrorMessage(error));
      }
    });
  }

  resendCode(): void {
    if (!this.canResend()) {
      return;
    }

    this.resending.set(true);
    this.verifyError.set(null);
    this.resendNotice.set(null);
    this.authService.resendVerification({ email: this.pendingEmail() }).subscribe({
      next: (pending) => {
        this.resending.set(false);
        this.verifyCode.set('');
        this.resendNotice.set('A new code is on its way.');
        this.restartCountdown(pending.expiresInSeconds);
      },
      error: (error: unknown) => {
        this.resending.set(false);
        this.verifyError.set(this.extractErrorMessage(error));
      }
    });
  }

  private openVerifyModal(email: string, expiresInSeconds: number): void {
    this.pendingEmail.set(email);
    this.verifyCode.set('');
    this.verifyError.set(null);
    this.resendNotice.set(null);
    this.showVerifyModal.set(true);
    this.restartCountdown(expiresInSeconds);
  }

  private restartCountdown(expiresInSeconds: number): void {
    this.stopCountdown();
    this.secondsRemaining.set(expiresInSeconds);
    this.resendCooldown.set(RESEND_COOLDOWN_SECONDS);

    this.countdownTimer = setInterval(() => {
      this.secondsRemaining.update((value) => Math.max(0, value - 1));
      this.resendCooldown.update((value) => Math.max(0, value - 1));
      if (this.secondsRemaining() === 0) {
        this.stopCountdown();
      }
    }, 1000);
  }

  private stopCountdown(): void {
    if (this.countdownTimer) {
      clearInterval(this.countdownTimer);
      this.countdownTimer = undefined;
    }
  }

  private extractErrorMessage(error: unknown): string {
    const httpError = error as { error?: { message?: string; errors?: Record<string, string[]> } };
    if (httpError?.error?.message) {
      return httpError.error.message;
    }
    const firstFieldError = Object.values(httpError?.error?.errors ?? {})[0]?.[0];
    return firstFieldError ?? 'Something went wrong. Please try again.';
  }
}
