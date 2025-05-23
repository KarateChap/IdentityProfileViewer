import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BusyService } from '../services/busy.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  busyService.busy();

  return next(req).pipe(
    // environment.production ? identity : delay(0),
    environment.production ? identity : delay(300), // dummy delay to show loading as if it's in production environment
    finalize(() => {
      busyService.idle();
    })
  );
};
