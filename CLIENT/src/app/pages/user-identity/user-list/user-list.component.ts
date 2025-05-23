import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  Output,
} from '@angular/core';
import { NgIf } from '@angular/common';
import { UserIdentity } from '../../../shared/models/user-identity.model';
import { Pagination } from '../../../shared/models/pagination';
import { BusyService } from '../../../shared/services/busy.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [NgIf],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent {
  @Input() userIdentities: UserIdentity[] = [];
  @Input() selectedUserId: number | null = null;
  @Input() loading = false; // Keeping for backward compatibility
  @Input() pagination: Pagination | null = null;
  @Input() loadingMore = false;

  @Output() userSelected = new EventEmitter<UserIdentity>();
  @Output() loadMore = new EventEmitter<void>();

  constructor(public busyService: BusyService) {}

  selectUser(user: UserIdentity): void {
    this.userSelected.emit(user);
  }

  trackByUser(user: UserIdentity, index?: number): string {
    const pageNum = this.pagination?.currentPage || 1;
    const indexInPage =
      index !== undefined ? index : this.userIdentities.indexOf(user);

    return `page${pageNum}-index${indexInPage}-id${user.id}`;
  }
}
