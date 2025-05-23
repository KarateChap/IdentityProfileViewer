import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = 0;
  isLoading = signal(false);

  busy() {
    this.busyRequestCount++;

    this.isLoading.set(true);
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.isLoading.set(false);
    }
  }
}
