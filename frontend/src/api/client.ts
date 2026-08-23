import axios from "axios";
import type { ProblemDetails } from "../types";

export const apiClient = axios.create({ baseURL: "https://localhost:7075", withCredentials: true });

apiClient.interceptors.response.use(
  response => response,
  error => {
    const url = String(error.config?.url ?? "");
    if (error.response?.status === 401 && !url.startsWith("/auth/") && window.location.pathname !== "/login") window.location.assign("/login");
    return Promise.reject(error);
  },
);

export function getApiErrorMessage(error: unknown) {
  if (axios.isAxiosError<ProblemDetails>(error)) {
    const data = error.response?.data;
    return data?.detail || data?.errors?.[0]?.description || data?.title || "Не удалось выполнить запрос.";
  }
  if (error instanceof Error) return error.message;
  return "Не удалось выполнить запрос.";
}
