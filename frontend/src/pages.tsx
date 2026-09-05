import { type FormEvent, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AnimatePresence, motion } from "framer-motion";
import { Check, Clock3, Eye, Pencil, Plus } from "lucide-react";
import { AuthShell, Avatar, AvatarGroup, Button, Card, Input, PageHeader } from "./components";
import { getApiErrorMessage } from "./api/client";
import { useLoginMutation, useRegisterMutation, useTaskStatusMutation, useTasksQuery, useUsersQuery, useUserStatsQuery } from "./api/hooks";
import { cn, dueLabel, pageMotion } from "./lib";
import type { Task, TaskStatus } from "./types";
import { RecreateTaskModal } from "./task-modals";
import { TaskDetailsModal } from "./task-details";

const labels: Record<TaskStatus, string> = { ToDo: "К выполнению", InProgress: "В работе", Done: "Готово" };
type DetailTarget = { id: string; edit: boolean } | null;
function StatusDot({ status }: { status: TaskStatus }) { return <span className={cn("h-2 w-2 rounded-full", status === "Done" ? "bg-emerald-500" : status === "InProgress" ? "bg-amber-500" : "bg-stone-300")} />; }
function State({ pending, error, children }: { pending: boolean; error: boolean; children: React.ReactNode }) { return pending ? <p className="text-sm text-stone-500">Загрузка...</p> : error ? <Card className="p-8 text-center text-sm text-red-700">Ошибка загрузки. Попробуйте обновить страницу.</Card> : <>{children}</>; }

