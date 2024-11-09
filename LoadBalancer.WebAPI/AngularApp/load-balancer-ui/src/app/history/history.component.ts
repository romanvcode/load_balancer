import { Component, OnInit } from '@angular/core';
import { TaskService } from '../services/task.service';
import { TaskState } from '../models/task-state.model';
import { MatButtonModule } from '@angular/material/button';
import { DatePipe, NgFor } from '@angular/common';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [DatePipe, NgFor],
  templateUrl: './history.component.html',
  styleUrl: './history.component.css'
})
export class HistoryComponent implements OnInit {
  taskHistory: TaskState[] = [];
  currentPageTasks: TaskState[] = [];
  currentPage: number = 0;
  pageSize: number = 10;
  totalPages: number = 0;

  constructor(private taskService: TaskService) {}

  ngOnInit() {
    this.taskService.getTaskHistory().subscribe(
      (history) => {
        this.taskHistory = history;
        this.totalPages = Math.ceil(this.taskHistory.length / this.pageSize);
        this.updatePage();
      },
      (error) => {
        console.error('Error fetching task history:', error);
      }
    );
  }

  updatePage() {
    const start = this.currentPage * this.pageSize;
    const end = start + this.pageSize;
    this.currentPageTasks = this.taskHistory.slice(start, end);
  }

  prevPage() {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.updatePage();
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages - 1) {
      this.currentPage++;
      this.updatePage();
    }
  }
}
