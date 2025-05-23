import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription, timer } from 'rxjs';
import { Alert, AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss'],
})
export class AlertComponent implements OnInit, OnDestroy {
  alertSubscription!: Subscription;
  alerts: Alert[] = [];
  visible = false;

  constructor(private alertService: AlertService) {}

  ngOnInit() {
    this.alertSubscription = this.alertService.onAlert().subscribe((alert) => {
      if (!alert) {
        this.alerts = [];
        this.visible = false;
        return;
      }

      this.alerts = [alert];
      this.visible = true;

      if (alert.autoClose !== false) {
        timer(5000).subscribe(() => {
          this.removeAlert(alert);
        });
      }
    });
  }

  ngOnDestroy() {
    if (this.alertSubscription) {
      this.alertSubscription.unsubscribe();
    }
  }

  removeAlert(alert: Alert) {
    if (!this.alerts.includes(alert)) return;

    this.alerts = this.alerts.filter((x) => x !== alert);

    if (this.alerts.length === 0) {
      this.visible = false;
    }
  }

  cssClass(alert: Alert) {
    if (!alert) return '';

    const alertTypeClass = {
      'alert-info': alert.type === 'info',
      'alert-success': alert.type === 'success',
      'alert-warning': alert.type === 'warning',
      'alert-error': alert.type === 'error',
    };

    return alertTypeClass[`alert-${alert.type}`]
      ? `alert-${alert.type}`
      : 'alert-info';
  }
}
