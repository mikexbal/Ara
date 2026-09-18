import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Mirrors Ara.Application.Common.Validation.PasswordPolicy on the backend —
// keep both in sync if this ever changes.
export const PASSWORD_REQUIREMENTS =
  'At least 12 characters, with a number, an uppercase letter, and a special character.';

const PASSWORD_PATTERN = /^(?=.*[0-9])(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{12,}$/;

export function isPasswordValid(password: string): boolean {
  return PASSWORD_PATTERN.test(password);
}

export const passwordComplexityValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value as string;
  return !value || isPasswordValid(value) ? null : { passwordComplexity: true };
};
