import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs';
import { AccountService } from '../services/account.service';
import { AlertService } from '../services/alert.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const accountService = inject(AccountService);
  const alertService = inject(AlertService);

  return next(req).pipe(
    catchError((error) => {
      if (error) {
        const errorStatus = error.status;
        const errorMessage = error.error;

        switch (errorStatus) {
          case 400:
            const validationErrors = errorMessage.errors;

            if (validationErrors) {
              const modalStateErrors = [];
              for (const key in validationErrors) {
                if (validationErrors[key]) {
                  modalStateErrors.push(validationErrors[key]);
                }
              }
              throw modalStateErrors.flat();
            } else {
              console.log(errorMessage);
            }
            break;
          case 401:
            errorMessage.length > 0
              ? alertService.error(errorMessage)
              : alertService.error('You are not authorized');

            accountService.logout();
            router.navigateByUrl('/');

            alertService.error(errorMessage);
            break;
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = {
              state: { error: errorMessage },
            };
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            console.log('Something unexpected went wrong');
            break;
        }
      }
      throw error;
    })
  );
};
