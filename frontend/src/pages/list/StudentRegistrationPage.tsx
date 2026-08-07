import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "react-router-dom";
import { useState } from "react";
import { toast } from "react-toastify";
import InputField from "@/components/InputField";
import {
  studentRegistrationSchema,
  StudentRegistrationSchema,
  PAYMENT_METHODS,
  RELATIONSHIP_TYPES,
} from "@/lib/formValidationSchemas";
import type { FieldError } from "react-hook-form";

// ── Mock FK data (replace with real API calls in Task #2) ─────────────────────

const MOCK_LEVELS = [
  { id: "lvl-1", name: "Beginner" },
  { id: "lvl-2", name: "Elementary" },
  { id: "lvl-3", name: "Pre-Intermediate" },
  { id: "lvl-4", name: "Intermediate" },
  { id: "lvl-5", name: "Upper-Intermediate" },
  { id: "lvl-6", name: "Advanced" },
];

const MOCK_SUBJECTS = [
  { id: "sub-1", name: "English" },
  { id: "sub-2", name: "Mathematics" },
  { id: "sub-3", name: "Physics" },
  { id: "sub-4", name: "Chemistry" },
  { id: "sub-5", name: "French" },
];

const MOCK_PLANS = [
  { id: "plan-1", name: "Monthly — 3,000 DA / month" },
  { id: "plan-2", name: "Quarterly — 8,400 DA / 3 months" },
  { id: "plan-3", name: "Annual — 30,000 DA / year" },
  { id: "plan-4", name: "Intensive — 12,000 DA (accelerated)" },
];

const MOCK_GROUPS = [
  { id: "grp-1", name: "Group A — Morning  08:00–10:00" },
  { id: "grp-2", name: "Group B — Afternoon  14:00–16:00" },
  { id: "grp-3", name: "Group C — Evening  18:00–20:00" },
  { id: "grp-4", name: "Group D — Weekend  09:00–12:00" },
];

const MOCK_GENDERS = [
  { id: "gender-1", name: "Male" },
  { id: "gender-2", name: "Female" },
];

const MOCK_INTAKES = [
  { id: "intake-1", label: "John Doe — English · Jan 2026" },
  { id: "intake-2", label: "Jane Smith — Mathematics · Feb 2026" },
  { id: "intake-3", label: "Michael Johnson — Physics · Mar 2026" },
  { id: "intake-4", label: "Emily Brown — English · Apr 2026" },
  { id: "intake-6", label: "Amira Benali — Mathematics · Jun 2026" },
];

// ── Payment method metadata ───────────────────────────────────────────────────

const METHOD_META: Record<string, { label: string; icon: string; note: string }> = {
  Cash:         { label: "Cash",          icon: "/finance.png",      note: "No reference needed" },
  CreditCard:   { label: "Credit Card",   icon: "/upload.png",       note: "Ref. code required" },
  DebitCard:    { label: "Debit Card",    icon: "/upload.png",       note: "No reference needed" },
  BankTransfer: { label: "Bank Transfer", icon: "/singleBranch.png", note: "Ref. code required" },
  Check:        { label: "Check",         icon: "/result.png",       note: "Ref. code required" },
};

const REF_REQUIRED = ["CreditCard", "BankTransfer", "Check"];

// ── Shared sub-components ─────────────────────────────────────────────────────

/** Section card with a colored left accent stripe. */
function SectionCard({
  children,
  accent = "border-gray-200",
}: {
  children: React.ReactNode;
  accent?: string;
}) {
  return (
    <div className={`bg-white rounded-xl border-l-4 ${accent} shadow-sm p-5`}>
      {children}
    </div>
  );
}

