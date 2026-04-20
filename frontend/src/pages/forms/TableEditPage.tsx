import { Link, useParams } from "react-router-dom";
import FormPageRenderer from "@/components/FormPageRenderer";
import type { FormContainerProps } from "@/components/FormContainer";
import { classesData, examsData, subjectsData, intakesData } from "@/lib/data";
import {
  getMockStudentById,
  getMockTeacherById,
} from "@/lib/mockSchool";

function normalizeTable(
  tableParam: string | undefined
): { formTable: FormContainerProps["table"]; listSlug: string } | null {
  const t = tableParam?.toLowerCase();
  if (!t) return null;

  const map: Record<string, { formTable: FormContainerProps["table"]; listSlug: string }> =
    {
      teachers: { formTable: "teacher", listSlug: "teachers" },
      students: { formTable: "student", listSlug: "students" },
      subjects: { formTable: "subject", listSlug: "subjects" },
      classes: { formTable: "class", listSlug: "classes" },
      exams: { formTable: "exam", listSlug: "exams" },
      intakes: { formTable: "intake", listSlug: "intakes" },
      teacher: { formTable: "teacher", listSlug: "teachers" },
      student: { formTable: "student", listSlug: "students" },
      subject: { formTable: "subject", listSlug: "subjects" },
      class: { formTable: "class", listSlug: "classes" },
      exam: { formTable: "exam", listSlug: "exams" },
      intake: { formTable: "intake", listSlug: "intakes" },
    };

  return map[t] ?? null;
}

function toDatetimeLocalValue(d: Date) {
  // datetime-local expects a "YYYY-MM-DDTHH:mm" style string.
  return d.toISOString().slice(0, 16);
}

export default function TableEditPage() {
  const { table, id } = useParams<{ table: string; id: string }>();
  const normalized = normalizeTable(table);

  if (!normalized || !id) {
    return (
      <div className="p-4 m-4 bg-white rounded-md">
        <p className="text-gray-600">Form not supported.</p>
        <Link to="/list" className="text-blue-600 text-sm mt-2 inline-block">
          Back
        </Link>
      </div>
    );
  }

  let formData: any = null;

  if (normalized.formTable === "teacher") {
    formData = getMockTeacherById(id);
  } else if (normalized.formTable === "student") {
    formData = getMockStudentById(id);
  } else if (normalized.formTable === "subject") {
    const subjectId = Number(id);
    const s = subjectsData.find((x) => x.id === subjectId);
    formData = s
      ? {
          id: s.id,
          name: s.name,
          // Form schema expects teacher IDs as strings; renderer uses a dummy teacher with id=1.
          teachers: ["1"],
        }
      : null;
  } else if (normalized.formTable === "class") {
    const classId = Number(id);
    const c = classesData.find((x) => x.id === classId);
    formData = c
      ? {
          id: c.id,
          name: c.name,
          capacity: c.capacity,
          // Renderer provides a dummy grade with id=1; keep defaults consistent.
          gradeId: 1,
          // ClassForm currently uses `data.teachers` as the supervisor defaultValue.
          teachers: "1",
        }
      : null;
  } else if (normalized.formTable === "exam") {
    const examId = Number(id);
    const e = examsData.find((x) => x.id === examId);

    if (!e) {
      formData = null;
    } else {
      const start = new Date(`${e.date}T12:00:00`);
      const end = new Date(start.getTime() + 2 * 60 * 60 * 1000);

      formData = {
        id: e.id,
        title: `${e.subject} Exam`,
        startTime: toDatetimeLocalValue(start),
        endTime: toDatetimeLocalValue(end),
        // ExamForm currently uses `data.teachers` as the lesson defaultValue.
        teachers: 1,
      };
    }
  } else if (normalized.formTable === "intake") {
    const intakeId = Number(id);
    const i = intakesData.find((x) => x.id === intakeId);
    
    formData = i ? {
      id: i.id,
      firstName: i.firstName,
      lastName: i.lastName,
      email: i.email,
      phone: i.phone,
      dateOfBirth: i.dateOfBirth,
      gender: i.gender,
      leadSourceId: i.leadSourceId,
      opcId: i.opcId,
      intakeDate: i.intakeDate,
    } : null;
  }

  if (!formData) {
    return (
      <div className="bg-white p-4 rounded-md m-4 mt-0">
        <p className="text-gray-600">Item not found.</p>
        <Link
          to={`/list/${normalized.listSlug}`}
          className="text-blue-600 text-sm mt-2 inline-block"
        >
          Back to {normalized.listSlug}
        </Link>
      </div>
    );
  }

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-lg font-semibold">Update {table}</h1>
        <Link
          to={`/list/${normalized.listSlug}`}
          className="text-sm text-gray-600 hover:underline"
        >
          Back to {normalized.listSlug}
        </Link>
      </div>
      <FormPageRenderer table={normalized.formTable} type="update" data={formData} />
    </div>
  );
}

