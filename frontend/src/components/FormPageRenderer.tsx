"use client";

import type { FormContainerProps } from "./FormContainer";
import { Dispatch, SetStateAction, Suspense, lazy, useState } from "react";
import { toast } from "react-toastify";
import {
  deleteClass,
  deleteExam,
  deleteStudent,
  deleteSubject,
  deleteTeacher,
} from "@/lib/actions";

const TeacherForm = lazy(() => import("./forms/TeacherForm"));
const StudentForm = lazy(() => import("./forms/StudentForm"));
const SubjectForm = lazy(() => import("./forms/SubjectForm"));
const ClassForm = lazy(() => import("./forms/ClassForm"));
const ExamForm = lazy(() => import("./forms/ExamForm"));

const deleteActionMap: Record<string, any> = {
  subject: deleteSubject,
  class: deleteClass,
  teacher: deleteTeacher,
  student: deleteStudent,
  exam: deleteExam,
  // Keep parity with the existing modal behavior (even though some mappings are demo-ish).
  parent: deleteSubject,
  lesson: deleteSubject,
  assignment: deleteSubject,
  result: deleteSubject,
  attendance: deleteSubject,
  event: deleteSubject,
  announcement: deleteSubject,
};

export default function FormPageRenderer({
  table,
  type,
  data,
  id,
}: FormContainerProps) {
  const [, setOpen] = useState(false);

  let relatedData: any = {};
  if (type !== "delete") {
    switch (table) {
      case "subject":
        relatedData = { teachers: [{ id: 1, name: "John", surname: "Doe" }] };
        break;
      case "class":
        relatedData = {
          teachers: [{ id: 1, name: "John", surname: "Doe" }],
          grades: [{ id: 1, level: 1 }],
        };
        break;
      case "teacher":
        relatedData = { subjects: [{ id: 1, name: "Math" }] };
        break;
      case "student":
        relatedData = {
          classes: [{ id: 1, name: "1A", _count: { students: 10 } }],
          grades: [{ id: 1, level: 1 }],
        };
        break;
      case "exam":
        relatedData = { lessons: [{ id: 1, name: "Math" }] };
        break;
      default:
        relatedData = {};
        break;
    }
  }

  const forms: Partial<
    Record<
      FormContainerProps["table"],
      (
        setOpen: Dispatch<SetStateAction<boolean>>,
        type: "create" | "update",
        data?: any,
        relatedData?: any
      ) => JSX.Element
    >
  > = {
    subject: (setOpen, type, data, relatedData) => (
      <SubjectForm
        type={type}
        data={data}
        setOpen={setOpen}
        relatedData={relatedData}
      />
    ),
    class: (setOpen, type, data, relatedData) => (
      <ClassForm
        type={type}
        data={data}
        setOpen={setOpen}
        relatedData={relatedData}
      />
    ),
    teacher: (setOpen, type, data, relatedData) => (
      <TeacherForm
        type={type}
        data={data}
        setOpen={setOpen}
        relatedData={relatedData}
      />
    ),
    student: (setOpen, type, data, relatedData) => (
      <StudentForm
        type={type}
        data={data}
        setOpen={setOpen}
        relatedData={relatedData}
      />
    ),
    exam: (setOpen, type, data, relatedData) => (
      <ExamForm
        type={type}
        data={data}
        setOpen={setOpen}
        relatedData={relatedData}
      />
    ),
  };

  return (
    <Suspense fallback={<h1>Loading...</h1>}>
      {type === "delete" && id ? (
        <form
          className="p-4 flex flex-col gap-4"
          onSubmit={async (e) => {
            e.preventDefault();
            const fd = new FormData(e.currentTarget);
            const deleteFn = deleteActionMap[table];
            const res = await deleteFn({ success: false, error: false }, fd);

            if (res?.success) {
              toast(`${table} has been deleted!`);
              setOpen(false);
              window.location.reload();
            }
          }}
        >
          <input type="hidden" name="id" value={String(id)} readOnly />
          <span className="text-center font-medium">
            All data will be lost. Are you sure you want to delete this {table}?
          </span>
          <button className="bg-red-700 text-white py-2 px-4 rounded-md border-none w-max self-center">
            Delete
          </button>
        </form>
      ) : type === "create" || type === "update" ? (
        forms[table] ? (
          forms[table](
            setOpen,
            type,
            data,
            relatedData
          ) as JSX.Element
        ) : (
          <div className="p-4">
            <h1 className="text-lg font-semibold">Form not found!</h1>
          </div>
        )
      ) : (
        <div className="p-4">
          <h1 className="text-lg font-semibold">Form not found!</h1>
        </div>
      )}
    </Suspense>
  );
}

