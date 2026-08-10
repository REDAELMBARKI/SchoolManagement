# RBAC Design - School Management System

**Version**: 2.0 (Final)  
**Date**: August 1, 2026  
**Purpose**: Role-based access control specification for all API endpoints

---

## 🎯 System Roles Overview

### Admin Panel Access (4 Roles)

#### 1. SuperAdmin (School Owner)
**Admin Panel**: ✅ Full Access (All Branches)  
**Scope**: Cross-branch visibility  

**Responsibilities**:
- Manage all branches (create, update, delete branches)
- View reports and data across all branches
- Configure system-wide settings
- Manage users and roles globally
- Full access to all modules and data

---

#### 2. Director (Branch Manager)
**Admin Panel**: ✅ Full Access (Single Branch)  
**Scope**: Single branch only  

**Responsibilities**:
- **Financial Control**: Invoices, payments, refunds, expenses, commissions, payroll
- **Academic Control**: Schedules, groups, grades, absences, level tests, enrollments
- **Staff Management**: Teachers, Administrators, Receptionists
- **Student Management**: Create, update, delete students
- **Reports**: All branch analytics and reports
- **Settings**: Branch-specific configuration
- ❌ Cannot access other branches' data

---

#### 3. Administrator (Academic Coordinator)
**Admin Panel**: ✅ Limited Access (Academic Only)  
**Scope**: Single branch only  

**Responsibilities**:
- Manage level tests and academic assessments
- Manage enrollments (assign students to groups)
- Manage attendance and absences
- Manage schedules and timetables
- Manage groups (classes/cohorts)
- Manage teachers and subject assignments
- Record and manage grades
- Manage students (create, update, view)
- Manage subjects, levels, rooms
- View academic reports
- ❌ **NO access to financial operations** (payments, invoices, refunds, expenses)
- ❌ Cannot access other branches' data

---

#### 4. Receptionist (Front Desk)
**Admin Panel**: ✅ Very Limited Access (Intakes & Attendance)  
**Scope**: Single branch only  

**Responsibilities**:
- Manage intakes/leads (create, update, view, convert to students)
- Record attendance and absences
- View student information (read-only)
- View groups and schedules (read-only)
- Record registration payments (when students first register)
- ❌ **Cannot manage**: Academic configuration, staff, financial operations (except registration payments), reports
- ❌ Cannot access other branches' data

---

### No Admin Panel Access (2 Roles)

#### 5. Teacher (Instructor)
**Admin Panel**: ❌ No Access  
**Scope**: Assigned groups only  
**Access Via**: Mobile App / Teacher Portal Only  

**Responsibilities**:
- View assigned groups and schedules
- Record grades for assigned groups
- Record absences for assigned groups
- View students in assigned groups (read-only)
- ❌ No admin panel access
- ❌ Cannot see unassigned groups or other teachers' data

---

#### 6. CommercialAgent (Sales Representative)
**Admin Panel**: ❌ No Access  
**Scope**: Own leads only  
**Access Via**: Mobile App / Sales Portal Only  

**Responsibilities**:
- Manage own intakes/leads (create, update, view)
- Convert own leads to students
- View own commission records and conversion stats
- ❌ No admin panel access
- ❌ Cannot see other agents' leads or data

---

#### ⚠️ OPC Entity (Not a System Role)
**Note**: OPC is stored in the database as a Person entity but has **NO system role or login access**. OPC staff are recorded for organizational purposes only.

---

## 📊 Permission Matrix

### Legend
- ✅ = Full Access (Create, Read, Update, Delete)
- 📖 = Read Only
- 🔒 = Own Data Only
- ❌ = No Access
- 🏢 = Branch Isolated (single branch only)
- 👥 = Assigned Groups Only

---

## 1. Academic Module

