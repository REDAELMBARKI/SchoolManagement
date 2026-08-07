import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "react-router-dom";
import { useState } from "react";
import { toast } from "react-toastify";
import InputField from "@/components/InputField";
import DatePicker from "@/components/ui/DatePicker";
import DateTimePicker from "@/components/ui/DateTimePicker";
import ObjectSelect from "@/components/ui/ObjectSelect";
import PrimitiveSelect from "@/components/ui/PrimitiveSelect";
import {
  studentRegistrationSchema,
  StudentRegistrationSchema,
  PAYMENT_METHODS,
  RELATIONSHIP_TYPES,
} from "@/lib/formValidationSchemas";

// ── Mock FK data ───────────────────────────────────────────────────────────────
// These UUIDs intentionally mirror the shape of the Guid foreign keys expected
// by the API. Replace these arrays with query hooks when the API endpoints are
// connected; the form contract does not need to change.

const MOCK_LEVELS = [
  { key: "7b5df1a2-1c8b-4a91-9a52-1e5f3d4b7001", value: "Beginner · A1 Foundations" },
  { key: "7b5df1a2-1c8b-4a91-9a52-1e5f3d4b7002", value: "Elementary · A2 Everyday communication" },
  { key: "7b5df1a2-1c8b-4a91-9a52-1e5f3d4b7003", value: "Pre-Intermediate · B1 Core fluency" },
  { key: "7b5df1a2-1c8b-4a91-9a52-1e5f3d4b7004", value: "Intermediate · B1+ Confident conversation" },
  { key: "7b5df1a2-1c8b-4a91-9a52-1e5f3d4b7005", value: "Upper-Intermediate · B2 Academic and professional" },
  { key: "7b5df1a2-1c8b-4a91-9a52-1e5f3d4b7006", value: "Advanced · C1 Precision and mastery" },
];

const MOCK_SUBJECTS = [
  { key: "1d2e3f40-5a6b-47c8-9d0e-1f2a3b4c5001", value: "English · Language · 12 groups" },
  { key: "1d2e3f40-5a6b-47c8-9d0e-1f2a3b4c5002", value: "Mathematics · Sciences · 8 groups" },
  { key: "1d2e3f40-5a6b-47c8-9d0e-1f2a3b4c5003", value: "Physics · Sciences · 5 groups" },
  { key: "1d2e3f40-5a6b-47c8-9d0e-1f2a3b4c5004", value: "Chemistry · Sciences · 4 groups" },
  { key: "1d2e3f40-5a6b-47c8-9d0e-1f2a3b4c5005", value: "French · Language · 6 groups" },
];

const MOCK_PLANS = [
  { key: "2e3f4051-6a7b-48c9-0d1e-2f3a4b5c6001", value: "Monthly · 3,000 DA / month" },
  { key: "2e3f4051-6a7b-48c9-0d1e-2f3a4b5c6002", value: "Quarterly · 8,400 DA / 3 months" },
  { key: "2e3f4051-6a7b-48c9-0d1e-2f3a4b5c6003", value: "Annual · 30,000 DA / year" },
  { key: "2e3f4051-6a7b-48c9-0d1e-2f3a4b5c6004", value: "Intensive · 12,000 DA accelerated" },
];

const MOCK_GROUPS = [
  { key: "", value: "No group preference" },
  { key: "3f405162-7a8b-49d0-1e2f-3a4b5c6d7001", value: "Group A · Morning · 08:00–10:00" },
  { key: "3f405162-7a8b-49d0-1e2f-3a4b5c6d7002", value: "Group B · Afternoon · 14:00–16:00" },
  { key: "3f405162-7a8b-49d0-1e2f-3a4b5c6d7003", value: "Group C · Evening · 18:00–20:00" },
  { key: "3f405162-7a8b-49d0-1e2f-3a4b5c6d7004", value: "Group D · Weekend · 09:00–12:00" },
];

