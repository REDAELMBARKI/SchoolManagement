import FormModal from "./FormModal";
import { role } from "@/lib/data";

export type FormContainerProps = {
  table:
    | "teacher"
    | "student"
    | "parent"
    | "subject"
    | "class"
    | "lesson"
    | "exam"
    | "assignment"
    | "result"
    | "attendance"
    | "event"
    | "announcement";
  type: "create" | "update" | "delete";
  data?: any;
  id?: number | string;
};

const FormContainer = ({ table, type, data, id }: FormContainerProps) => {
  let relatedData = {};

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
        break;
    }
  }

  return (
    <div className="">
      <FormModal
        table={table}
        type={type}
        data={data}
        id={id}
        relatedData={relatedData}
      />
    </div>
  );
};

export default FormContainer;
