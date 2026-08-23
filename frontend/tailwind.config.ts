import type { Config } from "tailwindcss";

export default {
  content: ["./index.html", "./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      fontFamily: { sans: ["Inter", "ui-sans-serif", "system-ui", "sans-serif"] },
      colors: {
        studio: {
          ink: "#07080d",
          panel: "#101219",
          muted: "#8f98aa",
          mint: "#51f0b3",
          coral: "#ff7a90",
          violet: "#9b8cff",
          sky: "#63d7ff",
          amber: "#ffd166"
        }
      },
      boxShadow: {
        glow: "0 24px 90px rgba(81,240,179,0.12)",
        panel: "0 18px 60px rgba(0,0,0,0.35)"
      }
    }
  },
  plugins: []
} satisfies Config;