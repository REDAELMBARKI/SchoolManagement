import FormModal from "./FormModal";

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
    | "announcement"
    | "intake";
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
      case "intake":
        relatedData = {
          subjects: [
            { id: "sub-1", name: "English" },
            { id: "sub-2", name: "Math" },
            { id: "sub-3", name: "Physics" },
            { id: "sub-4", name: "Chemistry" },
            { id: "sub-5", name: "Biology" },
          ],
          branches: [
            { id: "branch-1", name: "Main Branch" },
            { id: "branch-2", name: "North Campus" },
          ],
          genders: [
            { id: "gender-1", name: "Male" },
            { id: "gender-2", name: "Female" },
          ],
          leadSources: [
            { id: "ls-1", name: "Website" },
            { id: "ls-2", name: "Referral" },
            { id: "ls-3", name: "Walk-in" },
            { id: "ls-4", name: "Social Media" },
            { id: "ls-5", name: "Email Campaign" },
          ],
          commercialAgents: [
            { id: "agent-1", name: "Sara Opc" },
            { id: "agent-2", name: "Karim Ref" },
            { id: "agent-3", name: "Youcef Opc" },
          ],
        };
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
