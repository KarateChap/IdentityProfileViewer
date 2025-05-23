import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DatePipe, NgIf } from '@angular/common';
import { UserIdentity } from '../../../shared/models/user-identity.model';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [NgIf, DatePipe],
  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.scss',
})
export class UserDetailsComponent {
  @Input() user: UserIdentity | null = null;

  @Output() editRequested = new EventEmitter<void>();

  requestEdit(): void {
    this.editRequested.emit();
  }
}
