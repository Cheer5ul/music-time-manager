export type TaskStatus = "ToDo" | "InProgress" | "Done";

export interface User {
  id: string;
  username: string;
}

export interface Subtask {
  id: string;
  title: string | null;
  status: TaskStatus;
  isOverdue: boolean;
  taskId: string;
  assignees?: User[] | null;
}

export interface Task {
  id: string;
  title: string | null;
  description: string | null;
  dueDate: string;
  createdAt: string;
  status: TaskStatus;
  isOverdue: boolean;
  createdBy: string;
  recreatedFromTaskId: string | null;
  assignees: User[] | null;
  subtasks: Subtask[] | null;
}

export interface UserStats { completedTasks: number; missedTasks: number; }
export interface ProblemDetails { title?: string; detail?: string; status?: number; errors?: Array<{ code: string; description: string }>; }
