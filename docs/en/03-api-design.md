# 📋 03 — REST API Design

> The source of truth for the contract between frontend and backend.

---

## 1. General Conventions

| Aspect | Decision |
|---|---|
| Base URL | `/api/v1` |
| Format | JSON, `camelCase` |
| Dates | ISO 8601, UTC (`2026-07-15T00:00:00Z`) |
| Identifiers | `Guid`, as a string |
| Authentication | JWT in an `HttpOnly` cookie (`access_token`). Not "Bearer" — the token is never sent in the `Authorization` header |
| Authorization | `[Auth]` = requires a valid token. No roles, no resource ownership — any logged-in member can edit/delete any task or subtask |
| Content-Type | `application/json`, except `204 No Content` on DELETE |

### 1.1 Pagination

Not applied to `GET /tasks` / `GET /subtasks` — a small team board, the whole list loads at once. Applied to historical queries (`GET /tasks?status=Done&...` over an arbitrary period): `?page=1&pageSize=20`, response:
```json
{ "items": [...], "page": 1, "pageSize": 20, "totalCount": 143 }
```
Non-paginated lists (`GET /users`) — a flat array.

### 1.2 Errors — RFC 7807 ProblemDetails

```json
{
  "type": "https://example.com/errors/validation-failed",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "detail": "Task must have at least one assignee.",
  "instance": "/api/v1/tasks/3fa85f64-.../assignees",
  "errors": { "assigneeIds": ["Must contain at least one item."] }
}
```

| Code | When |
|---|---|
| `200 OK` | Successful GET / PATCH / PUT with a body |
| `201 Created` | Successful POST |
| `204 No Content` | Successful DELETE, or PATCH/PUT with no body |
| `400 Bad Request` | DTO-level validation |
| `401 Unauthorized` | Missing / invalid JWT |
| `404 Not Found` | Resource doesn't exist |
| `409 Conflict` | Application-level conflict (username taken) |
| `422 Unprocessable Entity` | Domain-level invariant violated |

---

## 2. Auth

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | `/auth/register` | — | `{ username, password }` → creates a user, sets the cookie. Open to any visitor |
| POST | `/auth/login` | — | `{ username, password }` → sets the `HttpOnly` cookie |
| POST | `/auth/logout` | ✅ | Invalidates the cookie |
| GET | `/auth/me` | ✅ | Current user — used by the frontend to restore the session on load |

---

## 3. Users

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | `/users` | ✅ | List of all members (for assignee selectors) |
| GET | `/users/{id}` | ✅ | User profile |
| PATCH | `/users/{id}/username` | ✅ Self | `{ newUsername }` |
| PATCH | `/users/{id}/password` | ✅ Self | `{ currentPassword, newPassword }` |
| GET | `/users/{id}/stats` | ✅ | `{ completedCount, missedCount }` — an aggregate query, nothing stored |

`DELETE /users/{id}` is not designed for v1 — it would break `createdBy` on historical tasks.

---

## 4. Tasks

### 4.1 Status Model

`TaskStatus` in the DB — **3 values**: `ToDo | InProgress | Done`. `Overdue` is a computed flag in the response, not a DB value, and cannot be set manually:

```json
{
  "status": "ToDo",
  "isOverdue": true,
  "dueDate": "2026-07-01T00:00:00Z"
}
```
`isOverdue = dueDate < UtcNow && status != Done`. There is no background status transition — it's computed on every read, always up to date, no delay.

### 4.2 Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | `/tasks` | ✅ | List with filters (4.3) |
| GET | `/tasks/{id}` | ✅ | Task with nested `subtasks[]`, `assignees[]` |
| POST | `/tasks` | ✅ | Create. `createdBy` comes from the token |
| PATCH | `/tasks/{id}` | ✅ | `{ title?, description?, dueDate? }` |
| PATCH | `/tasks/{id}/status` | ✅ | `{ status: ToDo\|InProgress\|Done }`. `Done` uses the same endpoint as any other transition — no separate `POST .../complete` is needed: it's the same action, "change status", with one handler that has a side effect of crediting "+" to statistics on transition to `Done` |
| PUT | `/tasks/{id}/assignees` | ✅ | `{ userIds: [...] }` — full replace. 422 if empty |
| POST | `/tasks/{id}/recreate` | ✅ | See 4.4 |
| DELETE | `/tasks/{id}` | ✅ | Cascades to delete subtasks |

### 4.3 `GET /tasks` Filters

```
?status=ToDo|InProgress|Done
?isOverdue=true|false     // translated server-side into a predicate (dueDate < now && status != Done), not a WHERE on a column
?assigneeId={guid}
?createdBy={guid}
?dueBefore={date}
?dueAfter={date}
?hasAssignees=true|false  // convenience filter to find "unfinished" tasks with no assignee at all
```
Combined via `&`, all optional.

