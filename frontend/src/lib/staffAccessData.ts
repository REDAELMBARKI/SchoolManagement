// ─── Types ────────────────────────────────────────────────────────────────────

export type PermAction = "view" | "edit" | "delete";

export type AreaPermissions = {
  view: boolean;
  edit: boolean;
  delete: boolean;
};

export type Role = {
  id: string;
  name: string;
  description: string;
  permissions: Record<string, AreaPermissions>; // area → actions
};

export type StaffMember = {
  id: string;
  name: string;
  email: string;
  phone?: string;
  roleIds: string[];
  extraPermissions: Record<string, AreaPermissions>; // sparse — only non-default overrides
};

// ─── Permission areas ─────────────────────────────────────────────────────────

export const AREAS = [
  "Students",
  "Fees",
  "Grades",
  "Attendance",
  "Timetable",
  "Reports",
  "Communications",
  "Staff",
] as const;

export type Area = (typeof AREAS)[number];

export const blank = (): AreaPermissions => ({ view: false, edit: false, delete: false });

// ─── Mock roles ───────────────────────────────────────────────────────────────

export const rolesData: Role[] = [
  {
    id: "role-teacher",
    name: "Teacher",
    description: "Classroom teachers who manage grades and attendance.",
    permissions: {
      Students:       { view: true,  edit: false, delete: false },
      Fees:           { view: false, edit: false, delete: false },
      Grades:         { view: true,  edit: true,  delete: false },
      Attendance:     { view: true,  edit: true,  delete: false },
      Timetable:      { view: true,  edit: false, delete: false },
      Reports:        { view: true,  edit: false, delete: false },
      Communications: { view: true,  edit: true,  delete: false },
      Staff:          { view: false, edit: false, delete: false },
    },
  },
  {
    id: "role-accountant",
    name: "Accountant",
    description: "Finance staff responsible for fees and billing.",
    permissions: {
      Students:       { view: true,  edit: false, delete: false },
      Fees:           { view: true,  edit: true,  delete: true  },
      Grades:         { view: false, edit: false, delete: false },
      Attendance:     { view: false, edit: false, delete: false },
      Timetable:      { view: false, edit: false, delete: false },
      Reports:        { view: true,  edit: false, delete: false },
      Communications: { view: true,  edit: false, delete: false },
      Staff:          { view: false, edit: false, delete: false },
    },
  },
  {
    id: "role-front-desk",
    name: "Front Desk",
    description: "Reception and enrollment intake staff.",
    permissions: {
      Students:       { view: true,  edit: true,  delete: false },
      Fees:           { view: true,  edit: false, delete: false },
      Grades:         { view: false, edit: false, delete: false },
      Attendance:     { view: true,  edit: false, delete: false },
      Timetable:      { view: true,  edit: false, delete: false },
      Reports:        { view: false, edit: false, delete: false },
      Communications: { view: true,  edit: true,  delete: false },
      Staff:          { view: false, edit: false, delete: false },
    },
  },
  {
    id: "role-head-teacher",
    name: "Head Teacher",
    description: "Senior academic staff with broader oversight.",
    permissions: {
      Students:       { view: true,  edit: true,  delete: false },
      Fees:           { view: true,  edit: false, delete: false },
      Grades:         { view: true,  edit: true,  delete: true  },
      Attendance:     { view: true,  edit: true,  delete: false },
      Timetable:      { view: true,  edit: true,  delete: false },
      Reports:        { view: true,  edit: true,  delete: false },
      Communications: { view: true,  edit: true,  delete: false },
      Staff:          { view: true,  edit: false, delete: false },
    },
  },
  {
    id: "role-branch-admin",
    name: "Branch Admin",
    description: "Full admin access within a single branch.",
    permissions: {
      Students:       { view: true,  edit: true,  delete: true  },
      Fees:           { view: true,  edit: true,  delete: true  },
      Grades:         { view: true,  edit: true,  delete: true  },
      Attendance:     { view: true,  edit: true,  delete: true  },
      Timetable:      { view: true,  edit: true,  delete: true  },
      Reports:        { view: true,  edit: true,  delete: false },
      Communications: { view: true,  edit: true,  delete: true  },
      Staff:          { view: true,  edit: true,  delete: false },
    },
  },
];

// ─── Mock staff ───────────────────────────────────────────────────────────────

export const staffData: StaffMember[] = [
  {
    id: "staff-1",
    name: "Mr. Amine Benali",
    email: "amine@school.com",
    phone: "+213 550 001 001",
    roleIds: ["role-teacher"],
    extraPermissions: {
      Reports: { view: true, edit: false, delete: false }, // one special exception
    },
  },
  {
    id: "staff-2",
    name: "Ms. Sara Mansouri",
    email: "sara@school.com",
    phone: "+213 550 002 002",
    roleIds: ["role-head-teacher"],
    extraPermissions: {},
  },
  {
    id: "staff-3",
    name: "Mr. Karim Tabet",
    email: "karim@school.com",
    phone: "+213 550 003 003",
    roleIds: ["role-accountant"],
    extraPermissions: {},
  },
  {
    id: "staff-4",
    name: "Ms. Nadia Aïssa",
    email: "nadia@school.com",
    phone: "+213 550 004 004",
    roleIds: ["role-front-desk", "role-teacher"],
    extraPermissions: {},
  },
  {
    id: "staff-5",
    name: "Mr. Yacine Hamdi",
    email: "yacine@school.com",
    phone: "+213 550 005 005",
    roleIds: ["role-branch-admin"],
    extraPermissions: {},
  },
  {
    id: "staff-6",
    name: "Ms. Rima Zerrouk",
    email: "rima@school.com",
    phone: "+213 550 006 006",
    roleIds: ["role-teacher"],
    extraPermissions: {
      Fees: { view: true, edit: false, delete: false },
    },
  },
];

// ─── Helpers ─────────────────────────────────────────────────────────────────

/** Returns the merged effective permissions for a staff member (roles + extras). */
export const effectivePermissions = (
  member: StaffMember,
): Record<string, AreaPermissions> => {
  const result: Record<string, AreaPermissions> = {};
  for (const area of AREAS) {
    result[area] = { view: false, edit: false, delete: false };
  }
  // Layer 1 — roles
  for (const roleId of member.roleIds) {
    const role = rolesData.find((r) => r.id === roleId);
    if (!role) continue;
    for (const area of AREAS) {
      const p = role.permissions[area];
      if (!p) continue;
      if (p.view)   result[area].view   = true;
      if (p.edit)   result[area].edit   = true;
      if (p.delete) result[area].delete = true;
    }
  }
  // Layer 2 — extra permissions (additive only)
  for (const area of AREAS) {
    const extra = member.extraPermissions[area];
    if (!extra) continue;
    if (extra.view)   result[area].view   = true;
    if (extra.edit)   result[area].edit   = true;
    if (extra.delete) result[area].delete = true;
  }
  return result;
};

/** Count how many staff hold a given role. */
export const memberCount = (roleId: string) =>
  staffData.filter((s) => s.roleIds.includes(roleId)).length;
