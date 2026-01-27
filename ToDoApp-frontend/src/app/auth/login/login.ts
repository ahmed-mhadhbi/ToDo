import { Component } from '@angular/core';
import { TokenService } from '../../core/services/token';
import { AuthService } from '../../core/services/auth';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule , RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})

export class Login {
  loginForm;
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private tokenService: TokenService,
    private router: Router
  ) {

  this.loginForm = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });
  }
  
 submit() {
    if (this.loginForm.invalid) return;

    this.authService.login(this.loginForm.value)
      .subscribe((res: any) => {
        this.tokenService.save(
          res.data.accessToken,
          res.data.refreshToken
        );
        localStorage.setItem('accessToken', res.data.accessToken);
        localStorage.setItem('refreshToken', res.data.refreshToken);

        console.log('Logged in ✅');
        console.log('LOGIN RESPONSE 👉', res);
        this.router.navigate(['/todos']);
        
      });

}}