| Resource | SuperAdmin | Director | Administrator | Receptionist | Teacher |
|----------|------------|----------|---------------|--------------|---------|
| **Students** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 🔒 (from leads) | ❌ |
| Read   | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 👥 (assigned groups) |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Groups (Classes/Cohorts)** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read   | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 👥 (assigned) |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Schedules** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read   | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 👥 (assigned) |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Check Conflicts | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Grades** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | 🏢 👥 (assigned groups) |
| Read   | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | 🏢 👥 (assigned groups) |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | 🏢 👥 (assigned groups) |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Absences** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 👥 (assigned groups) |
| Read   | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 👥 (assigned groups) |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 👥 (assigned groups) |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Teachers** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read   | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🔒 (self only) |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | 🔒 (self profile) |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Assign to Groups | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Subjects** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 📖 |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Levels** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 📖 |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

| **Rooms** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 📖 |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

---

## 2. Financial Module

| Resource | SuperAdmin | Director | Administrator | Receptionist | Teacher |
|----------|------------|----------|---------------|--------------|---------|
| **Enrollments** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 🔒 (from leads) | ❌ |
| Read | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | ❌ |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Invoices** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Add Charge | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Generate PDF | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Payments** |
| Registration Payment | ✅ | 🏢 ✅ | ❌ | 🏢 ✅ (at registration) | ❌ |
| Settlement Payment | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read All | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Refunds** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Expenses** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

---

## 3. HR & Payroll Module

| Resource | SuperAdmin | Director | Administrator | Receptionist | Teacher |
|----------|------------|----------|---------------|--------------|---------|
| **Commercial Agents** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Commissions** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Commission Tiers** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Payroll Payments** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

---

## 4. Marketing & Leads Module

| Resource | SuperAdmin | Director | Administrator | Receptionist | Teacher |
|----------|------------|----------|---------------|--------------|---------|
| **Intakes (Leads)** |
| Create | ✅ | 🏢 ✅ | ❌ | 🏢 ✅ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | 🏢 ✅ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | 🏢 ✅ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Convert to Student | ✅ | 🏢 ✅ | ❌ | 🏢 ✅ | ❌ |

| **Ads** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Platforms** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Lead Sources** |
| Create | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | ❌ | 🏢 📖 | ❌ |
| Update | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

---

## 5. System Configuration

| Resource | SuperAdmin | Director | Administrator | Receptionist | Teacher |
|----------|------------|----------|---------------|--------------|---------|
| **Branches** |
| Create | ✅ | ❌ | ❌ | ❌ | ❌ |
| Read | ✅ | 🔒 (own) | 🔒 (own) | 🔒 (own) | 🔒 (own) |
| Update | ✅ | 🔒 (own) | ❌ | ❌ | ❌ |
| Delete | ✅ | ❌ | ❌ | ❌ | ❌ |

| **Genders** |
| Create | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Read | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 📖 |
| Update | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |
| Delete | ✅ | 🏢 ✅ | ❌ | ❌ | ❌ |

| **Media/Files** |
| Upload | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 ✅ | ❌ |
| Read | ✅ | 🏢 ✅ | 🏢 ✅ | 🏢 📖 | 🏢 📖 |
| Delete | ✅ | 🏢 ✅ | 🏢 ✅ | ❌ | ❌ |

---

## 📋 Controller Authorization Reference

### StudentController
- **GET /api/students** → SuperAdmin, Director, Administrator, Receptionist, Teacher
- **GET /api/students/{id}** → SuperAdmin, Director, Administrator, Receptionist, Teacher (assigned groups)
- **POST /api/students** → SuperAdmin, Director, Administrator, Receptionist
- **PUT /api/students/{id}** → SuperAdmin, Director, Administrator
- **DELETE /api/students/{id}** → SuperAdmin, Director, Administrator

### EnrollmentController
- **GET /api/enrollments** → SuperAdmin, Director, Administrator
- **GET /api/enrollments/{id}** → SuperAdmin, Director, Administrator, Receptionist
- **POST /api/enrollments** → SuperAdmin, Director, Administrator, Receptionist
- **PUT /api/enrollments/{id}** → SuperAdmin, Director, Administrator
- **DELETE /api/enrollments/{id}** → SuperAdmin, Director

