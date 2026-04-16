import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import TableSearch from "@/components/TableSearch";
import { ITEM_PER_PAGE } from "@/lib/settings";
import { attendanceData } from "@/lib/data";
import { useSearchParams } from "react-router-dom";

type AttendanceRow = {
  id: number;
  student: string;
  class: string;
  lesson: string;
  date: Date;
  present: boolean;
};

const AttendanceListPage = () => {
  const [searchParams] = useSearchParams();
  const page = searchParams.get("page") ?? undefined;

  const columns = [
    { header: "Student", accessor: "student" },
    { header: "Class", accessor: "class", className: "hidden md:table-cell" },
    { header: "Lesson", accessor: "lesson", className: "hidden md:table-cell" },
    { header: "Date", accessor: "date", className: "hidden lg:table-cell" },
    {
      header: "Present",
      accessor: "present",
      className: "hidden lg:table-cell",
    },
  ];

  const renderRow = (item: AttendanceRow) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight"
    >
      <td className="p-4 font-medium">{item.student}</td>
      <td className="hidden md:table-cell">{item.class}</td>
      <td className="hidden md:table-cell">{item.lesson}</td>
      <td className="hidden lg:table-cell">
        {new Intl.DateTimeFormat("en-US").format(item.date)}
      </td>
      <td className="hidden lg:table-cell">
        <span
          className={
            item.present
              ? "text-green-600 font-medium"
              : "text-red-500 font-medium"
          }
        >
          {item.present ? "Yes" : "No"}
        </span>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;

  const rows: AttendanceRow[] = attendanceData.map((a) => ({
    id: a.id,
    student: a.student,
    class: a.class,
    lesson: a.lesson,
    date: new Date(`${a.date}T12:00:00`),
    present: a.present,
  }));

  const count = rows.length;
  const data = rows.slice(
    ITEM_PER_PAGE * (p - 1),
    ITEM_PER_PAGE * (p - 1) + ITEM_PER_PAGE
  );

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between">
        <h1 className="hidden md:block text-lg font-semibold">Attendance</h1>
        <div className="flex flex-col md:flex-row items-center gap-4 w-full md:w-auto">
          <TableSearch />
          <div className="flex items-center gap-4 self-end">
            <button
              type="button"
              className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow"
            >
              <img src="/filter.png" alt="" width={14} height={14} />
            </button>
            <button
              type="button"
              className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow"
            >
              <img src="/sort.png" alt="" width={14} height={14} />
            </button>
          </div>
        </div>
      </div>
      <Table columns={columns} renderRow={renderRow} data={data} />
      <Pagination page={p} count={count} />
    </div>
  );
};

export default AttendanceListPage;
