import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { JsonPipe, NgIf } from '@angular/common';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [RouterLink, NgIf, JsonPipe],
  templateUrl: './server-error.component.html',
  styleUrl: './server-error.component.scss',
})
export class ServerErrorComponent implements OnInit {
  error: any;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.['error'];
  }

  ngOnInit(): void {
    if (this.error) {
      console.error('Server error details:', this.error);
    }
  }
}
