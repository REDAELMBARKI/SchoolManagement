# Frontend ↔ Backend Integration Plan

> Generated from a full audit of the .NET API controllers, domain entities, and DTOs.
> Goal: replace mock/demo data and stubbed actions with real API calls, and add the pages the backend already supports but the frontend doesn't surface yet.

---

## 0. API basics

- **Base URL:** `http://localhost:5298` (from `launchSettings.json`)
- **Auth:** `POST /api/auth/login` → returns a raw JWT string. Send as `Authorization: Bearer <token>`.
- ⚠️ **Auth middleware is currently commented out** (`app.UseAuthentication()` is disabled in `Program.cs`). JWT is issued but not enforced — all endpoints are anonymous right now. The frontend should still send the token so it works once the middleware is re-enabled.
- **Branch scoping:** Most write endpoints read `BranchId` from the JWT claim (`ICurrentUserContext`). The frontend does **not** send `BranchId` in request bodies — the server fills it. (Login currently hardcodes `BranchId = "12"` in `JwtService`.)
- **Soft delete:** Entities use `DeletedAt`; deleted records are filtered server-side. No special handling needed on the frontend.
- **IDs:** All entity IDs are `Guid` (strings in JSON). The frontend currently uses numeric IDs in mock data — these must be treated as strings when wired to the API.

---

## 1. Infrastructure to add first

### 1.1 API client (`src/lib/api.ts`)
- Create a pre-configured axios instance with `baseURL` from an env var (`VITE_API_URL`).
- Request interceptor: attach `Authorization: Bearer <token>` from `localStorage`.
- Response interceptor: on `401`, clear token + redirect to `/`.
- Export typed helper functions per resource (see sections below).

### 1.2 Auth context (`src/lib/auth.tsx`)
- Replace the offline "Enter as Admin / Student" demo with a real login form.
- Store JWT in `localStorage` (token only — no user object returned by the API today).
- Decode claims client-side for role display (the API does not yet return a role claim; see open questions).
- Provide a `useAuth()` hook: `{ token, branchId, login, logout }`.
- Gate `DashboardLayout` behind auth: redirect to `/` if no token.

### 1.3 Types (`src/types/api.ts`)
- Generate TS interfaces from the response DTOs (StudentResponseDto, EnrollmentResponseDto, IntakeResponseDto, InvoiceResponseDto, PaymentResponseDto, RefundResponseDto, CommissionResponseDto, GroupResponseDto, ScheduleResponseDto, GroupedScheduleDto, SubjectResponseDto, LevelResponseDto, RoomResponseDto, DayResponseDto, TimeSlotResponseDto, TeacherResponseDto, GroupTeacherResponseDto, PlanResponseDto, EnrollmentPlanResponseDto, CommercialAgentResponseDto, StudentResponsableResponseDto, LeadSourceResponseDto + OpcResponseDto + AdResponseDto, BranchResponseDto, GenderResponseDto, MediaResponseDto).
- Mirror request DTOs as TS types for form payloads.

### 1.4 Env
- Add `VITE_API_URL=http://localhost:5298` to `.env` (and `.env.example`).

---

## 2. Endpoint map (what the backend exposes today)

