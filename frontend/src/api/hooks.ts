import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getMe, login, register, type Credentials } from "./auth";
import { assignTaskUsers, createTask, deleteTask, getTask, getTasks, recreateTask, type CreateTaskRequest, type RecreateTaskRequest, type TaskFilters, updateTask, updateTaskStatus, type UpdateTaskRequest } from "./tasks";
import { assignSubtaskUsers, createSubtask, deleteSubtask, updateSubtaskStatus, updateSubtaskTitle } from "./subtasks";
import { getUserStats, getUsers } from "./users";
import type { TaskStatus, User } from "../types";

const tasksKey = ["tasks"] as const;
const taskKey = (id: string) => ["tasks", "detail", id] as const;
export const useMeQuery = () => useQuery({ queryKey: ["auth", "me"], queryFn: getMe, retry: false, staleTime: 60_000 });
export const useTasksQuery = (filters?: TaskFilters) => useQuery({ queryKey: [...tasksKey, filters ?? {}], queryFn: () => getTasks(filters) });
export const useTaskQuery = (id: string | null) => useQuery({ queryKey: taskKey(id ?? ""), queryFn: () => getTask(id!), enabled: Boolean(id) });
export const useUsersQuery = () => useQuery({ queryKey: ["users"], queryFn: getUsers, staleTime: 60_000 });
export const useUserStatsQuery = (userId: string) => useQuery({ queryKey: ["users", userId, "stats"], queryFn: () => getUserStats(userId), staleTime: 60_000 });
export const useLoginMutation = () => useMutation({ mutationFn: (body: Credentials) => login(body) });
export const useRegisterMutation = () => useMutation({ mutationFn: (body: Credentials) => register(body) });
export function useCreateTaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: async ({ task, assigneeIds }: { task: CreateTaskRequest; assigneeIds: string[] }) => {
  const currentUser = queryClient.getQueryData<User>(["auth", "me"]);
  const beforeIds = new Set((currentUser ? await getTasks({ createdBy: currentUser.id }) : []).map(item => item.id));
  let created = await createTask(task);
  if (!created?.id && currentUser) {
    const createdByUser = await getTasks({ createdBy: currentUser.id });
    created = createdByUser.filter(item => !beforeIds.has(item.id)).sort((a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt))[0];
  }
  if (assigneeIds.length && !created?.id) throw new Error("Задача создана, но сервер не вернул её идентификатор для назначения исполнителей.");
  if (assigneeIds.length && created) await assignTaskUsers(created.id, assigneeIds);
  return created;
}, onSuccess: () => queryClient.invalidateQueries({ queryKey: tasksKey }) }); }
export function useTaskStatusMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, status }: { id: string; status: TaskStatus }) => updateTaskStatus(id, status), onSuccess: () => queryClient.invalidateQueries({ queryKey: tasksKey }) }); }
export function useRecreateTaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, body }: { id: string; body: RecreateTaskRequest }) => recreateTask(id, body), onSuccess: () => queryClient.invalidateQueries({ queryKey: tasksKey }) }); }
function refreshTask(queryClient: ReturnType<typeof useQueryClient>, id: string) { return Promise.all([queryClient.invalidateQueries({ queryKey: tasksKey }), queryClient.invalidateQueries({ queryKey: taskKey(id) })]); }
export function useUpdateTaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, body }: { id: string; body: UpdateTaskRequest }) => updateTask(id, body), onSuccess: (_, variables) => refreshTask(queryClient, variables.id) }); }
export function useDeleteTaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: (id: string) => deleteTask(id), onSuccess: () => queryClient.invalidateQueries({ queryKey: tasksKey }) }); }
export function useAssignTaskUsersMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, userIds }: { id: string; userIds: string[] }) => assignTaskUsers(id, userIds), onSuccess: (_, variables) => refreshTask(queryClient, variables.id) }); }
export function useCreateSubtaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ taskId, title }: { taskId: string; title: string }) => createSubtask(taskId, title), onSuccess: (_, variables) => refreshTask(queryClient, variables.taskId) }); }
export function useSubtaskStatusMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, taskId, status }: { id: string; taskId: string; status: TaskStatus }) => updateSubtaskStatus(id, status), onSuccess: (_, variables) => refreshTask(queryClient, variables.taskId) }); }
export function useUpdateSubtaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, taskId, title }: { id: string; taskId: string; title: string }) => updateSubtaskTitle(id, title), onSuccess: (_, variables) => refreshTask(queryClient, variables.taskId) }); }
export function useDeleteSubtaskMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, taskId }: { id: string; taskId: string }) => deleteSubtask(id), onSuccess: (_, variables) => refreshTask(queryClient, variables.taskId) }); }
export function useAssignSubtaskUsersMutation() { const queryClient = useQueryClient(); return useMutation({ mutationFn: ({ id, taskId, userIds }: { id: string; taskId: string; userIds: string[] }) => assignSubtaskUsers(id, userIds), onSuccess: (_, variables) => refreshTask(queryClient, variables.taskId) }); }
