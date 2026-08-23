import { apiClient } from "./client";
import type { Task, TaskStatus } from "../types";

export interface TaskFilters { status?: TaskStatus; isOverdue?: boolean; assigneeId?: string; createdBy?: string; dueBefore?: string; dueAfter?: string; hasAssignees?: boolean; }
export interface CreateTaskRequest { title: string; description?: string; dueDate: string; }
export interface UpdateTaskRequest { title?: string; description?: string; dueDate?: string; }
export interface RecreateTaskRequest { dueDate: string; title?: string; description?: string; assigneeIds?: string[]; }
export const getTasks = async (params?: TaskFilters) => (await apiClient.get<Task[]>("/tasks", { params })).data;
export const getTask = async (id: string) => (await apiClient.get<Task>(`/tasks/${id}`)).data;
// The server's runtime returns the newly-created task; this permits the documented follow-up assignment request.
export const createTask = async (body: CreateTaskRequest) => (await apiClient.post<Task | undefined>("/tasks", body)).data;
export const updateTask = (id: string, body: UpdateTaskRequest) => apiClient.patch<void>(`/tasks/${id}`, body);
export const deleteTask = (id: string) => apiClient.delete<void>(`/tasks/${id}`);
export const updateTaskStatus = (id: string, status: TaskStatus) => apiClient.patch<void>(`/tasks/${id}/status`, undefined, { params: { status } });
export const assignTaskUsers = (id: string, userIds: string[]) => apiClient.put<void>(`/tasks/${id}/assignees`, { userIds });
export const recreateTask = (id: string, body: RecreateTaskRequest) => {
  const params = new URLSearchParams();
  params.set("DueDate", body.dueDate);
  if (body.title !== undefined) params.set("Title", body.title);
  if (body.description !== undefined) params.set("Description", body.description);
  body.assigneeIds?.forEach(id => params.append("AssigneeIds", id));
  return apiClient.post<void>(`/tasks/${id}/recreate`, undefined, { params });
};
