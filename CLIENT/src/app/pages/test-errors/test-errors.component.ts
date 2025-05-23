import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AlertService } from '../../shared/services/alert.service';

@Component({
  selector: 'app-test-errors',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.scss',
})
export class TestErrorsComponent {
  private http = inject(HttpClient);
  private alertService = inject(AlertService);
  baseUrl = environment.apiUrl;

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        console.error(error);
        this.alertService.error('404 Not Found Error');
      },
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        console.error(error);
        this.alertService.error('400 Bad Request Error');
      },
    });
  }

  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        console.error(error);
        this.alertService.error('500 Server Error');
      },
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        console.error(error);
        this.alertService.error('401 Unauthorized Error');
      },
    });
  }

  // getValidationError() {
  //   this.http.post(this.baseUrl + 'account/register', {}).subscribe({
  //     next: (response) => console.log(response),
  //     error: (error) => {
  //       console.error(error);
  //       this.alertService.error('Validation Error');
  //     }
  //   });
  // }
}
