import { Routes } from '@angular/router';
import { UserIdentityComponent } from './components/user-identity/user-identity.component';

export const routes: Routes = [
  { path: '', component: UserIdentityComponent },
  { path: '**', redirectTo: '' }
];
