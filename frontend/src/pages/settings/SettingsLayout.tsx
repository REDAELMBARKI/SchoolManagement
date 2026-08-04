import { Link, Outlet, useLocation } from "react-router-dom";
import { ShieldCheck, Users } from "lucide-react";
import { role } from "@/lib/data";

const groups = [
  {
    label: "Staff & Access",
    adminOnly: true,
    items: [
      { label: "Roles",         href: "/settings/roles",   icon: ShieldCheck },
      { label: "Staff Members", href: "/settings/staff",   icon: Users       },
    ],
  },
];

export default function SettingsLayout() {
  const location = useLocation();
  const isAdmin = role === "admin";

  // Default landing — just show the welcome card if at /settings exactly
  const atRoot = location.pathname === "/settings";

  return (
    <div className="flex gap-0 m-4 mt-0 min-h-[calc(100vh-8rem)]">
      {/* ── Left sub-nav ─────────────────────────────────────────────────── */}
      <aside className="w-52 flex-shrink-0 bg-white rounded-l-md border-r border-gray-100 p-4 flex flex-col gap-6">
        <h2 className="text-xs font-semibold text-gray-400 uppercase tracking-wide">Settings</h2>

        {groups.map((group) => {
          if (group.adminOnly && !isAdmin) return null;
          return (
            <div key={group.label} className="flex flex-col gap-1">
              <span className="text-[11px] font-semibold text-gray-400 uppercase tracking-wide px-2 mb-1">
                {group.label}
              </span>
              {group.items.map((item) => {
                const active = location.pathname.startsWith(item.href);
                const Icon = item.icon;
                return (
                  <Link
                    key={item.href}
                    to={item.href}
                    className={`flex items-center gap-2.5 px-3 py-2 rounded-md text-sm transition-colors
                      ${active
                        ? "bg-lamaSkyLight text-sky-800 font-medium"
                        : "text-gray-500 hover:bg-gray-50"}`}
                  >
                    <Icon size={15} className={active ? "text-sky-600" : "text-gray-400"} />
                    {item.label}
                  </Link>
                );
              })}
            </div>
          );
        })}
      </aside>

      {/* ── Right content ─────────────────────────────────────────────────── */}
      <main className="flex-1 bg-white rounded-r-md overflow-hidden">
        {atRoot ? (
          <div className="flex flex-col items-center justify-center h-full gap-3 text-center p-12">
            <div className="w-14 h-14 rounded-full bg-lamaSkyLight flex items-center justify-center">
              <ShieldCheck size={24} className="text-sky-500" />
            </div>
            <p className="text-gray-600 font-medium text-sm">Settings</p>
            <p className="text-gray-400 text-xs max-w-xs">
              {isAdmin
                ? "Choose a section from the left to manage roles and staff access."
                : "You don't have access to any settings sections."}
            </p>
          </div>
        ) : (
          <Outlet />
        )}
      </main>
    </div>
  );
}
