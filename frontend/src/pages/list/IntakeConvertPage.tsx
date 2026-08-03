import { useNavigate, useParams, Link } from "react-router-dom";
import { intakesData } from "@/lib/data";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import InputField from "@/components/InputField";
import { useState } from "react";
import { toast } from "react-toastify";
import {
  intakeConvertSchema,
  IntakeConvertSchema,
} from "@/lib/formValidationSchemas";
import { convertIntakeToStudent } from "@/lib/actions";

const RELATIONSHIP_OPTIONS = [
  "Father",
  "Mother",
  "Guardian",
  "Grandfather",
  "Grandmother",
  "Uncle",
  "Aunt",
  "Other",
];

const IntakeConvertPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  // Mock fetch by ID — replace with real API call when backend is ready
  const intake = intakesData.find((i) => String(i.id) === id);

  const [showGuardian, setShowGuardian] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IntakeConvertSchema>({
    resolver: zodResolver(intakeConvertSchema),
    defaultValues: {
      guardianRelationship: "Father",
    },
  });

  if (!intake) {
    return (
      <div className="bg-white p-8 rounded-md m-4 flex flex-col items-center gap-4">
        <img src="/student.png" alt="" width={48} height={48} className="opacity-30" />
        <p className="text-gray-500 text-sm">Intake not found.</p>
        <Link
          to="/list/intakes"
          className="text-blue-500 text-sm hover:underline flex items-center gap-1"
        >
          <img src="/lesson.png" alt="" width={14} height={14} />
          Back to Intakes
        </Link>
      </div>
    );
  }

  const onSubmit = handleSubmit(async (formData) => {
    setIsSubmitting(true);
    const result = await convertIntakeToStudent(
      { success: false, error: false },
      { ...formData, intakeId: intake.id }
    );
    setIsSubmitting(false);
    if (result.success) {
      toast.success("Student account created successfully!");
      navigate("/list/intakes");
    } else {
      toast.error("Something went wrong. Please try again.");
    }
  });

  return (
    <div className="m-4 flex flex-col gap-4">
      {/* Breadcrumb */}
      <div className="flex items-center gap-2 text-xs text-gray-400">
        <Link to="/list/intakes" className="hover:text-gray-600 flex items-center gap-1">
          <img src="/lesson.png" alt="" width={12} height={12} />
          Intakes
        </Link>
        <span>/</span>
        <span className="text-gray-600">Convert to Student</span>
      </div>

      <div className="flex items-center justify-between">
        <h1 className="text-lg font-semibold text-gray-700">
          Convert Intake to Student
        </h1>
        <Link
          to="/list/intakes"
          className="flex items-center gap-2 text-xs text-gray-500 hover:text-gray-700 bg-white border border-gray-200 px-3 py-2 rounded-md"
        >
          <img src="/lesson.png" alt="" width={14} height={14} />
          Back to Intakes
        </Link>
      </div>

      {/* Intake Info Banner */}
      <div className="bg-lamaSkyLight border border-sky-200 rounded-md p-4 flex flex-col md:flex-row md:items-center gap-4">
        {/* Avatar */}
        <div className="w-14 h-14 rounded-full bg-lamaSky flex items-center justify-center text-white font-bold text-xl flex-shrink-0">
          {intake.firstName[0]}
          {intake.lastName[0]}
        </div>
        <div className="flex flex-col gap-1 flex-1">
          <p className="font-semibold text-gray-700 text-sm">
            {intake.firstName} {intake.lastName}
          </p>
          <div className="flex flex-wrap gap-x-6 gap-y-1 text-xs text-gray-500">
            <span className="flex items-center gap-1">
              <img src="/mail.png" alt="" width={12} height={12} />
              {intake.email}
            </span>
            <span className="flex items-center gap-1">
              <img src="/phone.png" alt="" width={12} height={12} />
              {intake.phone}
            </span>
            <span className="flex items-center gap-1">
              <img src="/date.png" alt="" width={12} height={12} />
              DOB: {intake.dateOfBirth}
            </span>
            <span className="flex items-center gap-1">
              <img src="/maleFemale.png" alt="" width={12} height={12} />
              {intake.gender}
            </span>
          </div>
        </div>
        <div className="text-xs bg-blue-100 text-blue-600 px-3 py-1 rounded-full font-medium self-start md:self-center">
          Converting to Student
        </div>
      </div>

      <div className="bg-blue-50 border border-blue-100 rounded-md px-4 py-3 text-xs text-blue-700 flex items-start gap-2">
        <span className="mt-0.5 text-base leading-none">ℹ️</span>
        <span>
          This will create a student account for{" "}
          <strong>
            {intake.firstName} {intake.lastName}
          </strong>{" "}
          linked to this intake. Personal details are pre-filled from the intake
          record and will be transferred automatically.
        </span>
      </div>

      <form onSubmit={onSubmit} className="flex flex-col gap-6">
        {/* Section: Login Credentials */}
        <div className="bg-white rounded-md p-6 flex flex-col gap-4">
          <span className="text-xs text-gray-400 font-medium uppercase tracking-wide">
            Set Login Credentials
          </span>
          <div className="flex flex-wrap gap-4">
            <InputField
              label="Username *"
              name="username"
              register={register}
              error={errors.username}
              inputProps={{ placeholder: "e.g. ahmed.benali" }}
            />
            <InputField
              label="Password *"
              name="password"
              type="password"
              register={register}
              error={errors.password}
              inputProps={{ placeholder: "Min. 8 characters" }}
            />
          </div>
        </div>

        {/* Section: Guardian (optional) */}
        <div className="bg-white rounded-md p-6 flex flex-col gap-4">
          <div className="flex items-center justify-between">
            <span className="text-xs text-gray-400 font-medium uppercase tracking-wide">
              Guardian / Parent
            </span>
            <button
              type="button"
              onClick={() => setShowGuardian((v) => !v)}
              className="text-xs text-blue-500 hover:text-blue-700 flex items-center gap-1 font-medium"
            >
              {showGuardian ? "− Remove Guardian" : "+ Add Guardian (optional)"}
            </button>
          </div>

          {!showGuardian && (
            <p className="text-xs text-gray-400 italic">
              No guardian will be linked. You can add one later from the student
              profile.
            </p>
          )}

          {showGuardian && (
            <div className="flex flex-wrap gap-4">
              <InputField
                label="First Name"
                name="guardianFirstName"
                register={register}
                error={errors.guardianFirstName}
                inputProps={{ placeholder: "Guardian first name" }}
              />
              <InputField
                label="Last Name"
                name="guardianLastName"
                register={register}
                error={errors.guardianLastName}
                inputProps={{ placeholder: "Guardian last name" }}
              />
              <InputField
                label="Phone *"
                name="guardianPhone"
                register={register}
                error={errors.guardianPhone}
                inputProps={{ placeholder: "+213 550 ..." }}
              />
              <InputField
                label="Email"
                name="guardianEmail"
                type="email"
                register={register}
                error={errors.guardianEmail}
                inputProps={{ placeholder: "guardian@example.com" }}
              />
              <div className="flex flex-col gap-2 w-full md:w-1/4">
                <label className="text-xs text-gray-500">Relationship</label>
                <select
                  className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full"
                  {...register("guardianRelationship")}
                >
                  {RELATIONSHIP_OPTIONS.map((r) => (
                    <option key={r} value={r}>
                      {r}
                    </option>
                  ))}
                </select>
                {errors.guardianRelationship?.message && (
                  <p className="text-xs text-red-400">
                    {errors.guardianRelationship.message.toString()}
                  </p>
                )}
              </div>
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex items-center justify-end gap-3">
          <Link
            to="/list/intakes"
            className="px-5 py-2 text-sm text-gray-600 bg-white border border-gray-200 rounded-md hover:bg-gray-50"
          >
            Cancel
          </Link>
          <button
            type="submit"
            disabled={isSubmitting}
            className="flex items-center gap-2 px-5 py-2 text-sm text-white bg-blue-400 rounded-md hover:bg-blue-500 disabled:opacity-60 disabled:cursor-not-allowed"
          >
            {isSubmitting ? (
              <>
                <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                Creating…
              </>
            ) : (
              <>
                <img src="/student.png" alt="" width={16} height={16} className="brightness-0 invert" />
                Create Student Account
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default IntakeConvertPage;