/** Numbered section header with icon. */
function SectionHeader({
  n,
  icon,
  title,
  subtitle,
  bubble,
}: {
  n: number;
  icon: string;
  title: string;
  subtitle: string;
  bubble: string; // Tailwind bg class for number bubble
}) {
  return (
    <div className="flex items-center gap-3 mb-5">
      <div
        className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold text-gray-700 flex-shrink-0 ${bubble}`}
      >
        {n}
      </div>
      <div className="flex items-center gap-2">
        <img src={`/${icon}.png`} alt="" width={15} height={15} className="opacity-50" />
        <div>
          <p className="text-sm font-semibold text-gray-800 leading-none">{title}</p>
          <p className="text-[11px] text-gray-400 mt-0.5">{subtitle}</p>
        </div>
      </div>
    </div>
  );
}

/** Reusable select wrapper that matches InputField's visual style. */
function SelectField({
  label,
  name,
  register,
  error,
  children,
  widthClass = "w-full md:w-1/4",
  required,
}: {
  label: string;
  name: string;
  register: any;
  error?: FieldError;
  children: React.ReactNode;
  widthClass?: string;
  required?: boolean;
}) {
  return (
    <div className={`flex flex-col gap-2 ${widthClass}`}>
      <label className="text-xs text-gray-500">
        {label}
        {required && <span className="text-red-400 ml-0.5">*</span>}
      </label>
      <select
        {...register(name)}
        className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full bg-white focus:outline-none focus:ring-lamaSky"
      >
        {children}
      </select>
      {error?.message && (
        <p className="text-xs text-red-400">{error.message}</p>
      )}
    </div>
  );
}

// ── Page ──────────────────────────────────────────────────────────────────────

export default function StudentRegistrationPage() {
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<StudentRegistrationSchema>({
    resolver: zodResolver(studentRegistrationSchema),
    defaultValues: {
      student: { registrationMode: "direct" },
      payment:  { method: "Cash" },
      hasGuardian: false,
    },
  });

  const registrationMode = watch("student.registrationMode");
  const paymentMethod    = watch("payment.method");
  const hasGuardian      = watch("hasGuardian");

  const onSubmit = handleSubmit(async (data) => {
    setSubmitting(true);

    // Build StudentRegistrationRequestDto — matches the backend shape exactly.
    const dto = {
      studentRegReq: {
        firstName: data.student.firstName,
        lastName:  data.student.lastName,
        email:     data.student.email   || null,
        phone:     data.student.phone,
        dateOfBirth: data.student.dateOfBirth,
        genderId:  data.student.genderId || null,
        levelId:   data.student.levelId,
        isDirectRegistration: data.student.registrationMode === "direct",
        intakeId:  data.student.registrationMode === "intake"
          ? data.student.intakeId || null
          : null,
      },
      enrollmentRegReq: {
        // StudentId will be assigned by the backend after student creation.
        studentId:       "00000000-0000-0000-0000-000000000000",
        levelId:         data.student.levelId,
        subjectId:       data.enrollment.subjectId,
        planId:          data.enrollment.planId,
        preferedGroupId: data.enrollment.preferedGroupId || null,
        notes:           data.enrollment.notes           || null,
      },
      paymentRegReq: {
        amountPaid:            data.payment.amountPaid,
        transferFees:          data.payment.transferFees ?? null,
        paidAt:                data.payment.paidAt        || null,
        method:                data.payment.method,
        externalReferenceCode: data.payment.externalReferenceCode || null,
      },
      responsableRegReq: data.hasGuardian
        ? {
            firstName:    data.responsable.firstName    || "",
            lastName:     data.responsable.lastName     || "",
            email:        data.responsable.email        || null,
            phone:        data.responsable.phone        || "",
            relationship: data.responsable.relationship || "",
            genderId:     data.responsable.genderId     || null,
          }
        : null,
      periodStart:    data.periodStart    || null,
      periodEnd:      data.periodEnd      || null,
      invoiceDueDate: data.invoiceDueDate || null,
      chargeDueDate:  data.chargeDueDate  || null,
    };

    // TODO (Task #2): POST /api/students/register
    console.log("Student registration payload:", dto);
    await new Promise((r) => setTimeout(r, 700));

    setSubmitting(false);
    toast.success("Student registered successfully!");
  });

  // Cast nested errors to FieldError to satisfy InputField's prop type.
  const se  = errors.student   as any;
  const ee  = errors.enrollment as any;
  const pe  = errors.payment    as any;
  const re  = errors.responsable as any;

  return (
    <div className="flex flex-col gap-4 m-4 mt-0">

      {/* ── Page header ─────────────────────────────────────────────────── */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-lamaSky flex items-center justify-center flex-shrink-0">
            <img src="/student.png" alt="" width={20} height={20} />
          </div>
          <div>
            <h1 className="text-lg font-semibold text-gray-800 leading-tight">
              New Student Registration
            </h1>
            <p className="text-xs text-gray-400">
              Complete all required sections to enrol a new student
            </p>
          </div>
        </div>
        <Link
          to="/list/students"
          className="flex items-center gap-1.5 text-sm text-gray-400 hover:text-gray-600 transition-colors"
        >
          <img src="/close.png" alt="" width={11} height={11} />
          Cancel
        </Link>
      </div>

      <form onSubmit={onSubmit} className="flex flex-col gap-4" noValidate>

        {/* ══ SECTION 1 — Student Information ══════════════════════════════ */}
        <SectionCard accent="border-lamaSky">
          <SectionHeader
            n={1}
            icon="student"
            title="Student Information"
            subtitle="Personal details, identity and registration type"
            bubble="bg-lamaSky"
          />

          {/* Registration mode toggle */}
          <div className="flex flex-col gap-2 mb-5">
            <label className="text-xs text-gray-500 font-medium">
              Registration Type <span className="text-red-400">*</span>
            </label>
            <div className="flex gap-2 flex-wrap">
              {(["direct", "intake"] as const).map((mode) => (
                <button
                  key={mode}
                  type="button"
                  onClick={() => {
                    setValue("student.registrationMode", mode);
                    if (mode === "direct") setValue("student.intakeId", "");
                  }}
                  className={`flex items-center gap-2 py-2 px-4 rounded-lg text-sm font-medium border-2 transition-all ${
                    registrationMode === mode
                      ? "bg-lamaSkyLight border-lamaSky text-gray-700"
                      : "bg-white border-gray-200 text-gray-400 hover:border-gray-300"
                  }`}
                >
                  <img
                    src={mode === "direct" ? "/create.png" : "/student.png"}
                    alt=""
                    width={14}
                    height={14}
                    className={registrationMode === mode ? "opacity-70" : "opacity-30"}
                  />
                  {mode === "direct" ? "Direct Registration" : "Convert from Intake"}
                </button>
              ))}
            </div>
            <p className="text-[11px] text-gray-400">
              {registrationMode === "direct"
                ? "Student will be created without a prior intake record."
                : "Link this registration to an existing intake enquiry."}
            </p>
          </div>

          {/* Intake select — only when mode = intake */}
          {registrationMode === "intake" && (
            <div className="mb-5 flex flex-wrap gap-4">
              <SelectField
                label="Linked Intake"
                name="student.intakeId"
                register={register}
                error={se?.intakeId}
                widthClass="w-full md:w-1/2"
                required
              >
                <option value="">— Select an intake —</option>
                {MOCK_INTAKES.map((i) => (
                  <option key={i.id} value={i.id}>
                    {i.label}
                  </option>
                ))}
              </SelectField>
            </div>
          )}

          {/* Personal fields */}
          <div className="flex flex-wrap gap-4">
            <InputField
              label="First Name *"
              name="student.firstName"
              register={register}
              error={se?.firstName}
            />
            <InputField
              label="Last Name *"
              name="student.lastName"
              register={register}
              error={se?.lastName}
            />
            <InputField
              label="Phone *"
              name="student.phone"
              register={register}
              error={se?.phone}
              inputProps={{ placeholder: "+213 5XX XXX XXX" }}
            />
            <InputField
              label="Email"
              type="email"
              name="student.email"
              register={register}
              error={se?.email}
              inputProps={{ placeholder: "student@example.com" }}
            />
            <InputField
              label="Date of Birth *"
              type="date"
              name="student.dateOfBirth"
              register={register}
              error={se?.dateOfBirth}
            />
            <SelectField
              label="Gender"
              name="student.genderId"
              register={register}
              error={se?.genderId}
            >
              <option value="">— Not specified —</option>
              {MOCK_GENDERS.map((g) => (
                <option key={g.id} value={g.id}>{g.name}</option>
              ))}
            </SelectField>
            <SelectField
              label="Level"
              name="student.levelId"
              register={register}
              error={se?.levelId}
              required
            >
              <option value="">— Select level —</option>
              {MOCK_LEVELS.map((l) => (
                <option key={l.id} value={l.id}>{l.name}</option>
              ))}
            </SelectField>
          </div>
        </SectionCard>

        {/* ══ SECTION 2 — Enrollment ════════════════════════════════════════ */}
        <SectionCard accent="border-lamaPurple">
          <SectionHeader
            n={2}
            icon="subject"
            title="Enrollment"
            subtitle="Subject, pricing plan and preferred group"
            bubble="bg-lamaPurple"
          />
          <div className="flex flex-wrap gap-4">
            <SelectField
              label="Subject"
              name="enrollment.subjectId"
              register={register}
              error={ee?.subjectId}
              required
            >
              <option value="">— Select subject —</option>
              {MOCK_SUBJECTS.map((s) => (
                <option key={s.id} value={s.id}>{s.name}</option>
              ))}
            </SelectField>

            <SelectField
              label="Pricing Plan"
              name="enrollment.planId"
              register={register}
              error={ee?.planId}
              required
            >
              <option value="">— Select plan —</option>
              {MOCK_PLANS.map((p) => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </SelectField>

            <SelectField
              label="Preferred Group"
              name="enrollment.preferedGroupId"
              register={register}
              error={ee?.preferedGroupId}
            >
              <option value="">— No preference —</option>
              {MOCK_GROUPS.map((g) => (
                <option key={g.id} value={g.id}>{g.name}</option>
              ))}
            </SelectField>

            {/* Notes — wider */}
            <div className="flex flex-col gap-2 w-full md:w-3/4">
              <label className="text-xs text-gray-500">Enrollment Notes</label>
              <textarea
                {...register("enrollment.notes")}
                rows={2}
                className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full resize-none focus:outline-none focus:ring-lamaPurple"
                placeholder="Any special notes for this enrollment…"
              />
              {ee?.notes?.message && (
                <p className="text-xs text-red-400">{ee.notes.message}</p>
              )}
            </div>
          </div>
        </SectionCard>

        {/* ══ SECTION 3 — Payment ══════════════════════════════════════════ */}
        <SectionCard accent="border-lamaYellow">
          <SectionHeader
            n={3}
            icon="finance"
            title="Registration Payment"
            subtitle="Amount paid and payment method details"
            bubble="bg-lamaYellow"
          />

          {/* Payment method card-picker */}
          <div className="flex flex-col gap-2 mb-5">
            <label className="text-xs text-gray-500 font-medium">
              Payment Method <span className="text-red-400">*</span>
            </label>
            <div className="flex flex-wrap gap-2">
              {PAYMENT_METHODS.map((method) => {
                const meta     = METHOD_META[method];
                const selected = paymentMethod === method;
                return (
                  <button
                    key={method}
                    type="button"
                    onClick={() => {
                      setValue("payment.method", method);
                      // Clear ref code when switching to a non-ref method
                      if (!REF_REQUIRED.includes(method)) {
                        setValue("payment.externalReferenceCode", "");
                      }
                    }}
                    className={`flex items-center gap-2.5 py-2.5 px-3.5 rounded-xl border-2 text-left transition-all ${
                      selected
                        ? "bg-lamaYellowLight border-lamaYellow shadow-sm"
                        : "bg-white border-gray-200 text-gray-400 hover:border-gray-300"
                    }`}
                  >
                    <img
                      src={meta.icon}
                      alt=""
                      width={16}
                      height={16}
                      className={selected ? "opacity-70" : "opacity-30"}
                    />
                    <div>
                      <p className={`text-xs font-semibold leading-none ${selected ? "text-gray-800" : "text-gray-500"}`}>
                        {meta.label}
                      </p>
                      <p className="text-[10px] text-gray-400 mt-0.5">{meta.note}</p>
                    </div>
                  </button>
                );
              })}
            </div>
            {pe?.method?.message && (
              <p className="text-xs text-red-400">{pe.method.message}</p>
            )}
          </div>

          {/* Payment fields */}
          <div className="flex flex-wrap gap-4">
            <InputField
              label="Amount Paid (DA) *"
              type="number"
              name="payment.amountPaid"
              register={register}
              error={pe?.amountPaid}
              inputProps={{ min: 0.01, step: "0.01", placeholder: "0.00" }}
            />
            <InputField
              label="Transfer Fees (DA)"
              type="number"
              name="payment.transferFees"
              register={register}
              error={pe?.transferFees}
              inputProps={{ min: 0, step: "0.01", placeholder: "0.00" }}
            />
            <InputField
              label="Payment Date & Time"
              type="datetime-local"
              name="payment.paidAt"
              register={register}
              error={pe?.paidAt}
            />
            {/* Reference code — conditional on method */}
            {REF_REQUIRED.includes(paymentMethod) && (
              <InputField
                label="External Reference Code *"
                name="payment.externalReferenceCode"
                register={register}
                error={pe?.externalReferenceCode}
                inputProps={{ placeholder: "e.g. TRX-20260806-001" }}
              />
            )}
          </div>
        </SectionCard>

        {/* ══ SECTION 4 — Guardian (optional) ══════════════════════════════ */}
        <SectionCard accent={hasGuardian ? "border-lamaPurple" : "border-gray-200"}>
          {/* Section header + toggle in same row */}
          <div className="flex items-center justify-between mb-5">
            <SectionHeader
              n={4}
              icon="parent"
              title="Guardian / Parent"
              subtitle="Optional — link a parent or guardian to this student"
              bubble={hasGuardian ? "bg-lamaPurple" : "bg-gray-100"}
            />
            {/* iOS-style toggle */}
            <button
              type="button"
              aria-label="Toggle guardian"
              onClick={() => setValue("hasGuardian", !hasGuardian)}
              className={`relative flex-shrink-0 inline-flex h-6 w-11 items-center rounded-full transition-colors duration-200 ${
                hasGuardian ? "bg-lamaPurple" : "bg-gray-200"
              }`}
            >
              <span
                className={`inline-block h-4 w-4 transform rounded-full bg-white shadow transition-transform duration-200 ${
                  hasGuardian ? "translate-x-6" : "translate-x-1"
                }`}
              />
            </button>
          </div>

          {hasGuardian ? (
            <div className="flex flex-wrap gap-4">
              <InputField
                label="First Name *"
                name="responsable.firstName"
                register={register}
                error={re?.firstName}
              />
              <InputField
                label="Last Name *"
                name="responsable.lastName"
                register={register}
                error={re?.lastName}
              />
              <InputField
                label="Phone *"
                name="responsable.phone"
                register={register}
                error={re?.phone}
                inputProps={{ placeholder: "+213 5XX XXX XXX" }}
              />
              <InputField
                label="Email"
                type="email"
                name="responsable.email"
                register={register}
                error={re?.email}
              />
              <SelectField
                label="Relationship"
                name="responsable.relationship"
                register={register}
                error={re?.relationship}
                required
              >
                <option value="">— Select relationship —</option>
                {RELATIONSHIP_TYPES.map((r) => (
                  <option key={r} value={r}>{r}</option>
                ))}
              </SelectField>
              <SelectField
                label="Gender"
                name="responsable.genderId"
                register={register}
              >
                <option value="">— Not specified —</option>
                {MOCK_GENDERS.map((g) => (
                  <option key={g.id} value={g.id}>{g.name}</option>
                ))}
              </SelectField>
            </div>
          ) : (
            <p className="text-xs text-gray-400 italic">
              Toggle on to add a parent or guardian for this student.
            </p>
          )}
        </SectionCard>

        {/* ══ SECTION 5 — Registration Dates (optional) ════════════════════ */}
        <SectionCard accent="border-gray-200">
          <SectionHeader
            n={5}
            icon="calendar"
            title="Registration Dates"
            subtitle="Optional — enrollment period and invoice / charge deadlines"
            bubble="bg-gray-100"
          />
          <div className="flex flex-wrap gap-4">
            <InputField
              label="Period Start"
              type="date"
              name="periodStart"
              register={register}
              error={errors.periodStart as FieldError | undefined}
            />
            <InputField
              label="Period End"
              type="date"
              name="periodEnd"
              register={register}
              error={errors.periodEnd as FieldError | undefined}
            />
            <InputField
              label="Invoice Due Date"
              type="date"
              name="invoiceDueDate"
              register={register}
              error={errors.invoiceDueDate as FieldError | undefined}
            />
            <InputField
              label="Charge Due Date"
              type="date"
              name="chargeDueDate"
              register={register}
              error={errors.chargeDueDate as FieldError | undefined}
            />
          </div>
        </SectionCard>

        {/* ── Submit row ──────────────────────────────────────────────────── */}
        <div className="flex items-center justify-between pb-2">
          <Link
            to="/list/students"
            className="text-sm text-gray-400 hover:text-gray-600 transition-colors"
          >
            ← Back to Students
          </Link>

          <button
            type="submit"
            disabled={submitting}
            className="flex items-center gap-2 bg-lamaSky hover:bg-sky-200 text-gray-700 font-semibold py-2.5 px-7 rounded-lg text-sm transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
          >
            {submitting ? (
              <>
                <div className="w-4 h-4 border-2 border-gray-500 border-t-transparent rounded-full animate-spin" />
                Registering…
              </>
            ) : (
              <>
                <img src="/create.png" alt="" width={14} height={14} />
                Register Student
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
}
