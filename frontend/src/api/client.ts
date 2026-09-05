import axios from "axios";
import type { ProblemDetails } from "../types";
import errorMessages from "../i18n/errors.ru.json";
import { toast } from "sonner";

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? "https://localhost:7075",
  withCredentials: true,
});

apiClient.interceptors.response.use(
  response => response,
  error => {
    const url = String(error.config?.url ?? "");
    if (error.response?.status === 429) {
      const retryAfter = Number(error.response.headers?.["retry-after"]);
      toast.error(
        Number.isFinite(retryAfter) && retryAfter > 0
          ? `Слишком много запросов. Попробуйте снова через ${retryAfter} секунд.`
          : "Слишком много запросов. Подождите немного и попробуйте снова. Перезагрузите страницу",
        { id: "rate-limit" },
      );
    }
    if (error.response?.status === 401 && !url.startsWith("/auth/") && window.location.pathname !== "/login") window.location.assign("/login");
    return Promise.reject(error);
  },
);

export function getApiErrorMessage(error: unknown) {
  if (axios.isAxiosError<ProblemDetails>(error)) {
    const data = error.response?.data;
    const code = data?.errors?.[0]?.code;
    if (code && code in errorMessages) return errorMessages[code as keyof typeof errorMessages];
    return data?.detail || data?.errors?.[0]?.description || data?.title || "Не удалось выполнить запрос.";
  }
  if (error instanceof Error) return error.message;
  return "Не удалось выполнить запрос.";
}