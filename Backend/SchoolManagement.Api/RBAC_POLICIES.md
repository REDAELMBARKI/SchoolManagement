# RBAC Policies Specification - School Management System

**Version**: 1.0  
**Date**: August 1, 2026  
**Purpose**: Define authorization policies with their requirements (roles, claims, conditions)

---

## 📋 Policy Structure

Each policy defines:
- **Policy Name**: Unique identifier for the policy
- **Required Roles**: One or more roles that satisfy this policy
- **Required Claims**: Specific JWT claims that must be present
- **Additional Requirements**: Business logic conditions (branch ownership, group assignment, etc.)
- **Purpose**: What this policy protects

---

## 🔐 System-Wide Policies

### Policy: `IsSuperAdmin`
- **Required Roles**: SuperAdmin
- **Required Claims**: None
- **Additional Requirements**: None
- **Purpose**: Full system access across all branches

---

### Policy: `HasBranchAccess`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**: 
  - `branchId` (must be valid GUID)
- **Additional Requirements**: 
  - Non-SuperAdmin users must have BranchId claim
  - BranchId must exist in database
- **Purpose**: Ensure user belongs to a branch (multi-tenancy)

---

### Policy: `AdminPanelAccess`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist
- **Required Claims**: None
- **Additional Requirements**: None
- **Purpose**: Access to admin panel/dashboard

---

### Policy: `FullBranchAdmin`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**: None
- **Additional Requirements**: None
- **Purpose**: Full administrative access to branch operations

---

## 💰 Financial Workflow Policies

### Policy: `ManageInvoices`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**: 
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Must own the invoice's branch
  - SuperAdmin: No branch restriction
- **Purpose**: Create, update, delete invoices and add charges

---

### Policy: `RecordPayments`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Must own the payment's branch
  - SuperAdmin: No branch restriction
- **Purpose**: Record invoice settlement payments

---

### Policy: `RecordRegistrationPayment`
- **Required Roles**: SuperAdmin, Director, Receptionist
- **Required Claims**:
  - `branchId` (all roles)
- **Additional Requirements**:
  - Payment must be for student in same branch
  - Payment must be at time of student registration/enrollment
- **Purpose**: Record initial registration payment when student enrolls

---

### Policy: `IssueRefunds`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Must own the payment's branch
  - Refund amount cannot exceed payment amount
- **Purpose**: Issue refunds on payments

---

### Policy: `ManageExpenses`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Expense must belong to their branch
- **Purpose**: Record and manage branch expenses

---

### Policy: `ViewFinancialReports`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Can only view own branch reports
- **Purpose**: Access financial analytics and reports

---

## 🎓 Academic Workflow Policies

### Policy: `ManageStudents`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Director/Administrator: Student must belong to their branch
- **Purpose**: Create, update, delete students

---

### Policy: `CreateStudent`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Student will be assigned to user's branch
  - Receptionist: Can only create from their own intakes
- **Purpose**: Register new students

---

### Policy: `ViewStudents`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Director/Administrator/Receptionist: Students in same branch
  - Teacher: Students in assigned groups only
- **Purpose**: View student information

---

### Policy: `ManageGroups`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Director/Administrator: Group must belong to their branch
- **Purpose**: Create, update, delete groups (classes/cohorts)

---

### Policy: `ViewGroups`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Director/Administrator/Receptionist: Groups in same branch
  - Teacher: Only assigned groups
- **Purpose**: View group information

---

### Policy: `ManageSchedules`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Schedule must belong to user's branch
- **Purpose**: Create, update, delete schedules and check conflicts

---

### Policy: `ViewSchedules`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Teacher: Only schedules for assigned groups
  - Others: Schedules in same branch
- **Purpose**: View timetables and schedules

---

### Policy: `RecordGrades`
- **Required Roles**: SuperAdmin, Director, Administrator, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
  - `userId` (Teacher: to verify assignment)
- **Additional Requirements**:
  - Teacher: Must be assigned to the group
  - Teacher: Can only grade students in assigned groups
  - Others: Grade must belong to their branch
- **Purpose**: Create and update student grades

---

### Policy: `ViewGrades`
- **Required Roles**: SuperAdmin, Director, Administrator, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Teacher: Only grades for assigned groups
  - Others: Grades in same branch
- **Purpose**: View student academic performance

---

### Policy: `RecordAbsences`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
  - `userId` (Teacher: to verify assignment)
- **Additional Requirements**:
  - Teacher: Must be assigned to the group
  - Others: Absence must belong to their branch
- **Purpose**: Mark student attendance/absences

---

### Policy: `ManageEnrollments`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Enrollment must belong to user's branch
  - Student and Group must be in same branch
- **Purpose**: Enroll students in groups, update enrollments

---

### Policy: `CreateEnrollment`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Student must belong to user's branch
  - Receptionist: Can only enroll from their own converted leads
