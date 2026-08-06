import { z } from "zod";

export const subjectSchema = z.object({
  id: z.coerce.number().optional(),
  name: z.string().min(1, { message: "Subject name is required!" }),
  teachers: z.array(z.string()), //teacher ids
});

export type SubjectSchema = z.infer<typeof subjectSchema>;

export const classSchema = z.object({
  id: z.coerce.number().optional(),
  name: z.string().min(1, { message: "Subject name is required!" }),
  capacity: z.coerce.number().min(1, { message: "Capacity name is required!" }),
  gradeId: z.coerce.number().min(1, { message: "Grade name is required!" }),
  supervisorId: z.coerce.string().optional(),
});

export type ClassSchema = z.infer<typeof classSchema>;

export const teacherSchema = z.object({
  id: z.string().optional(),
  username: z
    .string()
    .min(3, { message: "Username must be at least 3 characters long!" })
    .max(20, { message: "Username must be at most 20 characters long!" }),
  password: z
    .string()
    .min(8, { message: "Password must be at least 8 characters long!" })
    .optional()
    .or(z.literal("")),
  name: z.string().min(1, { message: "First name is required!" }),
  surname: z.string().min(1, { message: "Last name is required!" }),
  email: z
    .string()
    .email({ message: "Invalid email address!" })
    .optional()
    .or(z.literal("")),
  phone: z.string().optional(),
  address: z.string(),
  img: z.string().optional(),
  bloodType: z.string().min(1, { message: "Blood Type is required!" }),
  birthday: z.coerce.date({ message: "Birthday is required!" }),
  sex: z.enum(["MALE", "FEMALE"], { message: "Sex is required!" }),
  subjects: z.array(z.string()).optional(), // subject ids
});

export type TeacherSchema = z.infer<typeof teacherSchema>;

export const studentSchema = z.object({
  id: z.string().optional(),
  username: z
    .string()
    .min(3, { message: "Username must be at least 3 characters long!" })
    .max(20, { message: "Username must be at most 20 characters long!" }),
  password: z
    .string()
    .min(8, { message: "Password must be at least 8 characters long!" })
    .optional()
    .or(z.literal("")),
  name: z.string().min(1, { message: "First name is required!" }),
  surname: z.string().min(1, { message: "Last name is required!" }),
  email: z
    .string()
    .email({ message: "Invalid email address!" })
    .optional()
    .or(z.literal("")),
  phone: z.string().optional(),
  address: z.string(),
  img: z.string().optional(),
  bloodType: z.string().min(1, { message: "Blood Type is required!" }),
  birthday: z.coerce.date({ message: "Birthday is required!" }),
  sex: z.enum(["MALE", "FEMALE"], { message: "Sex is required!" }),
  gradeId: z.coerce.number().min(1, { message: "Grade is required!" }),
  classId: z.coerce.number().min(1, { message: "Class is required!" }),
  parentId: z.string().min(1, { message: "Parent Id is required!" }),
});

export type StudentSchema = z.infer<typeof studentSchema>;

export const intakeConvertSchema = z
  .object({
    username: z
      .string()
      .min(3, { message: "Username must be at least 3 characters long!" })
      .max(20, { message: "Username must be at most 20 characters long!" }),
    password: z
      .string()
      .min(8, { message: "Password must be at least 8 characters long!" }),
    guardianFirstName: z.string().optional(),
    guardianLastName: z.string().optional(),
    guardianPhone: z.string().optional(),
    guardianEmail: z
      .string()
      .email({ message: "Invalid email address!" })
      .optional()
      .or(z.literal("")),
    guardianRelationship: z
      .enum([
        "Father",
        "Mother",
        "Guardian",
        "Grandfather",
        "Grandmother",
        "Uncle",
        "Aunt",
        "Other",
      ])
      .optional(),
  })
  .superRefine((data, ctx) => {
    const hasGuardianInfo =
      data.guardianFirstName || data.guardianLastName || data.guardianEmail;
    if (hasGuardianInfo && !data.guardianPhone) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Guardian phone is required when adding a guardian",
        path: ["guardianPhone"],
      });
    }
  });

