import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { JsonPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-bad-request',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, JsonPipe],
  templateUrl: './bad-request.component.html',
  styleUrl: './bad-request.component.scss',
})
export class BadRequestComponent implements OnInit {
  errors: string[] = [];
  errorDetails: any;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras?.state;

    if (state && state['errors']) {
      if (Array.isArray(state['errors'])) {
        this.errors = state['errors'];
      }
    }

    this.errorDetails = state?.['error'];
  }

  ngOnInit(): void {
    if (this.errors.length > 0) {
      console.error('Validation errors:', this.errors);
    }
    if (this.errorDetails) {
      console.error('Error details:', this.errorDetails);
    }
  }
}
