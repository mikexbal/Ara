import { Routes } from '@angular/router';

import { Home } from './features/home/home';
import { CreateAccount } from './features/auth/create-account/create-account';
import { SignIn } from './features/auth/sign-in/sign-in';
import { Account } from './features/account/account';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'sign-in', component: SignIn },
  { path: 'create-account', component: CreateAccount },
  { path: 'account', component: Account, canActivate: [authGuard] }
];
