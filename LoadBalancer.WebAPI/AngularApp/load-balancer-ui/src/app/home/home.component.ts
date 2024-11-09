import { Component } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { interval, Subscription, switchMap } from 'rxjs';
import { TaskService } from '../services/task.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [MatProgressBarModule, NgFor, NgIf, FormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  size: number = 0;
  progress: number = 0;
  result: string | undefined = '';
  private pollingSubscription: Subscription = new Subscription();
  
  constructor(private taskService: TaskService) {}

  onSubmit() {
    this.startPolling();
    const size = this.size;

    this.taskService.submitTask(size).subscribe({
      next: (taskState) => {
        this.result = taskState.result;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  startPolling(): void {
    this.pollingSubscription = interval(1000)
      .pipe(switchMap(() => this.taskService.getTaskProgress()))
      .subscribe({
        next: (progress) => this.progress = progress,
        error: (err) => console.error('Error fetching progress:', err)
      });
  }

  ngOnDestroy(): void {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }
}
