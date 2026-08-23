import { apiClient } from "./client";
import type { User } from "../types";

export interface Credentials { username: string; password: string; }
export const register = (body: Credentials) => apiClient.post<void>("/auth/register", body);
export const login = (body: Credentials) => apiClient.post<void>("/auth/login", body);
export const logout = () => apiClient.post<void>("/auth/logout");
export const getMe = async () => (await apiClient.get<User>("/auth/me")).data;
