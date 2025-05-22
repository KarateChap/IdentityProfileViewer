import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import {
  UserIdentity,
  UserIdentityUpdate,
} from '../models/user-identity.model';

@Injectable({
  providedIn: 'root',
})
export class UserIdentityService {
  private apiUrl = 'https://localhost:5001/api/useridentities'; // I can create an environment variables to store this securely but for the sake of simplicity, I add it here as it is.

  constructor(private http: HttpClient) {}

  getUserIdentity(id: number): Observable<UserIdentity> {
    return this.http
      .get<UserIdentity>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  getAllUserIdentities(): Observable<UserIdentity[]> {
    return this.http
      .get<UserIdentity[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  updateUserIdentity(
    id: number,
    update: UserIdentityUpdate
  ): Observable<UserIdentity> {
    return this.http
      .patch<UserIdentity>(`${this.apiUrl}/${id}`, update)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(() => errorMessage);
  }
}