export type IntakeConvertSchema = z.infer<typeof intakeConvertSchema>;

// ── Intake ──────────────────────────────────────────────────────────────────

export const INTAKE_STATUSES = [
  "New",
  "Contacted",
  "Interested",
  "Enrolled",
  "NotInterested",
] as const;

export type IntakeStatus = (typeof INTAKE_STATUSES)[number];

export const intakeSchema = z
  .object({
    id: z.string().optional(),
    firstName: z
      .string()
      .min(3, { message: "First name must be at least 3 characters!" })
      .max(50, { message: "First name must be at most 50 characters!" }),
    lastName: z
      .string()
      .min(3, { message: "Last name must be at least 3 characters!" })
      .max(50, { message: "Last name must be at most 50 characters!" }),
    email: z
      .string()
      .email({ message: "Invalid email address!" })
      .max(255)
      .optional()
      .or(z.literal("")),
    phone: z
      .string()
      .max(20, { message: "Phone must be at most 20 characters!" })
      .optional()
      .or(z.literal("")),
    dateOfBirth: z.string().optional().or(z.literal("")),
    genderId: z.string().optional().or(z.literal("")),
    subjectId: z.string().min(1, { message: "Subject is required!" }),
    branchId: z.string().optional().or(z.literal("")),
    intakeDate: z.string().min(1, { message: "Intake date is required!" }),
    status: z.enum(INTAKE_STATUSES, { message: "Status is required!" }),
    followUpDate: z.string().optional().or(z.literal("")),
    notes: z.string().optional().or(z.literal("")),
    isIndependent: z.boolean(),
    // Backend LeadSourceType enum only accepts "Opc" | "Ad"
    leadSourceType: z
      .enum(["Opc", "Ad"] as const)
      .optional()
      .or(z.literal("")),
    leadSourceId: z.string().optional().or(z.literal("")),
    commercialAgentId: z.string().optional().or(z.literal("")),
    totalFees: z.coerce
      .number({ invalid_type_error: "Total fees must be a number!" })
      .positive({ message: "Total fees must be greater than 0!" }),
    amountPaid: z.coerce
      .number({ invalid_type_error: "Amount paid must be a number!" })
      .min(0, { message: "Amount paid must be 0 or more!" }),
  })
  .refine(
    (data) => {
      if (data.followUpDate && data.intakeDate && data.followUpDate <= data.intakeDate)
        return false;
      return true;
    },
    { message: "Follow-up date must be after intake date!", path: ["followUpDate"] }
  )
  .refine(
    (data) => {
      if (data.amountPaid > data.totalFees) return false;
      return true;
    },
    { message: "Amount paid cannot exceed total fees!", path: ["amountPaid"] }
  )
  .refine(
    (data) => {
      // When the intake is NOT independent, a lead source must be identified
      if (!data.isIndependent && (!data.leadSourceId || data.leadSourceId.trim() === ""))
        return false;
      return true;
    },
    {
      message: "Lead source is required for non-independent intakes!",
      path: ["leadSourceId"],
    }
  );

export type IntakeSchema = z.infer<typeof intakeSchema>;

// ── Exam ─────────────────────────────────────────────────────────────────────

export const examSchema = z.object({
  id: z.coerce.number().optional(),
  title: z.string().min(1, { message: "Title name is required!" }),
  startTime: z.coerce.date({ message: "Start time is required!" }),
  endTime: z.coerce.date({ message: "End time is required!" }),
  lessonId: z.coerce.number({ message: "Lesson is required!" }),
});

export type ExamSchema = z.infer<typeof examSchema>;

// ── Student Registration ──────────────────────────────────────────────────────
// Mirrors StudentRegistrationRequestDto + sub-DTOs from the backend.

export const PAYMENT_METHODS = [
  "Cash",
  "CreditCard",
  "DebitCard",
  "BankTransfer",
  "Check",
] as const;
export type PaymentMethodType = (typeof PAYMENT_METHODS)[number];

export const RELATIONSHIP_TYPES = [
  "Father",
  "Mother",
  "Brother",
  "Sister",
  "Guardian",
  "Tutor",
  "Other",
] as const;

