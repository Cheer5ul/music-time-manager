# 📋 03 — REST API Design

> Источник истины по контракту между фронтендом и бэкендом.

---

## 1. Общие конвенции

| Аспект | Решение |
|---|---|
| Base URL | `/api/v1` |
| Формат | JSON, `camelCase` |
| Даты | ISO 8601, UTC (`2026-07-15T00:00:00Z`) |
| Идентификаторы | `Guid`, строкой |
| Аутентификация | JWT в `HttpOnly` cookie (`access_token`). Не "Bearer" — токен не передаётся в заголовке `Authorization` |
| Авторизация | `[Auth]` = нужен валидный токен. Ролей и владения ресурсом нет — любой залогиненный участник может редактировать/удалять любую задачу и подзадачу |
| Content-Type | `application/json`, кроме `204 No Content` на DELETE |

### 1.1 Пагинация

Не применяется к `GET /tasks` / `GET /subtasks` — маленькая рабочая доска, весь список грузится разом. Применяется к историческим выборкам (`GET /tasks?status=Done&...` за произвольный период): `?page=1&pageSize=20`, ответ:
```json
{ "items": [...], "page": 1, "pageSize": 20, "totalCount": 143 }
```
Непагинируемые списки (`GET /users`) — плоский массив.

### 1.2 Ошибки — RFC 7807 ProblemDetails

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

| Код | Когда |
|---|---|
| `200 OK` | Успешный GET / PATCH / PUT с телом |
| `201 Created` | Успешный POST |
| `204 No Content` | Успешный DELETE, PATCH/PUT без тела |
| `400 Bad Request` | DTO-level валидация |
| `401 Unauthorized` | Нет / невалиден JWT |
| `404 Not Found` | Ресурс не существует |
| `409 Conflict` | Application-level конфликт (username занят) |
| `422 Unprocessable Entity` | Domain-level инвариант нарушен |

---

## 2. Auth

| Method | Path | Auth | Описание |
|---|---|---|---|
| POST | `/auth/register` | — | `{ username, password }` → создаёт пользователя, ставит cookie. Открыто для любого посетителя |
| POST | `/auth/login` | — | `{ username, password }` → ставит `HttpOnly` cookie |
| POST | `/auth/logout` | ✅ | Инвалидирует cookie |
| GET | `/auth/me` | ✅ | Текущий пользователь — фронт использует для восстановления сессии при загрузке |

---

## 3. Users

| Method | Path | Auth | Описание |
|---|---|---|---|
| GET | `/users` | ✅ | Список всех участников (для селекторов исполнителей) |
| GET | `/users/{id}` | ✅ | Профиль пользователя |
| PATCH | `/users/{id}/username` | ✅ Self | `{ newUsername }` |
| PATCH | `/users/{id}/password` | ✅ Self | `{ currentPassword, newPassword }` |
| GET | `/users/{id}/stats` | ✅ | `{ completedCount, missedCount }` — агрегатный запрос, ничего не хранится |

`DELETE /users/{id}` не спроектирован в v1 — ломает `createdBy` на исторических задачах.

---

## 4. Tasks

### 4.1 Модель статуса

`TaskStatus` в БД — **3 значения**: `ToDo | InProgress | Done`. `Overdue` — вычисляемый флаг в response, не значение из БД и не вводится вручную:

```json
{
  "status": "ToDo",
  "isOverdue": true,
  "dueDate": "2026-07-01T00:00:00Z"
}
```
`isOverdue = dueDate < UtcNow && status != Done`. Никакого фонового перевода статуса нет — считается на каждом чтении, всегда актуально, никакой задержки.

### 4.2 Эндпоинты

| Method | Path | Auth | Описание |
|---|---|---|---|
| GET | `/tasks` | ✅ | Список с фильтрами (4.3) |
| GET | `/tasks/{id}` | ✅ | Задача с `subtasks[]`, `assignees[]` |
| POST | `/tasks` | ✅ | Создать. `createdBy` — из токена |
| PATCH | `/tasks/{id}` | ✅ | `{ title?, description?, dueDate? }` |
| PATCH | `/tasks/{id}/status` | ✅ | `{ status: ToDo\|InProgress\|Done }`. `Done` — тот же эндпоинт, что и остальные переходы, отдельного `POST .../complete` не нужно: это одно и то же действие "сменить статус", у него один обработчик с побочным эффектом начисления "+" в статистику при переходе в `Done` |
| PUT | `/tasks/{id}/assignees` | ✅ | `{ userIds: [...] }` — полная замена. 422, если пусто |
| POST | `/tasks/{id}/recreate` | ✅ | См. 4.4 |
| DELETE | `/tasks/{id}` | ✅ | Каскадно удаляет subtasks |

### 4.3 Фильтры `GET /tasks`

```
?status=ToDo|InProgress|Done
?isOverdue=true|false     // транслируется сервером в предикат (dueDate < now && status != Done), не в WHERE по колонке
?assigneeId={guid}
?createdBy={guid}
?dueBefore={date}
?dueAfter={date}
?hasAssignees=true|false  // удобный фильтр найти "недооформленные" задачи без единого исполнителя
```
Комбинируются через `&`, все опциональны.

