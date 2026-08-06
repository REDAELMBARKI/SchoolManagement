import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { role } from "@/lib/data";

// ─── Quick-action definitions (two actions only) ──────────────────────────────

const QUICK_ACTIONS = [
  {
    label: "New Intake",
    description: "Register an enquiry or prospective student",
    href: "/list/intakes/new",
    icon: "/create.png",          // PNG icon from public/
    bg: "bg-lamaSkyLight",
    border: "border-lamaSky",
    iconBg: "bg-lamaSky",
  },
  {
    label: "New Student",
    description: "Add a student record directly to the system",
    href: "/list/students/new",
    icon: "/student.png",
    bg: "bg-lamaPurpleLight",
    border: "border-lamaPurple",
    iconBg: "bg-lamaPurple",
  },
];

// ─── Skeleton card ────────────────────────────────────────────────────────────

function SkeletonCard() {
  return (
    <div className="flex items-center gap-4 p-4 rounded-xl border border-gray-100 bg-gray-50 animate-pulse">
      <div className="w-12 h-12 rounded-full bg-gray-200 flex-shrink-0" />
      <div className="flex flex-col gap-2 flex-1 min-w-0">
        <div className="h-3.5 w-28 rounded bg-gray-200" />
        <div className="h-2.5 w-44 rounded bg-gray-100" />
      </div>
      <div className="w-6 h-6 rounded-full bg-gray-200 flex-shrink-0" />
    </div>
  );
}

// ─── Centered modal ───────────────────────────────────────────────────────────

function QuickAddPanel({ onClose }: { onClose: () => void }) {
  // Brief skeleton phase so the modal doesn't just pop in
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const t = setTimeout(() => setLoading(false), 520);
    return () => clearTimeout(t);
  }, []);

  // Lock body scroll while the modal is open
  useEffect(() => {
    const prev = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => { document.body.style.overflow = prev; };
  }, []);

  return (
    /* Full-screen backdrop — blurred + scroll-locked */
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm"
      onClick={onClose}
    >
      {/* Modal card */}
      <div
        className="relative bg-white rounded-2xl shadow-2xl w-[90%] max-w-md mx-4 overflow-hidden"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-5 border-b border-gray-100">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-lamaYellow flex items-center justify-center">
              <img src="/create.png" alt="" width={16} height={16} />
            </div>
            <div>
              <h2 className="text-sm font-semibold text-gray-800 leading-none">
                Quick Actions
              </h2>
              <p className="text-[11px] text-gray-400 mt-0.5">
                What would you like to create?
              </p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="w-7 h-7 flex items-center justify-center rounded-full hover:bg-gray-100 transition-colors"
            aria-label="Close"
          >
            <img src="/close.png" alt="close" width={12} height={12} />
          </button>
        </div>

        {/* Body */}
        <div className="px-6 py-5 flex flex-col gap-3">
          {loading ? (
            <>
              <SkeletonCard />
              <SkeletonCard />
            </>
          ) : (
            QUICK_ACTIONS.map((action) => (
              <Link
                key={action.label}
                to={action.href}
                onClick={onClose}
                className={`group flex items-center gap-4 p-4 rounded-xl border ${action.border} ${action.bg}
                  hover:shadow-md transition-all duration-150`}
              >
                {/* Icon bubble */}
                <div
                  className={`w-12 h-12 rounded-full ${action.iconBg} flex items-center justify-center flex-shrink-0`}
                >
                  <img src={action.icon} alt="" width={22} height={22} />
                </div>

                {/* Text */}
                <div className="flex flex-col gap-0.5 min-w-0">
                  <span className="text-sm font-semibold text-gray-800 group-hover:text-gray-900">
                    {action.label}
                  </span>
                  <span className="text-xs text-gray-500 truncate">
                    {action.description}
                  </span>
                </div>

                {/* Arrow */}
                <span className="ml-auto text-gray-300 group-hover:text-gray-500 group-hover:translate-x-1 transition-all text-base select-none">
                  →
                </span>
              </Link>
            ))
          )}
        </div>

        {/* Footer hint */}
        <div className="px-6 pb-4 text-[10px] text-gray-400 text-center">
          You can also access these from the sidebar menu
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
