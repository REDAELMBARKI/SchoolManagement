import FormContainer from "@/components/FormContainer";
import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import TableSearch from "@/components/TableSearch";
import { role, intakesData } from "@/lib/data";
import { useSearchParams } from "react-router-dom";

type IntakeList = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: Date;
  gender: string;
  leadSourceId: number;
  opcId: number;
  intakeDate: Date;
  leadSource: { name: string } | null;
  opc: { name: string } | null;
};

const IntakeListPage = () => {
  const [searchParams] = useSearchParams();
  const page = searchParams.get("page") ?? undefined;

  const columns = [
    {
      header: "Name",
      accessor: "name",
    },
    {
      header: "Email",
      accessor: "email",
      className: "hidden md:table-cell",
    },
    {
      header: "Phone",
      accessor: "phone",
      className: "hidden md:table-cell",
    },
    {
      header: "Lead Source",
      accessor: "leadSource",
      className: "hidden md:table-cell",
    },
    {
      header: "OPC",
      accessor: "opc",
      className: "hidden md:table-cell",
    },
    {
      header: "Intake Date",
      accessor: "intakeDate",
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

  const renderRow = (item: IntakeList) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight"
    >
      <td className="flex items-center gap-4 p-4">
        {item.firstName} {item.lastName}
      </td>
      <td className="hidden md:table-cell">{item.email}</td>
      <td className="hidden md:table-cell">{item.phone}</td>
      <td className="hidden md:table-cell">{item.leadSource?.name || "-"}</td>
      <td className="hidden md:table-cell">{item.opc?.name || "-"}</td>
      <td className="hidden md:table-cell">
        {new Intl.DateTimeFormat("en-US").format(item.intakeDate)}
      </td>
      <td>
        <div className="flex items-center gap-2">
          {role === "admin" && (
            <>
              <FormContainer table="intake" type="update" data={item} />
              <FormContainer table="intake" type="delete" id={item.id} />
            </>
          )}
        </div>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;

  const data: IntakeList[] = intakesData.map((i) => ({
    id: i.id,
    firstName: i.firstName,
    lastName: i.lastName,
    email: i.email,
    phone: i.phone,
    dateOfBirth: new Date(`${i.dateOfBirth}T12:00:00`),
    gender: i.gender,
    leadSourceId: i.leadSourceId,
    opcId: i.opcId,
    intakeDate: new Date(`${i.intakeDate}T12:00:00`),
    leadSource: { name: i.leadSource },
    opc: { name: i.opc },
  }));
  const count = data.length;

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      <div className="flex items-center justify-between">
        <h1 className="hidden md:block text-lg font-semibold">
          All Intakes
        </h1>
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
              <FormContainer table="intake" type="create" />
            )}
          </div>
        </div>
      </div>
      <Table columns={columns} renderRow={renderRow} data={data} />
      <Pagination page={p} count={count} />
    </div>
  );
};

export default IntakeListPage;
