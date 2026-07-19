# 📋 01 — Requirements & User Flows
---

## 📝 1. Overview

> An application for solving time-management and organization problems.
> An interactive task manager for a group, with a simple interface, that makes
> it easy to create tasks, track their progress, statistics, requirements, and
> each member's contribution. Aimed at music production work.

---

## 👥 2. User Roles

| Role | Description | Permissions |
|---|---|---|
| Member | A regular team member, registered on the site | Creates, edits, and deletes **any** task and subtask (not just their own), sees everyone's tasks |

>💡 There is currently no role separation — all members have equal permissions.

> ⚠️ **Open registration:** any unregistered, logged-out visitor can create an account, no invite required. This is a deliberate departure from the original "private, invite-only team" idea. Practical consequence: the app has no entry barrier. If this becomes a problem, an invite-token or an admin role approving new members will be needed. Open question — see section 7.

---

## ⚙️ 3. Functional Requirements

### ✅ 3.1 Tasks

- A user can create a task with the fields:
  - 🏷️ **title**
  - 📝 **description** (optional)
  - 📅 **due date (DueDate)**
  - 📅 **creation date (CreatedAt)**
  - 👤👥 **assignee(s)**
  - 🚦 **status** (details below)
- 📂 A task can contain **subtasks**
- 🗓️ A task is displayed on the calendar relative to the current time marker and smoothly moves as time passes
- ⏰ Overdue tasks (past DueDate, not completed) are shown separately from today's tasks

### 📂 3.2 Subtasks

- 🔗 A subtask belongs to **one task**; it inherits urgency (DueDate) from it — it has no DueDate of its own
- Example:
  - 🎵 Task — make a track
  - 🎼 Subtasks
    - 🥁 write a beat
    - ✍️ write lyrics
    - 🎤 record vocals
    - 🎚️ mix
- A user can create a subtask with the fields:
  - 🏷️ title
  - 🚦 **status**
  - 👤👥 **assignee(s)**

### 3.3 Statuses

- ⏳ To Do
- 🔄 In Progress
- ✅ Done
- ❌ Overdue

> ⚙️ **Overdue is a computed state, not a stored field.** In the database, `status` (for both Task and Subtask) holds only 3 values: `ToDo`, `InProgress`, `Done`. "Overdue" is not a separate status value — it's a derived flag computed at request time: `dueDate < now AND status != Done` (for Subtask, based on the parent Task's `dueDate`, since it has none of its own).
>
> We deliberately decided against storing an `Overdue` status via a periodic background job — the option was considered, but it either introduces a delay between job ticks (DB status temporarily "lies"), or requires precise per-task scheduling, which is over-engineering at this project's scale. Computing it on the fly always gives an accurate result without a single extra piece of infrastructure.
>
> The only thing that genuinely needs a background `IHostedService` in this project is sending an email/in-app "task is overdue" notification (that's push, it can't be computed on request). This doesn't need a stored status — a timestamp of the last notification sent per task is enough to avoid duplicate emails.

### 📦 3.4 Completed Tasks
- ✅ Once completed, a task moves into the completed group.
- 👤 Every user listed as an assignee gets a "+" in their completed-tasks statistics when the task becomes completed
- 👥 Every user listed as an assignee gets a "+" in their completed-subtasks statistics when the subtask becomes completed

### 3.5 Incomplete Tasks
- ❌ Overdue tasks (see 3.3 — computed) are shown separately.
- 👥 An assignee who fails to complete a task/subtask in time gets a "-" in their completed-subtasks statistics (also computed via an aggregate query, not stored)
- 🔄 A user can recreate an incomplete task. The new task retains the previous one's data (except `dueDate`, which is set anew), but allows instantly changing individual fields in the same action. The new task stores a reference to the original (`recreatedFromTaskId`).

### 🔐 3.6 Authentication

- The app has simple authentication (login/password). Only logged-in users can do any work with tasks.
- Registration is open to any logged-out visitor (see the warning in section 2).

## 🔄 4. User Flows

### 🔐 Flow 1 - Register/Login
1. 🛂 The user registers/logs into the app with a login and password

### ✨ Flow 2 — Creating a Task

> ⚙️ **Technically, creating a task is two separate requests**, but to the user it looks like one continuous action (no visible break on the frontend):

1. 📄 Open the page
2. ➕ Click "Create Task"
3. ✍️ Fill in the minimal fields: **title** (required), **dueDate** (required), description (optional). `createdBy` is set automatically from the token
4. 💾 Save → `POST /tasks` creates the task **with no assignees**
5. 🔔 Immediately (in the same UI flow, no page reload) a step appears prompting "add assignees and subtasks" — the task already exists (it has an `Id`), so `PUT /tasks/{id}/assignees` and `POST /tasks/{id}/subtasks` can be called
6. 📋 See the task in the list

> ⚠️ A task with no assignees yet is not an error or a forbidden state — it's a normal intermediate step. While `assignees` is empty, the frontend can show a "needs assignees" badge (checking `assignees.length === 0` on the field already present in the response — no separate DB flag needed).

### 🔨 Flow 3 — Completing a Task
1. ✅ Mark a subtask as completed
2. 🗓️ Over time, the task's position on the calendar shifts

### 🏁 Flow 4 — Task Completion Outcome
1. 📦 A completed task moves into the completed archive.
2. ❌ An incomplete one — into the overdue list (computed, never physically "moved" in the DB).

### 🔄 Flow 5 — Recreating an Overdue Task
1. ❌ The user sees an overdue task in the archive
2. 🔁 Clicks "Recreate"
3. ✍️ Optionally changes some fields (must specify a new `dueDate`)
4. 💾 Saves — a new task is created, the old one stays in the archive, marked as the source (`recreatedFromTaskId` on the new task points to it)

---

## 📏 5. Business Rules & Constraints

- ✏️ Can a user edit someone else's task?
  - **Yes.** There is no "only the creator can edit" restriction — any logged-in member can edit and delete any task or subtask.
- 🗑️ Can a task be deleted if it has subtasks?
  - Yes, subtasks are deleted via cascade.
- 👥 Can a task be created with no assignees?
  - **Yes, temporarily.** A task is created in the first step with a minimal set of fields (title, due date, description) — with no assignees. They're added in a separate step immediately after creation (see Flow 2 above). To the user this looks like one action; technically it's two requests in a row.
- 👥 What happens to a task if an assignee is removed?
  - Once a task has at least one assignee, they can be removed down to one remaining. Emptying the list entirely via removal is **not allowed**. This restriction applies only to removal, not creation — see the point above.
- 📅 Can a task have no due date?
  - Not at this stage.
- 🔁 Who can recreate an overdue task?
  - Any logged-in user.

---

## 🚧 6. Out of Scope for v1

- 📱 SMS notifications
- 🔑 OAuth
- ⚡ SignalR / Real-Time
- 📅 Tasks without a due date
- 👮 Roles and permissions
- 🎟️ Invite system for registration
- ⏱️ Stored/scheduled Overdue status — computed on the fly (see 3.3)

---

## ❓ 7. Open Questions

- [ ] 💭 Translate files and UI into English
- [ ] 💭 Is an invite barrier needed for registration
- [ ] 💭 `DELETE /users/{id}` — not designed yet, decide when actually needed