> ⚠️ **Invariant change:** `POST /tasks` no longer requires `assigneeIds` — the task is created in the first step with no assignees, they're added right after via `PUT /tasks/{id}/assignees` (that endpoint still requires `MinLength(1)` — once the list is non-empty, it can't be emptied). Reason: `TaskAssignee` requires an already-existing `TaskId`, so creating a task and assigning users can't physically be one atomic request without a single "create everything at once" endpoint, which was deliberately rejected. A task with an empty `assignees[]` is a valid intermediate state, not an error; the frontend detects this via `assignees.length === 0` on the field already present in the response, no separate DB flag needed.

### 4.4 `POST /tasks/{id}/recreate`

- The original task is not deleted or mutated — it stays in the archive.
- A new `Task` is created: it clones `title`, `description`, `assignees`, gets a new `dueDate` (required), and `recreatedFromTaskId` = the original's id.
```json
POST /tasks/{id}/recreate
{
  "dueDate": "2026-07-20T00:00:00Z",
  "title": "optional",
  "description": null,
  "assigneeIds": ["...", "..."]
}
```
Response: `201 Created`, body — the new task, including `recreatedFromTaskId`.

---

## 5. Subtasks

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | `/subtasks` | ✅ | Flat list: `?status=&isOverdue=&assigneeId=&taskId=` |
| POST | `/tasks/{taskId}/subtasks` | ✅ | Creation, nested under the task |
| PATCH | `/subtasks/{id}` | ✅ | `{ title? }` |
| PATCH | `/subtasks/{id}/status` | ✅ | `{ status: ToDo\|InProgress\|Done }` |
| PUT | `/subtasks/{id}/assignees` | ✅ | Replace the set, 422 if empty |
| DELETE | `/subtasks/{id}` | ✅ | |

Like `Task`, `Subtask` is created with no assignees (`POST /tasks/{taskId}/subtasks` doesn't require `assigneeIds`) — same reason: the `SubtaskId` must exist before records in `Subtask_Assignee` can be created. Assignees are added right after via `PUT /subtasks/{id}/assignees`.

`isOverdue` for a Subtask is computed from the parent Task's `dueDate` — Subtask has no `dueDate` of its own (see `02-database-design.md`). On `GET /subtasks`, the server must join `Task` to compute this flag.

Editing rights — `[Auth]`, no creator-based restriction (removed project-wide).

---

## 6. Key DTOs

```csharp
public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime DueDate,
    DateTime CreatedAt,
    TaskStatus Status,          // ToDo | InProgress | Done
    bool IsOverdue,              // computed, not from the DB
    Guid CreatedBy,
    Guid? RecreatedFromTaskId,
    IReadOnlyList<UserSummary> Assignees,
    IReadOnlyList<SubtaskResponse> Subtasks
);

public record TaskCreateRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    [Required] DateTime DueDate
    // AssigneeIds is deliberately absent — the task is created with no assignees,
    // they're added via a separate PUT /tasks/{id}/assignees call right after
);

public record SubtaskCreateRequest(
    [Required, MaxLength(200)] string Title
    // same as Task — no AssigneeIds, added via PUT /subtasks/{id}/assignees
);

public record TaskRecreateRequest(
    [Required] DateTime DueDate,
    [MaxLength(200)] string? Title,
    [MaxLength(2000)] string? Description,
    [MinLength(1)] List<Guid>? AssigneeIds
);

public record TaskStatusUpdateRequest(
    [Required] TaskStatus Status   // ToDo | InProgress | Done only
);

public record AssigneesUpdateRequest(
    [Required, MinLength(1)] List<Guid> UserIds
);
```

`MinLength(1)` on `AssigneeIds` — formally a DTO-level validation (400), but semantically a domain invariant. Duplicate the check at the Service/Domain layer (422) — this is a guarantee that must hold regardless of how the entity is invoked.

---

## 7. Background Jobs (`IHostedService`)

No job is needed for task status — `Overdue` is computed on read (see 4.1).

The only real candidate for a background service in this project is sending an overdue email/in-app notification. It:
- doesn't store a status, only `overdueNotifiedAt: DateTime?` on Task/Subtask for idempotency (to avoid sending the email twice)
- runs on a schedule, checks `isOverdue == true AND overdueNotifiedAt == null`, sends the email, sets the timestamp
- doesn't block the API if it breaks — tasks still correctly show `isOverdue`, because that's computed independently

If notifications aren't part of the near-term sprint, this section can safely be deferred — the rest of the API doesn't depend on it.

---

## 8. Resolved Questions

- ~~Who can create a user~~ → open registration, `POST /auth/register`, no auth
- ~~Recreate traceability~~ → `recreatedFromTaskId`, self-referencing FK
- ~~Subtask editing rights~~ → removed entirely, `createdBy` on Subtask not needed
- ~~Overdue — store or compute~~ → compute, no background job, no delay

## 9. Remaining Open Questions

- [ ] Is a registration barrier needed (invite-token)
- [ ] `DELETE /users/{id}` — decide when actually needed
- [ ] Implementation of Overdue email notifications — whenever there's time, doesn't block the rest of the API
