# Commission Tracking Feature - Implementation Summary

## Overview
Commission Tracking is a comprehensive financial feature for managing commissions for OPCs (phone/in-person lead handlers) and Commercial Agents based on enrollments.

---

## ✅ What Was Already Implemented

The Commission feature was already 95% complete before this session with:

### Domain Layer ✅
- **Entity:** `Commission.cs` with rich domain logic
  - Factory methods: `CreateForOpc()`, `CreateForAgent()`
  - Domain methods: `Block()`, `Approve()`, `MarkAsPaid()`
  - Validation rules and business constraints
  - Support for both OPC (per-enrollment) and Agent (monthly tiered) commission types

- **Enums:**
  - `CommissionStatus`: Approved, Blocked, Paid
  - `EarnerType`: Opc, CommercialAgent

### Infrastructure Layer ✅
- **Repository:** `CommissionRepository.cs` with specialized queries:
  - `GetByEarnerAsync()` - Get commissions by OPC or Agent
  - `GetByPeriodAsync()` - Get commissions for a specific month
  - `CountAgentEnrollmentsForMonthAsync()` - Count sales for agent
  - `OpcCommissionExistsForEnrollmentAsync()` - Idempotency guard
  - `GetApprovedByPeriodAsync()` - For salary lockout job
  - `GetOpcCommissionByEnrollmentAsync()` - For auto-blocking

### Application Layer ✅
- **Service:** `CommissionService.cs` with full business logic:
  - OPC commission processing (event-driven per enrollment)
  - Agent monthly tiered commission calculation
  - Salary lockout automation (day 13, 8pm UTC)
  - Commission blocking (manual and auto on enrollment drop)
  - Approve and MarkAsPaid workflows

- **Event Handlers:**
  - `OpcCommissionHandler` - Creates OPC commission when enrollment created
  - `EnrollmentDroppedCommissionHandler` - Auto-blocks commission when enrollment dropped

- **Settings:** `CommissionSettings.cs` with tiered structure

### API Layer (Partial) ⚠️
- **Controller:** `CommissionController.cs` with basic endpoints:
  - GET `/api/commissions/earner/{earnerId}?earnerType=X`
  - GET `/api/commissions/period?year=2026&month=8`
  - POST `/api/commissions/{id}/block`

---

## ✅ What Was Added THIS Session

### Missing Controller Endpoints Added

1. **GET `/api/commissions/{id}`** - Get commission by ID
   - Allows viewing details of a specific commission
   - Returns 404 if not found

2. **GET `/api/commissions`** - Get all commissions (admin view)
   - View all commissions across all earners and periods
   - Useful for admin dashboard

3. **POST `/api/commissions/{id}/approve`** - Approve blocked commission
   - Allows re-activating a previously blocked commission
   - Only works before salary lockout
   - Returns 400 if already paid or approved

4. **POST `/api/commissions/{id}/mark-paid`** - Manual paid override
   - Manually mark commission as paid
   - Typically handled by automated salary lockout job
   - Returns 400 if not approved

### Service Interface Methods Added

Added method signatures to `ICommissionService.cs`:
- `Task<CommissionResponseDto> GetByIdAsync(Guid id)`
- `Task<List<CommissionResponseDto>> GetAllAsync()`
- `Task<CommissionResponseDto> ApproveAsync(Guid id)` (already had implementation)
- `Task<CommissionResponseDto> MarkAsPaidAsync(Guid id)` (already had implementation)

### Service Implementation Methods Added

Added implementations to `CommissionService.cs`:
- `GetByIdAsync()` - Retrieve single commission with NotFoundException
- `GetAllAsync()` - Retrieve all commissions

---

## 📋 Complete API Reference

### Query Endpoints

| Method | Endpoint | Description | Query Params |
|--------|----------|-------------|--------------|
| GET | `/api/commissions` | Get all commissions | None |
| GET | `/api/commissions/{id}` | Get commission by ID | None |
| GET | `/api/commissions/earner/{earnerId}` | Get by earner (OPC/Agent) | `?earnerType=Opc` or `CommercialAgent` |
| GET | `/api/commissions/period` | Get by month | `?year=2026&month=8` |

### Command Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| POST | `/api/commissions/{id}/block` | Block commission | `{ "reason": "string" }` |
| POST | `/api/commissions/{id}/approve` | Approve blocked commission | None |
| POST | `/api/commissions/{id}/mark-paid` | Mark as paid (manual) | None |

---

## 🔄 Business Workflows

### 1. OPC Commission Flow (Event-Driven)
```
Enrollment Created Event
  → OpcCommissionHandler
    → StudentId → Intake → OpcLeadSource → OpcId
    → CommissionService.ProcessOpcCommissionAsync()
      → Create Commission (Status: Approved)
      → Amount: Fixed from settings
      → PeriodMonth: Current month
```

