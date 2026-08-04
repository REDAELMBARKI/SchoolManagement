import { useState, useRef, useEffect } from "react";
import {
  Plus,
  X,
  Save,
  Clock,
  ChevronDown,
  Pencil,
  Trash2,
  AlertCircle,
  CheckCircle2,
} from "lucide-react";

// ─── Constants ────────────────────────────────────────────────────────────────
const PIXELS_PER_MINUTE = 2.4; // ruler + boxes use the same constant
const DAY_START = 8 * 60;      // 08:00 in minutes
const DAY_END   = 18 * 60;     // 18:00 in minutes

const DAYS = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

const DURATION_OPTIONS = [
  { label: "30 min", value: 30 },
  { label: "1 h",    value: 60 },
  { label: "1 h 30", value: 90 },
  { label: "2 h",    value: 120 },
];

// ─── Mock lookup data (replace with real API calls when backend is ready) ─────
const MOCK_GROUPS = [
  { id: "g1", name: "Group A – Math L2" },
  { id: "g2", name: "Group B – English L1" },
  { id: "g3", name: "Group C – Science L3" },
];

const MOCK_SUBJECTS = [
  { id: "s1", name: "Mathematics" },
  { id: "s2", name: "English" },
  { id: "s3", name: "Science" },
  { id: "s4", name: "History" },
  { id: "s5", name: "Geography" },
];

const MOCK_TEACHERS = [
  { id: "t1", name: "Mr. Amine" },
  { id: "t2", name: "Ms. Sara" },
  { id: "t3", name: "Mr. Karim" },
  { id: "t4", name: "Ms. Nadia" },
];

const MOCK_ROOMS = [
  { id: "r1", name: "Room 101" },
  { id: "r2", name: "Room 102" },
  { id: "r3", name: "Room A" },
  { id: "r4", name: "Lab 1" },
];

// ─── Types ────────────────────────────────────────────────────────────────────
type Session = {
  scheduleId: string;
  timeSlotId: string;
  startTime: string; // "HH:MM:SS"
  endTime: string;
  durationMinutes: number;
  room:    { id: string; name: string };
  teacher: { id: string; name: string };
  subject: { id: string; name: string };
};

type DayPlan = {
  dayId: string;
  dayName: string;
  sessions: Session[];
};

type TimePlan = {
  groupId: string;
  days: DayPlan[];
};

// ─── Helpers ─────────────────────────────────────────────────────────────────
const toMinutes = (t: string) => {
  const [h, m] = t.split(":").map(Number);
  return h * 60 + m;
};

const toTimeStr = (m: number) => {
  const h = Math.floor(m / 60).toString().padStart(2, "0");
  const min = (m % 60).toString().padStart(2, "0");
  return `${h}:${min}:00`;
};

const formatDisplay = (t: string) => t.slice(0, 5); // "08:00"

/** Rechain all sessions in a day starting from DAY_START. */
const rechain = (sessions: Session[]): Session[] => {
  let cursor = DAY_START;
  return sessions.map((s) => {
    const start = cursor;
    const end   = start + s.durationMinutes;
    cursor = end;
    return {
      ...s,
      startTime: toTimeStr(start),
      endTime:   toTimeStr(end),
    };
  });
};

const uid = () => Math.random().toString(36).slice(2);

// ─── Ruler ───────────────────────────────────────────────────────────────────
const TimeRuler = () => {
  const ticks = [];
  for (let m = DAY_START; m <= DAY_END; m += 30) {
    const left = (m - DAY_START) * PIXELS_PER_MINUTE;
    const isHour = m % 60 === 0;
    const h = Math.floor(m / 60).toString().padStart(2, "0");
    const min = (m % 60).toString().padStart(2, "0");
    ticks.push(
      <div key={m} className="absolute flex flex-col items-start" style={{ left }}>
        <span
          className={`text-[10px] leading-none select-none whitespace-nowrap ${
            isHour ? "text-gray-500 font-semibold" : "text-gray-400"
          }`}
        >
          {h}:{min}
        </span>
        <div className={`mt-1 ${isHour ? "h-3 w-px bg-gray-300" : "h-2 w-px bg-gray-200"}`} />
      </div>
    );
  }
  return (
    <div
      className="relative h-8 flex-shrink-0 border-b border-gray-100"
      style={{ width: (DAY_END - DAY_START) * PIXELS_PER_MINUTE }}
    >
      {ticks}
    </div>
  );
};