/** Methods that require an external reference code (matches backend validator). */
const REF_CODE_REQUIRED: string[] = ["CreditCard", "BankTransfer", "Check"];

export const studentRegistrationSchema = z
  .object({
    // ── StudentRequestDto ──────────────────────────────────────────────────
    student: z.object({
      firstName: z.string().min(3, "At least 3 characters").max(50),
      lastName:  z.string().min(3, "At least 3 characters").max(50),
      email:     z.string().email("Invalid email").max(255).optional().or(z.literal("")),
      phone:     z.string().min(1, "Phone is required").max(20),
      dateOfBirth: z.string().min(1, "Date of birth is required"),
      genderId:  z.string().optional().or(z.literal("")),
      levelId:   z.string().min(1, "Level is required"),
      /** "direct" → IsDirectRegistration=true; "intake" → IntakeId must be set */
      registrationMode: z.enum(["direct", "intake"]),
      intakeId:  z.string().optional().or(z.literal("")),
    }),

    // ── EnrollmentRequestDto ───────────────────────────────────────────────
    enrollment: z.object({
      subjectId:        z.string().min(1, "Subject is required"),
      planId:           z.string().min(1, "Plan is required"),
      preferedGroupId:  z.string().optional().or(z.literal("")),
      notes:            z.string().optional().or(z.literal("")),
    }),

    // ── RegistrationPaymentRequestDto ──────────────────────────────────────
    payment: z.object({
      amountPaid:    z.coerce.number().gt(0, "Amount must be greater than zero"),
      transferFees:  z.coerce.number().min(0, "Cannot be negative").optional(),
      paidAt:        z.string().optional().or(z.literal("")),
      method:        z.enum(PAYMENT_METHODS, {
        errorMap: () => ({ message: "Select a payment method" }),
      }),
      externalReferenceCode: z.string().optional().or(z.literal("")),
    }),

    // ── StudentResponsableRequestDto (optional) ────────────────────────────
    hasGuardian: z.boolean(),
    responsable: z.object({
      firstName:    z.string().optional().or(z.literal("")),
      lastName:     z.string().optional().or(z.literal("")),
      email:        z.string().optional().or(z.literal("")),
      phone:        z.string().optional().or(z.literal("")),
      relationship: z.string().optional().or(z.literal("")),
      genderId:     z.string().optional().or(z.literal("")),
    }),

    // ── Top-level dates ────────────────────────────────────────────────────
    periodStart:    z.string().optional().or(z.literal("")),
    periodEnd:      z.string().optional().or(z.literal("")),
    invoiceDueDate: z.string().optional().or(z.literal("")),
    chargeDueDate:  z.string().optional().or(z.literal("")),
  })
  .superRefine((data, ctx) => {
    // IntakeId required when mode is "intake"
    if (data.student.registrationMode === "intake" && !data.student.intakeId) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Please select an intake",
        path: ["student", "intakeId"],
      });
    }
    // External reference code required for bank/credit/check payments
    if (
      REF_CODE_REQUIRED.includes(data.payment.method) &&
      !data.payment.externalReferenceCode
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Reference code is required for this payment method",
        path: ["payment", "externalReferenceCode"],
      });
    }
    // Guardian fields required when hasGuardian is true
    if (data.hasGuardian) {
      if (!data.responsable.firstName || data.responsable.firstName.length < 3)
        ctx.addIssue({ code: z.ZodIssueCode.custom, message: "At least 3 characters", path: ["responsable", "firstName"] });
      if (!data.responsable.lastName || data.responsable.lastName.length < 3)
        ctx.addIssue({ code: z.ZodIssueCode.custom, message: "At least 3 characters", path: ["responsable", "lastName"] });
      if (!data.responsable.phone)
        ctx.addIssue({ code: z.ZodIssueCode.custom, message: "Phone is required", path: ["responsable", "phone"] });
      if (!data.responsable.relationship)
        ctx.addIssue({ code: z.ZodIssueCode.custom, message: "Relationship is required", path: ["responsable", "relationship"] });
    }
  });

export type StudentRegistrationSchema = z.infer<typeof studentRegistrationSchema>;
