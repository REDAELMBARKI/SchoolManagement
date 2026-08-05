import { useState, useRef, useEffect } from "react";
import { Link } from "react-router-dom";
import { role } from "@/lib/data";

// ─── Quick-action definitions ─────────────────────────────────────────────────

type Action = {
  label: string;
  description: string;
  href: string;
  icon: string;          // emoji / text icon kept lightweight
  accent: string;        // left-stripe + icon colour (tailwind text/border class)
  accentBg: string;      // icon container bg
  roles: string[];
};

type ActionGroup = {
  label: string;
  actions: Action[];
};

const ACTION_GROUPS: ActionGroup[] = [
  {
    label: "Enrolment",
    actions: [
      {
        label: "New Intake",
        description: "Register an enquiry or prospective student",
        href: "/list/intakes/new",
        icon: "📥",
        accent: "text-sky-400 border-sky-400",
        accentBg: "bg-sky-950",
        roles: ["admin"],
      },
      {
        label: "New Enrolment",
        description: "Enrol a student into a class or programme",
        href: "/list/students/new",
        icon: "🎓",
        accent: "text-emerald-400 border-emerald-400",
        accentBg: "bg-emerald-950",
        roles: ["admin"],
      },
    ],
  },
  {
    label: "People",
    actions: [
      {
        label: "New Student",
        description: "Add a student record to the system",
        href: "/list/students/new",
        icon: "👤",
        accent: "text-purple-400 border-purple-400",
        accentBg: "bg-purple-950",
        roles: ["admin"],
      },
      {
        label: "New Teacher",
        description: "Create a teacher profile and assign classes",
        href: "/list/teachers/new",
        icon: "🧑‍🏫",
        accent: "text-amber-400 border-amber-400",
        accentBg: "bg-amber-950",
        roles: ["admin"],
      },
      {
        label: "New Parent",
        description: "Link a parent or guardian to a student",
        href: "/list/parents/new",
        icon: "👨‍👩‍👧",
        accent: "text-rose-400 border-rose-400",
        accentBg: "bg-rose-950",
        roles: ["admin"],
      },
    ],
  },
  {
    label: "Academics",
    actions: [
      {
        label: "New Exam",
        description: "Schedule an upcoming exam or test",
        href: "/list/exams/new",
        icon: "📝",
        accent: "text-indigo-400 border-indigo-400",
        accentBg: "bg-indigo-950",
        roles: ["admin", "teacher"],
      },
      {
        label: "New Assignment",
        description: "Create a homework or coursework task",
        href: "/list/assignments/new",
        icon: "📋",
        accent: "text-cyan-400 border-cyan-400",
        accentBg: "bg-cyan-950",
        roles: ["admin", "teacher"],
      },
      {
        label: "New Announcement",
        description: "Broadcast a message to staff and students",
        href: "/list/announcements/new",
        icon: "📢",
        accent: "text-orange-400 border-orange-400",
        accentBg: "bg-orange-950",
        roles: ["admin", "teacher"],
      },
    ],
  },
];

// ─── Quick-action panel ───────────────────────────────────────────────────────

function QuickAddPanel({ onClose }: { onClose: () => void }) {
  const userRole = role as string;

  const visibleGroups = ACTION_GROUPS.map((g) => ({
    ...g,
    actions: g.actions.filter((a) => a.roles.includes(userRole)),
  })).filter((g) => g.actions.length > 0);

  return (
    /* Backdrop */
    <div
      className="fixed inset-0 z-40"
      onClick={onClose}
    >
      {/* Panel */}
      <div
        className="absolute right-4 top-[64px] w-[520px] rounded-xl overflow-hidden shadow-2xl border border-white/10"
        style={{ background: "#111113" }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Panel header */}
        <div
          className="flex items-center justify-between px-5 py-4 border-b border-white/10"
          style={{ background: "#18181b" }}
        >
          <div className="flex items-center gap-3">
            <div
              className="w-7 h-7 rounded flex items-center justify-center text-sm font-black"
              style={{ background: "#e8540a", color: "#fff" }}
            >
              +
            </div>
            <span className="text-white font-semibold text-sm tracking-wide">
              Quick Actions
            </span>
          </div>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-white transition-colors text-lg leading-none"
          >
            ×
          </button>
        </div>

        {/* Groups */}
        <div className="p-5 flex flex-col gap-5">
          {visibleGroups.map((group) => (
            <div key={group.label} className="flex flex-col gap-2">
              {/* Group label */}
              <span
                className="text-[10px] font-bold uppercase tracking-[0.12em]"
                style={{ color: "#e8540a" }}
              >
                {group.label}
              </span>

              {/* Action cards */}
              <div className="grid grid-cols-1 gap-1.5">
                {group.actions.map((action) => (
                  <Link
                    key={action.label}
                    to={action.href}
                    onClick={onClose}
                    className="group flex items-center gap-4 px-3 py-3 rounded-lg border border-white/5
                      hover:border-white/20 transition-all duration-150 relative overflow-hidden"
                    style={{ background: "#1c1c1f" }}
                  >
                    {/* Left accent line */}
                    <div
                      className={`absolute left-0 top-0 bottom-0 w-[3px] opacity-0 group-hover:opacity-100 transition-opacity ${action.accent.split(" ")[1] ? "" : ""}`}
                      style={{
                        background: action.accentBg
                          .replace("bg-", "")
                          .includes("sky")   ? "#38bdf8"
                          : action.accentBg.includes("emerald") ? "#34d399"
                          : action.accentBg.includes("purple")  ? "#a78bfa"
                          : action.accentBg.includes("amber")   ? "#fbbf24"
                          : action.accentBg.includes("rose")    ? "#fb7185"
                          : action.accentBg.includes("indigo")  ? "#818cf8"
                          : action.accentBg.includes("cyan")    ? "#22d3ee"
                          : "#fb923c",
                      }}
                    />

                    {/* Icon */}
                    <div
                      className={`w-9 h-9 rounded-lg flex items-center justify-center text-base flex-shrink-0 ${action.accentBg} border border-white/5 group-hover:border-white/15 transition-colors`}
                    >
                      {action.icon}
                    </div>

                    {/* Text */}
                    <div className="flex flex-col gap-0.5 min-w-0">
                      <span className="text-sm font-semibold text-white/90 group-hover:text-white transition-colors">
                        {action.label}
                      </span>
                      <span className="text-xs text-gray-500 group-hover:text-gray-400 transition-colors truncate">
                        {action.description}
                      </span>
                    </div>

                    {/* Arrow */}
                    <span className="ml-auto text-gray-600 group-hover:text-white/60 group-hover:translate-x-0.5 transition-all text-sm">
                      →
                    </span>
                  </Link>
                ))}
              </div>
            </div>
          ))}
        </div>

        {/* Footer */}
        <div
          className="px-5 py-3 border-t border-white/10 text-[11px] text-gray-600"
          style={{ background: "#18181b" }}
        >
          Quick-add shortcuts for the most common school ERP operations.
        </div>
      </div>
    </div>
  );
}

