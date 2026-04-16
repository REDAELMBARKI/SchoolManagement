import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import TableSearch from "@/components/TableSearch";
import FormActionButtonLink from "@/components/FormActionButtonLink";
import { role, subjectsData } from "@/lib/data";
import { useSearchParams } from "react-router-dom";

type SubjectList = {
  id: number;
  name: string;
  teachers: { name: string }[];
};

const SubjectListPage = () => {
  const [searchParams] = useSearchParams();
  const page = searchParams.get("page") ?? undefined;

  const columns = [
    {
      header: "Subject Name",
      accessor: "name",
    },
    {
      header: "Teachers",
      accessor: "teachers",
      className: "hidden md:table-cell",
    },
    {
      header: "Actions",
      accessor: "action",
    },
  ];

  const renderRow = (item: SubjectList) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight"
    >
      <td className="flex items-center gap-4 p-4">{item.name}</td>
      <td className="hidden md:table-cell">
        {item.teachers.map((teacher) => teacher.name).join(",")}
      </td>
      <td>
        <div className="flex items-center gap-2">
          {role === "admin" && (
            <>
              <FormActionButtonLink
                to={`/list/subjects/${item.id}/edit`}
                variant="update"
              />
              <FormActionButtonLink
                to={`/list/subjects/${item.id}/delete`}
                variant="delete"
              />
            </>
          )}
        </div>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;

  const data: SubjectList[] = subjectsData.map((s) => ({
    id: s.id,
    name: s.name,
    teachers: s.teachers.map((name) => ({ name })),
  }));
  const count = data.length;

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between">
        <h1 className="hidden md:block text-lg font-semibold">All Subjects</h1>
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
              <FormActionButtonLink to="/list/subjects/new" variant="create" />
            )}
          </div>
        </div>
      </div>
      <Table columns={columns} renderRow={renderRow} data={data} />
      <Pagination page={p} count={count} />
    </div>
  );
};

export default SubjectListPage;
