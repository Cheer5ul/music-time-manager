import { apiClient } from "./client";
import type { Subtask, TaskStatus } from "../types";

export interface SubtaskFilters { status?: TaskStatus; isOverdue?: boolean; assigneeId?: string; taskId?: string; }
export const getSubtasks = async (params?: SubtaskFilters) => (await apiClient.get<Subtask[]>("/subtasks", { params })).data;
export const createSubtask = (taskId: string, title: string) => apiClient.post<void>(`/tasks/${taskId}/subtasks`, { title });
export const updateSubtaskTitle = (id: string, newTitle: string) => apiClient.patch<void>(`/subtasks/${id}`, { newTitle });
export const deleteSubtask = (id: string) => apiClient.delete<void>(`/subtasks/${id}`);
export const updateSubtaskStatus = (id: string, status: TaskStatus) => apiClient.patch<void>(`/subtasks/${id}/status`, undefined, { params: { status } });
export const assignSubtaskUsers = (id: string, userIds: string[]) => apiClient.put<void>(`/subtasks/${id}/assignees`, { userIds });
