import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { AuthService } from '../../core/auth.service';
import { TaskItem, TaskItemStatus, TaskService } from '../../core/task.service';
import { TasksComponent } from './tasks.component';

describe('TasksComponent', () => {
  let fixture: ComponentFixture<TasksComponent>;
  let component: TasksComponent;
  let taskService: jasmine.SpyObj<TaskService>;
  let authService: jasmine.SpyObj<AuthService>;

  const task: TaskItem = {
    id: '22222222-2222-2222-2222-222222222222',
    userId: '11111111-1111-1111-1111-111111111111',
    title: 'Prepare interview',
    description: 'Review the project.',
    status: TaskItemStatus.Pending,
    dueDate: '2026-07-01T00:00:00Z',
    createdAt: '2026-06-22T00:00:00Z',
    updatedAt: '2026-06-22T00:00:00Z',
  };

  beforeEach(async () => {
    taskService = jasmine.createSpyObj<TaskService>('TaskService', [
      'getTasks',
      'createTask',
      'updateTask',
      'deleteTask',
    ]);
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['logout']);

    taskService.getTasks.and.returnValue(of([task]));
    taskService.createTask.and.returnValue(of({
      ...task,
      id: '33333333-3333-3333-3333-333333333333',
      title: 'Build UI',
    }));
    taskService.updateTask.and.returnValue(of({
      ...task,
      title: 'Updated title',
      status: TaskItemStatus.Completed,
    }));
    taskService.deleteTask.and.returnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [TasksComponent],
      providers: [
        { provide: TaskService, useValue: taskService },
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TasksComponent);
    component = fixture.componentInstance;
  });

  it('loads and renders tasks on init', () => {
    fixture.detectChanges();

    expect(taskService.getTasks).toHaveBeenCalled();
    expect(component.tasks).toEqual([task]);
    expect(fixture.nativeElement.textContent).toContain('Prepare interview');
  });

  it('creates a task from the form', () => {
    fixture.detectChanges();
    component.form.setValue({
      title: 'Build UI',
      description: 'Connect Angular to the API.',
      dueDate: '2026-07-03',
      status: TaskItemStatus.Pending,
    });

    component.submit();

    expect(taskService.createTask).toHaveBeenCalledWith({
      title: 'Build UI',
      description: 'Connect Angular to the API.',
      dueDate: '2026-07-03',
    });
    expect(component.tasks[0].title).toBe('Build UI');
  });

  it('updates the selected task from the form', () => {
    fixture.detectChanges();

    component.startEdit(task);
    component.form.patchValue({
      title: 'Updated title',
      status: TaskItemStatus.Completed,
    });
    component.submit();

    expect(taskService.updateTask).toHaveBeenCalledWith(task.id, {
      title: 'Updated title',
      description: 'Review the project.',
      dueDate: '2026-07-01',
      status: TaskItemStatus.Completed,
    });
    expect(component.editingTaskId).toBeNull();
    expect(component.tasks[0].title).toBe('Updated title');
  });

  it('deletes a task after confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    fixture.detectChanges();

    component.deleteTask(task);

    expect(taskService.deleteTask).toHaveBeenCalledWith(task.id);
    expect(component.tasks).toEqual([]);
  });
});
