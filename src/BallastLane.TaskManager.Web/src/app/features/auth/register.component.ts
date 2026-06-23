import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { ApiErrorResponse } from '../../core/auth.models';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <main class="auth-shell">
      <section class="auth-panel">
        <p class="eyebrow">BallastLane Task Manager</p>
        <h1>Create account</h1>

        <form [formGroup]="form" (ngSubmit)="submit()" novalidate>
          <label>
            Email
            <input type="email" formControlName="email" autocomplete="email">
          </label>

          <label>
            Password
            <input type="password" formControlName="password" autocomplete="new-password">
          </label>

          <p *ngIf="errorMessage" class="error">{{ errorMessage }}</p>

          <button type="submit" [disabled]="form.invalid || isSubmitting">
            {{ isSubmitting ? 'Creating...' : 'Create account' }}
          </button>
        </form>

        <p class="switch-auth">
          Already registered?
          <a routerLink="/login">Sign in</a>
        </p>
      </section>
    </main>
  `,
})
export class RegisterComponent {
  readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(8)],
    }),
  });

  isSubmitting = false;
  errorMessage = '';

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => this.router.navigateByUrl('/tasks'),
      error: (error: HttpErrorResponse) => {
        this.errorMessage = (error.error as ApiErrorResponse | undefined)?.message
          ?? 'Unable to create the account.';
        this.isSubmitting = false;
      },
    });
  }
}
