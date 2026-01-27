import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, Validators } from '@angular/forms';
import { TodoService } from '../../core/services/todo';
import { FormBuilder } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './todo-list.html',
  styleUrls: ['./todo-list.scss']

})

export class TodoListComponent implements OnInit {
  todos: any[] = [];
  newTodoTitle = '';
  
  changePwdForm;

  constructor(private todoService: TodoService, private fb: FormBuilder) {
  this.changePwdForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.todoService.getMyTodos().subscribe(res => {
      this.todos = res.data ?? res;
    });
  }

  addTodo(): void {
    if (!this.newTodoTitle.trim()) return;

    this.todoService.createTodo({ title: this.newTodoTitle })
      .subscribe(todo => {
        this.todos.unshift(todo); // add to list immediately
        this.newTodoTitle = '';    // clear input
      });
  }
  deleteTodo(id: string): void {
    this.todoService.deleteTodo(id)
      .subscribe(() => {
        this.todos = this.todos.filter(todo => todo.id !== id);
      });
  }

  

submitChangePassword() {
  if (this.changePwdForm.invalid) return;

  this.todoService
    .changePassword(this.changePwdForm.value as any)
    .subscribe({
      next: () => {
        alert('Password updated successfully ✅');
        this.changePwdForm.reset();
      },
      error: (err) => {
        alert(err.error?.[0]?.description || 'Password change failed');
      }
    });
}
}