### InvoiceController
- **GET /api/invoices** → SuperAdmin, Director
- **GET /api/invoices/{id}** → SuperAdmin, Director
- **GET /api/invoices/enrollment/{enrollmentId}** → SuperAdmin, Director
- **POST /api/invoices/{id}/charge** → SuperAdmin, Director
- **POST /api/invoices/{id}/generate-pdf** → SuperAdmin, Director

### PaymentController
- **GET /api/payments** → SuperAdmin, Director
- **GET /api/payments/{id}** → SuperAdmin, Director
- **POST /api/payments/registration** → SuperAdmin, Director, Receptionist
- **POST /api/payments/settle** → SuperAdmin, Director
- **POST /api/payments/{id}/refund** → SuperAdmin, Director
- **GET /api/payments/{id}/refunds** → SuperAdmin, Director

### ScheduleController
- **GET /api/schedules** → SuperAdmin, Director, Administrator, Receptionist, Teacher
- **GET /api/schedules/{id}** → SuperAdmin, Director, Administrator, Teacher
- **POST /api/schedules** → SuperAdmin, Director, Administrator
- **PUT /api/schedules/{id}** → SuperAdmin, Director, Administrator
- **DELETE /api/schedules/{id}** → SuperAdmin, Director, Administrator
- **GET /api/schedules/conflicts** → SuperAdmin, Director, Administrator

### GradeController
- **GET /api/grades/student/{studentId}** → SuperAdmin, Director, Administrator, Teacher
- **POST /api/grades** → SuperAdmin, Director, Administrator, Teacher (assigned groups)
- **PUT /api/grades/{id}** → SuperAdmin, Director, Administrator, Teacher (assigned groups)
- **DELETE /api/grades/{id}** → SuperAdmin, Director, Administrator

### AbsenceController
- **GET /api/absences/student/{studentId}** → SuperAdmin, Director, Administrator, Receptionist, Teacher
- **POST /api/absences** → SuperAdmin, Director, Administrator, Receptionist, Teacher (assigned groups)
- **PUT /api/absences/{id}** → SuperAdmin, Director, Administrator, Receptionist, Teacher (assigned groups)
- **DELETE /api/absences/{id}** → SuperAdmin, Director, Administrator

### IntakeController
- **GET /api/intakes** → SuperAdmin, Director, Receptionist
- **GET /api/intakes/{id}** → SuperAdmin, Director, Receptionist
- **POST /api/intakes** → SuperAdmin, Director, Receptionist
- **PUT /api/intakes/{id}** → SuperAdmin, Director, Receptionist
- **DELETE /api/intakes/{id}** → SuperAdmin, Director

### GroupController
- **GET /api/groups** → SuperAdmin, Director, Administrator, Receptionist, Teacher
- **GET /api/groups/{id}** → SuperAdmin, Director, Administrator, Teacher
- **POST /api/groups** → SuperAdmin, Director, Administrator
- **PUT /api/groups/{id}** → SuperAdmin, Director, Administrator
- **DELETE /api/groups/{id}** → SuperAdmin, Director, Administrator

### TeacherController
- **GET /api/teachers** → SuperAdmin, Director, Administrator, Receptionist, Teacher
- **GET /api/teachers/{id}** → SuperAdmin, Director, Administrator, Teacher (self)
- **POST /api/teachers** → SuperAdmin, Director, Administrator
- **PUT /api/teachers/{id}** → SuperAdmin, Director, Administrator, Teacher (self profile)
- **DELETE /api/teachers/{id}** → SuperAdmin, Director

### SubjectController
- **GET /api/subjects** → SuperAdmin, Director, Administrator, Receptionist, Teacher
- **GET /api/subjects/{id}** → SuperAdmin, Director, Administrator, Teacher
- **POST /api/subjects** → SuperAdmin, Director, Administrator
- **PUT /api/subjects/{id}** → SuperAdmin, Director, Administrator
- **DELETE /api/subjects/{id}** → SuperAdmin, Director, Administrator

### ExpenseController
- **GET /api/expenses** → SuperAdmin, Director
- **GET /api/expenses/{id}** → SuperAdmin, Director
- **POST /api/expenses** → SuperAdmin, Director
- **PUT /api/expenses/{id}** → SuperAdmin, Director
- **DELETE /api/expenses/{id}** → SuperAdmin, Director

