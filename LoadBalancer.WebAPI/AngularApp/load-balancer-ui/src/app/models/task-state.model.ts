export interface TaskState {
    id?: number;
    state: string;
    progress: number;
    createdAt: Date;
    result?: string;
  }