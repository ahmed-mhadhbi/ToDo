import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  register(data: any) {
    return this.http.post(`${this.baseUrl}/register`, data);
  }

  login(data: any) {
    return this.http.post(`${this.baseUrl}/login`, data);
  }

  refreshToken(refreshToken: string) {
    return this.http.post(`${this.baseUrl}/refresh`, { refreshToken });
  }

  resetPassword(data: any) {
  return this.http.post(`${this.baseUrl}/reset-password`, data);
}

  forgotPassword(data: any) {
    return this.http.post(`${this.baseUrl}/forgot-password`, data);
  } 
  verifyOtp(email: string, otp: string) {
  return this.http.post('/api/auth/verify-email', {email, otp});
}


}
