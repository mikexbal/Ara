import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import {
  AuthResponse,
  AuthUser,
  LoginRequest,
  RegisterRequest,
  RegistrationPendingResponse,
  ResendVerificationRequest,
  VerifyEmailRequest
} from '../models/auth';

const TOKEN_KEY = 'tp_auth_token';
const USER_KEY = 'tp_auth_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  readonly currentUser = signal<AuthUser | null>(this.readStoredUser());
  readonly isAuthenticated = computed(() => this.currentUser() !== null);

  /** Creates the account and triggers a verification email. No session is started yet. */
  register(request: RegisterRequest): Observable<RegistrationPendingResponse> {
    return this.http.post<RegistrationPendingResponse>('/api/auth/register', request);
  }

  resendVerification(request: ResendVerificationRequest): Observable<RegistrationPendingResponse> {
    return this.http.post<RegistrationPendingResponse>('/api/auth/resend-verification', request);
  }

  /** Confirms the code and, on success, starts the session (matches login's behavior). */
  verifyEmail(request: VerifyEmailRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>('/api/auth/verify-email', request)
      .pipe(tap((response) => this.setSession(response)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>('/api/auth/login', request)
      .pipe(tap((response) => this.setSession(response)));
  }

  logout(): void {
    try {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
    } catch {
      // Storage unavailable (private browsing, etc.) — nothing to clean up.
    }
    this.currentUser.set(null);
  }

  getToken(): string | null {
    try {
      return localStorage.getItem(TOKEN_KEY);
    } catch {
      return null;
    }
  }

  private setSession(response: AuthResponse): void {
    try {
      localStorage.setItem(TOKEN_KEY, response.token);
      localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    } catch {
      // Storage unavailable — session just won't survive a reload.
    }
    this.currentUser.set(response.user);
  }

  private readStoredUser(): AuthUser | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? (JSON.parse(raw) as AuthUser) : null;
    } catch {
      return null;
    }
  }
}
