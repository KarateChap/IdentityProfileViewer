import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
  inject,
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';
import {
  UserIdentity,
  UserIdentityUpdate,
} from '../../../shared/models/user-identity.model';
import { BusyService } from '../../../shared/services/busy.service';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgClass],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss',
})
export class UserEditComponent implements OnInit, OnChanges, OnDestroy {
  @Input() user: UserIdentity | null = null;
  @Input() loading = false;

  @Output() saveChanges = new EventEmitter<UserIdentityUpdate>();
  @Output() cancelEdit = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  public busyService: BusyService = inject(BusyService);

  editForm!: FormGroup;
  formSubmitted = false;

  ngOnInit(): void {
    this.initializeForm();
    // If user data is already available during initialization, populate the form
    if (this.user) {
      this.resetForm();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['user'] && this.user && this.editForm) {
      this.resetForm();
    }
  }

  ngOnDestroy(): void {
    // Cleanup if needed
  }

  initializeForm(): void {
    const defaultValues = this.user ? {
      fullName: this.user.fullName,
      email: this.user.email,
      isActive: this.user.isActive
    } : {
      fullName: '',
      email: '',
      isActive: true
    };
    
    this.editForm = this.fb.group({
      fullName: [defaultValues.fullName, [Validators.required, Validators.minLength(2)]],
      email: [defaultValues.email, [Validators.required, Validators.email]],
      isActive: [defaultValues.isActive],
    });
  }

  resetForm(): void {
    if (this.user) {
      this.editForm.patchValue({
        fullName: this.user.fullName,
        email: this.user.email,
        isActive: this.user.isActive,
      });
    }
  }

  submit(): void {
    this.formSubmitted = true;

    if (!this.user || this.editForm.invalid) return;

    const update: UserIdentityUpdate = {};
    const formValues = this.editForm.value;

    if (formValues.fullName !== this.user.fullName) {
      update.fullName = formValues.fullName;
    }
    if (formValues.email !== this.user.email) {
      update.email = formValues.email;
    }
    if (formValues.isActive !== this.user.isActive) {
      update.isActive = formValues.isActive;
    }

    if (Object.keys(update).length === 0) {
      this.cancelEdit.emit();
      return;
    }

    this.saveChanges.emit(update);
  }

  isEmptyForm() {
    return (
      this.editForm.get('fullName')?.value.trim() === '' ||
      this.editForm.get('email')?.value.trim() === ''
    );
  }

  cancel(): void {
    this.resetForm();
    this.formSubmitted = false;
    this.cancelEdit.emit();
  }
}
