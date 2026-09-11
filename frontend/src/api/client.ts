import axios from "axios";
import type { ProblemDetails } from "../types";
import errorMessages from "../i18n/errors.ru.json";
import { toast } from "sonner";
import i18n from "../i18n";

const apiBaseUrl = import.meta.env.VITE_API_URL?.replace(/\/$/, "") || "https://localhost:7075";

export const apiClient = axios.create({ baseURL: apiBaseUrl, withCredentials: true });

apiClient.interceptors.response.use(
  response => response,
  error => {
    const url = String(error.config?.url ?? "");
    if (error.response?.status === 429) {
      const retryAfter = Number(error.response.headers?.["retry-after"]);
      toast.error(
        Number.isFinite(retryAfter) && retryAfter > 0
          ? i18n.t("errors.rateLimitRetry", { count: retryAfter })
          : i18n.t("errors.rateLimit"),
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
    const serverMessage = data?.detail || data?.errors?.[0]?.description || data?.title;
    if (serverMessage) return serverMessage;
    return error.response ? i18n.t("errors.request") : i18n.t("errors.network");
  }
  if (error instanceof Error) return error.message;
  return i18n.t("errors.request");
}