### Implemented & ready
| Resource | Endpoint | Notes |
|---|---|---|
| Auth | `POST /api/auth/login` | `{ email, password }` → `string token` |
| Students | `GET/POST /api/students`, `GET/PUT/DELETE /api/students/{id}` | CRUD |
| Student registration | `POST /api/students/register` | Composite: student + enrollment + payment in one call |
| Enrollments | `GET/POST /api/enrollments`, `GET/PUT/DELETE /api/enrollments/{id}` | CRUD |
| Enrollments | `POST /api/enrollments/{id}/drop` | `{ reason }` |
| Enrollments | `POST /api/enrollments/{id}/complete` | `{ notes }` |
| Intakes | `GET/POST /api/intakes`, `GET/PUT/DELETE /api/intakes/{id}` | CRUD |
| Invoices | `GET/POST /api/invoices`, `GET/PUT/DELETE /api/invoices/{id}` | CRUD |
| Invoices | `POST /api/invoices/{id}/waive` | `{ waivedAmount, reason }` |
| Invoices | `POST /api/invoices/{id}/cancel` | `{ reason }` |
| Payments | `GET /api/payments`, `GET /api/payments/{id}` | Read |
| Payments | `POST /api/payments/registration` | Registration payment |
| Payments | `POST /api/payments/settle` | Settle a charge via invoice |
| Payments | `POST /api/payments/{id}/refund` | `{ amount, reason }` |
| Payments | `GET /api/payments/{id}/refunds` | List refunds for a payment |
| Commissions | `GET /api/commissions/earner/{earnerId}?earnerType={Opc|CommercialAgent}` | |
| Commissions | `GET /api/commissions/period?year=&month=` | |
| Commissions | `POST /api/commissions/{id}/block` | `{ reason }` |
| Groups | `GET/POST /api/groups`, `GET/PUT/DELETE /api/groups/{id}` | CRUD |
| Schedules | `GET /api/schedules/group/{groupId}` | Returns `GroupedScheduleDto` (day → time slots) |
| Ads | `GET /api/ads` | Read only (POST stubbed) |
| OPCs | `GET /api/opcs` | Read only (POST stubbed) |
| Lead sources | `GET /api/lead-sources` | Read only |
| Genders | `GET /api/genders` | Read only (POST stubbed) |
| Media | `POST /api/media` | `multipart/form-data`: file + collection + mediaType |

### Stubbed (throws NotImplementedException) — do not wire yet
- `POST /api/ads`, `POST /api/opcs`, `POST /api/genders`

### Entities with NO controller yet (backend gap)
Teachers, Subjects, Levels, Rooms, Parents (StudentResponsable), Absences, Grades, Expenses, Plans, Branches, Platforms, PayrollPayments, AuditLogs.
→ The frontend can keep mock data for these until controllers are added, or we can scaffold read-only pages later.

---

## 3. Page-by-page work

### 3.1 LoginPage (`src/pages/LoginPage.tsx`)
- Replace the two demo buttons with an email/password form.
- Call `POST /api/auth/login`, store token, redirect to the role-appropriate dashboard.
- Keep a "demo mode" toggle only if we want offline access before auth is enforced.

### 3.2 StudentListPage (`src/pages/list/StudentListPage.tsx`)
- Replace `studentsData` mock with `GET /api/students`.
- Columns map to `StudentResponseDto`: `firstName`, `lastName`, `email`, `phone`, `dateOfBirth`, `branchId`.
- Row "view" link → `/list/students/{id}` (string GUID).
- Create/Edit/Delete currently use `FormActionButtonLink` → route to form pages. Wire these to `POST/PUT/DELETE /api/students`.

### 3.3 SingleStudentPage (`src/pages/list/SingleStudentPage.tsx`)
- Replace `getMockStudentById` with `GET /api/students/{id}`.
- The response includes `enrollments`, `studentResponsables`, `intake`, `gender`, `branch` — surface these.
- "Shortcuts" section links to lessons/teachers/exams — these have no backend yet, keep as placeholders.

### 3.4 TeacherListPage (`src/pages/list/TeacherListPage.tsx`)
- No `GET /api/teachers` endpoint exists. **Keep mock data** until a controller is added. Flag as a backend gap.

### 3.5 SingleTeacherPage (`src/pages/list/SingleTeacherPage.tsx`)
- Same as above — no endpoint. Keep mock.

### 3.6 ParentListPage (`src/pages/list/ParentListPage.tsx`)
- `StudentResponsable` entity exists but no controller. Keep mock. Flag as backend gap.

### 3.7 SubjectListPage (`src/pages/list/SubjectListPage.tsx`)
- No `GET /api/subjects` endpoint. Keep mock. Flag as backend gap.

### 3.8 ClassListPage (`src/pages/list/ClassListPage.tsx`)
- Maps to **Groups** (`GET /api/groups`). Rename the page concept from "Classes" to "Groups" to match the domain.
- Columns: `name`, `capacity`, `period`, `level.name`, `subject.name`.
- Create/Edit/Delete → `POST/PUT/DELETE /api/groups`.

### 3.9 LessonListPage (`src/pages/list/LessonListPage.tsx`)
- No backend entity for "Lessons" (closest is Schedule). Keep mock for now. Flag as a domain gap.

### 3.10 ExamListPage (`src/pages/list/ExamListPage.tsx`)
- No exam endpoint. Keep mock. Flag as backend gap.

