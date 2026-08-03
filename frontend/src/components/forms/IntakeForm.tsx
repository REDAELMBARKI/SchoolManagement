import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import InputField from "../InputField";
import { Dispatch, SetStateAction, useEffect, useState } from "react";
import { toast } from "react-toastify";
import {
  intakeSchema,
  IntakeSchema,
  INTAKE_STATUSES,
} from "@/lib/formValidationSchemas";

// ── Section header ──────────────────────────────────────────────────────────
const SectionTitle = ({ icon, label }: { icon: string; label: string }) => (
  <div className="flex items-center gap-2 mb-2">
    <img src={`/${icon}.png`} alt="" width={14} height={14} className="opacity-60" />
    <span className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
      {label}
    </span>
  </div>
);

// ── Select field ─────────────────────────────────────────────────────────────
type SelectFieldProps = {
  label: string;
  name: string;
  register: any;
  error?: any;
  children: React.ReactNode;
  widthClass?: string;
};
const SelectField = ({
  label,
  name,
  register,
  error,
  children,
  widthClass = "w-full md:w-1/4",
}: SelectFieldProps) => (
  <div className={`flex flex-col gap-2 ${widthClass}`}>
    <label className="text-xs text-gray-500">{label}</label>
    <select
      className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full bg-white"
      {...register(name)}
    >
      {children}
    </select>
    {error?.message && (
      <p className="text-xs text-red-400">{error.message.toString()}</p>
    )}
  </div>
);

