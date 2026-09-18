import { Component, ElementRef, HostListener, Input, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-user-menu',
  imports: [RouterLink],
  templateUrl: './user-menu.html'
})
export class UserMenu {
  /** 'dark' for a transparent header over a photo (white text); 'light' for a plain white header. */
  @Input() variant: 'light' | 'dark' = 'dark';

  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly elementRef = inject(ElementRef<HTMLElement>);

  protected readonly isAuthenticated = this.authService.isAuthenticated;
  protected readonly currentUser = this.authService.currentUser;
  protected readonly open = signal(false);

  protected readonly initials = computed(() => {
    const user = this.currentUser();
    if (!user) {
      return '';
    }
    const initials = `${user.firstName.trim()[0] ?? ''}${user.lastName.trim()[0] ?? ''}`.toUpperCase();
    return initials || user.email[0]?.toUpperCase() || '?';
  });

  toggle(): void {
    this.open.update((value) => !value);
  }

  close(): void {
    this.open.set(false);
  }

  signOut(): void {
    this.close();
    this.authService.logout();
    this.router.navigateByUrl('/');
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.elementRef.nativeElement.contains(event.target as Node)) {
      this.close();
    }
  }
}
