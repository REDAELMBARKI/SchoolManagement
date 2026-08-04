import { useState } from "react";
import { Link } from "react-router-dom";
import { Users, Plus, Search, ChevronRight, X, AlertCircle } from "lucide-react";
import { staffData, rolesData, type StaffMember, AREAS, blank } from "@/lib/staffAccessData";

export default function StaffListPage() {
  const [staff, setStaff]     = useState<StaffMember[]>(staffData);
  const [search, setSearch]   = useState("");
  const [showAdd, setShowAdd] = useState(false);

  // New staff form state
  const [newName,  setNewName]  = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [newPhone, setNewPhone] = useState("");
  const [newRoles, setNewRoles] = useState<string[]>([]);
  const [emailErr, setEmailErr] = useState("");

  const filtered = staff.filter((s) => {
    const q = search.toLowerCase();
    return (
      s.name.toLowerCase().includes(q) ||
      s.email.toLowerCase().includes(q)
    );
  });

  const toggleRole = (id: string) =>
    setNewRoles((prev) => prev.includes(id) ? prev.filter((r) => r !== id) : [...prev, id]);

  const handleAdd = () => {
    if (!newName.trim() || !newEmail.trim()) return;
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(newEmail)) {
      setEmailErr("Please enter a valid email address.");
      return;
    }
    const member: StaffMember = {
      id: `staff-${Date.now()}`,
      name: newName.trim(),
      email: newEmail.trim(),
      phone: newPhone.trim() || undefined,
      roleIds: newRoles,
      extraPermissions: {},
    };
    staffData.push(member);
    setStaff([...staffData]);
    setShowAdd(false);
    setNewName(""); setNewEmail(""); setNewPhone(""); setNewRoles([]); setEmailErr("");
  };

  return (
    <div className="p-6 flex flex-col gap-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-base font-semibold text-gray-700 flex items-center gap-2">
            <Users size={17} className="text-gray-400" /> Staff Members
          </h1>
          <p className="text-xs text-gray-400 mt-0.5">
            {staff.length} staff account{staff.length !== 1 ? "s" : ""} in this school.
          </p>
        </div>
        <button
          onClick={() => setShowAdd(true)}
          className="flex items-center gap-1.5 bg-lamaSky text-sky-900 text-xs font-medium
            px-3 py-2 rounded-md hover:brightness-95 transition-colors"
        >
          <Plus size={14} /> Add staff
        </button>
      </div>

      {/* Search */}
      <div className="relative">
        <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-300" />
        <input
          type="text"
          placeholder="Search by name or email…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full pl-9 pr-3 py-2 text-sm ring-[1.5px] ring-gray-200 rounded-md
            focus:outline-none focus:ring-lamaSky"
        />
      </div>

      {/* List */}
      <div className="flex flex-col gap-2">
        {filtered.length === 0 && (
          <p className="text-sm text-gray-400 text-center py-8">No staff found.</p>
        )}
        {filtered.map((member) => {
          const roles = member.roleIds
            .map((id) => rolesData.find((r) => r.id === id)?.name)
            .filter(Boolean);
          const hasExtra = Object.values(member.extraPermissions).some(
            (p) => p.view || p.edit || p.delete
          );

          return (
            <Link
              key={member.id}
              to={`/settings/staff/${member.id}`}
              className="flex items-center gap-4 p-4 rounded-lg border border-gray-100
                hover:border-gray-200 hover:shadow-sm transition-all group"
            >
              {/* Avatar */}
              <div className="w-9 h-9 rounded-full bg-lamaPurpleLight flex items-center justify-center
                text-xs font-bold text-purple-800 flex-shrink-0 uppercase">
                {member.name.split(" ").map((n) => n[0]).slice(0, 2).join("")}
              </div>

              {/* Info */}
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-gray-700">{member.name}</p>
                <p className="text-xs text-gray-400 truncate">{member.email}</p>
                <div className="flex flex-wrap gap-1.5 mt-1.5">
                  {roles.map((r) => (
                    <span key={r} className="text-[11px] bg-lamaSkyLight text-sky-700
                      px-2 py-0.5 rounded-full font-medium">
                      {r}
                    </span>
                  ))}
                  {roles.length === 0 && (
                    <span className="text-[11px] text-gray-300 italic">No roles assigned</span>
                  )}
                  {hasExtra && (
                    <span className="text-[11px] bg-amber-50 text-amber-600 border border-amber-200
                      px-2 py-0.5 rounded-full font-medium">
                      + extra permissions
                    </span>
                  )}
                </div>
              </div>

              <ChevronRight size={15} className="text-gray-300 group-hover:text-gray-500 transition-colors flex-shrink-0" />
            </Link>
          );
        })}
      </div>

      {/* Add staff modal */}
      {showAdd && (
        <div className="fixed inset-0 z-50 bg-black/30 flex items-center justify-center" onClick={() => setShowAdd(false)}>
          <div className="bg-white rounded-xl shadow-2xl w-[420px] p-6 flex flex-col gap-4 max-h-[90vh] overflow-y-auto" onClick={(e) => e.stopPropagation()}>
            <div className="flex items-center justify-between">
              <h2 className="text-sm font-semibold text-gray-700">Add a staff account</h2>
              <button onClick={() => setShowAdd(false)} className="text-gray-400 hover:text-gray-600"><X size={16} /></button>
            </div>

            {[
              { label: "Full name", value: newName, set: setNewName, placeholder: "e.g. Sara Mansouri", required: true },
              { label: "Email address", value: newEmail, set: setNewEmail, placeholder: "sara@school.com", required: true },
              { label: "Phone", value: newPhone, set: setNewPhone, placeholder: "+213 …", required: false },
            ].map(({ label, value, set, placeholder, required }) => (
              <div key={label} className="flex flex-col gap-1.5">
                <label className="text-xs text-gray-500">
                  {label} {required && <span className="text-red-400">*</span>}
                </label>
                <input
                  value={value}
                  onChange={(e) => { set(e.target.value); if (label === "Email address") setEmailErr(""); }}
                  placeholder={placeholder}
                  className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm focus:outline-none focus:ring-lamaSky"
                />
                {label === "Email address" && emailErr && (
                  <p className="text-xs text-red-400 flex items-center gap-1"><AlertCircle size={11} />{emailErr}</p>
                )}
              </div>
            ))}

            <div className="flex flex-col gap-1.5">
              <label className="text-xs text-gray-500">Assign roles</label>
              <div className="flex flex-wrap gap-2">
                {rolesData.map((r) => (
                  <button
                    key={r.id}
                    onClick={() => toggleRole(r.id)}
                    className={`text-xs px-3 py-1.5 rounded-full border transition-colors font-medium
                      ${newRoles.includes(r.id)
                        ? "bg-lamaSkyLight border-lamaSky text-sky-800"
                        : "border-gray-200 text-gray-400 hover:border-gray-300"}`}
                  >
                    {newRoles.includes(r.id) && "✓ "}{r.name}
                  </button>
                ))}
              </div>
              <p className="text-[11px] text-gray-400 mt-0.5">
                You can add or change roles later from the staff member's page.
              </p>
            </div>

            <div className="flex justify-end gap-2 pt-1">
              <button onClick={() => setShowAdd(false)} className="text-xs text-gray-500 px-3 py-2 rounded-md hover:bg-gray-50">Cancel</button>
              <button
                onClick={handleAdd}
                disabled={!newName.trim() || !newEmail.trim()}
                className={`text-xs font-medium px-4 py-2 rounded-md transition-colors
                  ${newName.trim() && newEmail.trim() ? "bg-lamaSky text-sky-900 hover:brightness-95" : "bg-gray-100 text-gray-400 cursor-not-allowed"}`}
              >
                Add staff member
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