### 3.11 AssignmentListPage (`src/pages/list/AssignmentListPage.tsx`)
- No backend. Keep mock.

### 3.12 ResultListPage (`src/pages/list/ResultListPage.tsx`)
- No backend (Grade entity exists, no controller). Keep mock. Flag as backend gap.

### 3.13 EventListPage (`src/pages/list/EventListPage.tsx`)
- No backend. Keep mock.

### 3.14 AnnouncementListPage (`src/pages/list/AnnouncementListPage.tsx`)
- No backend. Keep mock.

### 3.15 AttendanceListPage (`src/pages/list/AttendanceListPage.tsx`)
- Absence entity exists, no controller. Keep mock. Flag as backend gap.

### 3.16 MessagesPage (`src/pages/list/MessagesPage.tsx`)
- No backend. Keep placeholder.

### 3.17 IntakeListPage (`src/pages/list/IntakeListPage.tsx`)
- Replace `intakesData` mock with `GET /api/intakes`.
- Columns map to `IntakeResponseDto`: `firstName`, `lastName`, `email`, `phone`, `leadSource`, `status`, `intakeDate`, `totalFees`, `amountPaid`.
- Create/Edit/Delete → `POST/PUT/DELETE /api/intakes`.
- **Note:** `IntakeRequestDto` requires a nested `LeadSourceRequestDto` (`{ sourceType: "Opc"|"Ad", sourceId }`) and `subjectId`. The current `IntakeForm` doesn't send these correctly — needs a real lead-source selector populated from `GET /api/lead-sources` and a subject selector (no endpoint yet — flag gap).

### 3.18 NEW: EnrollmentListPage (`src/pages/list/EnrollmentListPage.tsx`)
- Does not exist in the frontend yet. Backend has full CRUD + drop/complete.
- Columns: student name, subject, group, status, enrolledAt, creditBalance.
- Actions: view, drop (`POST /{id}/drop`), complete (`POST /{id}/complete`).
- Add to `Menu.tsx` and `App.tsx` routes.

### 3.19 NEW: InvoiceListPage (`src/pages/list/InvoiceListPage.tsx`)
- Does not exist. Backend has full CRUD + waive/cancel.
- Columns: enrollmentId, periodStart–periodEnd, dueDate, totalAmount, paidAmount, status.
- Actions: view, waive (`POST /{id}/waive`), cancel (`POST /{id}/cancel`).
- Add to menu + routes.

### 3.20 NEW: PaymentListPage (`src/pages/list/PaymentListPage.tsx`)
- Does not exist. Backend has read + registration/settle/refund.
- Columns: enrollmentId, amount, method, status, paidAt, currencyCode.
- Actions: record payment (`POST /payments/registration` or `/settle`), refund (`POST /{id}/refund`), view refunds (`GET /{id}/refunds`).
- Add to menu + routes.

### 3.21 NEW: CommissionListPage (`src/pages/list/CommissionListPage.tsx`)
- Does not exist. Backend has earner/period queries + block.
- Columns: earner, earnerType, amount, periodMonth, status.
- Filter by earner + by period (year/month).
- Action: block (`POST /{id}/block`).
- Add to menu + routes (admin only).

### 3.22 NEW: ScheduleView (on Group/Class detail)
- `GET /api/schedules/group/{groupId}` returns `GroupedScheduleDto` (day → time slots with room/subject/teacher).
- Replace the mock `BigCalendarContainer` data for a group with this endpoint.
- The current `BigCalendarContainer` ignores its `id` prop — wire it up.

---

## 4. Forms to update

### 4.1 IntakeForm (`src/components/forms/IntakeForm.tsx`)
- Current form sends flat fields; backend needs `LeadSourceRequestDto` nested object and `subjectId`.
- Populate lead-source dropdown from `GET /api/lead-sources`.
- Populate subject dropdown (no endpoint yet — gap).
- Add fields: `totalFees`, `amountPaid`, `isIndependent`, `followUpDate`, `notes`, `commercialAgentId`.
- Align with `IntakeRequestDto` / `IntakeCommand`.

### 4.2 StudentForm (`src/components/forms/StudentForm.tsx`)
- Current form has fields the backend doesn't accept (`username`, `password`, `bloodType`, `classId`, `parentId`, `img`).
- Backend `StudentRequestDto` wants: `firstName`, `lastName`, `email`, `phone`, `dateOfBirth`, `genderId`, `levelId`, `isDirectRegistration`, `intakeId`.
- Remove unsupported fields or keep them as UI-only if we add them to the backend later.
- Gender dropdown → `GET /api/genders`. Level dropdown → no endpoint (gap).

