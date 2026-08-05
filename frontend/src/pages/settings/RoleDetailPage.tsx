import { useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import {
  ShieldCheck, ChevronLeft, Pencil, Trash2, AlertCircle, Users,
} from "lucide-react";
import { rolesData, staffData, AREAS, memberCount, type AreaPermissions } from "@/lib/staffAccessData";

// ─── Claim definitions ────────────────────────────────────────────────────────
// Add more entries here and every area card picks them up automatically.

type ClaimKey = keyof AreaPermissions;

const CLAIMS: { key: ClaimKey; label: string; description: string; color: string; bg: string; border: string }[] = [
  {
    key: "view",
    label: "Can view",
    description: "Read-only access — see records but make no changes.",
    color: "text-sky-700",
    bg: "bg-sky-50",
    border: "border-sky-300",
  },
  {
    key: "edit",
    label: "Can edit",
    description: "Create and update records in this area.",
    color: "text-purple-700",
    bg: "bg-purple-50",
    border: "border-purple-300",
  },
  {
    key: "delete",
    label: "Can delete",
    description: "Permanently remove records. Use with caution.",
    color: "text-red-600",
    bg: "bg-red-50",
    border: "border-red-300",
  },
];

// ─── Sub-components ───────────────────────────────────────────────────────────

function ClaimCheckbox({
  checked,
  label,
  description,
  color,
  bg,
  border,
  onChange,
}: {
  checked: boolean;
  label: string;
  description: string;
  color: string;
  bg: string;
  border: string;
  onChange: () => void;
}) {
  return (
    <label
      className={`flex items-start gap-3 p-3 rounded-lg border cursor-pointer select-none transition-all
        ${checked ? `${bg} ${border}` : "bg-white border-gray-100 hover:border-gray-200 hover:bg-gray-50"}`}
    >
      {/* Custom checkbox */}
      <div className="mt-0.5 flex-shrink-0">
        <input type="checkbox" className="sr-only" checked={checked} onChange={onChange} />
        <div
          className={`w-4.5 h-4.5 w-[18px] h-[18px] rounded-[4px] border-2 flex items-center justify-center transition-all
            ${checked ? `${border.replace("border-", "border-")} bg-current` : "border-gray-300 bg-white"}`}
          style={{ borderColor: checked ? undefined : undefined }}
        >
          {checked && (
            <svg width="10" height="8" viewBox="0 0 10 8" fill="none">
              <path d="M1 4L3.5 6.5L9 1" stroke="white" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          )}
        </div>
      </div>

      {/* Text */}
      <div className="flex flex-col gap-0.5 min-w-0">
        <span className={`text-xs font-semibold leading-tight ${checked ? color : "text-gray-500"}`}>
          {label}
        </span>
        <span className="text-[11px] text-gray-400 leading-snug">{description}</span>
      </div>
    </label>
  );
}

function AreaCard({
  area,
  perms,
  onToggle,
}: {
  area: string;
  perms: AreaPermissions;
  onToggle: (key: ClaimKey) => void;
}) {
  const enabledCount = CLAIMS.filter((c) => perms[c.key]).length;

  return (
    <div className="flex flex-col gap-3 bg-white border border-gray-200 rounded-xl p-4 shadow-sm">
      {/* Card header */}
      <div className="flex items-center justify-between">
        <span className="text-sm font-semibold text-gray-700">{area}</span>
        <span
          className={`text-[10px] font-medium px-2 py-0.5 rounded-full
            ${enabledCount > 0
              ? "bg-lamaSkyLight text-sky-700"
              : "bg-gray-100 text-gray-400"}`}
        >
          {enabledCount} / {CLAIMS.length} claims
        </span>
      </div>

      {/* Divider */}
      <div className="h-px bg-gray-100" />

      {/* Claims */}
      <div className="flex flex-col gap-2">
        {CLAIMS.map((claim) => (
          <ClaimCheckbox
            key={claim.key}
            checked={perms[claim.key] ?? false}
            label={claim.label}
            description={claim.description}
            color={claim.color}
            bg={claim.bg}
            border={claim.border}
            onChange={() => onToggle(claim.key)}
          />
        ))}
      </div>
    </div>
  );
}

// ─── Main page ────────────────────────────────────────────────────────────────

type Perms = Record<string, AreaPermissions>;

export default function RoleDetailPage() {
  const { roleId } = useParams<{ roleId: string }>();
  const navigate   = useNavigate();

  const original = rolesData.find((r) => r.id === roleId);
  const [name, setName]           = useState(original?.name ?? "");
  const [perms, setPerms]         = useState<Perms>(
    JSON.parse(JSON.stringify(original?.permissions ?? {}))
  );
  const [editingName, setEditingName]   = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [saved, setSaved]               = useState(false);

  if (!original) {
    return (
      <div className="p-6 flex flex-col gap-3">
        <Link to="/settings/roles" className="text-xs text-sky-600 flex items-center gap-1 hover:underline">
          <ChevronLeft size={13} /> Back to Roles
        </Link>
        <p className="text-sm text-gray-400">Role not found.</p>
      </div>
    );
  }

  const count = memberCount(roleId!);

  const toggle = (area: string, key: ClaimKey) => {
    setPerms((prev) => ({
      ...prev,
      [area]: { ...prev[area], [key]: !prev[area]?.[key] },
    }));
    setSaved(false);
  };

  const handleSave = () => {
    const idx = rolesData.findIndex((r) => r.id === roleId);
    if (idx !== -1) {
      rolesData[idx].name        = name;
      rolesData[idx].permissions = JSON.parse(JSON.stringify(perms));
    }
    setSaved(true);
    setTimeout(() => setSaved(false), 2500);
  };

  const handleDelete = () => {
    const idx = rolesData.findIndex((r) => r.id === roleId);
    if (idx !== -1) rolesData.splice(idx, 1);
    navigate("/settings/roles");
  };

  const members = staffData.filter((s) => s.roleIds.includes(roleId!));

  return (
    <div className="p-6 flex flex-col gap-6 max-w-5xl">
      {/* Back */}
      <Link to="/settings/roles" className="text-xs text-sky-600 flex items-center gap-1 hover:underline w-fit">
        <ChevronLeft size={13} /> Back to Roles
      </Link>

      {/* Title row */}
      <div className="flex items-start justify-between gap-4">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-lamaSkyLight flex items-center justify-center flex-shrink-0">
            <ShieldCheck size={18} className="text-sky-600" />
          </div>
          <div>
            {editingName ? (
              <input
                autoFocus
                value={name}
                onChange={(e) => { setName(e.target.value); setSaved(false); }}
                onBlur={() => setEditingName(false)}
                onKeyDown={(e) => e.key === "Enter" && setEditingName(false)}
                className="text-base font-semibold text-gray-700 border-b border-gray-300 focus:outline-none focus:border-lamaSky bg-transparent"
              />
            ) : (
              <button onClick={() => setEditingName(true)} className="flex items-center gap-1.5 group">
                <span className="text-base font-semibold text-gray-700">{name}</span>
                <Pencil size={12} className="text-gray-300 group-hover:text-gray-500 transition-colors" />
              </button>
            )}
            <p className="text-xs text-gray-400 flex items-center gap-1 mt-0.5">
              <Users size={11} /> {count} {count === 1 ? "person" : "people"} hold this role
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <button
            onClick={() => setConfirmDelete(true)}
            className="flex items-center gap-1.5 text-xs text-red-400 hover:text-red-600 hover:bg-red-50 px-3 py-2 rounded-md transition-colors"
          >
            <Trash2 size={13} /> Delete role
          </button>
          <button
            onClick={handleSave}
            className={`text-xs font-medium px-4 py-2 rounded-md transition-all
              ${saved ? "bg-green-100 text-green-700" : "bg-lamaSky text-sky-900 hover:brightness-95"}`}
          >
            {saved ? "✓ Saved" : "Save changes"}
          </button>
        </div>
      </div>

      {/* Legend */}
      <div className="flex flex-wrap gap-3">
        {CLAIMS.map((c) => (
          <div key={c.key} className={`flex items-center gap-2 text-xs px-3 py-1.5 rounded-full border ${c.bg} ${c.border} ${c.color}`}>
            <span className="font-medium">{c.label}</span>
            <span className="text-[10px] opacity-70">— {c.description.split("—")[0].trim()}</span>
          </div>
        ))}
      </div>

      {/* Permissions card grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {AREAS.map((area) => (
          <AreaCard
            key={area}
            area={area}
            perms={perms[area] ?? { view: false, edit: false, delete: false }}
            onToggle={(key) => toggle(area, key)}
          />
        ))}
      </div>

      {/* Members */}
      {members.length > 0 && (
        <div className="flex flex-col gap-2">
          <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wide">
            Staff with this role
          </h3>
          <div className="flex flex-wrap gap-2">
            {members.map((m) => (
              <Link
                key={m.id}
                to={`/settings/staff/${m.id}`}
                className="flex items-center gap-2 bg-gray-50 hover:bg-lamaSkyLight border border-gray-100 rounded-full px-3 py-1.5 transition-colors"
              >
                <div className="w-5 h-5 rounded-full bg-lamaSky flex items-center justify-center text-[10px] font-bold text-sky-900 uppercase flex-shrink-0">
                  {m.name[0]}
                </div>
                <span className="text-xs text-gray-600">{m.name}</span>
              </Link>
            ))}
          </div>
        </div>
      )}

      {/* Delete confirm modal */}
      {confirmDelete && (
        <div
          className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center"
          onClick={() => setConfirmDelete(false)}
        >
          <div
            className="bg-white rounded-xl shadow-2xl w-[380px] p-6 flex flex-col gap-4"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-center gap-3">
              <div className="w-9 h-9 rounded-full bg-red-50 flex items-center justify-center">
                <AlertCircle size={18} className="text-red-500" />
              </div>
              <div>
                <h2 className="text-sm font-semibold text-gray-700">Remove "{name}"?</h2>
                <p className="text-xs text-gray-400 mt-0.5">This cannot be undone.</p>
              </div>
            </div>
            {count > 0 && (
              <div className="bg-amber-50 border border-amber-200 rounded-md px-3 py-2.5 text-xs text-amber-700">
                ⚠️ {count} staff member{count > 1 ? "s" : ""} currently hold this role. Their access will change immediately.
              </div>
            )}
            <div className="flex justify-end gap-2">
              <button
                onClick={() => setConfirmDelete(false)}
                className="text-xs text-gray-500 px-3 py-2 rounded-md hover:bg-gray-50"
              >
                Keep it
              </button>
              <button
                onClick={handleDelete}
                className="text-xs font-medium bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 transition-colors"
              >
                Yes, delete role
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