// ─── Navbar ───────────────────────────────────────────────────────────────────

const Navbar = () => {
  const userRole = role;
  const [open, setOpen] = useState(false);

  const today = new Date();
  const dateStr = today.toLocaleDateString("en-US", {
    weekday: "short",
    month: "short",
    day: "numeric",
  });

  const getSeason = (date: Date) => {
    const m = date.getMonth();
    if (m >= 2 && m <= 4) return "Spring";
    if (m >= 5 && m <= 7) return "Summer";
    if (m >= 8 && m <= 10) return "Fall";
    return "Winter";
  };

  return (
    <div className="flex items-center justify-between p-4 relative">
      {/* Left — search + date */}
      <div className="hidden md:flex items-center gap-4">
        <div className="flex items-center gap-2 text-xs rounded-full ring-[1.5px] ring-gray-300 px-2">
          <img src="/search.png" alt="" width={14} height={14} />
          <input
            type="text"
            placeholder="Search..."
            className="w-[200px] p-2 bg-transparent outline-none"
          />
        </div>
        <div className="text-sm text-gray-600">
          <span className="font-medium">{dateStr}</span>
          <span className="ml-2 text-xs text-gray-500">({getSeason(today)})</span>
        </div>
      </div>

      {/* Right */}
      <div className="flex items-center gap-4 justify-end w-full">

        {/* ── ROG-style Add button ─────────────────────────────────────── */}
        <button
          onClick={() => setOpen((v) => !v)}
          className="group relative flex items-center gap-2 px-4 py-2 rounded-lg font-bold text-sm
            overflow-hidden transition-all duration-150 select-none"
          style={{
            background: open ? "#c44208" : "#e8540a",
            color: "#fff",
            boxShadow: open
              ? "0 0 0 2px #e8540a44, 0 2px 12px #e8540a55"
              : "0 2px 8px #e8540a33",
          }}
          onMouseEnter={(e) => {
            if (!open) (e.currentTarget as HTMLButtonElement).style.background = "#d44c09";
          }}
          onMouseLeave={(e) => {
            if (!open) (e.currentTarget as HTMLButtonElement).style.background = "#e8540a";
          }}
        >
          {/* Shine sweep */}
          <div
            className="absolute inset-0 opacity-20 pointer-events-none"
            style={{
              background: "linear-gradient(105deg, transparent 40%, rgba(255,255,255,0.4) 50%, transparent 60%)",
            }}
          />

          {/* Plus icon */}
          <span
            className="w-5 h-5 rounded flex items-center justify-center text-base font-black leading-none flex-shrink-0"
            style={{ background: "rgba(0,0,0,0.25)" }}
          >
            +
          </span>

          <span className="tracking-wide">Add</span>

          {/* Chevron */}
          <span
            className="text-xs transition-transform duration-200"
            style={{ transform: open ? "rotate(180deg)" : "rotate(0deg)", opacity: 0.8 }}
          >
            ▾
          </span>
        </button>

        {/* Messages */}
        <div className="bg-white rounded-full w-7 h-7 flex items-center justify-center cursor-pointer">
          <img src="/message.png" alt="" width={20} height={20} />
        </div>

        {/* Announcements */}
        <div className="bg-white rounded-full w-7 h-7 flex items-center justify-center cursor-pointer relative">
          <img src="/announcement.png" alt="" width={20} height={20} />
          <div className="absolute -top-3 -right-3 w-5 h-5 flex items-center justify-center bg-purple-500 text-white rounded-full text-xs">
            1
          </div>
        </div>

        {/* User */}
        <div className="flex flex-col">
          <span className="text-xs leading-3 font-medium">John Doe</span>
          <span className="text-[10px] text-gray-500 text-right">{userRole}</span>
        </div>
        <img src="/avatar.png" alt="" width={36} height={36} className="rounded-full" />
      </div>

      {/* Quick-add panel */}
      {open && <QuickAddPanel onClose={() => setOpen(false)} />}
    </div>
  );
};

export default Navbar;
