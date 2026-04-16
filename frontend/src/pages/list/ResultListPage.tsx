import FormContainer from "@/components/FormContainer";
import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import TableSearch from "@/components/TableSearch";
import { ITEM_PER_PAGE } from "@/lib/settings";
import { role, resultsData } from "@/lib/data";
import { useSearchParams } from "react-router-dom";

type ResultList = {
  id: number;
  title: string;
  studentName: string;
  studentSurname: string;
  teacherName: string;
  teacherSurname: string;
  score: number;
  className: string;
  startTime: Date;
};

const ResultListPage = () => {
  const [searchParams] = useSearchParams();
  const queryParams = Object.fromEntries(searchParams.entries());
  const page = queryParams.page;

  const columns = [
    {
      header: "Title",
      accessor: "title",
    },
    {
      header: "Student",
      accessor: "student",
    },
    {
      header: "Score",
      accessor: "score",
      className: "hidden md:table-cell",
    },
    {
      header: "Teacher",
      accessor: "teacher",
      className: "hidden md:table-cell",
    },
    {
      header: "Class",
      accessor: "class",
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

  const renderRow = (item: ResultList) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight"
    >
      <td className="flex items-center gap-4 p-4">{item.title}</td>
      <td>
        {item.studentName} {item.studentSurname}
      </td>
      <td className="hidden md:table-cell">{item.score}</td>
      <td className="hidden md:table-cell">
        {item.teacherName} {item.teacherSurname}
      </td>
      <td className="hidden md:table-cell">{item.className}</td>
      <td className="hidden md:table-cell">
        {new Intl.DateTimeFormat("en-US").format(item.startTime)}
      </td>
      <td>
        <div className="flex items-center gap-2">
          {(role === "admin" || role === "teacher") && (
            <>
              <FormContainer table="result" type="update" data={item} />
              <FormContainer table="result" type="delete" id={item.id} />
            </>
          )}
        </div>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;

  const allRows: ResultList[] = resultsData.map((r) => {
    const teacherParts = r.teacher.trim().split(/\s+/);
    const studentParts = r.student.trim().split(/\s+/);
    return {
      id: r.id,
      title: `${r.type} — ${r.subject}`,
      studentName: studentParts[0] ?? "",
      studentSurname: studentParts.slice(1).join(" ") || "",
      teacherName: teacherParts[0] ?? "",
      teacherSurname: teacherParts.slice(1).join(" ") || "",
      score: r.score,
      className: r.class,
      startTime: new Date(`${r.date}T12:00:00`),
    };
  });

  let filtered = allRows;
  const search = queryParams.search?.toLowerCase();
  if (search) {
    filtered = filtered.filter(
      (row) =>
        row.title.toLowerCase().includes(search) ||
        row.studentName.toLowerCase().includes(search) ||
        row.studentSurname.toLowerCase().includes(search)
    );
  }
  if (queryParams.studentId) {
    filtered = filtered.filter(
      (row) => String(row.id) === queryParams.studentId
    );
  }

  const count = filtered.length;
  const data = filtered.slice(
    ITEM_PER_PAGE * (p - 1),
    ITEM_PER_PAGE * (p - 1) + ITEM_PER_PAGE
  );

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between">
        <h1 className="hidden md:block text-lg font-semibold">All Results</h1>
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
              <FormContainer table="result" type="create" />
            )}
          </div>
        </div>
      </div>
      <Table columns={columns} renderRow={renderRow} data={data} />
      <Pagination page={p} count={count} />
    </div>
  );
};

export default ResultListPage;
