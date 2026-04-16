import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import TableSearch from "@/components/TableSearch";
import FormActionButtonLink from "@/components/FormActionButtonLink";
import { role, classesData } from "@/lib/data";
import { useSearchParams } from "react-router-dom";

type ClassList = {
  id: number;
  name: string;
  capacity: number;
  grade: number;
  supervisor: { name: string; surname: string };
};

const ClassListPage = () => {
  const [searchParams] = useSearchParams();
  const page = searchParams.get("page") ?? undefined;

  const columns = [
    {
      header: "Class Name",
      accessor: "name",
    },
    {
      header: "Capacity",
      accessor: "capacity",
      className: "hidden md:table-cell",
    },
    {
      header: "Grade",
      accessor: "grade",
      className: "hidden md:table-cell",
    },
    {
      header: "Supervisor",
      accessor: "supervisor",
      className: "hidden md:table-cell",
    },
    ...(role === "admin"
      ? [
          {
            header: "Actions",
            accessor: "action",
          },
        ]
      : []),
  ];

  const renderRow = (item: ClassList) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight"
    >
      <td className="flex items-center gap-4 p-4">{item.name}</td>
      <td className="hidden md:table-cell">{item.capacity}</td>
      <td className="hidden md:table-cell">{item.grade}</td>
      <td className="hidden md:table-cell">
        {item.supervisor.name + " " + item.supervisor.surname}
      </td>
      <td>
        <div className="flex items-center gap-2">
          {role === "admin" && (
            <>
              <FormActionButtonLink
                to={`/list/classes/${item.id}/edit`}
                variant="update"
              />
              <FormActionButtonLink
                to={`/list/classes/${item.id}/delete`}
                variant="delete"
              />
            </>
          )}
        </div>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;

  const data: ClassList[] = classesData.map((c) => {
    const parts = c.supervisor.trim().split(/\s+/);
    return {
      id: c.id,
      name: c.name,
      capacity: c.capacity,
      grade: c.grade,
      supervisor: {
        name: parts[0] ?? "",
        surname: parts.slice(1).join(" "),
      },
    };
  });
  const count = data.length;

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between">
        <h1 className="hidden md:block text-lg font-semibold">All Classes</h1>
        <div className="flex flex-col md:flex-row items-center gap-4 w-full md:w-auto">
          <TableSearch />
          <div className="flex items-center gap-4 self-end">
            <button className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow">
              <img src="/filter.png" alt="" width={14} height={14} />
            </button>
            <button className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow">
              <img src="/sort.png" alt="" width={14} height={14} />
            </button>
            {role === "admin" && (
              <FormActionButtonLink
                to="/list/classes/new"
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

export default ClassListPage;