- **Purpose**: Enroll students in groups

---

## 👥 Staff Management Policies

### Policy: `ManageTeachers`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Teacher must belong to user's branch
- **Purpose**: Create, update, delete teachers

---

### Policy: `ViewTeachers`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - All: Teachers in same branch
  - Teacher: Can view self profile
- **Purpose**: View teacher information

---

### Policy: `UpdateOwnProfile`
- **Required Roles**: Teacher
- **Required Claims**:
  - `userId` (must match teacher ID)
- **Additional Requirements**:
  - Teacher can only update their own profile
  - Cannot change role, salary, branch
- **Purpose**: Teachers update personal information

---

### Policy: `ManageAdministrators`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Administrator must belong to their branch
- **Purpose**: Create, update, delete administrators

---

### Policy: `ManageReceptionists`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Receptionist must belong to their branch
- **Purpose**: Create, update, delete receptionists

---

### Policy: `ManageCommercialAgents`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Agent must belong to their branch
- **Purpose**: Create, update, delete commercial agents

---

### Policy: `AssignTeachersToGroups`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Teacher and Group must be in same branch
- **Purpose**: Assign/unassign teachers to groups

---

## 📊 HR & Payroll Policies

### Policy: `ManageCommissions`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Commission must belong to their branch
- **Purpose**: Create, calculate, update commissions

---

### Policy: `ViewOwnCommissions`
- **Required Roles**: CommercialAgent
- **Required Claims**:
  - `userId` (must match agent ID)
  - `branchId`
- **Additional Requirements**:
  - Agent can only view their own commission records
- **Purpose**: Commercial agents view their earnings

---

### Policy: `ManageCommissionTiers`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Tier configuration for their branch only
- **Purpose**: Configure commission tier structure

---

### Policy: `ManagePayroll`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Payroll for their branch only
- **Purpose**: Process and record payroll payments

---

## 📞 Intake & Lead Management Policies

### Policy: `ManageIntakes`
- **Required Roles**: SuperAdmin, Director, Receptionist
- **Required Claims**:
  - `branchId` (all roles)
- **Additional Requirements**:
  - Intake must belong to user's branch
  - Receptionist: Full access to all branch intakes
- **Purpose**: Create, update, view, delete intakes (leads)

---

### Policy: `ConvertIntakeToStudent`
- **Required Roles**: SuperAdmin, Director, Receptionist
- **Required Claims**:
  - `branchId` (all roles)
  - `userId` (to track who converted)
- **Additional Requirements**:
  - Intake must belong to user's branch
  - Creates student + enrollment + invoice in same transaction
- **Purpose**: Convert lead to registered student

---

### Policy: `ViewOwnIntakes`
- **Required Roles**: CommercialAgent
- **Required Claims**:
  - `userId` (must match agent ID)
  - `branchId`
- **Additional Requirements**:
  - Agent can only view intakes they created
- **Purpose**: Commercial agents view their leads

---

### Policy: `CreateIntakeAsAgent`
- **Required Roles**: CommercialAgent
- **Required Claims**:
  - `userId` (to set as lead owner)
  - `branchId`
- **Additional Requirements**:
  - Intake will be owned by this agent
  - Agent can only create for their branch
- **Purpose**: Commercial agents create new leads

---

### Policy: `UpdateOwnIntake`
- **Required Roles**: CommercialAgent
- **Required Claims**:
  - `userId` (must match intake owner)
  - `branchId`
- **Additional Requirements**:
  - Agent can only update intakes they own
- **Purpose**: Commercial agents update their leads

---

## 📢 Marketing Policies

### Policy: `ManageAds`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Ad must belong to their branch
- **Purpose**: Create, update, delete advertising campaigns

---

### Policy: `ViewAds`
- **Required Roles**: SuperAdmin, Director, CommercialAgent
- **Required Claims**:
  - `branchId` (Director, CommercialAgent)
- **Additional Requirements**:
  - Director/Agent: View ads in their branch only
- **Purpose**: View advertising campaign information

---

### Policy: `ManagePlatforms`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Platform must belong to their branch
- **Purpose**: Create, update, delete advertising platforms

---

### Policy: `ManageLeadSources`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Lead source must belong to their branch
- **Purpose**: Configure lead source channels

---

### Policy: `ViewLeadSources`
- **Required Roles**: SuperAdmin, Director, Receptionist, CommercialAgent
- **Required Claims**:
  - `branchId` (all roles)
- **Additional Requirements**:
  - View lead sources in same branch only
- **Purpose**: View available lead sources

---

## 🏢 Branch Management Policies

### Policy: `ManageBranches`
- **Required Roles**: SuperAdmin
- **Required Claims**: None
- **Additional Requirements**: None
- **Purpose**: Create, update, delete branches

---

