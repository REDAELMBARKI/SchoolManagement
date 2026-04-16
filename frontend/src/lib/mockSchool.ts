import {
  announcementsData,
  classesData,
  studentsData,
  teachersData,
} from "@/lib/data";

/** Plain types for UI-only mode (no Prisma / DB). */
export type MockStudentDetail = {
  id: string;
  username: string;
  name: string;
  surname: string;
  email: string | null;
  phone: string | null;
  address: string;
  img: string | null;
  bloodType: string;
  birthday: Date;
  class: {
    id: number;
    name: string;
    _count: { lessons: number };
  };
};

export type MockTeacherDetail = {
  id: string;
  username: string;
  name: string;
  surname: string;
  email: string | null;
  phone: string | null;
  address: string;
  img: string | null;
  bloodType: string;
  birthday: Date;
  _count: { subjects: number; lessons: number; classes: number };
};

export type MockAnnouncement = {
  title: string;
  description: string;
  date: Date;
};

function splitName(full: string): { name: string; surname: string } {
  const parts = full.trim().split(/\s+/);
  if (parts.length === 1) return { name: parts[0], surname: "" };
  return { name: parts[0], surname: parts.slice(1).join(" ") };
}

function classByName(className: string) {
  const c = classesData.find((x) => x.name === className) ?? classesData[0];
  return {
    id: c.id,
    name: c.name,
    _count: { lessons: 12 },
  };
}

export function getMockStudentById(id: string): MockStudentDetail | null {
  const numId = Number(id);
  const row = studentsData.find((s) => s.id === numId);
  if (!row) return null;
  const { name, surname } = splitName(row.name);
  const cls = classByName(row.class);
  return {
    id: String(row.id),
    username: row.studentId,
    name,
    surname,
    email: row.email,
    phone: row.phone,
    address: row.address,
    img: row.photo,
    bloodType: "A+",
    birthday: new Date(2012, 3, 15),
    class: cls,
  };
}

export function getMockTeacherById(id: string): MockTeacherDetail | null {
  const numId = Number(id);
  const row = teachersData.find((t) => t.id === numId);
  if (!row) return null;
  const { name, surname } = splitName(row.name);
  return {
    id: String(row.id),
    username: row.teacherId,
    name,
    surname,
    email: row.email,
    phone: row.phone,
    address: row.address,
    img: row.photo,
    bloodType: "O+",
    birthday: new Date(1988, 5, 20),
    _count: {
      subjects: row.subjects.length,
      lessons: 8,
      classes: row.classes.length,
    },
  };
}

export function getMockAnnouncements(limit = 3): MockAnnouncement[] {
  return announcementsData.slice(0, limit).map((a) => ({
    title: a.title,
    description: `Reminder for class ${a.class}: ${a.title}.`,
    date: new Date(`${a.date}T12:00:00`),
  }));
}

/** Parent dashboard: mock children linked to the demo parent user. */
export function getMockChildrenForParent(): {
  id: string;
  name: string;
  surname: string;
  classId: number;
}[] {
  return [
    { id: "1", name: "John", surname: "Doe", classId: 1 },
    { id: "2", name: "Jane", surname: "Doe", classId: 5 },
  ];
}

/** Student dashboard: first class the mock user belongs to. */
export function getMockStudentDashboardClassId(): number {
  return classesData[0]?.id ?? 1;
}
