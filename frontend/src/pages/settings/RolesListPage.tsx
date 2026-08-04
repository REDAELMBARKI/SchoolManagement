import { useState } from "react";
import { Link } from "react-router-dom";
import { ShieldCheck, Plus, Users, ChevronRight, Trash2, X, AlertCircle } from "lucide-react";
import { rolesData, memberCount, AREAS, blank, type Role } from "@/lib/staffAccessData";

const ROLE_COLORS = [
  "bg-lamaSkyLight text-sky-700",
  "bg-lamaPurpleLight text-purple-700",
  "bg-lamaYellowLight text-yellow-700",
  "bg-green-50 text-green-700",
  "bg-orange-50 text-orange-700",
];

export default function RolesListPage() {
  const [roles, setRoles] = useState<Role[]>(rolesData);
  const [showCreate, setShowCreate] = useState(false);
  const [newName, setNewName] = useState("");
  const [newDesc, setNewDesc] = useState("");
  const [confirmDelete, setConfirmDelete] = useState<string | null>(null);

  const handleCreate = () => {
    if (!newName.trim()) return;
    const role: Role = {
      id: `role-${Date.now()}`,
      name: newName.trim(),
      description: newDesc.trim(),
      permissions: Object.fromEntries(AREAS.map((a) => [a, blank()])),
    };
    setRoles((prev) => [...prev, role]);
    setNewName("");
    setNewDesc("");
    setShowCreate(false);
  };

  const handleDelete = (id: string) => {
    setRoles((prev) => prev.filter((r) => r.id !== id));
    setConfirmDelete(null);
  };

  return (
    <div className="p-6 flex flex-col gap-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-base font-semibold text-gray-700 flex items-center gap-2">
            <ShieldCheck size={17} className="text-gray-400" /> Roles
          </h1>
          <p className="text-xs text-gray-400 mt-0.5">
            Each role defines what a staff member is allowed to do.
          </p>
        </div>
        <button
          onClick={() => setShowCreate(true)}
          className="flex items-center gap-1.5 bg-lamaSky text-sky-900 text-xs font-medium
            px-3 py-2 rounded-md hover:brightness-95 transition-colors"
        >
          <Plus size={14} /> New role
        </button>
      </div>

      {/* Role cards */}
      <div className="flex flex-col gap-3">
        {roles.map((role, i) => {
          const count = memberCount(role.id);
          const colorCls = ROLE_COLORS[i % ROLE_COLORS.length];
          const permCount = Object.values(role.permissions)
            .flatMap((p) => [p.view, p.edit, p.delete])
            .filter(Boolean).length;

          return (
            <div
              key={role.id}
              className="flex items-center gap-4 p-4 rounded-lg border border-gray-100
                hover:border-gray-200 hover:shadow-sm transition-all group"
            >
              {/* Color dot */}
              <div className={`w-9 h-9 rounded-full flex items-center justify-center flex-shrink-0 ${colorCls}`}>
                <ShieldCheck size={16} />
              </div>

              {/* Info */}
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-gray-700">{role.name}</p>
                {role.description && (
                  <p className="text-xs text-gray-400 truncate mt-0.5">{role.description}</p>
                )}
                <div className="flex items-center gap-3 mt-1.5">
                  <span className="flex items-center gap-1 text-[11px] text-gray-400">
                    <Users size={11} /> {count} {count === 1 ? "person" : "people"}
                  </span>
                  <span className="text-[11px] text-gray-300">·</span>
                  <span className="text-[11px] text-gray-400">
                    {permCount} permission{permCount !== 1 ? "s" : ""} enabled
                  </span>
                </div>
              </div>

              {/* Actions */}
              <div className="flex items-center gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <button
                  onClick={() => setConfirmDelete(role.id)}
                  className="w-7 h-7 flex items-center justify-center rounded-full
                    text-gray-300 hover:text-red-400 hover:bg-red-50 transition-colors"
                  title="Delete role"
                >
                  <Trash2 size={13} />
                </button>
              </div>

              <Link
                to={`/settings/roles/${role.id}`}
                className="flex items-center gap-1 text-xs text-sky-600 hover:text-sky-800
                  font-medium transition-colors flex-shrink-0"
              >
                Edit permissions <ChevronRight size={13} />
              </Link>
            </div>
          );
        })}
      </div>

      {/* ── Create modal ───────────────────────────────────────────────────── */}
      {showCreate && (
        <div className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center" onClick={() => setShowCreate(false)}>
          <div className="bg-white rounded-xl shadow-2xl w-[380px] p-6 flex flex-col gap-4" onClick={(e) => e.stopPropagation()}>
            <div className="flex items-center justify-between">
              <h2 className="text-sm font-semibold text-gray-700">Create a new role</h2>
              <button onClick={() => setShowCreate(false)} className="text-gray-400 hover:text-gray-600"><X size={16} /></button>
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-xs text-gray-500">Role name <span className="text-red-400">*</span></label>
              <input
                autoFocus
                value={newName}
                onChange={(e) => setNewName(e.target.value)}
                placeholder="e.g. Accountant"
                className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full focus:outline-none focus:ring-lamaSky"
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-xs text-gray-500">Description <span className="text-gray-300">(optional)</span></label>
              <input
                value={newDesc}
                onChange={(e) => setNewDesc(e.target.value)}
                placeholder="What does this role do?"
                className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full focus:outline-none focus:ring-lamaSky"
              />
            </div>
            <p className="text-xs text-gray-400 bg-gray-50 rounded-md px-3 py-2">
              You'll set permissions after creating the role.
            </p>
            <div className="flex justify-end gap-2">
              <button onClick={() => setShowCreate(false)} className="text-xs text-gray-500 px-3 py-2 rounded-md hover:bg-gray-50">Cancel</button>
              <button
                onClick={handleCreate}
                disabled={!newName.trim()}
                className={`text-xs font-medium px-4 py-2 rounded-md transition-colors
                  ${newName.trim() ? "bg-lamaSky text-sky-900 hover:brightness-95" : "bg-gray-100 text-gray-400 cursor-not-allowed"}`}
              >
                Create role
              </button>
            </div>
          </div>
        </div>
      )}

      {/* ── Delete confirm ─────────────────────────────────────────────────── */}
      {confirmDelete && (() => {
        const role = roles.find((r) => r.id === confirmDelete)!;
        const count = memberCount(confirmDelete);
        return (
          <div className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center" onClick={() => setConfirmDelete(null)}>
            <div className="bg-white rounded-xl shadow-2xl w-[380px] p-6 flex flex-col gap-4" onClick={(e) => e.stopPropagation()}>
              <div className="flex items-center gap-3">
                <div className="w-9 h-9 rounded-full bg-red-50 flex items-center justify-center flex-shrink-0">
                  <AlertCircle size={18} className="text-red-500" />
                </div>
                <div>
                  <h2 className="text-sm font-semibold text-gray-700">Remove "{role.name}"?</h2>
                  <p className="text-xs text-gray-400 mt-0.5">This cannot be undone.</p>
                </div>
              </div>
              {count > 0 && (
                <div className="bg-amber-50 border border-amber-200 rounded-md px-3 py-2.5 text-xs text-amber-700">
                  ⚠️ {count} staff member{count > 1 ? "s" : ""} currently hold this role. Removing it will take away their access immediately.
                </div>
              )}
              <div className="flex justify-end gap-2">
                <button onClick={() => setConfirmDelete(null)} className="text-xs text-gray-500 px-3 py-2 rounded-md hover:bg-gray-50">Keep role</button>
                <button
                  onClick={() => handleDelete(confirmDelete)}
                  className="text-xs font-medium bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 transition-colors"
                >
                  Yes, remove role
                </button>
              </div>
            </div>
          </div>
        );
      })()}
    </div>
  );
}
