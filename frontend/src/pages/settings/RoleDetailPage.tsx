import { useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { ShieldCheck, ChevronLeft, Check, Pencil, Trash2, X, AlertCircle, Users } from "lucide-react";
import { rolesData, staffData, AREAS, memberCount, type AreaPermissions } from "@/lib/staffAccessData";

type Perms = Record<string, AreaPermissions>;

const ACTION_LABELS: Record<keyof AreaPermissions, string> = {
  view:   "Can view",
  edit:   "Can edit",
  delete: "Can delete",
};

const ACTION_COLORS: Record<keyof AreaPermissions, string> = {
  view:   "text-sky-600",
  edit:   "text-purple-600",
  delete: "text-red-500",
};

const Chip = ({ active, color, label, onClick }: {
  active: boolean; color: string; label: string; onClick: () => void;
}) => (
  <button
    onClick={onClick}
    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-medium border transition-all select-none
      ${active
        ? `${color === "text-sky-600"   ? "bg-lamaSkyLight border-lamaSky"     : ""}
           ${color === "text-purple-600" ? "bg-lamaPurpleLight border-lamaPurple" : ""}
           ${color === "text-red-500"    ? "bg-red-50 border-red-300"            : ""}
           ${color}`
        : "bg-white border-gray-200 text-gray-300 hover:border-gray-300 hover:text-gray-400"}`}
  >
    {active && <Check size={11} strokeWidth={2.5} />}
    {label}
  </button>
);

export default function RoleDetailPage() {
  const { roleId } = useParams<{ roleId: string }>();
  const navigate = useNavigate();

  const original = rolesData.find((r) => r.id === roleId);
  const [name, setName]     = useState(original?.name ?? "");
  const [perms, setPerms]   = useState<Perms>(
    JSON.parse(JSON.stringify(original?.permissions ?? {}))
  );
  const [editingName, setEditingName] = useState(false);
  const [confirmDelete, setConfirmDelete]   = useState(false);
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

  const toggle = (area: string, action: keyof AreaPermissions) => {
    setPerms((prev) => ({
      ...prev,
      [area]: { ...prev[area], [action]: !prev[area][action] },
    }));
    setSaved(false);
  };

  const handleSave = () => {
    // Persist into in-memory source (replace with API call)
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
    <div className="p-6 flex flex-col gap-6">
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
              <button
                onClick={() => setEditingName(true)}
                className="flex items-center gap-1.5 group"
              >
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
            className="flex items-center gap-1.5 text-xs text-red-400 hover:text-red-600
              hover:bg-red-50 px-3 py-2 rounded-md transition-colors"
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

      {/* Intro note */}
      <p className="text-xs text-gray-400 bg-gray-50 rounded-md px-4 py-3 leading-relaxed">
        Check the boxes below to control what this role allows. Changes apply to everyone who holds this role.
        Use plain language: <strong className="text-gray-500">Can view</strong> means read-only,
        <strong className="text-gray-500"> Can edit</strong> means create and update,
        <strong className="text-gray-500"> Can delete</strong> means permanently remove.
      </p>

      {/* Permissions grid */}
      <div className="flex flex-col gap-2">
        <div className="grid grid-cols-[1fr_auto_auto_auto] gap-x-4 px-4 pb-1">
          <span className="text-[11px] font-semibold text-gray-400 uppercase tracking-wide">Area</span>
          {(["view", "edit", "delete"] as const).map((a) => (
            <span key={a} className={`text-[11px] font-semibold uppercase tracking-wide ${ACTION_COLORS[a]}`}>
              {ACTION_LABELS[a]}
            </span>
          ))}
        </div>

        {AREAS.map((area, i) => (
          <div
            key={area}
            className={`grid grid-cols-[1fr_auto_auto_auto] gap-x-4 items-center
              px-4 py-3 rounded-lg ${i % 2 === 0 ? "bg-gray-50" : "bg-white border border-gray-50"}`}
          >
            <span className="text-sm font-medium text-gray-600">{area}</span>
            {(["view", "edit", "delete"] as const).map((action) => (
              <Chip
                key={action}
                active={perms[area]?.[action] ?? false}
                color={ACTION_COLORS[action]}
                label={ACTION_LABELS[action]}
                onClick={() => toggle(area, action)}
              />
            ))}
          </div>
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
                className="flex items-center gap-2 bg-gray-50 hover:bg-lamaSkyLight
                  border border-gray-100 rounded-full px-3 py-1.5 transition-colors"
              >
                <div className="w-5 h-5 rounded-full bg-lamaSky flex items-center justify-center
                  text-[10px] font-bold text-sky-900 uppercase flex-shrink-0">
                  {m.name[0]}
                </div>
                <span className="text-xs text-gray-600">{m.name}</span>
              </Link>
            ))}
          </div>
        </div>
      )}

      {/* Delete confirm */}
      {confirmDelete && (
        <div className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center" onClick={() => setConfirmDelete(false)}>
          <div className="bg-white rounded-xl shadow-2xl w-[380px] p-6 flex flex-col gap-4" onClick={(e) => e.stopPropagation()}>
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
              <button onClick={() => setConfirmDelete(false)} className="text-xs text-gray-500 px-3 py-2 rounded-md hover:bg-gray-50">Keep it</button>
              <button onClick={handleDelete} className="text-xs font-medium bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 transition-colors">
                Yes, delete role
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
