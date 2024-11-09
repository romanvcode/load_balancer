import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { HistoryComponent } from './history/history.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
  { path: 'history', component: HistoryComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
