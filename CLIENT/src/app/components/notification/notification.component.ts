import { Component, Input, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [NgIf],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss',
})
export class NotificationComponent implements OnInit {
  @Input() message = '';
  @Input() type: 'success' | 'error' | '' = '';
  @Input() autoHide = true;
  @Input() duration = 5000;

  visible = true;

  ngOnInit(): void {
    if (this.autoHide && this.message) {
      setTimeout(() => {
        this.visible = false;
      }, this.duration);
    }
  }
}