// ─── Session Block ────────────────────────────────────────────────────────────
const BLOCK_COLORS = [
  "bg-lamaSkyLight border-lamaSky text-sky-800",
  "bg-lamaPurpleLight border-lamaPurple text-purple-800",
  "bg-lamaYellowLight border-lamaYellow text-yellow-800",
  "bg-green-50 border-green-300 text-green-800",
  "bg-red-50 border-red-300 text-red-800",
  "bg-orange-50 border-orange-300 text-orange-800",
];

const colorFor = (subjectId: string) => {
  const idx = MOCK_SUBJECTS.findIndex((s) => s.id === subjectId);
  return BLOCK_COLORS[(idx >= 0 ? idx : 0) % BLOCK_COLORS.length];
};

type SessionBlockProps = {
  session: Session;
  onClick: () => void;
};

const SessionBlock = ({ session, onClick }: SessionBlockProps) => {
  const left  = (toMinutes(session.startTime) - DAY_START) * PIXELS_PER_MINUTE;
  const width = session.durationMinutes * PIXELS_PER_MINUTE - 2;
  const color = colorFor(session.subject.id);

  return (
    <div
      className={`absolute top-1 bottom-1 rounded border cursor-pointer select-none overflow-hidden
        hover:brightness-95 transition-all ${color}`}
      style={{ left, width }}
      onClick={onClick}
      title={`${session.subject.name} · ${session.teacher.name} · ${session.room.name}`}
    >
      <div className="px-2 py-1 h-full flex flex-col justify-center">
        <span className="text-[11px] font-semibold leading-tight truncate">{session.subject.name}</span>
        <span className="text-[10px] leading-tight truncate opacity-70">{session.teacher.name}</span>
        <span className="text-[10px] leading-tight truncate opacity-60">
          {formatDisplay(session.startTime)} – {formatDisplay(session.endTime)}
        </span>
      </div>
    </div>
  );
};

// ─── Popover ─────────────────────────────────────────────────────────────────
type PopoverProps = {
  mode: "add" | "edit";
  dayName: string;
  startTime: string; // computed, read-only
  groupId: string;
  initial?: {
    duration: number;
    subjectId: string;
    teacherId: string;
    roomId: string;
  };
  onConfirm: (data: { duration: number; subjectId: string; teacherId: string; roomId: string }) => string | null;
  onDelete?: () => void;
  onClose: () => void;
};

