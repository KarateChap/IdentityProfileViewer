import { Component, OnInit } from '@angular/core';
import {
  UserIdentity,
  UserIdentityUpdate,
} from '../../models/user-identity.model';
import { UserIdentityService } from '../../services/user-identity.service';
import { NgIf } from '@angular/common';
import { UserListComponent } from '../user-list/user-list.component';
import { UserDetailsComponent } from '../user-details/user-details.component';
import { UserEditComponent } from '../user-edit/user-edit.component';
import { NotificationComponent } from '../notification/notification.component';

@Component({
  selector: 'app-user-identity',
  standalone: true,
  imports: [
    NgIf,
    UserListComponent,
    UserDetailsComponent,
    UserEditComponent,
    NotificationComponent,
  ],
  templateUrl: './user-identity.component.html',
  styleUrl: './user-identity.component.scss',
})
export class UserIdentityComponent implements OnInit {
  userIdentities: UserIdentity[] = [];
  selectedUser: UserIdentity | null = null;
  editMode = false;
  loading = false;
  message = '';
  messageType: 'success' | 'error' | '' = '';

  constructor(private userIdentityService: UserIdentityService) {}

  ngOnInit(): void {
    this.loadUserIdentities();
  }

  loadUserIdentities(): void {
    this.loading = true;
    this.userIdentityService.getAllUserIdentities().subscribe({
      next: (users) => {
        this.userIdentities = users;
        this.loading = false;
        if (users.length > 0 && !this.selectedUser) {
          this.onUserSelected(users[0]);
        }
      },
      error: (error) => {
        this.showMessage('Failed to load user identities: ' + '', 'error');
        this.loading = false;
      },
    });
  }

  onUserSelected(user: UserIdentity): void {
    this.selectedUser = user;
    this.editMode = false;
    this.clearMessage();
  }

  onEditRequested(): void {
    this.editMode = true;
    this.clearMessage();
  }

  onCancelEdit(): void {
    this.editMode = false;
    this.clearMessage();
  }

  onSaveChanges(update: UserIdentityUpdate): void {
    if (!this.selectedUser) return;

    if (Object.keys(update).length === 0) {
      this.showMessage('No changes detected', 'error');
      return;
    }

    this.loading = true;
    this.userIdentityService
      .updateUserIdentity(this.selectedUser.id, update)
      .subscribe({
        next: (updatedUser) => {
          this.selectedUser = updatedUser;

          // Update the user in the list
          const index = this.userIdentities.findIndex(
            (u) => u.id === updatedUser.id
          );
          if (index !== -1) {
            this.userIdentities[index] = updatedUser;
          }

          this.editMode = false;
          this.loading = false;
          this.showMessage(
            Object.keys(update).length === 0
              ? 'No changes detected'
              : 'User identity updated successfully',
            Object.keys(update).length === 0 ? 'error' : 'success'
          );
        },
        error: (error) => {
          this.showMessage('Failed to update user identity: ' + '', 'error');
          this.loading = false;
        },
      });
  }

  private showMessage(message: string, type: 'success' | 'error'): void {
    this.message = message;
    this.messageType = type;
  }

  private clearMessage(): void {
    this.message = '';
    this.messageType = '';
  }
}
