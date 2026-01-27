import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './verify-email.html',
    styleUrl: './verify-email.scss',
})
export class VerifyEmailComponent {
  email = '';
  otp = '';
  loading = false;
  message = '';
  error = '';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
    });
  }

  verify() {
    if (!this.otp || this.otp.length !== 6) {
      this.error = 'OTP must be 6 digits';
      return;
    }

    this.loading = true;
    this.error = '';
    this.message = '';

    this.http.post<any>('https://localhost:7075/api/auth/verify-email', {
      email: this.email,
      otp: this.otp
    }).subscribe({
      next: res => {
        this.message = 'Email verified successfully 🎉';
        this.loading = false;

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },
      error: err => {
        this.loading = false;
        this.error = err.error || 'Invalid or expired OTP';
      }
    });
  }
}
