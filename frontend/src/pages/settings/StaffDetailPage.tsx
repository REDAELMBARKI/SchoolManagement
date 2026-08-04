import { useState } from "react";
import { useParams, Link } from "react-router-dom";
import {
  ChevronLeft, Users, ShieldCheck, Plus, X, Check,
  AlertCircle, ShieldAlert, Eye, Pencil, Trash2,
} from "lucide-react";
import {
  staffData, rolesData, AREAS, effectivePermissions,
  type AreaPermissions,
} from "@/lib/staffAccessData";

const ACTION_ICONS = {
  view:   <Eye   size={11} className="text-sky-500"    />,
  edit:   <Pencil size={11} className="text-purple-500" />,
  delete: <Trash2 size={11} className="text-red-400"   />,
};

const ACTION_LABELS = { view: "Can view", edit: "Can edit", delete: "Can delete" };
const ACTION_CHIP: Record<keyof AreaPermissions, string> = {
  view:   "bg-lamaSkyLight text-sky-700 border-lamaSky",
  edit:   "bg-lamaPurpleLight text-purple-700 border-lamaPurple",
  delete: "bg-red-50 text-red-600 border-red-300",
};

export default function StaffDetailPage() {
  const { staffId } = useParams<{ staffId: string }>();
  const member = staffData.find((s) => s.id === staffId);

  const [roleIds,   setRoleIds]   = useState(member?.roleIds ?? []);
  const [extras,    setExtras]    = useState<Record<string, AreaPermissions>>(
    JSON.parse(JSON.stringify(member?.extraPermissions ?? {}))
  );
  const [showAddRole,    setShowAddRole]    = useState(false);
  const [confirmRemRole, setConfirmRemRole] = useState<string | null>(null);
  const [saved,          setSaved]          = useState(false);

  if (!member) {
    return (
      <div className="p-6 flex flex-col gap-3">
        <Link to="/settings/staff" className="text-xs text-sky-600 flex items-center gap-1 hover:underline">
          <ChevronLeft size={13} /> Back to Staff
        </Link>
        <p className="text-sm text-gray-400">Staff member not found.</p>
      </div>
    );
  }

  const assignedRoles = roleIds.map((id) => rolesData.find((r) => r.id === id)).filter(Boolean);
  const availableRoles = rolesData.filter((r) => !roleIds.includes(r.id));
  const effective = effectivePermissions({ ...member, roleIds, extraPermissions: extras });

  const addRole = (roleId: string) => {
    setRoleIds((p) => [...p, roleId]);
    member.roleIds = [...roleIds, roleId];
    setShowAddRole(false);
    setSaved(false);
  };

  const removeRole = (roleId: string) => {
    const next = roleIds.filter((id) => id !== roleId);
    setRoleIds(next);
    member.roleIds = next;
    setConfirmRemRole(null);
    setSaved(false);
  };

  const toggleExtra = (area: string, action: keyof AreaPermissions) => {
    setExtras((prev) => {
      const current = prev[area] ?? { view: false, edit: false, delete: false };
      const updated = { ...current, [action]: !current[action] };
      // Clean up fully-false entries
      const allOff = !updated.view && !updated.edit && !updated.delete;
      const next = { ...prev };
      if (allOff) delete next[area];
      else next[area] = updated;
      member.extraPermissions = next;
      return next;
    });
    setSaved(false);
  };

  const handleSave = () => {
    setSaved(true);
    setTimeout(() => setSaved(false), 2500);
  };

  const initials = member.name.split(" ").map((n) => n[0]).slice(0, 2).join("").toUpperCase();
  const hasExtras = Object.values(extras).some((p) => p.view || p.edit || p.delete);

  return (
    <div className="p-6 flex flex-col gap-6">
      {/* Back */}
      <Link to="/settings/staff" className="text-xs text-sky-600 flex items-center gap-1 hover:underline w-fit">
        <ChevronLeft size={13} /> Back to Staff
      </Link>

      {/* Profile */}
      <div className="flex items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <div className="w-11 h-11 rounded-full bg-lamaPurpleLight flex items-center justify-center
            text-sm font-bold text-purple-800 flex-shrink-0">
            {initials}
          </div>
          <div>
            <p className="text-base font-semibold text-gray-700">{member.name}</p>
            <p className="text-xs text-gray-400">{member.email}</p>
            {member.phone && <p className="text-xs text-gray-400">{member.phone}</p>}
          </div>
        </div>
        <button
          onClick={handleSave}
          className={`text-xs font-medium px-4 py-2 rounded-md transition-all
            ${saved ? "bg-green-100 text-green-700" : "bg-lamaSky text-sky-900 hover:brightness-95"}`}
        >
          {saved ? "✓ Saved" : "Save changes"}
        </button>
      </div>

      {/* ── Section 1: Assigned roles ───────────────────────────────────── */}
      <section className="flex flex-col gap-3">
        <div className="flex items-center justify-between">
          <h2 className="text-xs font-semibold text-gray-500 uppercase tracking-wide flex items-center gap-1.5">
            <ShieldCheck size={13} className="text-gray-400" /> Assigned Roles
          </h2>
          {availableRoles.length > 0 && (
            <button
              onClick={() => setShowAddRole(true)}
              className="flex items-center gap-1 text-xs text-sky-600 hover:text-sky-800 font-medium"
            >
              <Plus size={13} /> Add role
            </button>
          )}
        </div>

        {assignedRoles.length === 0 && (
          <p className="text-xs text-gray-400 italic">No roles assigned. This person currently has no access.</p>
        )}

        <div className="flex flex-col gap-2">
          {assignedRoles.map((role) => {
            if (!role) return null;
            const permCount = Object.values(role.permissions)
              .flatMap((p) => [p.view, p.edit, p.delete]).filter(Boolean).length;
            return (
              <div key={role.id} className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-100 group">
                <div className="w-8 h-8 rounded-full bg-lamaSkyLight flex items-center justify-center flex-shrink-0">
                  <ShieldCheck size={14} className="text-sky-600" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-700">{role.name}</p>
                  <p className="text-xs text-gray-400">{permCount} permissions enabled</p>
                </div>
                <Link
                  to={`/settings/roles/${role.id}`}
                  className="text-[11px] text-sky-500 hover:underline opacity-0 group-hover:opacity-100 transition-opacity"
                >
                  View role
                </Link>
                <button
                  onClick={() => setConfirmRemRole(role.id)}
                  className="w-7 h-7 flex items-center justify-center rounded-full text-gray-300
                    hover:text-red-400 hover:bg-red-50 transition-colors opacity-0 group-hover:opacity-100"
                  title="Remove role"
                >
                  <X size={13} />
                </button>
              </div>
            );
          })}
        </div>
      </section>

      {/* ── Section 2: Extra permissions ────────────────────────────────── */}
      <section className="flex flex-col gap-3">
        <div>
          <h2 className="text-xs font-semibold text-gray-500 uppercase tracking-wide flex items-center gap-1.5">
            <ShieldAlert size={13} className="text-amber-500" />
            <span>Extra Permissions</span>
            <span className="text-amber-400 font-light normal-case tracking-normal">— exceptions only</span>
          </h2>
          <p className="text-[11px] text-gray-400 mt-1 leading-relaxed max-w-lg">
            Only use this for a specific ability this person needs outside their normal role.
            Prefer updating the role itself whenever possible — extras are harder to audit.
          </p>
        </div>

        <div className="border border-amber-100 bg-amber-50/40 rounded-lg p-4 flex flex-col gap-3">
          {AREAS.map((area) => {
            const extra = extras[area] ?? { view: false, edit: false, delete: false };
            return (
              <div key={area} className="flex items-center gap-3">
                <span className="text-xs text-gray-500 w-28 flex-shrink-0">{area}</span>
                <div className="flex gap-2">
                  {(["view", "edit", "delete"] as const).map((action) => (
                    <button
                      key={action}
                      onClick={() => toggleExtra(area, action)}
                      title={ACTION_LABELS[action]}
                      className={`flex items-center gap-1 text-[11px] px-2.5 py-1 rounded-full border transition-all
                        ${extra[action]
                          ? ACTION_CHIP[action]
                          : "border-gray-200 text-gray-300 hover:border-gray-300 hover:text-gray-400"}`}
                    >
                      {extra[action] && <Check size={10} strokeWidth={2.5} />}
                      {ACTION_LABELS[action]}
                    </button>
                  ))}
                </div>
              </div>
            );
          })}
          {!hasExtras && (
            <p className="text-[11px] text-amber-500/70 italic">No extra permissions — this person relies entirely on their roles.</p>
          )}
        </div>
      </section>

      {/* ── Section 3: Everything this person can do ─────────────────────── */}
      <section className="flex flex-col gap-3">
        <h2 className="text-xs font-semibold text-gray-500 uppercase tracking-wide flex items-center gap-1.5">
          <Eye size={13} className="text-gray-400" /> Everything this person can do
        </h2>
        <p className="text-[11px] text-gray-400 -mt-1">
          Read-only summary — combines all roles and extra permissions.
        </p>

        <div className="flex flex-col gap-1.5">
          {AREAS.map((area) => {
            const p = effective[area];
            const active = [
              p.view   && "view",
              p.edit   && "edit",
              p.delete && "delete",
            ].filter(Boolean) as (keyof AreaPermissions)[];

            return (
              <div key={area} className={`flex items-center gap-3 px-4 py-2.5 rounded-md
                ${active.length > 0 ? "bg-white border border-gray-100" : "bg-gray-50"}`}>
                <span className={`text-xs font-medium w-28 flex-shrink-0 ${active.length > 0 ? "text-gray-600" : "text-gray-300"}`}>
                  {area}
                </span>
                {active.length === 0 ? (
                  <span className="text-[11px] text-gray-300 italic">No access</span>
                ) : (
                  <div className="flex flex-wrap gap-1.5">
                    {active.map((action) => (
                      <span key={action} className={`flex items-center gap-1 text-[11px] px-2.5 py-0.5 rounded-full border font-medium ${ACTION_CHIP[action]}`}>
                        {ACTION_ICONS[action]} {ACTION_LABELS[action]}
                      </span>
                    ))}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      </section>

      {/* ── Add role modal ──────────────────────────────────────────────── */}
      {showAddRole && (
        <div className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center" onClick={() => setShowAddRole(false)}>
          <div className="bg-white rounded-xl shadow-2xl w-[360px] p-6 flex flex-col gap-4" onClick={(e) => e.stopPropagation()}>
            <div className="flex items-center justify-between">
              <h2 className="text-sm font-semibold text-gray-700">Add a role</h2>
              <button onClick={() => setShowAddRole(false)} className="text-gray-400 hover:text-gray-600"><X size={16} /></button>
            </div>
            <div className="flex flex-col gap-2">
              {availableRoles.map((r) => (
                <button
                  key={r.id}
                  onClick={() => addRole(r.id)}
                  className="flex items-center gap-3 p-3 rounded-lg border border-gray-100
                    hover:border-lamaSky hover:bg-lamaSkyLight text-left transition-colors group"
                >
                  <ShieldCheck size={15} className="text-sky-400 flex-shrink-0" />
                  <div>
                    <p className="text-sm font-medium text-gray-700">{r.name}</p>
                    <p className="text-xs text-gray-400 truncate">{r.description}</p>
                  </div>
                </button>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* ── Confirm remove role ─────────────────────────────────────────── */}
      {confirmRemRole && (() => {
        const role = rolesData.find((r) => r.id === confirmRemRole);
        return (
          <div className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center" onClick={() => setConfirmRemRole(null)}>
            <div className="bg-white rounded-xl shadow-2xl w-[380px] p-6 flex flex-col gap-4" onClick={(e) => e.stopPropagation()}>
              <div className="flex items-center gap-3">
                <div className="w-9 h-9 rounded-full bg-amber-50 flex items-center justify-center">
                  <AlertCircle size={18} className="text-amber-500" />
                </div>
                <div>
                  <h2 className="text-sm font-semibold text-gray-700">Remove "{role?.name}" from {member.name}?</h2>
                  <p className="text-xs text-gray-400 mt-0.5">
                    They will immediately lose everything this role grants them.
                  </p>
                </div>
              </div>
              <div className="flex justify-end gap-2">
                <button onClick={() => setConfirmRemRole(null)} className="text-xs text-gray-500 px-3 py-2 rounded-md hover:bg-gray-50">Keep role</button>
                <button
                  onClick={() => removeRole(confirmRemRole)}
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
