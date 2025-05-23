import { Routes } from '@angular/router';
import { AuthComponent } from './pages/auth/auth.component';
import { authGuard } from './shared/guards/auth.guard';
import { TestErrorsComponent } from './pages/test-errors/test-errors.component';
import { UserIdentityComponent } from './pages/user-identity/user-identity.component';
import { NotFoundComponent } from './pages/errors/not-found/not-found.component';
import { ServerErrorComponent } from './pages/errors/server-error/server-error.component';

export const routes: Routes = [
  { path: '', component: AuthComponent },
  { path: 'auth', component: AuthComponent },
  {
    path: 'identities',
    canActivate: [authGuard],
    component: UserIdentityComponent,
  },
  { path: 'test-errors', component: TestErrorsComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: AuthComponent, pathMatch: 'full' },
];
