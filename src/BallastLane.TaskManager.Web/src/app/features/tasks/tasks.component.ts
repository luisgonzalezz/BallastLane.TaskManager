import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-tasks',
  imports: [CommonModule],
  template: `
    <main class="workspace-shell">
      <header class="workspace-header">
        <div>
          <p class="eyebrow">Task workspace</p>
          <h1>My tasks</h1>
        </div>
        <button type="button" class="secondary-button" (click)="logout()">Sign out</button>
      </header>

      <section class="empty-state">
        <h2>Task board is ready</h2>
        <p>The next stage will connect this workspace to the task API.</p>
      </section>
    </main>
  `,
})
export class TasksComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {
  }

  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