> ⚠️ **Изменение инварианта:** `POST /tasks` больше не требует `assigneeIds` — задача создаётся первым шагом без исполнителей, они добавляются сразу после через `PUT /tasks/{id}/assignees` (тот эндпоинт по-прежнему требует `MinLength(1)` — опустошить список после того, как он стал непустым, нельзя). Причина: `TaskAssignee` требует уже существующий `TaskId`, поэтому создание задачи и назначение исполнителей физически не могут быть одним атомарным запросом без сущностного "создать всё сразу" эндпоинта, от которого сознательно отказались. Задача с пустым `assignees[]` — валидное промежуточное состояние, не ошибка; фронт определяет это по `assignees.length === 0` в уже существующем поле ответа, без отдельного флага в БД.

### 4.4 `POST /tasks/{id}/recreate`

- Исходная задача не удаляется и не мутирует — остаётся в архиве.
- Создаётся новая `Task`: клонирует `title`, `description`, `assignees`, получает новый `dueDate` (обязателен), `recreatedFromTaskId` = id исходной.
```json
POST /tasks/{id}/recreate
{
  "dueDate": "2026-07-20T00:00:00Z",
  "title": "опционально",
  "description": null,
  "assigneeIds": ["...", "..."]
}
```
Response: `201 Created`, тело — новая задача с `recreatedFromTaskId`.

---

## 5. Subtasks

| Method | Path | Auth | Описание |
|---|---|---|---|
| GET | `/subtasks` | ✅ | Плоский список: `?status=&isOverdue=&assigneeId=&taskId=` |
| POST | `/tasks/{taskId}/subtasks` | ✅ | Создание, гнездится под задачу |
| PATCH | `/subtasks/{id}` | ✅ | `{ title? }` |
| PATCH | `/subtasks/{id}/status` | ✅ | `{ status: ToDo\|InProgress\|Done }` |
| PUT | `/subtasks/{id}/assignees` | ✅ | Замена набора, 422 если пусто |
| DELETE | `/subtasks/{id}` | ✅ | |

Как и `Task`, `Subtask` создаётся без исполнителей (`POST /tasks/{taskId}/subtasks` не требует `assigneeIds`) — та же причина: `SubtaskId` должен существовать до того, как создаются записи в `Subtask_Assignee`. Исполнители добавляются сразу после через `PUT /subtasks/{id}/assignees`.

`isOverdue` для Subtask вычисляется по `dueDate` **родительской Task** — у Subtask своего `dueDate` нет (см. `02-database-design.md`). При запросе `GET /subtasks` сервер обязан джойнить `Task` для вычисления этого флага.

Право редактирования — `[Auth]`, без ограничения по создателю (снято для всего проекта).

---

## 6. Ключевые DTO

```csharp
public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime DueDate,
    DateTime CreatedAt,
    TaskStatus Status,          // ToDo | InProgress | Done
    bool IsOverdue,              // computed, не из БД
    Guid CreatedBy,
    Guid? RecreatedFromTaskId,
    IReadOnlyList<UserSummary> Assignees,
    IReadOnlyList<SubtaskResponse> Subtasks
);

public record TaskCreateRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    [Required] DateTime DueDate
    // AssigneeIds сознательно отсутствует — задача создаётся без исполнителей,
    // они добавляются отдельным вызовом PUT /tasks/{id}/assignees сразу после
);

public record SubtaskCreateRequest(
    [Required, MaxLength(200)] string Title
    // аналогично Task — без AssigneeIds, добавляются через PUT /subtasks/{id}/assignees
);

public record TaskRecreateRequest(
    [Required] DateTime DueDate,
    [MaxLength(200)] string? Title,
    [MaxLength(2000)] string? Description,
    [MinLength(1)] List<Guid>? AssigneeIds
);

public record TaskStatusUpdateRequest(
    [Required] TaskStatus Status   // только ToDo | InProgress | Done
);

public record AssigneesUpdateRequest(
    [Required, MinLength(1)] List<Guid> UserIds
);
```

`MinLength(1)` на `AssigneeIds` — формально DTO-валидация (400), но семантически доменный инвариант. Дублируй проверку в Service/Domain-слое (422) — это защита, которая обязана держаться независимо от того, как вызвана сущность.

---

## 7. Background Jobs (`IHostedService`)

Для статуса задач джоба **не нужна** — `Overdue` вычисляется на чтении (см. 4.1).

Единственный кандидат на фоновую службу в проекте — отправка email/in-app уведомления о просрочке. Она:
- не хранит статус, только `overdueNotifiedAt: DateTime?` на Task/Subtask для идемпотентности (чтобы не слать письмо повторно)
- запускается по расписанию, проверяет `isOverdue == true AND overdueNotifiedAt == null`, шлёт письмо, проставляет метку
- не блокирует работу API, если сломается — задачи по-прежнему корректно показывают `isOverdue`, потому что это вычисляется независимо

Если уведомления не входят в ближайший спринт — этот раздел можно смело отложить, реализация API от него не зависит.

---

## 8. Закрытые вопросы

- ~~Кто может создать пользователя~~ → открытая регистрация, `POST /auth/register`, без auth
- ~~Трассируемость recreate~~ → `recreatedFromTaskId`, self-referencing FK
- ~~Право редактирования Subtask~~ → снято полностью, `createdBy` на Subtask не нужен
- ~~Overdue — хранить или вычислять~~ → вычислять, без background job, без задержки

## 9. Остающиеся открытые вопросы

- [ ] Нужен ли барьер на регистрацию (invite-token)
- [ ] `DELETE /users/{id}` — решить при необходимости
- [ ] Реализация email-уведомлений об Overdue — когда дойдут руки, не блокирует остальной API
