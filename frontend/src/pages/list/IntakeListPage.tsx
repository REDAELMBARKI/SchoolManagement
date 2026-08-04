import { useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import FormContainer from "@/components/FormContainer";
import Pagination from "@/components/Pagination";
import Table from "@/components/Table";
import { role, intakesData } from "@/lib/data";
import type { IntakeStatus } from "@/lib/formValidationSchemas";

// ── Types ────────────────────────────────────────────────────────────────────
// IntakeRow carries ALL fields needed by the edit form in addition to the
// display-only computed fields. Never strip data before passing to FormContainer.
type IntakeRow = {
  // display helpers
  id: number;
  subject: string;
  leadSource: string;
  commercialAgent: string;
  intakeDate: Date;

  // form fields — identical names to what IntakeForm defaultValues read from `data`
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  genderId: string;
  subjectId: string;
  branchId: string;
  status: IntakeStatus;
  followUpDate: string;
  notes: string;
  isIndependent: boolean;
  leadSourceType: string;
  leadSourceId: string;
  commercialAgentId: string;
  totalFees: number;
  amountPaid: number;
};

// ── Status badge ─────────────────────────────────────────────────────────────
const STATUS_STYLES: Record<IntakeStatus, string> = {
  New: "bg-lamaSky text-sky-700",
  Contacted: "bg-lamaYellow text-yellow-700",
  Interested: "bg-lamaPurple text-purple-700",
  Enrolled: "bg-green-100 text-green-700",
  NotInterested: "bg-red-100 text-red-500",
};

const STATUS_DISPLAY: Record<IntakeStatus, string> = {
  New: "New",
  Contacted: "Contacted",
  Interested: "Interested",
  Enrolled: "Enrolled",
  NotInterested: "Not Interested",
};

const StatusBadge = ({ status }: { status: IntakeStatus }) => (
  <span
    className={`text-[11px] font-medium px-2 py-0.5 rounded-full whitespace-nowrap ${
      STATUS_STYLES[status] ?? "bg-gray-100 text-gray-500"
    }`}
  >
    {STATUS_DISPLAY[status] ?? status}
  </span>
);

// ── Payment pill ─────────────────────────────────────────────────────────────
const PaymentBadge = ({
  totalFees,
  amountPaid,
}: {
  totalFees: number;
  amountPaid: number;
}) => {
  const pct = totalFees > 0 ? Math.round((amountPaid / totalFees) * 100) : 0;
  const fullyPaid = pct >= 100;
  return (
    <span
      className={`text-[11px] font-medium px-2 py-0.5 rounded-full whitespace-nowrap ${
        fullyPaid
          ? "bg-green-100 text-green-700"
          : pct > 0
          ? "bg-lamaYellowLight text-yellow-700"
          : "bg-red-50 text-red-500"
      }`}
    >
      {fullyPaid ? "Paid" : pct > 0 ? `${pct}% paid` : "Unpaid"}
    </span>
  );
};

// ── Filters ───────────────────────────────────────────────────────────────────
const STATUSES: Array<{ value: string; label: string }> = [
  { value: "", label: "All Statuses" },
  { value: "New", label: "New" },
  { value: "Contacted", label: "Contacted" },
  { value: "Interested", label: "Interested" },
  { value: "Enrolled", label: "Enrolled" },
  { value: "NotInterested", label: "Not Interested" },
];

// ── Page ─────────────────────────────────────────────────────────────────────
const IntakeListPage = () => {
  const [searchParams] = useSearchParams();
  const page = searchParams.get("page") ?? undefined;
  const [statusFilter, setStatusFilter] = useState("");
  const [search, setSearch] = useState("");

  const columns = [
    { header: "Intake", accessor: "name" },
    { header: "Subject", accessor: "subject", className: "hidden md:table-cell" },
    { header: "Lead Source", accessor: "leadSource", className: "hidden xl:table-cell" },
    { header: "Agent / OPC", accessor: "agent", className: "hidden xl:table-cell" },
    { header: "Intake Date", accessor: "intakeDate", className: "hidden lg:table-cell" },
    { header: "Status", accessor: "status", className: "hidden sm:table-cell" },
    { header: "Payment", accessor: "payment", className: "hidden lg:table-cell" },
    { header: "Actions", accessor: "action" },
  ];

  // Map to rows that carry ALL original fields — nothing stripped for the edit form
  const allRows: IntakeRow[] = intakesData.map((i) => ({
    id: i.id,
    // display helpers
    subject: i.subject ?? "—",
    leadSource: i.leadSource ?? "—",
    commercialAgent: i.opc ?? i.commercialAgent ?? "—",
    intakeDate: new Date(`${i.intakeDate}T12:00:00`),
    // form fields
    firstName: i.firstName,
    lastName: i.lastName,
    email: i.email,
    phone: i.phone,
    dateOfBirth: i.dateOfBirth ?? "",
    genderId: i.genderId ?? "",
    subjectId: i.subjectId ?? "",
    branchId: i.branchId ?? "",
    status: (i.status as IntakeStatus) ?? "New",
    followUpDate: i.followUpDate ?? "",
    notes: i.notes ?? "",
    isIndependent: i.isIndependent ?? false,
    leadSourceType: i.leadSourceType ?? "",
    leadSourceId: String(i.leadSourceId ?? ""),
    commercialAgentId: i.commercialAgentId ?? "",
    totalFees: i.totalFees ?? 0,
    amountPaid: i.amountPaid ?? 0,
  }));

  // Client-side filters (replace with query params once API is wired)
  const data = allRows.filter((item) => {
    const matchStatus = !statusFilter || item.status === statusFilter;
    const q = search.toLowerCase();
    const matchSearch =
      !q ||
      `${item.firstName} ${item.lastName}`.toLowerCase().includes(q) ||
      item.email.toLowerCase().includes(q) ||
      item.phone.includes(q);
    return matchStatus && matchSearch;
  });

  const renderRow = (item: IntakeRow) => (
    <tr
      key={item.id}
      className="border-b border-gray-200 even:bg-slate-50 text-sm hover:bg-lamaPurpleLight transition-colors"
    >
      {/* Avatar + Name */}
      <td className="flex items-center gap-3 p-4">
        <div className="w-9 h-9 rounded-full bg-lamaSky flex items-center justify-center text-sky-800 text-xs font-bold flex-shrink-0 uppercase">
          {item.firstName[0]}
          {item.lastName[0]}
        </div>
        <div className="flex flex-col min-w-0">
          <span className="font-medium text-gray-700 truncate">
            {item.firstName} {item.lastName}
          </span>
          <span className="text-xs text-gray-400 hidden md:block truncate">
            {item.email}
          </span>
          <span className="text-xs text-gray-400 md:hidden truncate">
            {item.phone}
          </span>
        </div>
      </td>

      {/* Subject */}
      <td className="hidden md:table-cell text-gray-600 text-sm">
        <div className="flex items-center gap-1">
          <img src="/subject.png" alt="" width={12} height={12} className="opacity-40" />
          {item.subject}
        </div>
      </td>

      {/* Lead Source */}
      <td className="hidden xl:table-cell text-gray-500 text-sm">
        {item.leadSource !== "—" ? (
          <span className="bg-lamaSkyLight text-sky-700 text-xs px-2 py-0.5 rounded-full">
            {item.leadSource}
          </span>
        ) : (
          <span className="text-gray-300">—</span>
        )}
      </td>

      {/* Agent / OPC */}
      <td className="hidden xl:table-cell text-gray-500 text-sm">
        {item.commercialAgent !== "—" ? (
          item.commercialAgent
        ) : (
          <span className="text-gray-300">—</span>
        )}
      </td>

      {/* Intake Date */}
      <td className="hidden lg:table-cell text-gray-500 text-sm">
        <div className="flex items-center gap-1">
          <img src="/date.png" alt="" width={12} height={12} className="opacity-40" />
          {new Intl.DateTimeFormat("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric",
          }).format(item.intakeDate)}
        </div>
      </td>

      {/* Status */}
      <td className="hidden sm:table-cell">
        <StatusBadge status={item.status} />
      </td>

      {/* Payment */}
      <td className="hidden lg:table-cell">
        <PaymentBadge totalFees={item.totalFees} amountPaid={item.amountPaid} />
      </td>

      {/* Actions — pass full `item` so edit form receives all defaultValues */}
      <td>
        <div className="flex items-center gap-2">
          {role === "admin" && (
            <Link
              to={`/list/intakes/${item.id}/convert`}
              title="Convert to Student"
              className="w-7 h-7 flex items-center justify-center rounded-full bg-lamaYellow hover:bg-yellow-300 transition-colors"
            >
              <img src="/student.png" alt="Convert to Student" width={14} height={14} />
            </Link>
          )}
          {role === "admin" && (
            <>
              {/* `data={item}` passes every form field — nothing stripped */}
              <FormContainer table="intake" type="update" data={item} />
              <FormContainer table="intake" type="delete" id={item.id} />
            </>
          )}
        </div>
      </td>
    </tr>
  );

  const p = page ? parseInt(page, 10) : 1;
  const count = data.length;

  return (
    <div className="bg-white p-4 rounded-md flex-1 m-4 mt-0">
      {/* ── Header ──────────────────────────────────────────────── */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-3 mb-2">
        <div className="flex items-center gap-3">
          <h1 className="text-lg font-semibold text-gray-700">All Intakes</h1>
          {count > 0 && (
            <span className="text-xs bg-lamaPurpleLight text-purple-600 px-2 py-0.5 rounded-full font-medium">
              {count}
            </span>
          )}
        </div>

        <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-3">
          {/* Search */}
          <div className="relative">
            <img
              src="/search.png"
              alt=""
              width={14}
              height={14}
              className="absolute left-3 top-1/2 -translate-y-1/2 opacity-40"
            />
            <input
              type="text"
              placeholder="Search by name, email…"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-8 pr-3 py-2 text-sm ring-[1.5px] ring-gray-200 rounded-md w-full sm:w-48 focus:outline-none focus:ring-lamaSky"
            />
          </div>

          {/* Status filter */}
          <div className="relative">
            <img
              src="/filter.png"
              alt=""
              width={12}
              height={12}
              className="absolute left-3 top-1/2 -translate-y-1/2 opacity-40 pointer-events-none"
            />
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="pl-8 pr-3 py-2 text-sm ring-[1.5px] ring-gray-200 rounded-md bg-white appearance-none cursor-pointer focus:outline-none focus:ring-lamaSky"
            >
              {STATUSES.map((s) => (
                <option key={s.value} value={s.value}>
                  {s.label}
                </option>
              ))}
            </select>
          </div>

          {/* Sort button */}
          <button
            className="w-8 h-8 flex items-center justify-center rounded-full bg-lamaYellow hover:bg-yellow-300 transition-colors flex-shrink-0"
            title="Sort"
          >
            <img src="/sort.png" alt="Sort" width={14} height={14} />
          </button>

          {/* Create button (admin) */}
          {role === "admin" && <FormContainer table="intake" type="create" />}
        </div>
      </div>

      {/* ── Table ───────────────────────────────────────────────── */}
      {data.length > 0 ? (
        <>
          <Table columns={columns} renderRow={renderRow} data={data} />
          <Pagination page={p} count={count} />
        </>
      ) : (
        /* Empty state */
        <div className="flex flex-col items-center justify-center py-16 gap-3">
          <div className="w-14 h-14 rounded-full bg-lamaSkyLight flex items-center justify-center">
            <img src="/profile.png" alt="" width={28} height={28} className="opacity-40" />
          </div>
          <p className="text-gray-500 font-medium text-sm">No intakes found</p>
          <p className="text-gray-400 text-xs">
            {statusFilter || search
              ? "Try adjusting your search or filters."
              : "Create your first intake to get started."}
          </p>
          {role === "admin" && !statusFilter && !search && (
            <FormContainer table="intake" type="create" />
          )}
        </div>
      )}
    </div>
  );
};

export default IntakeListPage;