function TaskRow({ task, onRecreate, onDetails, onEdit }: { task: Task; onRecreate: (task: Task) => void; onDetails: (id: string) => void; onEdit: (id: string) => void }) {
  const mutation = useTaskStatusMutation(); const completed = task.subtasks?.filter(s => s.status === "Done").length ?? 0;
  const action = task.isOverdue ? { label: "Пересоздать", run: () => onRecreate(task) } : task.status === "ToDo" ? { label: "Взяться за работу", run: () => mutation.mutate({ id: task.id, status: "InProgress" }) } : task.status === "InProgress" ? { label: "Выполнить", run: () => mutation.mutate({ id: task.id, status: "Done" }) } : null;
  return <motion.div layout initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0, scale: 0.98 }} transition={{ duration: 0.22 }}><Card className="px-4 py-3.5 hover:border-stone-300"><div className="flex flex-col gap-3 sm:flex-row sm:items-start"><div className="min-w-0 flex-1"><div className="flex flex-wrap items-center gap-2"><h3 className={cn("break-words font-medium", task.status === "Done" && "text-stone-400 line-through")}>{task.title ?? "Без названия"}</h3>{task.isOverdue && <span className="rounded bg-red-50 px-1.5 py-0.5 text-xs font-medium text-red-700">Просрочено</span>}{task.subtasks?.length ? <span className="rounded bg-stone-100 px-1.5 py-0.5 text-xs text-stone-600">Есть подзадачи</span> : null}</div>{task.description && <p className="mt-1 break-words text-sm text-stone-500">{task.description}</p>}<div className="mt-3 flex flex-wrap items-center gap-x-4 gap-y-2"><span className="inline-flex items-center gap-2 text-xs text-stone-500"><StatusDot status={task.status} />{labels[task.status]}</span><span className={cn("text-xs", task.isOverdue ? "text-red-600" : "text-stone-500")}><Clock3 className="mr-1 inline" size={13} />{dueLabel(task.dueDate)}</span>{task.subtasks?.length ? <span className="text-xs text-stone-500">{completed}/{task.subtasks.length} подзадач</span> : null}
  <AvatarGroup users={task.assignees ?? []} /></div></div><div className="flex w-full flex-wrap items-center gap-2 sm:w-auto sm:flex-nowrap"><button aria-label="Подробнее" onClick={() => onDetails(task.id)} className="rounded-lg border border-stone-300 p-1.5 text-stone-600 hover:bg-stone-100"><Eye size={16} /></button><button aria-label="Редактировать" onClick={() => onEdit(task.id)} className="rounded-lg border border-stone-300 p-1.5 text-stone-600 hover:bg-stone-100"><Pencil size={16} /></button>{action ? <motion.button whileTap={{ scale: 0.96 }} transition={{ duration: 0.12 }} disabled={mutation.isPending} onClick={action.run} className="rounded-lg border border-stone-300 px-3 py-1.5 text-xs font-medium text-stone-700 hover:bg-stone-100 disabled:opacity-50">{mutation.isPending ? "Сохраняем..." : action.label}</motion.button> : <span className="inline-flex items-center gap-1.5 text-xs text-emerald-700"><Check size={14} />Выполнено</span>}</div></div></Card></motion.div>;
}
function Section({ title, tasks, onRecreate, onDetails, onEdit }: { title: string; tasks: Task[]; onRecreate: (task: Task) => void; onDetails: (id: string) => void; onEdit: (id: string) => void }) { return <section><div className="mb-3 flex items-center gap-2"><h2 className="text-sm font-medium text-stone-700">{title}</h2><span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-500">{tasks.length}</span></div><div className="space-y-2"><AnimatePresence initial={false}>{tasks.map(task => <TaskRow key={task.id} task={task} onRecreate={onRecreate} onDetails={onDetails} onEdit={onEdit} />)}</AnimatePresence></div></section>; }

function AuthPage({ register }: { register?: boolean }) { const navigate = useNavigate(); const mutation = register ? useRegisterMutation() : useLoginMutation(); const [username, setUsername] = useState(""); const [password, setPassword] = useState(""); const submit = async (event: FormEvent) => { event.preventDefault(); try { await mutation.mutateAsync({ username, password }); navigate("/today", { replace: true }); } catch {} }; return <AuthShell title={register ? "Создать аккаунт" : "Войдите в аккаунт"} footer={register ? <>Уже есть аккаунт? <Link className="font-medium text-stone-900 underline underline-offset-4" to="/login">Войти</Link></> : <>Нет аккаунта? <Link className="font-medium text-stone-900 underline underline-offset-4" to="/register">Зарегистрироваться</Link></>}><form onSubmit={submit} className="space-y-4"><label className="block text-sm font-medium">Логин<Input value={username} onChange={e => setUsername(e.target.value)} className="mt-1.5" required /></label><label className="block text-sm font-medium">Пароль<Input value={password} onChange={e => setPassword(e.target.value)} className="mt-1.5" type="password" required /></label>{mutation.isError && <p className="text-sm text-red-700">{getApiErrorMessage(mutation.error)}</p>}<Button className="w-full" disabled={mutation.isPending}>{mutation.isPending ? "Подождите..." : register ? "Создать аккаунт" : "Войти"}</Button></form></AuthShell>; }
export const LoginPage = () => <AuthPage />;
export const RegisterPage = () => <AuthPage register />;

function TaskModals({ detail, setDetail, recreate, setRecreate }: { detail: DetailTarget; setDetail: (value: DetailTarget) => void; recreate: Task | null; setRecreate: (value: Task | null) => void }) { return <><RecreateTaskModal task={recreate} onClose={() => setRecreate(null)} /><TaskDetailsModal taskId={detail?.id ?? null} editInitially={detail?.edit} onClose={() => setDetail(null)} /></>; }
export function TodayPage() { const query = useTasksQuery(); const [recreate, setRecreate] = useState<Task | null>(null); const [detail, setDetail] = useState<DetailTarget>(null); const tasks = query.data ?? []; const todo = tasks.filter(t => !t.isOverdue && t.status === "ToDo"); const progress = tasks.filter(t => !t.isOverdue && t.status === "InProgress"); const overdue = tasks.filter(t => t.isOverdue); const openCreate = () => window.dispatchEvent(new Event("open-new-task")); const common = { onRecreate: setRecreate, onDetails: (id: string) => setDetail({ id, edit: false }), onEdit: (id: string) => setDetail({ id, edit: true }) }; return <motion.div {...pageMotion}><PageHeader title="Сегодня" description={new Intl.DateTimeFormat("ru-RU", { weekday: "long", day: "numeric", month: "long" }).format(new Date())} action={<Button onClick={openCreate} icon={<Plus size={16} />}>Новая задача</Button>} /><State pending={query.isPending} error={query.isError}><div className="space-y-8">{todo.length ? <Section title="К работе" tasks={todo} {...common} /> : null}{progress.length ? <Section title="В работе" tasks={progress} {...common} /> : null}{overdue.length ? <Section title="Просроченные" tasks={overdue} {...common} /> : null}{!todo.length && !progress.length && !overdue.length ? <Card className="p-5 text-sm text-stone-500">Задач пока нет.</Card> : null}</div></State><TaskModals detail={detail} setDetail={setDetail} recreate={recreate} setRecreate={setRecreate} /></motion.div>; }
export function TasksPage() { const [filter, setFilter] = useState<"all" | "active" | "done" | "overdue">("all"); const [recreate, setRecreate] = useState<Task | null>(null); const [detail, setDetail] = useState<DetailTarget>(null); const query = useTasksQuery(); const items = useMemo(() => (query.data ?? []).filter(t => filter === "active" ? t.status !== "Done" && !t.isOverdue : filter === "done" ? t.status === "Done" : filter === "overdue" ? t.isOverdue : true), [query.data, filter]); const tabs: Array<[typeof filter, string]> = [["all", "Все"], ["active", "Активные"], ["overdue", "Просроченные"], ["done", "Выполненные"]]; const common = { onRecreate: setRecreate, onDetails: (id: string) => setDetail({ id, edit: false }), onEdit: (id: string) => setDetail({ id, edit: true }) }; return <motion.div {...pageMotion}><PageHeader title="Все задачи" description="Общий список задач команды" action={<Button onClick={() => window.dispatchEvent(new Event("open-new-task"))} icon={<Plus size={16} />}>Новая задача</Button>} /><div className="mb-6 flex gap-5 border-b border-stone-200">{tabs.map(([key, label]) => <button key={key} onClick={() => setFilter(key)} className={cn("border-b-2 px-1 pb-2 text-sm", filter === key ? "border-stone-900 font-medium" : "border-transparent text-stone-500")}>{label}</button>)}</div><State pending={query.isPending} error={query.isError}>{items.length ? <Section title="Задачи" tasks={items} {...common} /> : <Card className="p-10 text-center text-sm text-stone-500">В этой категории пока нет задач.</Card>}</State><TaskModals detail={detail} setDetail={setDetail} recreate={recreate} setRecreate={setRecreate} /></motion.div>; }
function TeamMember({ user, tasksByAssignee }: { user: import("./types").User; tasksByAssignee: Map<string, Task[]> }) {
  const stats = useUserStatsQuery(user.id);
  const assigned = tasksByAssignee.get(user.id) ?? [];
  const active = `${assigned.filter(t => t.status !== "Done" && !t.isOverdue).length} активн.`;
  const completed = stats.isPending ? "считаем статистику..." : `${stats.data?.completedTasks ?? 0} выполнено`;
  return (
    <div className="flex flex-col gap-3 px-5 py-4 md:flex-row md:items-center md:gap-4">
      <div className="flex min-w-0 items-center gap-4"><Avatar user={user} size="lg" /><p className="truncate font-medium">{user.username}</p></div>
      <div className="grid grid-cols-2 gap-2 text-sm text-stone-500 md:ml-auto md:flex md:items-center md:gap-4">
        <p>{active} · {completed}</p>
        <p className="md:text-right">Назначено: <b className="text-stone-900">{assigned.length}</b><br />Пропущено: <b className="text-stone-900">{stats.data?.missedTasks ?? 0}</b></p>
      </div>
    </div>
  );
}

export function TeamPage() {
  const users = useUsersQuery();
  const tasksQuery = useTasksQuery();
  const tasksByAssignee = useMemo(() => {
    const map = new Map<string, Task[]>();
    for (const task of tasksQuery.data ?? []) {
      for (const assignee of task.assignees ?? []) {
        if (!map.has(assignee.id)) map.set(assignee.id, []);
        map.get(assignee.id)!.push(task);
      }
    }
    return map;
  }, [tasksQuery.data]);

  return (
    <motion.div {...pageMotion}>
      <PageHeader title="Команда" description="Участники и их текущая нагрузка" />
      <State pending={users.isPending} error={users.isError}>
        <div className="divide-y divide-stone-100 rounded-xl border border-stone-200 bg-white">
          {users.data?.map(user => <TeamMember key={user.id} user={user} tasksByAssignee={tasksByAssignee} />)}
        </div>
      </State>
    </motion.div>
  );
}