### 2. Commercial Agent Commission Flow (Monthly Job)
```
End of Month Job (Hangfire)
  → CommissionService.ProcessAgentMonthlyCommissionsAsync(year, month)
    → For each agent:
      → Count enrollments via Intake.CommercialAgentId
      → Resolve tier from settings
      → Create Commission (Status: Approved)
      → Amount: From tier
      → PeriodMonth: Target month
```

### 3. Commission Blocking Flow
```
Manual Block:
  Manager → POST /api/commissions/{id}/block
    → Commission.Block(reason)
    → Status: Blocked

Auto Block (Enrollment Dropped):
  Enrollment Dropped Event
    → EnrollmentDroppedCommissionHandler
      → CommissionService.BlockOpcCommissionByEnrollmentAsync()
        → Find linked OPC commission
        → Commission.Block("Enrollment dropped")
```

### 4. Salary Lockout Flow (Day 13, 8pm UTC)
```
Hangfire Job (13th of month, 8pm UTC)
  → CommissionService.ProcessSalaryLockoutAsync(year, month)
    → GetApprovedByPeriodAsync()
    → For each Approved commission:
      → Commission.MarkAsPaid()
      → Status: Paid (permanent, no changes allowed)
```

---

## 🔒 Business Rules

### Salary Lockout
- Calculated on day 13 at 8pm UTC for current month
- After lockout:
  - ✅ Paid commissions: Permanent, no changes
  - ✅ Blocked commissions: Stay blocked
  - ❌ Cannot create new commissions
  - ❌ Cannot block/approve/modify

### State Transitions
```
[Created] → Approved → Blocked → Approved (cycle allowed before lockout)
                    ↓
                  Paid (terminal state)
```

### Idempotency
- OPC: One commission per enrollment (checked by `SourceEnrollmentId`)
- Agent: One commission per agent per month (checked by `EarnerId` + `PeriodMonth`)

---

## 📊 Commission Types

### OPC Commission
- **Trigger:** Enrollment created
- **Amount:** Fixed (from `CommissionSettings.OpcFlatAmount`)
- **Period:** Month of enrollment
- **Tracking:** Links to `SourceEnrollmentId`
- **Auto-Block:** When enrollment is dropped

### Commercial Agent Commission
- **Trigger:** End of month job
- **Amount:** Tiered based on monthly sales count
- **Period:** Target calculation month
- **Tracking:** Records sales count and applied tier
- **Example Tiers:**
  ```json
  {
    "AgentTiers": [
      { "MinSalesCount": 1, "MaxSalesCount": 5, "Amount": 500 },
      { "MinSalesCount": 6, "MaxSalesCount": 10, "Amount": 1000 },
      { "MinSalesCount": 11, "MaxSalesCount": null, "Amount": 1500 }
    ]
  }
  ```

---

## 🗄️ Database Schema

### Commissions Table
| Column | Type | Description |
|--------|------|-------------|
| Id | Guid | Primary key |
| EarnerId | Guid | OpcId or CommercialAgentId |
| EarnerType | Enum | Opc or CommercialAgent |
| Amount | Decimal | Commission amount |
| PeriodMonth | DateOnly | Month earned (yyyy-MM-01) |
| Status | Enum | Approved, Blocked, Paid |
| SourceEnrollmentId | Guid? | For OPC only |
| SalesCountAtCalculation | Int? | For Agent only |
| AppliedTierMin | Int? | For Agent only |
| AppliedTierMax | Int? | For Agent only |
| BlockReason | String? | Set when blocked |
| CreatedAt | DateTime | Audit |
| UpdatedAt | DateTime | Audit |
| DeletedAt | DateTime? | Soft delete |

---

## ✅ Files Modified This Session

1. `SchoolManagement.Api/Controllers/CommissionController.cs`
   - Added 4 new endpoints (Approve, MarkAsPaid, GetById, GetAll)

2. `SchoolManagement.Application/Core/Interfaces/Services/ICommissionService.cs`
   - Added 4 method signatures

3. `SchoolManagement.Application/Core/Services/CommissionService.cs`
   - Added 2 method implementations (GetByIdAsync, GetAllAsync)

---

## 🎯 Checklist Status

✅ Story 26: Commission Tracking - **COMPLETE**

All requirements met:
- ✅ Calculate commission for agent (automated)
- ✅ Mark commission as paid
- ✅ View by agent
- ✅ View by period
- ✅ View by ID
- ✅ View all
- ✅ Block commission
- ✅ Approve commission

---

## 🚀 Next Steps

Commission Tracking is now 100% complete. Remaining ERP features:
1. Teacher management (full stack)
2. Subject management (controller + service)
3. Level management (controller + service)
4. Room management (controller + service)
5. Plan management (controller + service)
6. Refund management (controller only)
7. And more...
