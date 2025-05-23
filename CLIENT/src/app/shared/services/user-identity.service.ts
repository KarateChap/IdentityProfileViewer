import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import {
  UserIdentity,
  UserIdentityUpdate,
} from '../models/user-identity.model';
import { environment } from '../../../environments/environment';
import { UserIdentityParams } from '../models/user-identity-params';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class UserIdentityService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  private userIdentityCache = new Map<string, HttpResponse<UserIdentity[]>>();
  private userIdentityParams = signal(new UserIdentityParams());
  paginatedResult = signal<PaginatedResult<UserIdentity[]> | null>(null);
  loadingMore = signal(false);

  constructor() {}

  getUserIdentity(id: number): Observable<UserIdentity> {
    return this.http.get<UserIdentity>(`${this.baseUrl}identities/${id}`);
  }

  getAllUserIdentities(): Observable<UserIdentity[]> {
    return this.http.get<UserIdentity[]>(`${this.baseUrl}identities`);
  }

  getAllUserIdentitiesPaginated(
    loadMore = false
  ): Observable<PaginatedResult<UserIdentity[]>> {
    const cacheKey = Object.values(this.userIdentityParams()).join('-');
    const cachedResponse = this.userIdentityCache.get(cacheKey);

    if (cachedResponse && !loadMore) {
      const paginationHeader = cachedResponse.headers.get('Pagination');
      return of({
        items: cachedResponse.body!,
        pagination: paginationHeader ? JSON.parse(paginationHeader) : null,
      });
    }

    if (loadMore) {
      this.loadingMore.set(true);
      const currentParams = this.userIdentityParams();
      currentParams.pageNumber += 1;
      this.userIdentityParams.set(currentParams);
    }

    let params = this.getParams();

    return this.http
      .get<UserIdentity[]>(`${this.baseUrl}identities`, {
        observe: 'response',
        params,
      })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('Error with paginated request:', error);

          if (
            error.status === 500 &&
            (params.has('pageNumber') || params.has('pageSize'))
          ) {
            console.log('Falling back to non-paginated request');
            let fallbackParams = new HttpParams();
            if (params.has('searchString')) {
              fallbackParams = fallbackParams.append(
                'searchString',
                params.get('searchString')!
              );
            }

            return this.http
              .get<UserIdentity[]>(`${this.baseUrl}identities`, {
                observe: 'response',
                params: fallbackParams,
              })
              .pipe(
                map((fallbackResponse) => {
                  const items = fallbackResponse.body || [];
                  const paginatedResult: PaginatedResult<UserIdentity[]> = {
                    items: items,
                    pagination: {
                      currentPage: 1,
                      itemsPerPage: items.length,
                      totalItems: items.length,
                      totalPages: 1,
                    },
                  };

                  this.paginatedResult.set(paginatedResult);

                  if (loadMore) {
                    this.loadingMore.set(false);
                  }

                  return paginatedResult;
                })
              );
          }

          return throwError(() => error);
        }),
        map(
          (
            response:
              | HttpResponse<UserIdentity[]>
              | PaginatedResult<UserIdentity[]>
          ) => {
            if ('headers' in response) {
              const httpResponse = response as HttpResponse<UserIdentity[]>;
              const paginationHeader = httpResponse.headers.get('Pagination');
              const paginatedResult = this.paginatedResult();

              if (paginatedResult && paginatedResult.items && loadMore) {
                this.paginatedResult.set({
                  items: [...paginatedResult.items, ...httpResponse.body!],
                  pagination: paginationHeader
                    ? JSON.parse(paginationHeader)
                    : paginatedResult.pagination,
                });
              } else {
                this.paginatedResult.set({
                  items: httpResponse.body!,
                  pagination: paginationHeader
                    ? JSON.parse(paginationHeader)
                    : null,
                });
              }

              this.userIdentityCache.set(
                Object.values(this.userIdentityParams()).join('-'),
                httpResponse
              );
            } else {
            }

            if (loadMore) {
              this.loadingMore.set(false);
            }

            return this.paginatedResult()!;
          }
        )
      );
  }

  setUserIdentityParams(params: Partial<UserIdentityParams>): void {
    const currentParams = this.userIdentityParams();
    this.userIdentityParams.set({ ...currentParams, ...params });
  }

  resetUserIdentityParams(): void {
    this.userIdentityParams.set(new UserIdentityParams());
    this.paginatedResult.set(null);
  }

  getUserIdentityParams(): UserIdentityParams {
    return this.userIdentityParams();
  }

  private getParams(): HttpParams {
    const params = this.userIdentityParams();
    let httpParams = new HttpParams();

    // Only add pagination if needed - making these optional
    if (params.pageNumber && params.pageSize) {
      httpParams = httpParams.append(
        'pageNumber',
        params.pageNumber.toString()
      );
      httpParams = httpParams.append('pageSize', params.pageSize.toString());
    }

    if (params.searchString) {
      httpParams = httpParams.append('searchString', params.searchString);
    }

    return httpParams;
  }

  updateUserIdentity(
    id: number,
    update: UserIdentityUpdate
  ): Observable<UserIdentity> {
    return this.http.patch<UserIdentity>(
      `${this.baseUrl}identities/${id}`,
      update
    );
  }
}
