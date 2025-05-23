import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { map } from 'rxjs';
import { UserAuth } from '../models/user-auth.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  currentUser = signal<UserAuth | null>(null);
  roles = computed(() => {
    const user = this.currentUser();
    if (user && user.token) {
      const role = JSON.parse(atob(user.token.split('.')[1])).role;
      return Array.isArray(role) ? role : [role];
    }
    return [];
  });
  refreshTokenTimeout: ReturnType<typeof setTimeout> | null = null;

  login(userAuth: User) {
    return this.http
      .post<UserAuth>(this.baseUrl + 'account/login', userAuth)
      .pipe(
        map((user) => {
          if (user) {
            this.setCurrentUser(user);
            this.startRefreshTokenTimer(user);
          }
          return user;
        })
      );
  }

  register(userAuth: User) {
    return this.http.post<UserAuth>(
      this.baseUrl + 'account/register',
      userAuth
    );
  }

  setCurrentUser(user: UserAuth) {
    this.currentUser.set(user);
  }

  logout() {
    this.currentUser.set(null);
  }

  refreshToken() {
    this.stopRefreshTokenTimer();
    this.http
      .post<UserAuth>(this.baseUrl + 'account/refreshToken', {})
      .subscribe({
        next: (user) => {
          if (user) {
            this.setCurrentUser(user);
            console.log('Tokens has been refreshed');
            this.startRefreshTokenTimer(user);
          }
        },
        error: (err) => console.log(err),
      });
  }

  startRefreshTokenTimer(user: UserAuth) {
    const jwtToken = JSON.parse(atob(user.token.split('.')[1]));
    const expires = new Date(jwtToken.exp * 1000);
    const timeout = expires.getTime() - Date.now() - 30 * 1000;
    this.refreshTokenTimeout = setTimeout(() => this.refreshToken(), timeout);
    // console.log({ refreshTimeout: this.refreshTokenTimeout });
  }

  stopRefreshTokenTimer() {
    if (this.refreshTokenTimeout !== null) {
      clearTimeout(this.refreshTokenTimeout);
    }
  }
}