const MOCK_GENDERS = [
  { key: "", value: "Not specified" },
  { key: "4a516273-8b9c-40e1-2f3a-4b5c6d7e8001", value: "Male" },
  { key: "4a516273-8b9c-40e1-2f3a-4b5c6d7e8002", value: "Female" },
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

// ── Shared layout sub-components ───────────────────────────────────────────────

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

// ── Page ──────────────────────────────────────────────────────────────────────

export default function StudentRegistrationPage() {
  const [submitting, setSubmitting] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<StudentRegistrationSchema>({
    resolver: zodResolver(studentRegistrationSchema),
    defaultValues: {
      student: { email: "", genderId: "" },
      enrollment: { subjectId: "", planId: "", preferedGroupId: "", notes: "" },
      payment:  { method: "Cash", amountPaid: undefined, transferFees: undefined, paidAt: "" },
      hasGuardian: false,
      responsable: {
        firstName: "", lastName: "", email: "", phone: "", relationship: "", genderId: "",
      },
      periodStart: "", periodEnd: "", invoiceDueDate: "", chargeDueDate: "",
    },
  });

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
        isDirectRegistration: true,
      },
      enrollmentRegReq: {
        // StudentId will be assigned by the backend after student creation.
        // The service creates the student before enrollment; this sentinel
        // keeps the preview payload explicit until the API flow is connected.
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

    // Preview-only until the registration endpoint is connected.
    console.log("Student registration payload:", dto);
    await new Promise((r) => setTimeout(r, 700));

    setSubmitting(false);
    toast.success("Registration payload ready — API connection pending.");
  });

  // Cast nested errors to FieldError to satisfy InputField's prop type.
  const se  = errors.student   as any;
  const ee  = errors.enrollment as any;
  const pe  = errors.payment    as any;
  const re  = errors.responsable as any;

  return (
    <div
      className="min-h-full flex flex-col gap-5 p-4 md:p-6 xl:p-8 mt-0"
      style={{
        backgroundColor: "#fbf8f2",
        backgroundImage:
          "radial-gradient(#e9dfcf 0.7px, transparent 0.7px), linear-gradient(135deg, rgba(255,255,255,.55), rgba(245,238,225,.5))",
        backgroundSize: "12px 12px, 100% 100%",
      }}
    >

      {/* ── Page header ─────────────────────────────────────────────────── */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-3">
          <div className="w-11 h-11 rounded-2xl bg-[#e4f5fa] border border-[#c6e9f3] flex items-center justify-center flex-shrink-0 shadow-sm">
            <img src="/student.png" alt="" width={20} height={20} />
          </div>
          <div>
            <div className="flex items-center gap-2">
              <span className="uppercase tracking-[0.16em] text-[10px] font-bold text-[#9b8d78]">Admissions</span>
              <span className="h-1 w-1 rounded-full bg-[#d7c4a8]" />
              <span className="text-[10px] text-[#aa9d8a]">New record</span>
            </div>
            <h1 className="text-2xl font-semibold text-[#3f4548] leading-tight">
              New Student Registration
            </h1>
            <p className="text-xs text-[#948a7c] mt-1">
              Capture the student, enrollment, payment and guardian details in one place.
            </p>
          </div>
        </div>
        <Link
          to="/list/students"
          className="self-start sm:self-auto flex items-center gap-1.5 text-xs font-medium text-[#8f8577] hover:text-[#4f5e62] transition-colors rounded-lg px-3 py-2 hover:bg-white/70"
        >
          <img src="/close.png" alt="" width={11} height={11} />
          Cancel
        </Link>
      </div>

      <form onSubmit={onSubmit} className="flex flex-col gap-4" noValidate>

        {/* ══ SECTION 1 — Student Information ══════════════════════════════ */}
        <SectionCard accent="border-[#9ddced]">
          <SectionHeader
            n={1}
            icon="student"
            title="Student Information"
            subtitle="Personal details and identity"
            bubble="bg-lamaSky"
          />

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
            <Controller
              name="student.dateOfBirth"
              control={control}
              render={({ field, fieldState }) => (
                <DatePicker
                  label="Date of Birth"
                  required
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  error={fieldState.error?.message}
                />
              )}
            />
            <Controller
              name="student.genderId"
              control={control}
              render={({ field, fieldState }) => (
                <ObjectSelect
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  label="Gender"
                  options={MOCK_GENDERS}
                  isMulty={false}
                  placeholder="Not specified"
                  error={fieldState.error?.message}
                  className="w-full md:w-1/4"
                />
              )}
            />
            <Controller
              name="student.levelId"
              control={control}
              render={({ field, fieldState }) => (
                <ObjectSelect
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  label="Level *"
                  options={MOCK_LEVELS}
                  isMulty={false}
                  placeholder="Select level"
                  error={fieldState.error?.message}
                  className="w-full md:w-1/4"
                />
              )}
            />
          </div>
        </SectionCard>

        {/* ══ SECTION 2 — Enrollment ════════════════════════════════════════ */}
        <SectionCard accent="border-[#c6c4f4]">
          <SectionHeader
            n={2}
            icon="subject"
            title="Enrollment"
            subtitle="Subject, pricing plan and preferred group"
            bubble="bg-lamaPurple"
          />
          <div className="flex flex-wrap gap-4">
            <Controller
              name="enrollment.subjectId"
              control={control}
              render={({ field, fieldState }) => (
                <ObjectSelect
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  label="Subject *"
                  options={MOCK_SUBJECTS}
                  isMulty={false}
                  placeholder="Select subject"
                  error={fieldState.error?.message}
                  className="w-full md:w-1/4"
                />
              )}
            />

            <Controller
              name="enrollment.planId"
              control={control}
              render={({ field, fieldState }) => (
                <ObjectSelect
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  label="Pricing Plan *"
                  options={MOCK_PLANS}
                  isMulty={false}
                  placeholder="Select plan"
                  error={fieldState.error?.message}
                  className="w-full md:w-1/4"
                />
              )}
            />

            <Controller
              name="enrollment.preferedGroupId"
              control={control}
              render={({ field, fieldState }) => (
                <ObjectSelect
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  label="Preferred Group"
                  options={MOCK_GROUPS}
                  isMulty={false}
                  placeholder="No group preference"
                  error={fieldState.error?.message}
                  className="w-full md:w-1/4"
                />
              )}
            />

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
        <SectionCard accent="border-[#e9d47b]">
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
            <Controller
              name="payment.paidAt"
              control={control}
              render={({ field, fieldState }) => (
                <DateTimePicker
                  label="Payment Date & Time"
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  error={fieldState.error?.message}
                />
              )}
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
        <SectionCard accent={hasGuardian ? "border-[#c6c4f4]" : "border-[#e5ddd0]"}>
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
              <Controller
                name="responsable.relationship"
                control={control}
                render={({ field, fieldState }) => (
                  <PrimitiveSelect
                    value={field.value}
                    onChange={field.onChange}
                    onBlur={field.onBlur}
                    label="Relationship *"
                    options={RELATIONSHIP_TYPES}
                    isMulty={false}
                    placeholder="Select relationship"
                    error={fieldState.error?.message}
                    className="w-full md:w-1/4"
                  />
                )}
              />
              <Controller
                name="responsable.genderId"
                control={control}
                render={({ field, fieldState }) => (
                  <ObjectSelect
                    value={field.value}
                    onChange={field.onChange}
                    onBlur={field.onBlur}
                    label="Gender"
                    options={MOCK_GENDERS}
                    isMulty={false}
                    placeholder="Not specified"
                    error={fieldState.error?.message}
                    className="w-full md:w-1/4"
                  />
                )}
              />
            </div>
          ) : (
            <p className="text-xs text-gray-400 italic">
              Toggle on to add a parent or guardian for this student.
            </p>
          )}
        </SectionCard>

        {/* ══ SECTION 5 — Registration Dates (optional) ════════════════════ */}
        <SectionCard accent="border-[#e5ddd0]">
          <SectionHeader
            n={5}
            icon="calendar"
            title="Registration Dates"
            subtitle="Optional — enrollment period and invoice / charge deadlines"
            bubble="bg-gray-100"
          />
          <div className="flex flex-wrap gap-4">
            <Controller
              name="periodStart"
              control={control}
              render={({ field, fieldState }) => (
                <DatePicker
                  label="Period Start"
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  error={fieldState.error?.message}
                />
              )}
            />
            <Controller
              name="periodEnd"
              control={control}
              render={({ field, fieldState }) => (
                <DatePicker
                  label="Period End"
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  error={fieldState.error?.message}
                />
              )}
            />
            <Controller
              name="invoiceDueDate"
              control={control}
              render={({ field, fieldState }) => (
                <DatePicker
                  label="Invoice Due Date"
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  error={fieldState.error?.message}
                />
              )}
            />
            <Controller
              name="chargeDueDate"
              control={control}
              render={({ field, fieldState }) => (
                <DatePicker
                  label="Charge Due Date"
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  error={fieldState.error?.message}
                />
              )}
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
