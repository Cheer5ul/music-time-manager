# TrackFlow Studio Architecture

This frontend uses a feature-based React architecture designed for an ASP.NET Core Web API backend.

- src/components: reusable UI and layout primitives.
- src/features: product domains such as auth, dashboard, projects, tasks, timeline, statistics, profile, notifications, and settings.
- src/lib.ts: shared app utilities, Axios client, and motion presets.
- src/data.ts: typed mock data and API placeholders used until the backend endpoints are connected.
- src/types.ts: shared domain contracts for projects, tasks, members, priorities, statuses, and weekdays.

The first implementation is intentionally compact, but the feature folders are in place so pages, API services, hooks, and components can be split out cleanly as the backend grows.