### 4.3 ClassForm → GroupForm (`src/components/forms/ClassForm.tsx`)
- Rename to GroupForm. Align with `GroupRequestDto`: `name`, `capacity`, `period`, `levelId`, `subjectId`.
- Level/Subject dropdowns → no endpoints yet (gap).

### 4.4 ExamForm (`src/components/forms/ExamForm.tsx`)
- No exam endpoint. Keep as demo.

### 4.5 NEW forms needed
- **EnrollmentForm** — create enrollment (`EnrollmentRequestDto`): student, subject, level, plan, preferredSchedule, group, notes.
- **InvoiceForm** — create invoice (`InvoiceCommand`): enrollment, period dates, due date, optional charge.
- **PaymentForm** — registration payment + charge settlement.
- **RefundForm** — amount + reason.
- **CommissionBlockForm** — reason.

---

## 5. Mock data to retire

`src/lib/data.ts` — once a page is wired to the API, remove its mock array from this file. Keep mocks only for entities with no backend (teachers, parents, subjects, lessons, exams, assignments, results, events, announcements, attendance).

`src/lib/mockSchool.ts` — replace `getMockStudentById` / `getMockTeacherById` with API calls once endpoints exist.

`src/lib/actions.ts` — all stubbed actions return `{ success: true }`. Replace each with a real API call once the corresponding endpoint is wired.

`src/lib/mockUpload.ts` — replace with `POST /api/media` (multipart upload).

---

## 6. Menu & routing changes (`src/components/Menu.tsx`, `src/App.tsx`)

Add menu items (admin-only):
- **Enrollments** → `/list/enrollments`
- **Invoices** → `/list/invoices`
- **Payments** → `/list/payments`
- **Commissions** → `/list/commissions`

Add routes in `App.tsx` for the new list pages + their create/edit/delete sub-routes.

Rename "Classes" → "Groups" in the menu to match the domain (optional but recommended).

---

## 7. Role & access (`src/lib/settings.ts`)

`routeAccessMap` currently keys off demo roles. Once real roles are returned by the API (currently only `NameIdentifier`, `Email`, `Name`, `BranchId` claims — **no role claim**), update the map. Flag: backend needs to add a role claim to the JWT.

Until then, treat all authenticated users as admin for navigation purposes, or keep the demo role selector.

---

## 8. Open questions / backend gaps

1. **No role claim in JWT** — `JwtService` issues `NameIdentifier`, `Email`, `Name`, `BranchId` only. Need a role claim to drive frontend access control.
2. **Auth middleware disabled** — `app.UseAuthentication()` is commented out. Re-enable before relying on JWT for access control.
3. **BranchId hardcoded to "12"** in `JwtService.generateToken`. Should come from the user's actual branch.
4. **Missing controllers** for: Teachers, Subjects, Levels, Rooms, Parents (StudentResponsable), Absences, Grades, Expenses, Plans, Branches, Platforms, PayrollPayments.
5. **Stubbed POSTs**: ads, opcs, genders.
6. **No "Lessons" entity** — the frontend has a Lessons page but the domain has Schedules instead. Decide whether to map Lessons → Schedules or drop the page.
7. **StudentRegistrationService** is referenced by `StudentRegistrationController` but the service file is empty (`StudentRegistrationService.cs` is blank). This endpoint will fail at runtime.
8. **ScheduleService.GetGroupScheduleAsync** throws `NotImplementedException` — the schedule endpoint is not actually implemented despite being routed.

---

## 9. Suggested implementation order

1. **API client + auth** (axios instance, login form, token storage).
2. **Students** list + detail + forms (full CRUD exists).
3. **Intakes** list + forms (full CRUD exists; needs lead-source selector).
4. **Groups** (rename Classes) list + forms.
5. **Enrollments** new page + drop/complete actions.
6. **Invoices** new page + waive/cancel actions.
7. **Payments** new page + registration/settle/refund.
8. **Commissions** new page + block.
9. **Schedule** view on group detail (once `ScheduleService` is implemented).
10. Keep mock data for everything in the "backend gap" list.