const Popover = ({
  mode, dayName, startTime, groupId, initial, onConfirm, onDelete, onClose,
}: PopoverProps) => {
  const [duration,  setDuration]  = useState(initial?.duration  ?? 60);
  const [subjectId, setSubjectId] = useState(initial?.subjectId ?? "");
  const [teacherId, setTeacherId] = useState(initial?.teacherId ?? "");
  const [roomId,    setRoomId]    = useState(initial?.roomId    ?? "");
  const [error, setError] = useState<string | null>(null);

  const groupName = MOCK_GROUPS.find((g) => g.id === groupId)?.name ?? groupId;
  const canConfirm = subjectId && teacherId && roomId;

  const handleConfirm = () => {
    const err = onConfirm({ duration, subjectId, teacherId, roomId });
    if (err) setError(err);
  };

  const selectCls = "ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full focus:outline-none focus:ring-lamaSky bg-white";

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30" onClick={onClose}>
      <div
        className="bg-white rounded-xl shadow-2xl w-[340px] p-6 flex flex-col gap-4"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-start justify-between">
          <div>
            <h2 className="text-sm font-semibold text-gray-700">
              {mode === "add" ? "New block" : "Edit block"}
            </h2>
            <p className="text-xs text-gray-400 mt-0.5">
              {dayName} · starts at {formatDisplay(startTime)}
            </p>
          </div>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X size={16} />
          </button>
        </div>

        {/* Duration */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs text-gray-500">Duration</label>
          <div className="flex gap-1.5">
            {DURATION_OPTIONS.map((d) => (
              <button
                key={d.value}
                onClick={() => setDuration(d.value)}
                className={`flex-1 text-[11px] py-1.5 rounded-md border transition-colors
                  ${duration === d.value
                    ? "bg-lamaSky border-lamaSky text-sky-800 font-semibold"
                    : "border-gray-200 text-gray-500 hover:bg-lamaSkyLight"}`}
              >
                {d.label}
              </button>
            ))}
          </div>
        </div>

        {/* Subject */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs text-gray-500">Subject</label>
          <select className={selectCls} value={subjectId} onChange={(e) => setSubjectId(e.target.value)}>
            <option value="">Select subject…</option>
            {MOCK_SUBJECTS.map((s) => <option key={s.id} value={s.id}>{s.name}</option>)}
          </select>
        </div>

        {/* Teacher */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs text-gray-500">Teacher</label>
          <select className={selectCls} value={teacherId} onChange={(e) => setTeacherId(e.target.value)}>
            <option value="">Select teacher…</option>
            {MOCK_TEACHERS.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        </div>

        {/* Room */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs text-gray-500">Room</label>
          <select className={selectCls} value={roomId} onChange={(e) => setRoomId(e.target.value)}>
            <option value="">Select room…</option>
            {MOCK_ROOMS.map((r) => <option key={r.id} value={r.id}>{r.name}</option>)}
          </select>
        </div>

        {/* Group (locked) */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs text-gray-500">Group</label>
          <div className="ring-[1.5px] ring-gray-200 bg-gray-50 p-2 rounded-md text-sm text-gray-400 truncate">
            {groupName}
          </div>
        </div>

        {/* Error */}
        {error && (
          <div className="flex items-center gap-2 text-xs text-red-500 bg-red-50 px-3 py-2 rounded-md">
            <AlertCircle size={13} /> {error}
          </div>
        )}

        {/* Actions */}
        <div className="flex items-center gap-2 pt-1">
          {mode === "edit" && onDelete && (
            <button
              onClick={onDelete}
              className="flex items-center gap-1 text-xs text-red-500 hover:text-red-700 hover:bg-red-50 px-3 py-2 rounded-md transition-colors"
            >
              <Trash2 size={13} /> Delete
            </button>
          )}
          <button
            onClick={handleConfirm}
            disabled={!canConfirm}
            className={`ml-auto flex items-center gap-1.5 px-4 py-2 rounded-md text-xs font-medium transition-colors
              ${canConfirm
                ? "bg-lamaSky text-sky-900 hover:brightness-95"
                : "bg-gray-100 text-gray-400 cursor-not-allowed"}`}
          >
            <CheckCircle2 size={13} />
            {mode === "add" ? "Add block" : "Save changes"}
          </button>
        </div>
      </div>
    </div>
  );
};

// ─── Day Row ──────────────────────────────────────────────────────────────────
type DayRowProps = {
  day: DayPlan;
  onSessionClick: (session: Session) => void;
  onAddClick: (dayId: string, startTime: string) => void;
};

const DayRow = ({ day, onSessionClick, onAddClick }: DayRowProps) => {
  const lastEnd = day.sessions.length
    ? toMinutes(day.sessions[day.sessions.length - 1].endTime)
    : DAY_START;
  const addLeft = (lastEnd - DAY_START) * PIXELS_PER_MINUTE;
  const rowWidth = (DAY_END - DAY_START) * PIXELS_PER_MINUTE;

  return (
    <div className="flex items-stretch border-b border-gray-100 last:border-0">
      {/* Day label */}
      <div className="w-24 flex-shrink-0 flex items-center pr-3">
        <span className="text-xs font-medium text-gray-500 select-none">{day.dayName}</span>
      </div>

      {/* Session area */}
      <div
        className="relative flex-shrink-0 h-14"
        style={{ width: rowWidth }}
      >
        {/* Background grid lines */}
        {Array.from({ length: (DAY_END - DAY_START) / 30 }).map((_, i) => (
          <div
            key={i}
            className={`absolute top-0 bottom-0 border-l ${i % 2 === 0 ? "border-gray-100" : "border-gray-50"}`}
            style={{ left: i * 30 * PIXELS_PER_MINUTE }}
          />
        ))}

        {/* Sessions */}
        {day.sessions.map((s) => (
          <SessionBlock key={s.scheduleId} session={s} onClick={() => onSessionClick(s)} />
        ))}

        {/* Add button */}
        {lastEnd < DAY_END && (
          <button
            onClick={() => onAddClick(day.dayId, toTimeStr(lastEnd))}
            className="absolute top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-lamaYellow hover:bg-yellow-300
              flex items-center justify-center shadow-sm transition-colors z-10"
            style={{ left: addLeft + 4 }}
            title={`Add session at ${formatDisplay(toTimeStr(lastEnd))}`}
          >
            <Plus size={12} />
          </button>
        )}
      </div>
    </div>
  );
};

// ─── JSON Preview Panel ───────────────────────────────────────────────────────
const JsonPanel = ({ title, data }: { title: string; data: unknown }) => (
  <div className="flex flex-col gap-2">
    <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wide">{title}</h3>
    <pre className="bg-gray-900 text-green-400 text-[11px] rounded-lg p-4 overflow-auto max-h-56 leading-relaxed">
      {JSON.stringify(data, null, 2)}
    </pre>
  </div>
);

// ─── Main Page ────────────────────────────────────────────────────────────────
const ScheduleBuilderPage = () => {
  const [selectedGroupId, setSelectedGroupId] = useState("");
  const [timePlan, setTimePlan] = useState<TimePlan | null>(null);
  const [loading, setLoading] = useState(false);
  const [saved, setSaved] = useState(false);

  // Popover state
  type PopoverState = {
    mode: "add" | "edit";
    dayId: string;
    dayName: string;
    startTime: string;
    sessionIdx?: number;
  };
  const [popover, setPopover] = useState<PopoverState | null>(null);

  // ── Load timeplan when group is selected ────────────────────────────────────
  const loadGroup = async (groupId: string) => {
    setLoading(true);
    setSaved(false);
    // Simulate API call — replace with: fetch(`/api/groups/${groupId}/timeplan`)
    await new Promise((r) => setTimeout(r, 400));
    const plan: TimePlan = {
      groupId,
      days: DAYS.map((name, i) => ({
        dayId: `day-${i + 1}`,
        dayName: name,
        sessions: [],
      })),
    };
    setTimePlan(plan);
    setLoading(false);
  };

  const handleGroupChange = (id: string) => {
    setSelectedGroupId(id);
    if (id) loadGroup(id);
    else setTimePlan(null);
  };

  // ── Open popover for add ─────────────────────────────────────────────────
  const handleAddClick = (dayId: string, startTime: string) => {
    const day = timePlan?.days.find((d) => d.dayId === dayId);
    if (!day) return;
    setPopover({ mode: "add", dayId, dayName: day.dayName, startTime });
  };

  // ── Open popover for edit ────────────────────────────────────────────────
  const handleSessionClick = (dayId: string, session: Session) => {
    const day = timePlan?.days.find((d) => d.dayId === dayId);
    if (!day) return;
    const idx = day.sessions.findIndex((s) => s.scheduleId === session.scheduleId);
    setPopover({
      mode: "edit",
      dayId,
      dayName: day.dayName,
      startTime: session.startTime,
      sessionIdx: idx,
    });
  };

  // ── Confirm from popover ─────────────────────────────────────────────────
  const handleConfirm = (data: {
    duration: number; subjectId: string; teacherId: string; roomId: string;
  }): string | null => {
    if (!timePlan || !popover) return null;

    const subject = MOCK_SUBJECTS.find((s) => s.id === data.subjectId)!;
    const teacher = MOCK_TEACHERS.find((t) => t.id === data.teacherId)!;
    const room    = MOCK_ROOMS.find((r)    => r.id === data.roomId)!;

    setTimePlan((prev) => {
      if (!prev) return prev;
      return {
        ...prev,
        days: prev.days.map((day) => {
          if (day.dayId !== popover.dayId) return day;

          let sessions = [...day.sessions];

          if (popover.mode === "add") {
            sessions.push({
              scheduleId: uid(),
              timeSlotId: uid(),
              startTime: popover.startTime,
              endTime: popover.startTime,
              durationMinutes: data.duration,
              room, teacher, subject,
            });
          } else if (popover.mode === "edit" && popover.sessionIdx !== undefined) {
            sessions[popover.sessionIdx] = {
              ...sessions[popover.sessionIdx],
              durationMinutes: data.duration,
              room, teacher, subject,
            };
          }

          const rechained = rechain(sessions);

          // Overflow check
          if (rechained.length > 0) {
            const lastEnd = toMinutes(rechained[rechained.length - 1].endTime);
            if (lastEnd > DAY_END) {
              return day; // will be caught below via error return
            }
          }

          return { ...day, sessions: rechained };
        }),
      };
    });

    // Check for overflow BEFORE committing (compute inline to validate)
    const day = timePlan.days.find((d) => d.dayId === popover.dayId)!;
    let sessions = [...day.sessions];
    if (popover.mode === "add") {
      sessions.push({ scheduleId: "x", timeSlotId: "x", startTime: popover.startTime, endTime: popover.startTime, durationMinutes: data.duration, room, teacher, subject });
    } else if (popover.mode === "edit" && popover.sessionIdx !== undefined) {
      sessions[popover.sessionIdx] = { ...sessions[popover.sessionIdx], durationMinutes: data.duration, room, teacher, subject };
    }
    const rechained = rechain(sessions);
    const lastEnd = rechained.length > 0 ? toMinutes(rechained[rechained.length - 1].endTime) : DAY_START;
    if (lastEnd > DAY_END) {
      return "This schedule would push past 18:00. Reduce duration or remove earlier sessions.";
    }

    setPopover(null);
    setSaved(false);
    return null;
  };

  // ── Delete session ───────────────────────────────────────────────────────
  const handleDelete = () => {
    if (!timePlan || !popover || popover.sessionIdx === undefined) return;
    setTimePlan((prev) => {
      if (!prev) return prev;
      return {
        ...prev,
        days: prev.days.map((day) => {
          if (day.dayId !== popover.dayId) return day;
          const sessions = day.sessions.filter((_, i) => i !== popover.sessionIdx);
          return { ...day, sessions: rechain(sessions) };
        }),
      };
    });
    setPopover(null);
    setSaved(false);
  };

  // ── Save (POST payload) ──────────────────────────────────────────────────
  const buildPayload = () => {
    if (!timePlan) return null;
    return {
      branchId: "branch-1", // replace with real branchId from context
      schedules: timePlan.days.flatMap((day) =>
        day.sessions.map((s) => ({
          dayId:     day.dayId,
          roomId:    s.room.id,
          teacherId: s.teacher.id,
          subjectId: s.subject.id,
          groupId:   timePlan.groupId,
          startTime: s.startTime,
          endTime:   s.endTime,
        }))
      ),
    };
  };

  const handleSave = async () => {
    const payload = buildPayload();
    if (!payload) return;
    // Replace with: await fetch('/api/schedules', { method: 'POST', body: JSON.stringify(payload), ... })
    console.log("POST payload:", payload);
    setSaved(true);
    setTimeout(() => setSaved(false), 3000);
  };

  const popoverDay  = timePlan?.days.find((d) => d.dayId === popover?.dayId);
  const popoverSession = popover?.sessionIdx !== undefined ? popoverDay?.sessions[popover.sessionIdx] : undefined;

  const totalWidth = (DAY_END - DAY_START) * PIXELS_PER_MINUTE;

  return (
    <div className="flex flex-col gap-6 m-4 mt-0">
      {/* ── Page header ──────────────────────────────────────────────────── */}
      <div className="bg-white rounded-md p-4 flex items-center justify-between gap-4">
        <div>
          <h1 className="text-lg font-semibold text-gray-700 flex items-center gap-2">
            <Clock size={18} className="text-gray-400" />
            Schedule Builder
          </h1>
          <p className="text-xs text-gray-400 mt-0.5">
            Build and manage weekly timetables per group
          </p>
        </div>

        <div className="flex items-center gap-3">
          {/* Group selector */}
          <div className="relative">
            <select
              value={selectedGroupId}
              onChange={(e) => handleGroupChange(e.target.value)}
              className="ring-[1.5px] ring-gray-300 p-2 pl-3 pr-8 rounded-md text-sm appearance-none
                focus:outline-none focus:ring-lamaSky bg-white min-w-[200px]"
            >
              <option value="">Select a group…</option>
              {MOCK_GROUPS.map((g) => (
                <option key={g.id} value={g.id}>{g.name}</option>
              ))}
            </select>
            <ChevronDown size={14} className="absolute right-2.5 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none" />
          </div>

          {/* Save button */}
          {timePlan && (
            <button
              onClick={handleSave}
              className={`flex items-center gap-2 px-4 py-2 rounded-md text-sm font-medium transition-all
                ${saved ? "bg-green-100 text-green-700" : "bg-lamaSky text-sky-900 hover:brightness-95"}`}
            >
              {saved ? <CheckCircle2 size={15} /> : <Save size={15} />}
              {saved ? "Saved!" : "Save Schedule"}
            </button>
          )}
        </div>
      </div>

      {/* ── No group selected ────────────────────────────────────────────── */}
      {!selectedGroupId && (
        <div className="bg-white rounded-md p-16 flex flex-col items-center gap-3 text-center">
          <div className="w-14 h-14 rounded-full bg-lamaSkyLight flex items-center justify-center">
            <Clock size={24} className="text-sky-500" />
          </div>
          <p className="text-gray-500 font-medium text-sm">No group selected</p>
          <p className="text-gray-400 text-xs max-w-xs">
            Pick a group from the dropdown above to load and edit its weekly schedule.
          </p>
        </div>
      )}

      {/* ── Loading ──────────────────────────────────────────────────────── */}
      {selectedGroupId && loading && (
        <div className="bg-white rounded-md p-16 flex items-center justify-center">
          <div className="w-7 h-7 border-2 border-lamaSky border-t-transparent rounded-full animate-spin" />
        </div>
      )}

      {/* ── Grid ─────────────────────────────────────────────────────────── */}
      {timePlan && !loading && (
        <div className="bg-white rounded-md p-4 overflow-x-auto">
          <div style={{ minWidth: totalWidth + 100 }}>
            {/* Ruler */}
            <div className="flex">
              <div className="w-24 flex-shrink-0" /> {/* spacer for day labels */}
              <TimeRuler />
            </div>

            {/* Day rows */}
            <div className="flex flex-col">
              {timePlan.days.map((day) => (
                <DayRow
                  key={day.dayId}
                  day={day}
                  onSessionClick={(s) => handleSessionClick(day.dayId, s)}
                  onAddClick={handleAddClick}
                />
              ))}
            </div>
          </div>
        </div>
      )}

      {/* ── Debug JSON panels ─────────────────────────────────────────────── */}
      {timePlan && !loading && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <JsonPanel title="Live nested state" data={timePlan} />
          <JsonPanel title="Flattened POST payload" data={buildPayload()} />
        </div>
      )}

      {/* ── Popover ──────────────────────────────────────────────────────── */}
      {popover && timePlan && (
        <Popover
          mode={popover.mode}
          dayName={popover.dayName}
          startTime={popover.startTime}
          groupId={timePlan.groupId}
          initial={
            popoverSession
              ? {
                  duration:  popoverSession.durationMinutes,
                  subjectId: popoverSession.subject.id,
                  teacherId: popoverSession.teacher.id,
                  roomId:    popoverSession.room.id,
                }
              : undefined
          }
          onConfirm={handleConfirm}
          onDelete={popover.mode === "edit" ? handleDelete : undefined}
          onClose={() => setPopover(null)}
        />
      )}
    </div>
  );
};

export default ScheduleBuilderPage;
