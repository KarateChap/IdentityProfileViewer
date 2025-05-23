import { Component, inject, OnDestroy, OnInit, computed } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { AccountService } from '../../shared/services/account.service';
import {
  UserIdentity,
  UserIdentityUpdate,
} from '../../shared/models/user-identity.model';
import { UserListComponent } from './user-list/user-list.component';
import { UserDetailsComponent } from './user-details/user-details.component';
import { UserEditComponent } from './user-edit/user-edit.component';
import { TestErrorsComponent } from '../test-errors/test-errors.component';
import { UserIdentityService } from '../../shared/services/user-identity.service';
import { BusyService } from '../../shared/services/busy.service';
import { AlertService } from '../../shared/services/alert.service';
import { PaginatedResult, Pagination } from '../../shared/models/pagination';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-identity',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    UserListComponent,
    UserDetailsComponent,
    UserEditComponent,
    TestErrorsComponent,
  ],
  templateUrl: './user-identity.component.html',
  styleUrl: './user-identity.component.scss',
})
export class UserIdentityComponent implements OnInit, OnDestroy {
  userIdentities: UserIdentity[] = [];
  selectedUser: UserIdentity | null = null;
  editMode = false;
  loadingMore = false;
  pagination: Pagination | null = null;
  private subscriptions = new Subscription();
  busyService = inject(BusyService);
  userIdentityService = inject(UserIdentityService);
  alertService = inject(AlertService);
  accountService = inject(AccountService);

  isSuperAdmin = computed(() => {
    return this.accountService.roles().includes('Super Admin');
  });

  constructor() {}

  ngOnInit(): void {
    this.loadUserIdentities();
  }

  loadUserIdentities(): void {
    this.busyService.busy();

    this.userIdentityService.setUserIdentityParams({
      pageSize: 5,
      pageNumber: 1,
    });

    this.userIdentityService.getAllUserIdentitiesPaginated().subscribe({
      next: (response: PaginatedResult<UserIdentity[]>) => {
        this.userIdentities = response.items || [];
        this.pagination = response.pagination ?? null;

        this.busyService.idle();
        if (this.userIdentities.length > 0 && !this.selectedUser) {
          this.onUserSelected(this.userIdentities[0]);
        }
      },
      error: (error) => {
        console.error('Error loading user identities:', error);
        this.alertService.error(
          'Failed to load user identities. Please try again or contact support if the issue persists.'
        );
        this.busyService.idle();
        this.userIdentities = [];
        this.pagination = null;
      },
    });
  }

  onUserSelected(user: UserIdentity): void {
    this.selectedUser = user;
    this.editMode = false;
    this.alertService.clear();
  }

  onEditRequested(): void {
    this.editMode = true;
    this.alertService.clear();
  }

  onCancelEdit(): void {
    this.editMode = false;
    this.alertService.clear();
  }

  onSaveChanges(update: UserIdentityUpdate): void {
    if (!this.selectedUser) return;

    if (Object.keys(update).length === 0) {
      this.alertService.error('No changes detected');
      return;
    }

    this.busyService.busy();
    this.userIdentityService
      .updateUserIdentity(this.selectedUser.id, update)
      .subscribe({
        next: (updatedUser: UserIdentity) => {
          this.selectedUser = updatedUser;

          const index = this.userIdentities.findIndex(
            (u) => u.id === updatedUser.id
          );
          if (index !== -1) {
            this.userIdentities[index] = updatedUser;
          }

          this.editMode = false;
          this.busyService.idle();
          this.alertService.success('User identity updated successfully');
        },
        error: (error) => {
          this.alertService.error('Failed to update user identity: ' + error);
          this.busyService.idle();
        },
      });
  }

  // Navigate to a specific page
  onPageChange(pageNumber: number): void {
    if (
      this.pagination &&
      pageNumber >= 1 &&
      pageNumber <= this.pagination.totalPages
    ) {
      this.busyService.busy();
      this.userIdentityService.setUserIdentityParams({
        pageNumber: pageNumber,
      });

      this.userIdentityService.getAllUserIdentitiesPaginated().subscribe({
        next: (response: PaginatedResult<UserIdentity[]>) => {
          this.userIdentities = response.items || [];
          this.pagination = response.pagination ?? null;
          this.busyService.idle();
        },
        error: (error) => {
          console.error('Error loading page:', error);
          this.alertService.error('Failed to load page. Please try again.');
          this.busyService.idle();
        },
      });
    }
  }

  // For load more functionality (if needed in future)
  onLoadMore(): void {
    if (
      this.pagination &&
      this.pagination.currentPage < this.pagination.totalPages
    ) {
      this.loadingMore = true;
      this.userIdentityService.getAllUserIdentitiesPaginated(true).subscribe({
        next: (response: PaginatedResult<UserIdentity[]>) => {
          this.userIdentities = response.items || [];
          this.pagination = response.pagination ?? null;
          this.loadingMore = false;
        },
        error: (error) => {
          console.error('Error loading more users:', error);
          this.loadingMore = false;
          this.alertService.error(
            'Failed to load more users. Please try again.'
          );
        },
      });
    }
  }

  searchUsers(searchTerm: string): void {
    this.userIdentityService.resetUserIdentityParams();
    if (searchTerm) {
      this.userIdentityService.setUserIdentityParams({
        searchString: searchTerm,
      });
    }
    this.loadUserIdentities();
  }

  /**
   * Returns an array of page numbers for pagination
   */
  getPageNumbers(): number[] {
    if (!this.pagination) return [];
    return Array.from({ length: this.pagination.totalPages }, (_, i) => i);
  }

  /**
   * Track by function for ngFor optimization
   */
  trackByIndex(index: number): number {
    return index;
  }

  /**
   * Checks if the current user is a Super Admin
   */
  checkSuperAdminRole(): boolean {
    return this.isSuperAdmin();
  }

  ngOnDestroy(): void {
    // Clean up any subscriptions
    this.subscriptions.unsubscribe();
    this.alertService.clear();
  }
}
