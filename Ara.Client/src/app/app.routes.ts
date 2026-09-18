import { Routes } from '@angular/router';

import { Home } from './features/home/home';
import { CreateAccount } from './features/auth/create-account/create-account';
import { SignIn } from './features/auth/sign-in/sign-in';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'sign-in', component: SignIn },
  { path: 'create-account', component: CreateAccount }
];
