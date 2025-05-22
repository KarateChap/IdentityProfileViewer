import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { UserIdentity } from '../../models/user-identity.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [NgIf, NgFor],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent {
  @Input() userIdentities: UserIdentity[] = [];
  @Input() selectedUserId: number | null = null;
  @Input() loading = false;

  @Output() userSelected = new EventEmitter<UserIdentity>();

  selectUser(user: UserIdentity): void {
    this.userSelected.emit(user);
  }
}
