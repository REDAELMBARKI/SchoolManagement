import { Link, useParams } from "react-router-dom";
import FormPageRenderer from "@/components/FormPageRenderer";
import type { FormContainerProps } from "@/components/FormContainer";

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
      teacher: { formTable: "teacher", listSlug: "teachers" },
      student: { formTable: "student", listSlug: "students" },
      subject: { formTable: "subject", listSlug: "subjects" },
      class: { formTable: "class", listSlug: "classes" },
      exam: { formTable: "exam", listSlug: "exams" },
    };

  return map[t] ?? null;
}

export default function TableCreatePage() {
  const { table } = useParams<{ table: string }>();
  const normalized = normalizeTable(table);

  if (!normalized) {
    return (
      <div className="p-4 m-4 bg-white rounded-md">
        <p className="text-gray-600">Form not supported.</p>
        <Link to="/list" className="text-blue-600 text-sm mt-2 inline-block">
          Back
        </Link>
      </div>
    );
  }

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-lg font-semibold">Create {table}</h1>
        <Link
          to={`/list/${normalized.listSlug}`}
          className="text-sm text-gray-600 hover:underline"
        >
          Back to {normalized.listSlug}
        </Link>
      </div>
      <FormPageRenderer table={normalized.formTable} type="create" />
    </div>
  );
}

