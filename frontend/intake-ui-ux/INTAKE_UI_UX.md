# Intake UI/UX — Full Specification

> **Purpose:** This document is the single source of truth for every screen, form, field, validation rule, and UX decision in the Intake module. It is derived directly from the backend domain model (`Intake`, `Student`, `Enrollment`, `StudentResponsable`) and the existing frontend patterns (react-hook-form + zod, `InputField`, `Table`, `Pagination`, `FormModal`).

---

## Table of Contents

1. [Domain Overview](#1-domain-overview)
2. [Intake Lifecycle & Status Flow](#2-intake-lifecycle--status-flow)
3. [Pages & Routes](#3-pages--routes)
4. [Page 1 — Intake List](#4-page-1--intake-list)
5. [Page 2 — Create Intake Form](#5-page-2--create-intake-form)
6. [Page 3 — Intake Detail / Profile](#6-page-3--intake-detail--profile)
7. [Page 4 — Edit Intake Form](#7-page-4--edit-intake-form)
8. [Page 5 — Convert Intake to Student](#8-page-5--convert-intake-to-student)
9. [Page 6 — Student Full Registration (from Intake)](#9-page-6--student-full-registration-from-intake)
10. [Shared Components](#10-shared-components)
11. [Zod Schemas](#11-zod-schemas)
12. [API Action Mapping](#12-api-action-mapping)
13. [UX Decisions & Rules](#13-ux-decisions--rules)

---

## 1. Domain Overview

An **Intake** is a prospective student record — the lead/inquiry stage before someone becomes a registered student.

```
Lead comes in
     │
     ▼
┌──────────┐    ┌───────────┐    ┌────────────┐    ┌──────────────────┐
│   New    │───▶│ Contacted │───▶│ Interested │───▶│    Enrolled      │
└──────────┘    └───────────┘    └────────────┘    └──────────────────┘
                                        │
                                        ▼
                                 ┌──────────────┐
                                 │NotInterested │
                                 └──────────────┘
```

**Key fields from the backend `Intake` entity:**

| Field             | Type            | Required | Notes                                          |
|-------------------|-----------------|----------|------------------------------------------------|
| FirstName         | string          | ✅        |                                                |
| LastName          | string          | ✅        |                                                |
| Email             | string (email)  | ✅        | Validated as proper email                      |
| Phone             | string          | ✅        |                                                |
| DateOfBirth       | date?           | ❌        | If provided, person must be reasonable age     |
| GenderId          | Guid?           | ❌        |                                                |
| IntakeDate        | date            | ✅        | Cannot be in the future (≤ today)              |
| IntakeStatus      | enum            | ✅        | New / Contacted / Interested / Enrolled / NotInterested |
| FollowUpDate      | date?           | ❌        | If set, must be ≥ IntakeDate                   |
| Notes             | string?         | ❌        | Free text                                      |
| CommercialAgentId | Guid?           | ❌        | OPC / commercial agent                         |
| LeadSourceId      | Guid?           | ❌        | Required when IsIndependent = false            |
| SubjectId         | Guid            | ✅        | The subject the prospect is interested in      |
| BranchId          | Guid            | ✅        | Auto-injected from current branch context      |
| IsIndependent     | bool            | ✅        | If true, LeadSourceId must be null             |
| TotalFees         | decimal         | ✅        | Must be > 0                                    |
| AmountPaid        | decimal         | ✅        | 0 ≤ AmountPaid ≤ TotalFees                     |

**Computed (read-only, from backend):**

| Field           | Formula                    |
|-----------------|----------------------------|
| AmountRemaining | TotalFees − AmountPaid     |
| IsFullyPaid     | AmountPaid === TotalFees   |

---

## 2. Intake Lifecycle & Status Flow

### Status Descriptions

| Status        | Meaning                                              | UI Color Token     |
|---------------|------------------------------------------------------|--------------------|
| New           | Just registered, not yet contacted                   | `blue-400`         |
| Contacted     | Initial contact made (call/message)                  | `yellow-400`       |
| Interested    | Confirmed interest, negotiating or follow-up pending | `purple-400`       |
| Enrolled      | Converted — a Student record now exists              | `green-400`        |
| NotInterested | Dropped / no longer a prospect                       | `red-400`          |

### Rules
- Status **Enrolled** is set **automatically** by the backend when a `Student` is linked to this intake (domain event `NewStudentAssignedDomainEvent`). The UI should not expose a manual "Set to Enrolled" button — it shows the enrolled badge and a "View Student" link instead.
- All other status transitions are manual (user taps a status button).
- Once **Enrolled**, the intake record should be **read-only** except for Notes and FollowUpDate.

---

## 3. Pages & Routes

| Route                                  | Component                  | Access        |
|----------------------------------------|----------------------------|---------------|
| `/list/intakes`                        | `IntakeListPage`           | Admin, OPC    |
| `/list/intakes/new`                    | `IntakeCreatePage`         | Admin, OPC    |
| `/list/intakes/:id`                    | `IntakeDetailPage`         | Admin, OPC    |
| `/list/intakes/:id/edit`               | `IntakeEditPage`           | Admin, OPC    |
| `/list/intakes/:id/convert`            | `IntakeConvertPage`        | Admin only    |
| `/list/intakes/:id/register`           | `IntakeFullRegistrationPage` | Admin only  |

---

## 4. Page 1 — Intake List

**File:** `frontend/src/pages/list/IntakeListPage.tsx`  
*(Already exists — this spec defines the complete desired state.)*

### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│  [🔍 Search intakes...]   [Status ▾]  [Branch ▾]  [+ New Intake]│
├──────┬──────────┬──────────┬────────────┬────────────┬──────────┤
│      │ Name     │ Subject  │ Lead Src   │ OPC        │ Status   │
│ img  │ email    │          │            │            │ date     │
│      │ phone    │          │            │            │          │
├──────┼──────────┼──────────┼────────────┼────────────┼──────────┤
│ ...  │ ...      │ ...      │ ...        │ ...        │ ...      │
└──────┴──────────┴──────────┴────────────┴────────────┴──────────┘
[← Prev]                                              [Next →]
```

### Table Columns

| Column        | Value                                         | Responsive  |
|---------------|-----------------------------------------------|-------------|
| Avatar + Name | Initials avatar, full name, email below       | Always      |
| Phone         | phone number                                  | `md:table-cell` |
| Subject       | subject name                                  | Always      |
| Lead Source   | leadSource.name or "Independent"              | `hidden xl:table-cell` |
| OPC           | commercialAgent name or "—"                   | `hidden xl:table-cell` |
| Intake Date   | formatted date                                | `hidden lg:table-cell` |
| Status        | colored badge pill                            | Always      |
| Actions       | 👁 view · ✏️ edit · 🗑 delete (admin)          | Always      |

### Filters

- **Search** — filters by name or email (client-side on mock; query param on real API)
- **Status filter** — dropdown: All / New / Contacted / Interested / Enrolled / NotInterested
- **Subject filter** — dropdown populated from relatedData

### Row Click Behavior

Clicking anywhere on a row (except action buttons) navigates to `/list/intakes/:id`.

### Empty State

```
┌──────────────────────────────────────┐
│         📋                           │
│  No intakes found                    │
│  Adjust your filters or create one   │
│         [+ New Intake]               │
└──────────────────────────────────────┘
```

---

## 5. Page 2 — Create Intake Form

**File:** `frontend/src/pages/list/IntakeCreatePage.tsx`  
**Form component:** `frontend/src/components/forms/IntakeForm.tsx` (refactor)

### Layout — Two-Column Card

```
┌────────────────────────────────────────────────────────────┐
│  New Intake                                                │
├──────────────────────┬─────────────────────────────────────┤
│  PERSONAL INFO       │  INTAKE INFO                        │
│                      │                                     │
│  First Name *        │  Subject *                          │
│  Last Name  *        │  Intake Date *                      │
│  Email      *        │  Status                             │
│  Phone      *        │  Follow-up Date                     │
│  Date of Birth       │  Notes (textarea)                   │
│  Gender              │                                     │
├──────────────────────┼─────────────────────────────────────┤
│  SOURCE & AGENT      │  FINANCIAL                          │
│                      │                                     │
│  Independent? [✓]    │  Total Fees *                       │
│  Lead Source         │  Amount Paid *                      │
│    (hidden if        │  Amount Remaining (read-only)       │
│     Independent)     │                                     │
│  OPC / Agent         │                                     │
└──────────────────────┴─────────────────────────────────────┘
                              [Cancel]  [Create Intake →]
```

### Fields Detail

#### Section: Personal Info

| Field          | Input Type | Validation                                 | Placeholder           |
|----------------|------------|--------------------------------------------|-----------------------|
| First Name     | text       | required, min 2 chars                      | "e.g. Ahmed"          |
| Last Name      | text       | required, min 2 chars                      | "e.g. Benali"         |
| Email          | email      | required, valid email format               | "ahmed@example.com"   |
| Phone          | text       | required, min 8 chars                      | "+213 ..."            |
| Date of Birth  | date       | optional, max = 3 years ago from today     | —                     |
| Gender         | select     | optional — options loaded from relatedData | "Select gender"       |

#### Section: Intake Info

| Field          | Input Type | Validation                                  | Notes                                     |
|----------------|------------|---------------------------------------------|-------------------------------------------|
| Subject        | select     | required                                    | Populated from relatedData.subjects       |
| Intake Date    | date       | required, max = today                       | Defaults to today                         |
| Status         | select     | required, default = "New"                   | New/Contacted/Interested/NotInterested    |
| Follow-up Date | date       | optional, if set must be ≥ Intake Date      |                                           |
| Notes          | textarea   | optional, max 500 chars                     | Rows = 3                                  |

#### Section: Source & Agent

| Field          | Input Type | Validation                                                 | UX Behavior                                                    |
|----------------|------------|-------------------------------------------------------------|----------------------------------------------------------------|
| Is Independent | toggle/checkbox | —                                                      | When ON, hide Lead Source field and clear its value            |
| Lead Source    | select     | required when IsIndependent = false                         | Hidden when IsIndependent = true                               |
| OPC / Agent    | select     | optional                                                    | Populated from relatedData.commercialAgents                    |

#### Section: Financial

| Field            | Input Type | Validation                          | Notes                                      |
|------------------|------------|--------------------------------------|--------------------------------------------|
| Total Fees       | number     | required, > 0                        |                                            |
| Amount Paid      | number     | required, 0 ≤ value ≤ totalFees     | Live updates Amount Remaining display      |
| Amount Remaining | read-only  | —                                   | Computed: `totalFees - amountPaid`, shown in colored text (red if > 0, green if 0) |

### Submission Flow

1. Validate with Zod schema (`intakeSchema`)
2. POST `api/intakes` with `IntakeRequestDto` shape
3. On success → toast "Intake created successfully" → navigate to `/list/intakes/:newId`
4. On error → show server error message under the relevant field or in a top banner

---

## 6. Page 3 — Intake Detail / Profile

**File:** `frontend/src/pages/list/IntakeDetailPage.tsx` *(new)*

### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│  ← Back to Intakes                            [Edit]  [Delete]  │
├───────────────────────────────┬─────────────────────────────────┤
│                               │                                 │
│  👤  AHMED BENALI             │  STATUS TIMELINE                │
│      ahmed@example.com        │                                 │
│      +213 550 123 456         │  ● New          Jan 10          │
│      DOB: 15 Mar 2000         │  ● Contacted    Jan 12          │
│      Gender: Male             │  ○ Interested                   │
│                               │  ○ Enrolled                     │
├───────────────────────────────┤  ○ Not Interested               │
│  INTAKE INFO                  │                                 │
│  Subject:     Math            ├─────────────────────────────────┤
│  Branch:      Algiers         │  QUICK ACTIONS                  │
│  Intake Date: 10 Jan 2025     │                                 │
│  Lead Source: Facebook        │  [Mark as Contacted]            │
│  OPC:         Karim Ouali     │  [Mark as Interested]           │
│  Notes:       "..."           │  [Mark as Not Interested]       │
│  Follow-up:   15 Jan 2025     │                                 │
│                               │  ─────────────────────         │
├───────────────────────────────┤  [Convert to Student →]        │
│  FINANCIAL SUMMARY            │    (Admin only)                 │
│                               │                                 │
│  Total Fees:     5,000 DZD    │                                 │
│  Amount Paid:    2,000 DZD    │                                 │
│  Remaining:      3,000 DZD ⚠  │                                 │
│  [████████░░░░░░░░] 40%       │                                 │
│                               │                                 │
├───────────────────────────────┴─────────────────────────────────┤
│  LINKED STUDENTS                                                 │
│                                                                  │
│  ┌────────────────────────────────────────────────────────┐    │
│  │ No students linked yet.                                 │    │
│  │ Convert this intake to create the first student record. │    │
│  └────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

### Sections

#### Personal Info Card
- Avatar (initials-based, large, colored by status)
- Full name as H1
- Email, Phone, DOB, Gender as labeled rows

#### Intake Info Card
- Subject, Branch, Intake Date, Lead Source ("Independent" if flag is set), OPC, Follow-up Date, Notes

#### Financial Summary Card
- Total Fees, Amount Paid, Amount Remaining
- Progress bar (amountPaid / totalFees × 100%)
- Amount Remaining shown in `text-red-500` if > 0, `text-green-500` if fully paid

#### Status Timeline Card
- Vertical step list; completed steps filled, future steps outlined
- Each completed step shows the date it was set (if available from audit log)

#### Quick Actions Panel
- Buttons for allowed status transitions (excludes current status and Enrolled)
- **Enrolled** status: hide transition buttons, show "✅ Enrolled on [date]" + "View Student →" link
- **NotInterested**: show a "Reopen as New" option for recovery

#### Linked Students Section
- Table of students linked via `IntakeId` (from `IntakeResponseDto.Students`)
- Columns: Name, Phone, Enrollment Count, "View" link
- Empty state with call-to-action to convert

---

## 7. Page 4 — Edit Intake Form

**File:** `frontend/src/pages/list/IntakeEditPage.tsx` *(or via generic `/list/intakes/:id/edit`)*  
**Form component:** Same `IntakeForm.tsx`, `type="update"`

### Differences from Create

- All fields pre-populated from the existing intake record
- `IntakeDate` field is **read-only** (cannot change when after creation)
- `SubjectId` and `BranchId` are **read-only** when `status === Enrolled` (backend also blocks this)
- When `status === Enrolled`, only **Notes** and **FollowUpDate** are editable; all other fields are disabled with a notice: *"This intake has been converted to a student. Only notes and follow-up date can be updated."*
- Submit calls PUT `api/intakes/:id`

---

## 8. Page 5 — Convert Intake to Student

**File:** `frontend/src/pages/list/IntakeConvertPage.tsx` *(new)*  
**Access:** Admin only

This is a **confirmation + minimal data page**, not a full form. Its purpose is to create a `Student` record linked to this intake.

### Layout

```
┌──────────────────────────────────────────────────────────┐
│  Convert Intake to Student                               │
│  Ahmed Benali · Math · Algiers                           │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  ℹ️  This will create a Student account for Ahmed        │
│  Benali linked to this intake. Status will              │
│  automatically change to "Enrolled".                    │
│                                                          │
│  Set Login Credentials                                   │
│  ┌──────────────────────────────────────────────┐       │
│  │  Username *   [________________]              │       │
│  │  Password *   [________________]              │       │
│  └──────────────────────────────────────────────┘       │
│                                                          │
│  Add Guardian (optional)                                 │
│  ┌──────────────────────────────────────────────┐       │
│  │  Guardian First Name  [________________]      │       │
│  │  Guardian Last Name   [________________]      │       │
│  │  Guardian Phone *     [________________]      │       │
│  │  Guardian Email       [________________]      │       │
│  │  Relationship         [Father ▾]              │       │
│  └──────────────────────────────────────────────┘       │
│                                                          │
│  [Cancel]                    [Create Student Account →] │
└──────────────────────────────────────────────────────────┘
```

### Fields

| Field             | Source                      | Editable | Notes                                      |
|-------------------|-----------------------------|----------|--------------------------------------------|
| First Name        | Pre-filled from Intake      | No       | Shown as read-only banner text             |
| Last Name         | Pre-filled from Intake      | No       | Shown as read-only banner text             |
| Email             | Pre-filled from Intake      | No       | Shown as read-only banner text             |
| Phone             | Pre-filled from Intake      | No       | Shown as read-only banner text             |
| Date of Birth     | Pre-filled from Intake      | No       | Shown as read-only banner text             |
| IntakeId          | Current intake ID           | Hidden   | Sent in payload                            |
| BranchId          | Pre-filled from Intake      | Hidden   | Sent in payload                            |
| Username          | User input                  | ✅        | Required                                   |
| Password          | User input                  | ✅        | Required, min 8 chars                      |
| Guardian fields   | User input                  | ✅        | Optional block; phone required if block opened |
| Relationship type | Select                      | ✅        | Father/Mother/Guardian/Grandfather/Grandmother/Uncle/Aunt/Other |

### Submission Flow

1. POST `api/students` with `StudentCommand` (or `StudentRegistrationCommand` if including enrollment)
2. On success → toast "Student account created" → navigate to `/list/intakes/:id` (detail page now shows student badge)
3. Backend event `NewStudentAssignedDomainEvent` fires → intake status becomes **Enrolled** automatically

---

## 9. Page 6 — Student Full Registration (from Intake)

**File:** `frontend/src/pages/list/IntakeFullRegistrationPage.tsx` *(new)*  
**Access:** Admin only

This is the **full registration wizard** — student + enrollment + initial payment in one flow. Maps to `StudentRegistrationCommand` / `POST api/student-registrations`.

### Step Wizard Layout

```
Step 1 of 3           Step 2 of 3           Step 3 of 3
─────────────         ─────────────         ─────────────
Student Info     →    Enrollment       →    Payment
(pre-filled)          Details               Summary
```

### Step 1 — Student Info

Pre-filled from Intake (read-only display). Editable fields:

| Field      | Input     | Validation        |
|------------|-----------|-------------------|
| Username   | text      | required          |
| Password   | text      | required, min 8   |
| Guardian   | sub-form  | optional          |

### Step 2 — Enrollment Details

| Field         | Input  | Validation       | Notes                                         |
|---------------|--------|------------------|-----------------------------------------------|
| Subject       | select | required         | Pre-filled from Intake's subject; editable    |
| Group/Class   | select | required         | Filtered by selected subject                  |
| Branch        | select | required         | Pre-filled from Intake                        |
| Level         | select | optional         |                                               |
| Enrollment Plan | select | optional       | Pre-fills payment schedule                   |
| Schedule      | select | optional         |                                               |
| Notes         | textarea | optional       |                                               |

### Step 3 — Payment Summary

| Field            | Input    | Validation                         | Notes                                           |
|------------------|----------|------------------------------------|------------------------------------------------|
| Amount to Pay    | number   | required, > 0, ≤ totalFees         | Defaults to intake's `TotalFees`               |
| Payment Method   | select   | required                           | Cash / Bank Transfer / Check / Card            |
| Payment Reference | text    | optional / required for non-cash  | Required if method ≠ Cash                      |
| Notes            | textarea | optional                           |                                                 |
| Summary block    | read-only | —                                 | Shows fees, plan installments if plan selected  |

### Navigation

- **[← Back]** returns to previous step, preserving entered data
- **[Next →]** validates only current step's fields before advancing
- **[Confirm Registration]** on step 3 submits the full `StudentRegistrationCommand`
- Progress bar / step indicator at the top

### Submission Flow

1. POST `api/student-registrations`
2. On success → toast "Student registered successfully" → navigate to new student's page `/list/students/:newStudentId`

---

## 10. Shared Components

### `IntakeStatusBadge`

**File:** `frontend/src/components/IntakeStatusBadge.tsx`

```tsx
// Usage: <IntakeStatusBadge status="Interested" />
// Renders: pill with color-coded background and label
```

| Status        | Classes                                    |
|---------------|--------------------------------------------|
| New           | `bg-blue-100 text-blue-600`               |
| Contacted     | `bg-yellow-100 text-yellow-700`           |
| Interested    | `bg-purple-100 text-purple-600`           |
| Enrolled      | `bg-green-100 text-green-700`             |
| NotInterested | `bg-red-100 text-red-600`                 |

### `IntakeAvatar`

Initials-based avatar (first letter of firstName + lastName). Background color derived from status.

### `FinancialProgressBar`

**File:** `frontend/src/components/FinancialProgressBar.tsx`

```tsx
// Props: totalFees, amountPaid
// Renders: progress bar + labels for paid/remaining
```

- Bar fill = `amountPaid / totalFees * 100%`
- Green fill when fully paid, amber when partial, red when 0 paid

### `IntakeStatusActions`

**File:** `frontend/src/components/IntakeStatusActions.tsx`

Button group that renders only the valid next-status transitions:

```
Current: New           → show [Contacted] [Not Interested]
Current: Contacted     → show [Interested] [Not Interested]
Current: Interested    → show [Not Interested]   + [Convert to Student]
Current: NotInterested → show [Reopen as New]
Current: Enrolled      → show nothing (enrolled badge + view student link)
```

Each button calls PUT `api/intakes/:id` with the new status only.

---

## 11. Zod Schemas

**File:** `frontend/src/lib/formValidationSchemas.ts` — add the following schemas:

### `intakeSchema` (Create)

```ts
export const intakeSchema = z.object({
  id: z.string().optional(),
  firstName: z.string().min(2, "First name must be at least 2 characters"),
  lastName: z.string().min(2, "Last name must be at least 2 characters"),
  email: z.string().email("Enter a valid email address"),
  phone: z.string().min(8, "Enter a valid phone number"),
  dateOfBirth: z.coerce.date().optional(),
  genderId: z.string().optional(),
  intakeDate: z.coerce.date({
    required_error: "Intake date is required",
  }).max(new Date(), "Intake date cannot be in the future"),
  status: z.enum(["New", "Contacted", "Interested", "NotInterested"]).default("New"),
  followUpDate: z.coerce.date().optional(),
  notes: z.string().max(500).optional(),
  isIndependent: z.boolean().default(false),
  leadSourceId: z.string().optional(),
  commercialAgentId: z.string().optional(),
  subjectId: z.string().min(1, "Subject is required"),
  branchId: z.string().min(1, "Branch is required"),
  totalFees: z.number({ required_error: "Total fees is required" }).positive("Total fees must be greater than 0"),
  amountPaid: z.number().min(0, "Amount paid cannot be negative"),
}).superRefine((data, ctx) => {
  // Lead source required when not independent
  if (!data.isIndependent && !data.leadSourceId) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Lead source is required when not independent",
      path: ["leadSourceId"],
    });
  }
  // Amount paid cannot exceed total fees
  if (data.amountPaid > data.totalFees) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Amount paid cannot exceed total fees",
      path: ["amountPaid"],
    });
  }
  // Follow-up date must be after intake date
  if (data.followUpDate && data.followUpDate < data.intakeDate) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Follow-up date must be on or after the intake date",
      path: ["followUpDate"],
    });
  }
});

export type IntakeSchema = z.infer<typeof intakeSchema>;
```

### `intakeConvertSchema` (Convert to Student)

```ts
export const intakeConvertSchema = z.object({
  username: z.string().min(3, "Username must be at least 3 characters"),
  password: z.string().min(8, "Password must be at least 8 characters"),
  // Guardian — optional but phone required if any guardian field is filled
  guardianFirstName: z.string().optional(),
  guardianLastName: z.string().optional(),
  guardianPhone: z.string().optional(),
  guardianEmail: z.string().email().optional().or(z.literal("")),
  guardianRelationship: z.enum([
    "Father", "Mother", "Guardian",
    "Grandfather", "Grandmother",
    "Uncle", "Aunt", "Other"
  ]).optional(),
}).superRefine((data, ctx) => {
  const hasGuardianInfo =
    data.guardianFirstName || data.guardianLastName || data.guardianPhone;
  if (hasGuardianInfo && !data.guardianPhone) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Guardian phone is required",
      path: ["guardianPhone"],
    });
  }
});

export type IntakeConvertSchema = z.infer<typeof intakeConvertSchema>;
```

---

## 12. API Action Mapping

**File:** `frontend/src/lib/actions.ts` — add these server actions:

| Action Function          | Method | Endpoint                    | Payload type              |
|--------------------------|--------|-----------------------------|---------------------------|
| `createIntake`           | POST   | `api/intakes`               | `IntakeRequestDto`        |
| `updateIntake`           | PUT    | `api/intakes/:id`           | `UpdateIntakeRequestDto`  |
| `deleteIntake`           | DELETE | `api/intakes/:id`           | —                         |
| `getIntake`              | GET    | `api/intakes/:id`           | —                         |
| `updateIntakeStatus`     | PUT    | `api/intakes/:id`           | `{ status: IntakeStatus }` |
| `convertIntakeToStudent` | POST   | `api/students`              | `StudentCommand`          |
| `registerStudentFull`    | POST   | `api/student-registrations` | `StudentRegistrationCommand` |

### `IntakeRequestDto` shape (matches backend)

```ts
type IntakeRequestDto = {
  firstName: string;
  lastName: string;
  slug?: string;           // generated client-side as firstName-lastName or left for backend
  email: string;
  phone: string;
  dateOfBirth?: string;    // ISO date string
  genderId?: string;
  intakeDate: string;      // ISO date string
  status: "New" | "Contacted" | "Interested" | "NotInterested";
  followUpDate?: string;
  notes?: string;
  isIndependent: boolean;
  leadSourceId?: string | null;
  commercialAgentId?: string;
  subjectId: string;
  branchId: string;
  totalFees: number;
  amountPaid: number;
};
```

---

## 13. UX Decisions & Rules

### 1. Independent Toggle
When the user toggles **IsIndependent = true**, the Lead Source select disappears with a CSS transition (`opacity-0 h-0 overflow-hidden`), and its value is cleared from the form state. This mirrors the backend's logic of setting `LeadSourceId = null` when independent.

### 2. Amount Remaining Live Calculation
As the user types in **Amount Paid**, the **Amount Remaining** field updates in real-time without a network call. This is a `watch`-driven derived value in react-hook-form:
```ts
const totalFees = watch("totalFees") || 0;
const amountPaid = watch("amountPaid") || 0;
const remaining = totalFees - amountPaid;
```

### 3. Intake Date Defaults to Today
On the create form, `intakeDate` defaults to `new Date().toISOString().split("T")[0]`. The user can change it but never to a future date.

### 4. Status Cannot Be Set to "Enrolled" Manually
The `status` dropdown on the Create/Edit form excludes "Enrolled". That status is assigned by the backend event. The UI treats it as a terminal state badge.

### 5. Convert Button Visibility
The **Convert to Student** button only appears when:
- User role is Admin
- Intake status is **Interested** or **Contacted** (not NotInterested, not already Enrolled)
- The button is disabled (greyed out with tooltip) when status is New with a message: *"Mark the intake as Contacted or Interested before converting."*

### 6. Edit Restrictions When Enrolled
When `status === "Enrolled"`, the Edit page renders all fields as disabled except **Notes** and **Follow-up Date**, with a banner:
> ⚠ *This intake has been converted to a student. Only notes and follow-up date can be updated.*

### 7. Delete Confirmation
Deleting an intake shows a confirmation modal:
> *"Are you sure you want to delete this intake? This cannot be undone. If a student is linked to this intake, the student record will remain."*

### 8. Form Sections Collapsed on Mobile
On screens < `md`, the two-column form collapses to a single column, and the "Source & Agent" and "Financial" sections are inside `<details>` summary accordions that default to open.

### 9. Error Display
- Field-level errors appear directly below each input using the existing `InputField` `error` prop.
- Server-side errors (e.g., duplicate email) appear in a red banner at the top of the form.
- Success actions trigger `react-toastify` toasts.

### 10. Breadcrumb Navigation
All intake pages show a breadcrumb:
```
Dashboard > Intakes > [Page name]
```
On the detail page, the breadcrumb shows the intake's full name instead of an ID.

### 11. Pagination
Intake list uses the shared `Pagination` component. Default page size: **10**. The current page and search query are stored in URL search params (`?page=2&q=ahmed&status=Interested`) so the URL is shareable and browser-back-friendly.

### 12. Responsive Table
Following the existing pattern in `StudentListPage`, columns are hidden at smaller breakpoints:
- Phone: hidden below `md`
- Lead Source, OPC: hidden below `xl`
- Intake Date: hidden below `lg`

### 13. Color Consistency
All status color tokens must match across:
- `IntakeStatusBadge` component
- Status timeline in the detail page
- Avatar background color
- Filter dropdown option styling

### 14. Loading States
Every async action (form submit, status update, page load) shows:
- Buttons: disabled + spinner icon replacing label text
- Page load: skeleton cards matching the layout (not a global spinner)

---

## Appendix — `IntakeList` Type

```ts
// frontend/src/pages/list/IntakeListPage.tsx (local type)
type IntakeList = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  subject: { id: string; name: string };
  branch: { id: string; name: string };
  leadSource?: { id: string; name: string };
  commercialAgent?: { id: string; firstName: string; lastName: string };
  intakeDate: string;
  status: "New" | "Contacted" | "Interested" | "Enrolled" | "NotInterested";
  students: { id: string }[];
};
```

## Appendix — `IntakeResponseDto` Shape (from backend)

```ts
type IntakeResponseDto = {
  id: string;
  slug: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth?: string;
  gender?: { id: string; name: string };
  intakeDate: string;
  status: "New" | "Contacted" | "Interested" | "Enrolled" | "NotInterested";
  followUpDate?: string;
  notes?: string;
  isIndependent: boolean;
  totalFees: number;
  amountPaid: number;
  amountRemaining: number;        // computed by backend
  isFullyPaid: boolean;           // computed by backend
  leadSource?: { id: string; name: string; type: string };
  subject: { id: string; name: string };
  branch: { id: string; name: string };
  commercialAgent?: { id: string; firstName: string; lastName: string };
  students: {
    id: string;
    firstName: string;
    lastName: string;
    phone: string;
  }[];
};
```
