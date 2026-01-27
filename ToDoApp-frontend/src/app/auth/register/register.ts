import { Component } from '@angular/core';
import { TokenService } from '../../core/services/token';
import { AuthService } from '../../core/services/auth';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { RouterModule } from '@angular/router'
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  registerForm;
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private tokenService: TokenService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      fullname: ['', Validators.required]
    });
  }
  submit() {
  if (this.registerForm.invalid) return;

  this.authService.register(this.registerForm.value)
    .subscribe({
      next: (res) => {
        //console.log('REGISTER RESPONSE 👉', res);
        console.log('Registered ✅');

        
        this.router.navigate(['/verify-email'], {
  queryParams: { email: this.registerForm.value.email }});
      },
      error: err => console.error(err)
    });
}



}