// ── Main form ────────────────────────────────────────────────────────────────
const IntakeForm = ({
  type,
  data,
  setOpen,
  relatedData,
}: {
  type: "create" | "update";
  data?: any;
  setOpen: Dispatch<SetStateAction<boolean>>;
  relatedData?: any;
}) => {
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<IntakeSchema>({
    resolver: zodResolver(intakeSchema),
    defaultValues: {
      id: data?.id ? String(data.id) : undefined,
      firstName: data?.firstName ?? "",
      lastName: data?.lastName ?? "",
      email: data?.email ?? "",
      phone: data?.phone ?? "",
      dateOfBirth: data?.dateOfBirth
        ? String(data.dateOfBirth).slice(0, 10)
        : "",
      genderId: data?.genderId ?? "",
      subjectId: data?.subjectId ?? "",
      branchId: data?.branchId ?? "",
      intakeDate: data?.intakeDate
        ? String(data.intakeDate).slice(0, 10)
        : new Date().toISOString().slice(0, 10),
      status: data?.status ?? "New",
      followUpDate: data?.followUpDate
        ? String(data.followUpDate).slice(0, 10)
        : "",
      notes: data?.notes ?? "",
      isIndependent: data?.isIndependent ?? false,
      leadSourceType: data?.leadSourceType ?? "",
      leadSourceId: data?.leadSourceId ?? "",
      commercialAgentId: data?.commercialAgentId ?? "",
      totalFees: data?.totalFees ?? 0,
      amountPaid: data?.amountPaid ?? 0,
    },
  });

  const [submitting, setSubmitting] = useState(false);
  const [formState, setFormState] = useState({ success: false, error: false });

  const isIndependent = watch("isIndependent");
  const totalFees = watch("totalFees") ?? 0;
  const amountPaid = watch("amountPaid") ?? 0;
  const remaining = Math.max(0, Number(totalFees) - Number(amountPaid));

  const onSubmit = handleSubmit(async (formData) => {
    setSubmitting(true);

    // Build the request DTO that matches the backend IntakeCommand / UpdateIntakeCommand shape.
    // LeadSourceRequestDto only accepts { sourceType: "Opc" | "Ad", sourceId: Guid }.
    const requestDto = {
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email || null,
      phone: formData.phone || null,
      dateOfBirth: formData.dateOfBirth || null,
      genderId: formData.genderId || null,
      subjectId: formData.subjectId,
      branchId: formData.branchId || null,
      intakeDate: formData.intakeDate,
      status: formData.status,
      followUpDate: formData.followUpDate || null,
      notes: formData.notes || null,
      isIndependent: formData.isIndependent,
      // Nested leadSource object — only present when not independent and both parts are set
      leadSource:
        !formData.isIndependent &&
        formData.leadSourceType &&
        formData.leadSourceId
          ? {
              sourceType: formData.leadSourceType as "Opc" | "Ad",
              sourceId: formData.leadSourceId,
            }
          : null,
      commercialAgentId: formData.commercialAgentId || null,
      totalFees: formData.totalFees,
      amountPaid: formData.amountPaid,
    };

    // TODO (Task #2): replace with real API call —
    //   type === "create"
    //     ? api.post("/api/intakes", requestDto)
    //     : api.put(`/api/intakes/${formData.id}`, requestDto)
    console.log(`Intake ${type} payload:`, requestDto);
    await new Promise((r) => setTimeout(r, 400));

    setSubmitting(false);
    setFormState({ success: true, error: false });
  });

  useEffect(() => {
    if (formState.success) {
      toast.success(`Intake ${type === "create" ? "created" : "updated"} successfully!`);
      setOpen(false);
    }
  }, [formState.success, type, setOpen]);

  const {
    subjects = [],
    branches = [],
    genders = [],
    leadSources = [],
    commercialAgents = [],
  } = relatedData ?? {};

  const todayStr = new Date().toISOString().slice(0, 10);

  return (
    <div className="overflow-y-auto max-h-[75vh] pr-1">
      <form
        onSubmit={(e) => {
          e.preventDefault();
          onSubmit();
        }}
        className="flex flex-col gap-6"
      >
        {/* ── Hidden ID ──────────────────────────────────────────── */}
        {data?.id && (
          <InputField
            label="Id"
            name="id"
            defaultValue={String(data.id)}
            register={register}
            error={errors.id}
            hidden
          />
        )}

        {/* ── 1. Personal Information ────────────────────────────── */}
        <div className="flex flex-col gap-4">
          <SectionTitle icon="profile" label="Personal Information" />
          <div className="flex flex-wrap gap-4">
            <InputField
              label="First Name *"
              name="firstName"
              register={register}
              error={errors.firstName}
              inputProps={{ placeholder: "e.g. Ahmed" }}
            />
            <InputField
              label="Last Name *"
              name="lastName"
              register={register}
              error={errors.lastName}
              inputProps={{ placeholder: "e.g. Benali" }}
            />
            <InputField
              label="Email"
              name="email"
              type="email"
              register={register}
              error={errors.email}
              inputProps={{ placeholder: "example@email.com" }}
            />
            <InputField
              label="Phone"
              name="phone"
              register={register}
              error={errors.phone}
              inputProps={{ placeholder: "+213 550 ..." }}
            />
            <InputField
              label="Date of Birth"
              name="dateOfBirth"
              type="date"
              register={register}
              error={errors.dateOfBirth}
            />
            {genders.length > 0 ? (
              <SelectField
                label="Gender"
                name="genderId"
                register={register}
                error={errors.genderId}
              >
                <option value="">Select gender</option>
                {genders.map((g: { id: string; name: string }) => (
                  <option key={g.id} value={g.id}>
                    {g.name}
                  </option>
                ))}
              </SelectField>
            ) : (
              <SelectField
                label="Gender"
                name="genderId"
                register={register}
                error={errors.genderId}
              >
                <option value="">Select gender</option>
                <option value="male">Male</option>
                <option value="female">Female</option>
                <option value="other">Other</option>
              </SelectField>
            )}
          </div>
        </div>

        {/* ── 2. Enrollment Details ─────────────────────────────── */}
        <div className="flex flex-col gap-4">
          <SectionTitle icon="subject" label="Enrollment Details" />
          <div className="flex flex-wrap gap-4">
            <SelectField
              label="Subject *"
              name="subjectId"
              register={register}
              error={errors.subjectId}
            >
              <option value="">Select subject</option>
              {subjects.map((s: { id: string; name: string }) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </SelectField>

            {branches.length > 0 && (
              <SelectField
                label="Branch"
                name="branchId"
                register={register}
                error={errors.branchId}
              >
                <option value="">Select branch</option>
                {branches.map((b: { id: string; name: string }) => (
                  <option key={b.id} value={b.id}>
                    {b.name}
                  </option>
                ))}
              </SelectField>
            )}

            <InputField
              label="Intake Date *"
              name="intakeDate"
              type="date"
              register={register}
              error={errors.intakeDate}
              inputProps={{ max: todayStr }}
            />

            <SelectField
              label="Status"
              name="status"
              register={register}
              error={errors.status}
            >
              {INTAKE_STATUSES.map((s) => (
                <option key={s} value={s}>
                  {s === "NotInterested" ? "Not Interested" : s}
                </option>
              ))}
            </SelectField>

            <InputField
              label="Follow-up Date"
              name="followUpDate"
              type="date"
              register={register}
              error={errors.followUpDate}
            />
          </div>

          {/* Is Independent toggle — clears source fields when turned on */}
          <div className="flex items-center gap-3 p-3 bg-lamaSkyLight border border-sky-100 rounded-md w-fit">
            <input
              type="checkbox"
              id="isIndependent"
              checked={isIndependent}
              onChange={(e) => {
                setValue("isIndependent", e.target.checked, { shouldValidate: true });
                if (e.target.checked) {
                  // Clear all lead-source–related fields so the payload is semantically clean
                  setValue("leadSourceType", "", { shouldValidate: false });
                  setValue("leadSourceId", "", { shouldValidate: false });
                  setValue("commercialAgentId", "", { shouldValidate: false });
                }
              }}
              className="w-4 h-4 accent-sky-400 cursor-pointer"
            />
            <label
              htmlFor="isIndependent"
              className="text-xs text-gray-600 font-medium cursor-pointer select-none"
            >
              Independent intake (no lead source)
            </label>
          </div>
        </div>

        {/* ── 3. Lead Source (shown when NOT independent) ───────── */}
        {!isIndependent && (
          <div className="flex flex-col gap-4">
            <SectionTitle icon="more" label="Lead Source" />
            <div className="flex flex-wrap gap-4">
              {/* Only the two values the backend LeadSourceType enum accepts */}
              <SelectField
                label="Source Type *"
                name="leadSourceType"
                register={register}
                error={errors.leadSourceType}
              >
                <option value="">Select type</option>
                <option value="Opc">OPC</option>
                <option value="Ad">Ad / Advertisement</option>
              </SelectField>

              {leadSources.length > 0 ? (
                <SelectField
                  label="Lead Source"
                  name="leadSourceId"
                  register={register}
                  error={errors.leadSourceId}
                >
                  <option value="">Select lead source</option>
                  {leadSources.map((ls: { id: string; name: string }) => (
                    <option key={ls.id} value={ls.id}>
                      {ls.name}
                    </option>
                  ))}
                </SelectField>
              ) : (
                <InputField
                  label="Lead Source ID"
                  name="leadSourceId"
                  register={register}
                  error={errors.leadSourceId}
                  inputProps={{ placeholder: "Source identifier" }}
                />
              )}

              {commercialAgents.length > 0 && (
                <SelectField
                  label="Commercial Agent"
                  name="commercialAgentId"
                  register={register}
                  error={errors.commercialAgentId}
                >
                  <option value="">Select agent (optional)</option>
                  {commercialAgents.map((a: { id: string; name: string }) => (
                    <option key={a.id} value={a.id}>
                      {a.name}
                    </option>
                  ))}
                </SelectField>
              )}
            </div>
          </div>
        )}

        {/* ── 4. Financial Information ──────────────────────────── */}
        <div className="flex flex-col gap-4">
          <SectionTitle icon="finance" label="Financial Information" />
          <div className="flex flex-wrap gap-4">
            <InputField
              label="Total Fees *"
              name="totalFees"
              type="number"
              register={register}
              error={errors.totalFees}
              inputProps={{ placeholder: "0.00", min: "0", step: "0.01" }}
            />
            <InputField
              label="Amount Paid"
              name="amountPaid"
              type="number"
              register={register}
              error={errors.amountPaid}
              inputProps={{ placeholder: "0.00", min: "0", step: "0.01" }}
            />
            {/* Remaining display */}
            <div className="flex flex-col gap-2 w-full md:w-1/4">
              <label className="text-xs text-gray-500">Remaining</label>
              <div
                className={`ring-[1.5px] p-2 rounded-md text-sm font-medium ${
                  remaining > 0
                    ? "ring-lamaYellow bg-lamaYellowLight text-yellow-700"
                    : "ring-green-200 bg-green-50 text-green-700"
                }`}
              >
                {remaining > 0 ? `${remaining.toLocaleString()} remaining` : "Fully paid ✓"}
              </div>
            </div>
          </div>
        </div>

        {/* ── 5. Notes ──────────────────────────────────────────── */}
        <div className="flex flex-col gap-2">
          <label className="text-xs text-gray-500 flex items-center gap-1">
            <img src="/more.png" alt="" width={12} height={12} className="opacity-50" />
            Notes
          </label>
          <textarea
            {...register("notes")}
            rows={3}
            className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full resize-none"
            placeholder="Any additional notes about this intake..."
          />
        </div>

        {/* ── Error / Submit ─────────────────────────────────────── */}
        {formState.error && (
          <p className="text-xs text-red-500 text-center">
            Something went wrong. Please try again.
          </p>
        )}

        <button
          type="submit"
          disabled={submitting}
          className="flex items-center justify-center gap-2 bg-lamaSky hover:bg-sky-200 text-gray-700 font-medium py-2 px-4 rounded-md text-sm transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
        >
          {submitting ? (
            <>
              <span className="w-4 h-4 border-2 border-gray-500 border-t-transparent rounded-full animate-spin" />
              Saving…
            </>
          ) : (
            <>
              <img
                src={type === "create" ? "/create.png" : "/update.png"}
                alt=""
                width={14}
                height={14}
              />
              {type === "create" ? "Create Intake" : "Update Intake"}
            </>
          )}
        </button>
      </form>
    </div>
  );
};

export default IntakeForm;