### CommercialAgentController
- **GET /api/commercialagents** → SuperAdmin, Director
- **GET /api/commercialagents/{id}** → SuperAdmin, Director
- **POST /api/commercialagents** → SuperAdmin, Director
- **PUT /api/commercialagents/{id}** → SuperAdmin, Director
- **DELETE /api/commercialagents/{id}** → SuperAdmin, Director

### CommissionController
- **GET /api/commissions** → SuperAdmin, Director
- **GET /api/commissions/agent/{agentId}** → SuperAdmin, Director
- **POST /api/commissions** → SuperAdmin, Director
- **PUT /api/commissions/{id}** → SuperAdmin, Director

### BranchController
- **GET /api/branches** → SuperAdmin, Director (own), Administrator (own), Receptionist (own), Teacher (own)
- **GET /api/branches/{id}** → SuperAdmin, Director (own only)
- **POST /api/branches** → SuperAdmin
- **PUT /api/branches/{id}** → SuperAdmin, Director (own only)
- **DELETE /api/branches/{id}** → SuperAdmin

---

## 🔐 Authorization Rules

### Branch Isolation
- **SuperAdmin**: Can access data from all branches
- **All Other Roles**: Can only access data from their assigned branch
- Every data query (except SuperAdmin) must filter by `BranchId` from user context
- Creating new records automatically assigns current user's `BranchId`

### Teacher Assignment Rules
- Teachers can only access groups they are assigned to
- When recording grades or absences, validate teacher assignment to that group
- Teachers cannot see students outside their assigned groups

### Receptionist Restrictions
- Can only manage intakes and record attendance
- Can record registration payments when student first registers
- Cannot access financial reports, academic configuration, or staff management

### Administrator Restrictions
- Full academic control (schedules, groups, enrollments, grades, absences)
- **Cannot access any financial operations**: invoices, payments, refunds, expenses, commissions, payroll
- Can manage students and teachers

### Director Full Control
- Complete access to all branch operations (financial + academic)
- Can manage all staff within the branch
- Cannot access other branches' data

---

## 🎯 Quick Reference

### Who Can Access Financial Operations?
- **Invoices**: SuperAdmin, Director
- **Payments (Settlement)**: SuperAdmin, Director
- **Registration Payments**: SuperAdmin, Director, Receptionist
- **Refunds**: SuperAdmin, Director
- **Expenses**: SuperAdmin, Director
- **Commissions**: SuperAdmin, Director
- **Payroll**: SuperAdmin, Director

### Who Can Access Academic Operations?
- **Groups**: SuperAdmin, Director, Administrator
- **Schedules**: SuperAdmin, Director, Administrator
- **Enrollments**: SuperAdmin, Director, Administrator, Receptionist
- **Grades**: SuperAdmin, Director, Administrator, Teacher (assigned groups)
- **Absences**: SuperAdmin, Director, Administrator, Receptionist, Teacher (assigned groups)

### Who Can Manage Staff?
- **Teachers**: SuperAdmin, Director, Administrator
- **Commercial Agents**: SuperAdmin, Director
- **Administrators**: SuperAdmin, Director
- **Receptionists**: SuperAdmin, Director

---

## ✅ Summary

### Access Levels
1. **SuperAdmin** → Everything, all branches
2. **Director** → Everything in branch (financial + academic)
3. **Administrator** → Academic operations only (no financial access)
4. **Receptionist** → Intakes, attendance, registration payments
5. **Teacher** → Assigned groups only (grades, absences)
6. **CommercialAgent** → Not in permission tables (separate mobile app access)

### Key Principles
- Branch isolation for all non-SuperAdmin roles
- Financial operations: Director only (except registration payments)
- Academic operations: Director + Administrator
- Teachers: Assigned groups only
- Receptionist: Very limited access (intakes + attendance + registration payments)
- OPC entity exists in database but has no system role

---

**This document should be used as the single source of truth when implementing authorization on controllers and API endpoints.**
