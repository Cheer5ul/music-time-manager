import { apiClient } from "./client";
import type { User, UserStats } from "../types";

export const getUsers = async () => (await apiClient.get<User[]>("/users")).data;
export const getUser = async (id: string) => (await apiClient.get<User>(`/users/${id}`)).data;
export const getUserStats = async (id: string) => (await apiClient.get<UserStats>(`/users/${id}/stats`)).data;
export const updateUsername = (id: string, newUsername: string) => apiClient.patch<void>(`/users/${id}/username`, { newUsername });
export const updatePassword = (id: string, currentPassword: string, newPassword: string) => apiClient.patch<void>(`/users/${id}/password`, { currentPassword, newPassword });
