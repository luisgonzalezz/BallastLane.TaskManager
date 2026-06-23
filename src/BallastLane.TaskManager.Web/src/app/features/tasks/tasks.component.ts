import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth.service';
import { TaskItem, TaskItemStatus, TaskService } from '../../core/task.service';

@Component({
  selector: 'app-tasks',
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <main class="workspace-shell">
      <header class="workspace-header">
        <div>
          <p class="eyebrow">Task workspace</p>
          <h1>My tasks</h1>
        </div>
        <button type="button" class="secondary-button" (click)="logout()">Sign out</button>
      </header>

      <section class="task-layout">
        <form class="task-form-panel" [formGroup]="form" (ngSubmit)="submit()" novalidate>
          <div>
            <p class="eyebrow">{{ editingTaskId ? 'Update task' : 'New task' }}</p>
            <h2>{{ editingTaskId ? 'Edit details' : 'Create task' }}</h2>
          </div>

          <label>
            Title
            <input type="text" formControlName="title" autocomplete="off">
          </label>

          <label>
            Description
            <textarea formControlName="description" rows="4"></textarea>
          </label>

          <div class="task-form-grid">
            <label>
              Due date
              <input type="date" formControlName="dueDate">
            </label>

            <label>
              Status
              <select formControlName="status">
                <option *ngFor="let status of statuses" [ngValue]="status.value">
                  {{ status.label }}
                </option>
              </select>
            </label>
          </div>

          <p *ngIf="errorMessage" class="error">{{ errorMessage }}</p>

          <div class="form-actions">
            <button type="submit" [disabled]="form.invalid || isSaving">
              {{ isSaving ? 'Saving...' : editingTaskId ? 'Save changes' : 'Create task' }}
            </button>
            <button
              *ngIf="editingTaskId"
              type="button"
              class="secondary-button"
              (click)="cancelEdit()">
              Cancel
            </button>
          </div>
        </form>

        <section class="task-list-section">
          <div class="task-list-header">
            <div>
              <p class="eyebrow">Current work</p>
              <h2>{{ tasks.length }} {{ tasks.length === 1 ? 'task' : 'tasks' }}</h2>
            </div>
            <button type="button" class="secondary-button" (click)="loadTasks()" [disabled]="isLoading">
              Refresh
            </button>
          </div>

          <p *ngIf="isLoading" class="muted">Loading tasks...</p>

          <div *ngIf="!isLoading && tasks.length === 0" class="empty-state">
            <h2>No tasks yet</h2>
            <p>Create the first task to start testing the API from Angular.</p>
          </div>

          <article *ngFor="let task of tasks" class="task-row">
            <div class="task-row-main">
              <span class="status-pill" [class.completed]="task.status === TaskItemStatus.Completed">
                {{ statusLabels[task.status] }}
              </span>
              <h3>{{ task.title }}</h3>
              <p>{{ task.description || 'No description provided.' }}</p>
              <time>Due {{ task.dueDate | date: 'mediumDate' }}</time>
            </div>

            <div class="task-row-actions">
              <button type="button" class="secondary-button" (click)="startEdit(task)">Edit</button>
              <button type="button" class="secondary-button danger-button" (click)="deleteTask(task)">Delete</button>
            </div>
          </article>
        </section>
      </section>
    </main>
  `,
})
export class TasksComponent {
  protected readonly TaskItemStatus = TaskItemStatus;
  readonly statuses = [
    { value: TaskItemStatus.Pending, label: 'Pending' },
    { value: TaskItemStatus.InProgress, label: 'In progress' },
    { value: TaskItemStatus.Completed, label: 'Completed' },
  ];
  readonly statusLabels: Record<TaskItemStatus, string> = {
    [TaskItemStatus.Pending]: 'Pending',
    [TaskItemStatus.InProgress]: 'In progress',
    [TaskItemStatus.Completed]: 'Completed',
  };
  readonly form = new FormGroup({
    title: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(120)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.maxLength(1000)],
    }),
    dueDate: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    status: new FormControl(TaskItemStatus.Pending, {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  tasks: TaskItem[] = [];
  editingTaskId: string | null = null;
  isLoading = false;
  isSaving = false;
  errorMessage = '';

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly taskService: TaskService,
  ) {
  }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.getTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load tasks.';
        this.isLoading = false;
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.getRawValue();
    this.isSaving = true;
    this.errorMessage = '';

    if (this.editingTaskId) {
      this.taskService.updateTask(this.editingTaskId, formValue).subscribe({
        next: (updatedTask) => {
          this.tasks = this.tasks.map((task) => task.id === updatedTask.id ? updatedTask : task);
          this.cancelEdit();
          this.isSaving = false;
        },
        error: () => {
          this.errorMessage = 'Unable to update the task.';
          this.isSaving = false;
        },
      });

      return;
    }

    const { status: _status, ...createRequest } = formValue;
    this.taskService.createTask(createRequest).subscribe({
      next: (createdTask) => {
        this.tasks = [createdTask, ...this.tasks];
        this.form.reset({
          title: '',
          description: '',
          dueDate: '',
          status: TaskItemStatus.Pending,
        });
        this.isSaving = false;
      },
      error: () => {
        this.errorMessage = 'Unable to create the task.';
        this.isSaving = false;
      },
    });
  }

  startEdit(task: TaskItem): void {
    this.editingTaskId = task.id;
    this.form.setValue({
      title: task.title,
      description: task.description,
      dueDate: task.dueDate.slice(0, 10),
      status: task.status,
    });
  }

  cancelEdit(): void {
    this.editingTaskId = null;
    this.form.reset({
      title: '',
      description: '',
      dueDate: '',
      status: TaskItemStatus.Pending,
    });
  }

  deleteTask(task: TaskItem): void {
    if (!window.confirm(`Delete "${task.title}"?`)) {
      return;
    }

    this.taskService.deleteTask(task.id).subscribe({
      next: () => {
        this.tasks = this.tasks.filter((item) => item.id !== task.id);
      },
      error: () => {
        this.errorMessage = 'Unable to delete the task.';
      },
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
