import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../../shared/services/account.service';
import { Router } from '@angular/router';
import { BusyService } from '../../shared/services/busy.service';
import { User } from '../../shared/models/user.model';
import { AlertService } from '../../shared/services/alert.service';
import { NgClass } from '@angular/common';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-auth',
  imports: [ReactiveFormsModule, NgClass],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class AuthComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private alertService = inject(AlertService);
  busyService = inject(BusyService);
  isRegister = false;
  formSubmitted = false;
  authForm: FormGroup = new FormGroup({});
  private passwordSubscription?: Subscription;

  ngOnInit(): void {
    this.initializeForm();

    if (this.accountService.currentUser()) {
      this.router.navigateByUrl('identities');
    }
  }

  ngOnDestroy(): void {
    this.cleanupSubscription();
  }

  private cleanupSubscription(): void {
    if (this.passwordSubscription) {
      this.passwordSubscription.unsubscribe();
      this.passwordSubscription = undefined;
    }
  }

  initializeForm() {
    this.authForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  toggleForm() {
    this.isRegister = !this.isRegister;
    this.authForm.reset();

    this.cleanupSubscription();

    if (this.isRegister) {
      this.authForm
        .get('password')
        ?.setValidators([
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(20),
          this.hasDigit(),
          this.hasUppercase(),
        ]);

      this.authForm.addControl(
        'confirmPassword',
        this.fb.control('', [Validators.required, this.matchValues('password')])
      );

      this.passwordSubscription = this.authForm
        .get('password')
        ?.valueChanges.subscribe(() => {
          const confirmPasswordControl = this.authForm.get('confirmPassword');
          if (confirmPasswordControl) {
            confirmPasswordControl.updateValueAndValidity();
          }
        });
    } else {
      this.authForm.get('password')?.setValidators([Validators.required]);
      this.authForm.removeControl('confirmPassword');
    }

    this.authForm.get('password')?.updateValueAndValidity();
  }

  loginOrRegister() {
    this.formSubmitted = true;

    if (this.authForm.invalid) {
      return;
    }

    const userAuth: User = {
      email: this.authForm.value.email,
      password: this.authForm.value.password,
    };

    if (this.isRegister) {
      this.accountService.register(userAuth).subscribe({
        next: (_) => {
          this.alertService.success(
            'Sign Up Successful! Please login with your account'
          );
          this.authForm.reset();
          this.toggleForm();
        },
      });
    } else {
      this.accountService.login(userAuth).subscribe({
        next: (_) => {
          this.router.navigateByUrl('identities');
          this.alertService.success('Sign In successful!');
        },
      });
    }
  }

  // Custom Validators

  private matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true };
    };
  }

  private hasDigit(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const password = control.value;
      const hasDigit = /\d/.test(password);

      return hasDigit ? null : { hasDigit: true };
    };
  }

  private hasUppercase(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const password = control.value;
      const hasUppercase = /[A-Z]/.test(password);

      return hasUppercase ? null : { hasUppercase: true };
    };
  }
}
