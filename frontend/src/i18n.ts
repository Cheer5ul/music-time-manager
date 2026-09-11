import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import en from "./locales/en/common.json";
import ru from "./locales/ru/common.json";

export const LANGUAGE_STORAGE_KEY = "app_language";
const savedLanguage = localStorage.getItem(LANGUAGE_STORAGE_KEY);
const language = savedLanguage === "ru" || savedLanguage === "en" ? savedLanguage : "en";

i18n.use(initReactI18next).init({
  resources: { en: { common: en }, ru: { common: ru } },
  lng: language,
  fallbackLng: "en",
  defaultNS: "common",
  interpolation: { escapeValue: false },
});

export const setAppLanguage = (nextLanguage: "en" | "ru") => {
  localStorage.setItem(LANGUAGE_STORAGE_KEY, nextLanguage);
  return i18n.changeLanguage(nextLanguage);
};

export default i18n;
