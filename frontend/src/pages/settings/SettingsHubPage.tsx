import { Link } from "react-router-dom";
import { ShieldCheck, Users } from "lucide-react";
import { role } from "@/lib/data";

type HubCard = {
  icon: React.ElementType;
  label: string;
  description: string;
  href: string;
  accent: string;
  adminOnly?: boolean;
};

const cards: HubCard[] = [
  {
    icon: ShieldCheck,
    label: "Roles",
    description: "Define permission sets and control what each role can view, edit, or delete.",
    href: "/settings/roles",
    accent: "bg-lamaSky",
    adminOnly: true,
  },
  {
    icon: Users,
    label: "Staff Members",
    description: "Manage staff accounts, assign roles, and grant extra permissions.",
    href: "/settings/staff",
    accent: "bg-lamaPurple",
    adminOnly: true,
  },
];

export default function SettingsHubPage() {
  const isAdmin = role === "admin";

  const visible = cards.filter((c) => !c.adminOnly || isAdmin);

  return (
    <div className="p-6 flex flex-col gap-6">
      {/* Header */}
      <div className="flex flex-col gap-1">
        <h1 className="text-2xl font-bold text-gray-800 tracking-tight">Settings</h1>
        <p className="text-sm text-gray-400">
          Configure roles, staff access, and other system preferences.
        </p>
      </div>

      {/* Divider */}
      <div className="h-px bg-gray-100" />

      {/* Cards */}
      {visible.length === 0 ? (
        <p className="text-sm text-gray-400">You don't have access to any settings sections.</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {visible.map((card) => {
            const Icon = card.icon;
            return (
              <Link
                key={card.label}
                to={card.href}
                className="group relative flex items-stretch bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md hover:border-lamaSky transition-all duration-200 overflow-hidden"
              >
                {/* Left accent stripe */}
                <div
                  className={`w-1 flex-shrink-0 ${card.accent} opacity-70 group-hover:opacity-100 transition-opacity`}
                />

                {/* Card body */}
                <div className="flex items-center gap-4 px-5 py-5 flex-1">
                  {/* Icon container */}
                  <div className="flex-shrink-0 w-11 h-11 rounded-lg bg-gray-50 border border-gray-100 flex items-center justify-center group-hover:border-lamaSky transition-colors">
                    <Icon size={20} className="text-gray-400 group-hover:text-lamaSky transition-colors" />
                  </div>

                  {/* Text */}
                  <div className="flex flex-col gap-0.5">
                    <span className="font-semibold text-gray-800 text-sm group-hover:text-lamaSky transition-colors">
                      {card.label}
                    </span>
                    <span className="text-xs text-gray-400 leading-relaxed">
                      {card.description}
                    </span>
                  </div>

                  {/* Arrow */}
                  <div className="ml-auto text-gray-300 group-hover:text-lamaSky group-hover:translate-x-0.5 transition-all text-lg leading-none">
                    →
                  </div>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