### Policy: `ViewOwnBranch`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - Non-SuperAdmin: Can only view their assigned branch
- **Purpose**: View branch configuration and details

---

### Policy: `UpdateOwnBranch`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Director: Can only update their own branch
- **Purpose**: Update branch settings and configuration

---

## ⚙️ Configuration Policies

### Policy: `ManageGenders`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Gender configuration is branch-specific
- **Purpose**: Create, update gender options

---

### Policy: `ManageSubjects`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Subject must belong to user's branch
- **Purpose**: Create, update, delete subjects

---

### Policy: `ManageLevels`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Level must belong to user's branch
- **Purpose**: Create, update, delete academic levels

---

### Policy: `ManageRooms`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Room must belong to user's branch
- **Purpose**: Create, update, delete rooms

---

### Policy: `ManagePlans`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Plan must belong to user's branch
- **Purpose**: Create, update pricing plans

---

## 📁 Media Management Policies

### Policy: `UploadMedia`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist
- **Required Claims**:
  - `branchId` (all roles)
- **Additional Requirements**:
  - File size limits
  - Allowed file types
  - Media belongs to user's branch
- **Purpose**: Upload files and documents

---

### Policy: `ViewMedia`
- **Required Roles**: SuperAdmin, Director, Administrator, Receptionist, Teacher
- **Required Claims**:
  - `branchId` (all non-SuperAdmin)
- **Additional Requirements**:
  - View media in same branch only
- **Purpose**: View and download files

---

### Policy: `DeleteMedia`
- **Required Roles**: SuperAdmin, Director, Administrator
- **Required Claims**:
  - `branchId` (Director, Administrator)
- **Additional Requirements**:
  - Media must belong to user's branch
- **Purpose**: Delete uploaded files

---

## 🔄 Special Workflow Policies

### Policy: `StudentRegistrationFlow`
- **Required Roles**: SuperAdmin, Director, Receptionist
- **Required Claims**:
  - `branchId` (all roles)
  - `userId` (to track who registered)
- **Additional Requirements**:
  - Must create: Student + Enrollment + Invoice + (Optional) Registration Payment
  - All entities must belong to same branch
  - Atomic transaction
- **Purpose**: Complete student registration workflow

---

### Policy: `IntakeConversionFlow`
- **Required Roles**: SuperAdmin, Director, Receptionist
- **Required Claims**:
  - `branchId` (all roles)
  - `userId` (to track converter)
- **Additional Requirements**:
  - Intake must exist and belong to branch
  - Converts to: Student + Enrollment + Invoice
  - Updates intake status to "Converted"
  - Atomic transaction
- **Purpose**: Convert lead to student with full setup

---

### Policy: `PaymentSettlementFlow`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Invoice must exist with outstanding balance
  - Payment amount cannot exceed balance
  - Updates invoice paid amount and status
  - Atomic transaction
- **Purpose**: Settle invoice with payment

---

### Policy: `RefundProcessFlow`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Payment must exist
  - Refund amount cannot exceed payment amount
  - Updates payment status
  - Deducts from invoice paid amount
  - Atomic transaction
- **Purpose**: Process payment refund

---

### Policy: `CommissionCalculationFlow`
- **Required Roles**: SuperAdmin, Director
- **Required Claims**:
  - `branchId` (Director only)
- **Additional Requirements**:
  - Based on enrollment conversions
  - Follows commission tier structure
  - Tracks agent performance
- **Purpose**: Calculate and record agent commissions

---

## 📋 Policy Implementation Checklist

### When Creating a Policy:
1. **Define Policy Name** - Clear, descriptive identifier
2. **Specify Required Roles** - One or more roles
3. **List Required Claims** - JWT claims needed
4. **Document Additional Requirements** - Business logic conditions
5. **State Purpose** - What this policy protects
6. **Consider Branch Isolation** - For all non-SuperAdmin policies
7. **Consider Resource Ownership** - For user-specific data
8. **Handle Atomic Operations** - For multi-step workflows

---

## ✅ Summary

### Total Policies Defined: 60+

### Policy Categories:
- **System-Wide**: 4 policies
- **Financial Workflow**: 6 policies
- **Academic Workflow**: 11 policies
- **Staff Management**: 8 policies
- **HR & Payroll**: 4 policies
- **Intake & Lead Management**: 5 policies
- **Marketing**: 4 policies
- **Branch Management**: 3 policies
- **Configuration**: 6 policies
- **Media Management**: 3 policies
- **Special Workflows**: 5 policies

### Common Patterns:
- Branch isolation (all non-SuperAdmin)
- Resource ownership validation
- Role-based access
- Claim-based authorization
- Atomic transaction workflows
- Teacher group assignment validation
- Agent lead ownership validation

---

**This document should be used when implementing authorization policies in the application. Each policy can be translated into ASP.NET Core authorization policies or custom authorization handlers.**
