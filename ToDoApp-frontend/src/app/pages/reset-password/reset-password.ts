import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth';
import {  } from '@angular/common';

import { ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-reset-password',
  imports: [ ReactiveFormsModule ],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss',
})
export class ResetPassword implements OnInit {

  email = '';
  token = '';
  form!: FormGroup;
  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {

  this.form = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit() {
    this.email = this.route.snapshot.queryParamMap.get('email') || '';
    this.token = this.route.snapshot.queryParamMap.get('token') || '';

    this.token = decodeURIComponent(this.token);

    console.log('EMAIL:', this.email);
    console.log('TOKEN:', this.token);
  }

  submit() {
    if (this.form.invalid) return;

    this.authService.resetPassword({
      email: this.email,
      token: this.token,
      newPassword: this.form.value.newPassword
    }).subscribe({
      next: () => {
        alert('Password reset successful ✅');
        this.router.navigate(['/login']);
      },
      error: () => alert('Reset password failed ❌')
    });
  }
}