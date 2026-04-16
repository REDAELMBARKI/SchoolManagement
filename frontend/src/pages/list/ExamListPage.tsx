import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import TableSearch from "@/components/TableSearch";
import FormActionButtonLink from "@/components/FormActionButtonLink";
import { role, examsData } from "@/lib/data";
import { useSearchParams } from "react-router-dom";

type ExamList = {
  id: number;
  startTime: Date;
  lesson: {
    subject: { name: string };
    class: { name: string };
    teacher: { name: string; surname: string };
  };
};

const ExamListPage = () => {
  const [searchParams] = useSearchParams();
  const page = searchParams.get("page") ?? undefined;

  const columns = [
    {
      header: "Subject Name",
      accessor: "name",
    },
    {
      header: "Class",
      accessor: "class",
    },
    {
      header: "Teacher",
      accessor: "teacher",
      className: "hidden md:table-cell",
    },
    {
      header: "Date",
      accessor: "date",
      className: "hidden md:table-cell",
    },
    ...(role === "admin" || role === "teacher"
      ? [
          {
            header: "Actions",
            accessor: "action",
          },
        ]
      : []),
  ];

  const renderRow = (item: ExamList) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight"
    >
      <td className="flex items-center gap-4 p-4">{item.lesson.subject.name}</td>
      <td>{item.lesson.class.name}</td>
      <td className="hidden md:table-cell">
        {item.lesson.teacher.name + " " + item.lesson.teacher.surname}
      </td>
      <td className="hidden md:table-cell">
        {new Intl.DateTimeFormat("en-US").format(item.startTime)}
      </td>
      <td>
        <div className="flex items-center gap-2">
          {(role === "admin" || role === "teacher") && (
            <>
              <FormActionButtonLink
                to={`/list/exams/${item.id}/edit`}
                variant="update"
              />
              <FormActionButtonLink
                to={`/list/exams/${item.id}/delete`}
                variant="delete"
              />
            </>
          )}
        </div>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;

  const data: ExamList[] = examsData.map((e) => {
    const parts = e.teacher.trim().split(/\s+/);
    return {
      id: e.id,
      startTime: new Date(`${e.date}T12:00:00`),
      lesson: {
        subject: { name: e.subject },
        class: { name: e.class },
        teacher: {
          name: parts[0] ?? "",
          surname: parts.slice(1).join(" "),
        },
      },
    };
  });
  const count = data.length;

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between">
        <h1 className="hidden md:block text-lg font-semibold">All Exams</h1>
        <div className="flex flex-col md:flex-row items-center gap-4 w-full md:w-auto">
          <TableSearch />
          <div className="flex items-center gap-4 self-end">
            <button className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow">
              <img src="/filter.png" alt="" width={14} height={14} />
            </button>
            <button className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow">
              <img src="/sort.png" alt="" width={14} height={14} />
            </button>
            {(role === "admin" || role === "teacher") && (
              <FormActionButtonLink
                to="/list/exams/new"
                variant="create"
              />
            )}
          </div>
        </div>
      </div>
      <Table columns={columns} renderRow={renderRow} data={data} />
      <Pagination page={p} count={count} />
    </div>
  );
};

export default ExamListPage;
