import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { TodoListComponent } from './todo/todo-list/todo-list';
import { ResetPassword } from './pages/reset-password/reset-password';
import { ForgotPassword } from './pages/forgot-password/forgot-password';
import { VerifyEmailComponent } from './pages/verify-email/verify-email';
import { Dashboard } from './dashboard/dashboard';
// import { NavBarComponent } from './theme/layouts/admin-layout/nav-bar/nav-bar.component';
// import { NavRightComponent } from './theme/layouts/admin-layout/nav-bar/nav-right/nav-right.component';
// import { NavLeftComponent } from './theme/layouts/admin-layout/nav-bar/nav-left/nav-left.component';  


export const routes: Routes = [
  { path: 'login', component: Login },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'register', component: Register },
  { path: 'todos', component: TodoListComponent },
  { path: 'reset-password', component: ResetPassword },
  { path: 'forgot-password', component: ForgotPassword },
  { path: 'verify-email', component: VerifyEmailComponent },
  { path: 'dashboard', component: Dashboard },
  // { path: 'navbar', component: NavBarComponent },
  // { path: 'navright', component: NavRightComponent },
  // { path: 'navleft', component: NavLeftComponent }
];
