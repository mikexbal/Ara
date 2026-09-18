import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { UserMenu } from '../../shared/user-menu/user-menu';

type AccountTab = 'trips' | 'saved' | 'details' | 'security' | 'notifications';

const VALID_TABS: readonly AccountTab[] = ['trips', 'saved', 'details', 'security', 'notifications'];

@Component({
  selector: 'app-account',
  imports: [RouterLink, UserMenu],
  templateUrl: './account.html'
})
export class Account {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly user = this.authService.currentUser;

  protected readonly initials = computed(() => {
    const user = this.user();
    if (!user) {
      return '';
    }
    return `${user.firstName.trim()[0] ?? ''}${user.lastName.trim()[0] ?? ''}`.toUpperCase();
  });

  protected readonly memberSince = computed(() => {
    const createdAt = this.user()?.createdAt;
    return createdAt
      ? new Date(createdAt).toLocaleDateString('en-US', { month: 'long', year: 'numeric' })
      : '';
  });

  protected readonly selectedTab = signal<AccountTab>(this.readInitialTab());

  selectTab(tab: AccountTab): void {
    this.selectedTab.set(tab);
  }

  signOut(): void {
    this.authService.logout();
    this.router.navigateByUrl('/');
  }

  private readInitialTab(): AccountTab {
    const requested = this.route.snapshot.queryParamMap.get('tab');
    return (VALID_TABS as readonly string[]).includes(requested ?? '') ? (requested as AccountTab) : 'trips';
  }
}
