import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import {
  UserIdentity,
  UserIdentityUpdate,
} from '../../models/user-identity.model';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss',
})
export class UserEditComponent implements OnChanges {
  @Input() user: UserIdentity | null = null;
  @Input() loading = false;

  @Output() saveChanges = new EventEmitter<UserIdentityUpdate>();
  @Output() cancelEdit = new EventEmitter<void>();

  editForm = {
    fullName: '',
    email: '',
    isActive: true,
  };

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['user'] && this.user) {
      this.resetForm();
    }
  }

  resetForm(): void {
    if (this.user) {
      this.editForm = {
        fullName: this.user.fullName,
        email: this.user.email,
        isActive: this.user.isActive,
      };
    }
  }

  submit(): void {
    if (!this.user) return;

    const update: UserIdentityUpdate = {};

    if (this.editForm.fullName !== this.user.fullName) {
      update.fullName = this.editForm.fullName;
    }
    if (this.editForm.email !== this.user.email) {
      update.email = this.editForm.email;
    }
    if (this.editForm.isActive !== this.user.isActive) {
      update.isActive = this.editForm.isActive;
    }

    if (Object.keys(update).length === 0) {
      // No changes detected
      this.cancelEdit.emit();
      return;
    }

    this.saveChanges.emit(update);
  }

  isEmptyForm() {
    return (
      this.editForm.fullName.trim() === '' || this.editForm.email.trim() === ''
    );
  }

  cancel(): void {
    this.resetForm();
    this.cancelEdit.emit();
  }
}
