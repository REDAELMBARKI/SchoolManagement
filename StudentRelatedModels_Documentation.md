# Student-Related Models Documentation

## Table of Contents
1. [Base Classes](#base-classes)
2. [Core Student Models](#core-student-models)
3. [Enrollment & Academic Models](#enrollment--academic-models)
4. [People & Relationships](#people--relationships)
5. [Financial Models](#financial-models)
6. [Lead & Marketing Models](#lead--marketing-models)

---

## Base Classes

### BaseEntity
**File:** `Models/BaseEntity.cs`

| Attribute | Type | Description |
|-----------|------|-------------|
| Id | int | Primary key |
| CreatedAt | DateTime | Creation timestamp (UTC) |
| UpdatedAt | DateTime? | Last update timestamp |
| DeletedAt | DateTime? | Soft delete timestamp |

---

### Person (Abstract)
**File:** `Models/Person.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| FirstName | string | Person's first name |
| LastName | string | Person's last name |
| Slug | string | URL-friendly identifier |
| GenderId | int? | Foreign key to Gender |
| Gender | Gender? | Navigation property |

**Relationships:**
- Many-to-One: Gender (optional)

---

## Core Student Models

### Student
**File:** `Models/Student.cs`
**Inherits:** Person

| Attribute | Type | Description |
|-----------|------|-------------|
| Email | string? | Student email address |
| Phone | string | Student phone number |
| DateOfBirth | DateOnly | Birth date |
| IntakeId | int? | FK to Intake (if converted from intake) |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| StudentParents | ICollection<StudentParent> | Many-to-Many (via junction) |
| Enrollments | ICollection<Enrollment> | One-to-Many |
| Intake | Intake? | One-to-One (optional) |

---

## Enrollment & Academic Models

### Enrollment
**File:** `Models/Enrollment.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| EnrolledAt | DateTime | Enrollment timestamp (default: UTC now) |
| Status | string | "Active" / "Dropped" / "Completed" |
| StudentId | int | FK to Student |
| SubjectId | int | FK to Subject |
| GroupId | int | FK to Group |
| BranchId | int | FK to Branch |
| PlanId | int | FK to Plan (duration plan) |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Payments | ICollection<Payment> | One-to-Many |
| Subject | Subject | Many-to-One |
| Branch | Branch | Many-to-One |
| Plan | Plan | Many-to-One |
| Student | Student | Many-to-One |
| Group | Group | Many-to-One |

---

### Group
**File:** `Models/Group.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Name | string | Group name |
| Capacity | int | Max students (default: 15) |
| Period | string | "Morning" / "Afternoon" / "Evening" / "Weekend" |
| BranchId | int | FK to Branch |
| LevelId | int | FK to Level |
| LanguageId | int | FK to Language |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Enrollments | ICollection<Enrollment> | One-to-Many |
| Teachers | ICollection<GroupTeacher> | Many-to-Many (via junction) |
| Branch | Branch | Many-to-One |
| Level | Level | Many-to-One |
| Subject | Subject | Many-to-One |

---

### GroupTeacher (Junction)
**File:** `Models/GroupTeacher.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| TeacherId | int | FK to Teacher |
| GroupId | int | FK to Group |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Teacher | Teacher | Many-to-One |
| Group | Group | Many-to-One |

---

### Subject
**File:** `Models/Subject.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Name | string | Subject name |
| Slug | string | URL-friendly identifier |
| Description | string? | Subject description |
| BranchId | int | FK to Branch |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Enrollments | ICollection<Enrollment> | One-to-Many |
| Branch | Branch | Many-to-One |

---

### Level
**File:** `Models/Level.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Name | string | Level name |
| BranchId | int | FK to Branch |
| Order | int | Display order (default: 1) |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Branch | Branch | Many-to-One |

---

### Branch
**File:** `Models/Branch.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Name | string | Branch name |
| Slug | string | URL-friendly identifier |
| City | string | City location |
| Address | string | Full address |
| Phone | string? | Contact phone |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Enrollments | ICollection<Enrollment> | One-to-Many |

---

### Plan
**File:** `Models/Plan.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Name | string | Plan name (e.g., "1 Month", "Full Year") |
| DurationMonths | int | Duration in months (1, 3, 6, 12) |
| DiscountPercent | decimal? | Optional discount percentage |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Enrollments | ICollection<Enrollment> | One-to-Many |

---

## People & Relationships

### Parent
**File:** `Models/Parent.cs`
**Inherits:** Person

| Attribute | Type | Description |
|-----------|------|-------------|
| Email | string? | Parent email |
| Phone | string | Parent phone |
| Relationship | RelationshipType | Relationship to student (enum) |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| StudentParents | ICollection<StudentParent> | Many-to-Many (via junction) |

**Enum: RelationshipType**
- Father
- Mother
- Guardian
- Grandfather
- Grandmother
- Uncle
- Aunt
- Other

---

### StudentParent (Junction)
**File:** `Models/StudentParent.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| StudentId | int | FK to Student |
| ParentId | int | FK to Parent |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Student | Student | Many-to-One |
| Parent | Parent | Many-to-One |

---

### Gender
**File:** `Models/Gender.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Slug | string | URL-friendly identifier |
| Name | string | Gender name (e.g., "Male", "Female") |

---

### Teacher
**File:** `Models/Teacher.cs`
**Inherits:** Employee

| Attribute | Type | Description |
|-----------|------|-------------|
| Specialization | string | Teaching specialization |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Groups | ICollection<GroupTeacher> | Many-to-Many (via junction) |

---

### Employee
**File:** `Models/Employee.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| FirstName | string | Employee first name |
| LastName | string | Employee last name |
| Email | string? | Employee email |
| Phone | string | Employee phone |
| Slug | string | URL-friendly identifier |
| DateOfBirth | DateOnly? | Birth date |
| GenderId | int? | FK to Gender |
| Gender | Gender | Navigation property |
| HireDate | DateTime | Employment start date |
| Salary | decimal | Employee salary |
| BranchId | int | FK to Branch |
| Branch | Branch | Navigation property |

---

### CommercialAgent
**File:** `Models/CommercialAgent.cs`
**Inherits:** Employee

Empty subclass - inherits all from Employee.

---

## Financial Models

### Payment
**File:** `Models/Payment.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| EnrollmentId | int | FK to Enrollment |
| AmountPaid | decimal | Amount paid in this transaction |
| FeeAmount | decimal | Monthly fee for the period |
| PeriodStart | DateTime | Payment period start |
| PeriodEnd | DateTime | Payment period end |
| Status | string | "Pending" / "Paid" / "Partial" / "Failed" |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Enrollment | Enrollment | Many-to-One |

---

## Lead & Marketing Models

### Intake
**File:** `Models/Intake.cs`
**Inherits:** Person

| Attribute | Type | Description |
|-----------|------|-------------|
| Email | string? | Prospect email |
| Phone | string? | Prospect phone |
| DateOfBirth | DateOnly? | Birth date |
| IntakeDate | DateTime | Intake/registration date |
| Status | IntakeStatus | Current status (enum) |
| FollowUpDate | DateTime? | Scheduled follow-up |
| Notes | string? | Internal notes |
| CommercialAgentId | int? | FK to CommercialAgent |
| LeadSourceId | int? | FK to LeadSource |
| SubjectId | int | FK to Subject (interest) |
| BranchId | int | FK to Branch |
| ConvertedToStudentId | int? | FK to Student (if converted) |
| IsIndependent | bool | Independent registration flag |
| TotalFees | decimal | Total quoted fees |
| AmountPaid | decimal | Amount already paid |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| LeadSource | LeadSource? | Many-to-One (optional) |
| CommercialAgent | CommercialAgent? | Many-to-One (optional) |
| Subject | Subject | Many-to-One |
| Branch | Branch | Many-to-One |
| ConvertedToStudent | Student? | One-to-One (optional) |

**Enum: IntakeStatus**
- New
- Contacted
- Interested
- Enrolled
- NotInterested

---

### LeadSource
**File:** `Models/LeadSource.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| LeadSourceType | LeadSourceType | Type of lead source |
| OpcId | int? | FK to Opc (if OPC source) |
| AdId | int? | FK to Ad (if Ad source) |
| BranchId | int | FK to Branch |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Branch | Branch | Many-to-One |
| Opc | Opc? | Many-to-One (optional) |
| Ad | Ad? | Many-to-One (optional) |
| Intakes | ICollection<Intake> | One-to-Many |

**Enum: LeadSourceType**
- Opc
- Ad
- WalkIn
- Referral
- Website
- Phone

---

### Ad
**File:** `Models/Ad.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Name | string | Ad campaign name |
| Slug | string | URL-friendly identifier |
| PlatformId | int | FK to Platform |
| BranchId | int | FK to Branch |

**Navigation Properties:**
| Property | Type | Relationship |
|----------|------|--------------|
| Branch | Branch | Many-to-One |
| Platform | Platform | Many-to-One |

---

### Opc
**File:** `Models/Opc.cs`
**Inherits:** Employee

| Attribute | Type | Description |
|-----------|------|-------------|
| Intakes | IEnumerable<Intake> | Related intakes generated |

---

### Platform
**File:** `Models/Platform.cs`
**Inherits:** BaseEntity

| Attribute | Type | Description |
|-----------|------|-------------|
| Slug | string | URL-friendly identifier |
| Name | string | Platform name (e.g., "Facebook", "Google Ads") |

---

## Entity Relationship Diagram Summary

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  BaseEntity     │◄────┤     Person       │◄────┤     Student     │
└─────────────────┘     └──────────────────┘     └────────┬────────┘
                                                          │
         ┌────────────────┬────────────────┬──────────────┼──────────────┐
         │                │                │              │              │
         ▼                ▼                ▼              ▼              ▼
┌─────────────┐  ┌──────────────┐  ┌──────────────┐ ┌──────────┐ ┌──────────┐
│Enrollment   │  │StudentParent │  │    Intake    │ │  Gender  │ │  Group   │
└──────┬──────┘  └──────┬───────┘  └──────┬───────┘ └──────────┘ └────┬─────┘
       │                │                 │                          │
       │                │                 │                          │
       ▼                ▼                 ▼                          ▼
┌─────────────┐  ┌──────────────┐  ┌──────────────┐           ┌──────────────┐
│   Payment   │  │    Parent    │  │    Subject   │           │ GroupTeacher │
└─────────────┘  └──────────────┘  └──────────────┘           └──────┬───────┘
                                                                      │
                                                                      ▼
                                                                ┌──────────┐
                                                                │  Teacher │
                                                                └──────────┘
```

---

## Key Relationship Patterns

1. **Inheritance:** Student → Person → BaseEntity
2. **Many-to-Many with Junction:** Student ↔ Parent (via StudentParent)
3. **Many-to-Many with Junction:** Group ↔ Teacher (via GroupTeacher)
4. **One-to-Many:** Student → Enrollments → Payments
5. **Polymorphic Association:** LeadSource → Opc or Ad (via nullable FKs)
6. **Self-Referential:** Intake → Student (ConvertedToStudent)
