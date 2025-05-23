import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface Alert {
  type: 'info' | 'success' | 'warning' | 'error';
  message: string;
  autoClose?: boolean;
  keepAfterRouteChange?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private subject = new Subject<Alert | null>();

  // Enable subscribing to alerts observable
  onAlert(): Observable<Alert | null> {
    return this.subject.asObservable();
  }

  // Clear alerts
  clear() {
    this.subject.next(null);
  }

  // Show success alert
  success(message: string, options?: Partial<Alert>) {
    this.alert({ ...options, type: 'success', message, autoClose: true });
  }

  // Show error alert
  error(message: string, options?: Partial<Alert>) {
    this.alert({ ...options, type: 'error', message, autoClose: true });
  }

  // Show info alert
  info(message: string, options?: Partial<Alert>) {
    this.alert({ ...options, type: 'info', message, autoClose: true });
  }

  // Show warning alert
  warning(message: string, options?: Partial<Alert>) {
    this.alert({ ...options, type: 'warning', message, autoClose: true });
  }

  // Main alert method
  private alert(alert: Alert) {
    // Clear any existing alerts first
    this.clear();
    // Then add the new alert
    this.subject.next(alert);
  }
}
