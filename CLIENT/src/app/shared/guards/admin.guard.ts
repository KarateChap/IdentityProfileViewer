import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../services/account.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);

  if (accountService.roles().includes('Super Admin')) {
    return true;
  } else {
    console.log('You are not authorized');
    return false;
  }
};
