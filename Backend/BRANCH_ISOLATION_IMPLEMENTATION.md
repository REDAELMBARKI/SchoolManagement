# Branch Isolation Implementation Guide

**Strategy**: JWT-based branch isolation (NOT multi-tenancy)  
**Date**: August 1, 2026  
**Purpose**: Prevent users from seeing other branches' data

---

## 🎯 Core Concept

- **Branch Isolation**: Each user belongs to ONE branch (stored in database)
- **SuperAdmin Exception**: SuperAdmin sees all branches
- **JWT Claims**: BranchId included in token for authorization
- **Service-Level Filtering**: Every query filters by BranchId automatically

---

## 📋 Implementation Steps

### Step 1: User Account Structure

Each user account must have:
- `Id` (GUID)
- `Email`
- `PasswordHash`
- `Role` (SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent)
- `BranchId` (GUID) - **NULL for SuperAdmin only**
- `FirstName`, `LastName`
- `IsActive`

**Example User Records:**
```
SuperAdmin:
- Id: guid-1
- Email: owner@school.com
- Role: SuperAdmin
- BranchId: NULL (can access all branches)

Director (Branch A):
- Id: guid-2
- Email: director.branchA@school.com
- Role: Director
- BranchId: branch-A-guid

Receptionist (Branch A):
- Id: guid-3
- Email: reception.branchA@school.com
- Role: Receptionist
- BranchId: branch-A-guid

Director (Branch B):
- Id: guid-4
- Email: director.branchB@school.com
- Role: Director
- BranchId: branch-B-guid
```

---

### Step 2: JWT Token Structure

When user logs in, generate JWT with these claims:

**For Non-SuperAdmin Users:**
```json
{
  "sub": "user-guid",
  "nameid": "user-guid",
  "email": "director.branchA@school.com",
  "role": "Director",
  "branchId": "branch-A-guid",
  "exp": 1234567890
}
```

**For SuperAdmin:**
```json
{
  "sub": "user-guid",
  "nameid": "user-guid",
  "email": "owner@school.com",
  "role": "SuperAdmin",
  "branchId": "",
  "exp": 1234567890
}
```

**Important**: 
- SuperAdmin has empty/null `branchId` claim
- All other users MUST have valid `branchId`
- Frontend uses this to determine branch context

---

### Step 3: CurrentUserContext Service

Create a service that extracts claims from JWT:

**Interface:**
```
ICurrentUserContext:
  - Guid NameIdentifier { get; }
  - Guid BranchId { get; }
  - string Role { get; }
  - string Email { get; }
  - bool IsSuperAdmin { get; }
```

**Implementation Logic:**
- Extract claims from `HttpContext.User.Claims`
- Parse `branchId` claim to GUID
- If `branchId` is empty/null → User is SuperAdmin
- If `branchId` is invalid → Throw unauthorized
- Store in scoped service (per HTTP request)

**SuperAdmin Detection:**
```
IsSuperAdmin = Role == "SuperAdmin" OR BranchId == Guid.Empty
```

---

### Step 4: Authorization Requirements

**Requirement 1: Must Have BranchId (Non-SuperAdmin)**

Policy: `HasBranchAccess`
- Check: User has valid `branchId` claim OR is SuperAdmin
- Applied to: All controllers except Login
- Purpose: Ensure all users belong to a branch

**Requirement 2: Branch Ownership**

Policy: `OwnsBranch` (resource-specific)
- Check: Resource's BranchId matches user's BranchId
- Applied to: All data access operations
- Purpose: Prevent cross-branch data access

---

### Step 5: Service-Level Branch Filtering

**Every service method must filter by BranchId:**

**Pattern for GetAll:**
```
GetAllAsync():
  if (IsSuperAdmin):
    return GetAllFromDatabase()
  else:
    branchId = CurrentUserContext.BranchId
    return GetAllFromDatabase().Where(x => x.BranchId == branchId)
```

**Pattern for GetById:**
```
GetByIdAsync(id):
  entity = GetFromDatabase(id)
  
  if (entity == null):
    throw NotFoundException
  
  if (NOT IsSuperAdmin AND entity.BranchId != CurrentUserContext.BranchId):
    throw ForbiddenException
  
  return entity
```

**Pattern for Create:**
```
CreateAsync(command):
  if (NOT IsSuperAdmin):
    branchId = CurrentUserContext.BranchId
    command.BranchId = branchId  // Force user's branch
  
  entity = Mapper.ToDomain(command)
  Save(entity)
```

**Pattern for Update:**
```
UpdateAsync(id, command):
  entity = GetFromDatabase(id)
  
  if (entity == null):
    throw NotFoundException
  
  if (NOT IsSuperAdmin AND entity.BranchId != CurrentUserContext.BranchId):
    throw ForbiddenException
  
  // Update entity
  Save(entity)
```

**Pattern for Delete:**
```
DeleteAsync(id):
  entity = GetFromDatabase(id)
  
  if (entity == null):
    throw NotFoundException
  
  if (NOT IsSuperAdmin AND entity.BranchId != CurrentUserContext.BranchId):
    throw ForbiddenException
  
  Delete(entity)
```

---

### Step 6: Controller Authorization

**Apply to ALL controllers (except LoginController):**

```
[Authorize]
[RequireBranch]  // Custom attribute
public class StudentController : ControllerBase
{
    // All endpoints automatically branch-filtered
}
```

**RequireBranch Attribute:**
- Validates user has `branchId` claim (if not SuperAdmin)
- Returns 403 Forbidden if missing
- Applied at controller level

---

### Step 7: Database Seeding

**Seed Initial Data:**

1. **Create Branches:**
   - Branch A (name: "Main Campus", city: "Cairo")
   - Branch B (name: "East Branch", city: "Alexandria")

2. **Create SuperAdmin:**
   - Email: superadmin@school.com
   - Role: SuperAdmin
   - BranchId: NULL
   - Password: (hashed)

3. **Create Branch A Staff:**
   - Director: director.branchA@school.com, BranchId: Branch-A-Guid
   - Administrator: admin.branchA@school.com, BranchId: Branch-A-Guid
   - Receptionist: reception.branchA@school.com, BranchId: Branch-A-Guid

4. **Create Branch B Staff:**
   - Director: director.branchB@school.com, BranchId: Branch-B-Guid
   - Administrator: admin.branchB@school.com, BranchId: Branch-B-Guid

**Seeding Method:**
- Create during database initialization
- Use Entity Framework migrations
- Or admin panel (SuperAdmin creates users)

---

### Step 8: User Account Creation Process

**Who Can Create Users:**
- SuperAdmin: Can create users for ANY branch
- Director: Can create users for THEIR branch only

**Creation Flow:**

1. **SuperAdmin Creates User:**
   - Selects Branch from dropdown (or none for SuperAdmin)
   - Selects Role
   - User is assigned to selected branch

2. **Director Creates User:**
   - Branch is automatically set to Director's branch (no dropdown)
   - Selects Role (Administrator, Receptionist, Teacher, CommercialAgent)
   - User can only access this branch

3. **User Cannot Change Branch:**
   - Once created, user's branch is permanent
   - Only SuperAdmin can change user's branch (update operation)

---

### Step 9: Frontend Considerations

**After Login:**
1. Frontend receives JWT token
2. Decode token to get `branchId` and `role`
3. Store token in localStorage/sessionStorage
4. Display branch name in UI header (if not SuperAdmin)

**UI Behavior:**

**For SuperAdmin:**
- Show "All Branches" or branch selector dropdown
- Can view data from any branch
- Can switch between branches in UI

**For Branch-Specific Users:**
- Show current branch name (read-only)
- NO branch selector
- All data filtered automatically by backend

**User Experience:**
- Director of Branch A cannot see Branch B data
- Receptionist of Branch B cannot see Branch A intakes
- Teacher assigned to Branch A groups only sees those groups

---

### Step 10: Security Considerations

**Prevent Branch Hopping:**
- User cannot modify `branchId` in token (JWT is signed)
- Backend validates token signature
- Backend re-verifies BranchId on every request

**Prevent Privilege Escalation:**
- User cannot change their role via API
- Only SuperAdmin can modify user roles
- BranchId enforcement at service level (not controller)

**Audit Logging:**
- Log which user accessed which data
- Include BranchId in all audit logs
- Track cross-branch access attempts (should fail)

---

## 🔒 Authorization Flow Example

### Example 1: Director (Branch A) Views Students

**Request:**
```
GET /api/students
Authorization: Bearer <jwt-with-branchId-A>
```

**Backend Flow:**
1. Extract BranchId from JWT → `branch-A-guid`
2. Check IsSuperAdmin → `false`
3. Query: `SELECT * FROM Students WHERE BranchId = 'branch-A-guid'`
4. Return: Only Branch A students

**Result:** ✅ Success - Director sees only Branch A students

---

### Example 2: Director (Branch A) Tries to View Branch B Student

**Request:**
```
GET /api/students/{branch-B-student-id}
Authorization: Bearer <jwt-with-branchId-A>
```

**Backend Flow:**
1. Extract BranchId from JWT → `branch-A-guid`
2. Fetch student by ID → Student has `BranchId = branch-B-guid`
3. Compare: `branch-A-guid != branch-B-guid`
4. Throw: `ForbiddenException`

**Result:** ❌ 403 Forbidden - Director cannot access Branch B student

---

### Example 3: SuperAdmin Views All Students

**Request:**
```
GET /api/students
Authorization: Bearer <jwt-with-empty-branchId>
```

**Backend Flow:**
1. Extract BranchId from JWT → `null/empty`
2. Check IsSuperAdmin → `true`
3. Query: `SELECT * FROM Students` (no branch filter)
4. Return: All students from all branches

**Result:** ✅ Success - SuperAdmin sees all students

---

### Example 4: Receptionist (Branch A) Creates Student

**Request:**
```
POST /api/students
Authorization: Bearer <jwt-with-branchId-A>
Body: { "firstName": "Ahmed", "lastName": "Ali", ... }
```

**Backend Flow:**
1. Extract BranchId from JWT → `branch-A-guid`
2. Force `command.BranchId = branch-A-guid`
3. Create student with BranchId = Branch A
4. Return: Created student

**Result:** ✅ Success - Student automatically assigned to Branch A

---

### Example 5: Teacher (Branch A) Records Grade for Branch B Student

**Request:**
```
POST /api/grades
Authorization: Bearer <jwt-with-branchId-A>
Body: { "studentId": "branch-B-student", "score": 90, ... }
```

**Backend Flow:**
1. Extract BranchId from JWT → `branch-A-guid`
2. Fetch student → Student has `BranchId = branch-B-guid`
3. Validation: `branch-A-guid != branch-B-guid`
4. Throw: `ForbiddenException`

**Result:** ❌ 403 Forbidden - Teacher cannot grade students from other branches

---

## 📝 Implementation Checklist

### Phase 1: Database & Auth
- [ ] Add `BranchId` column to `DomainUser` table (nullable for SuperAdmin)
- [ ] Update user seeding to include `BranchId`
- [ ] Modify JWT generation to include `branchId` claim
- [ ] Create `ICurrentUserContext` service
- [ ] Implement `CurrentUserContext` from HttpContext claims

### Phase 2: Authorization
- [ ] Create `RequireBranchAttribute` authorization filter
- [ ] Apply `[RequireBranch]` to all controllers (except Login)
- [ ] Create `ForbiddenException` class
- [ ] Add 403 Forbidden error handling middleware

### Phase 3: Service Layer
- [ ] Update ALL service methods to filter by BranchId
- [ ] Add branch validation to Create operations
- [ ] Add branch validation to Update operations
- [ ] Add branch validation to Delete operations
- [ ] Test with SuperAdmin (should bypass filters)

### Phase 4: Testing
- [ ] Test SuperAdmin can see all branches
- [ ] Test Director A cannot see Branch B data
- [ ] Test Receptionist A cannot create students for Branch B
- [ ] Test Teacher A cannot grade Branch B students
- [ ] Test cross-branch access returns 403

### Phase 5: Frontend
- [ ] Display branch name in UI (for non-SuperAdmin)
- [ ] Remove branch selectors for non-SuperAdmin users
- [ ] Show "All Branches" indicator for SuperAdmin
- [ ] Handle 403 errors gracefully

---

## ✅ Validation Tests

### Test 1: Branch Isolation
- Login as Director (Branch A)
- Try to view students → Should see Branch A only
- Try to access Branch B student by ID → Should get 403

### Test 2: SuperAdmin Access
- Login as SuperAdmin
- View students → Should see all branches
- Access any student → Should succeed

### Test 3: Data Creation
- Login as Receptionist (Branch A)
- Create student → Should auto-assign to Branch A
- Verify student has BranchId = Branch A

### Test 4: Teacher Assignment
- Login as Teacher (Branch A)
- Try to record grade for Branch B student → Should get 403
- Record grade for Branch A student (assigned group) → Should succeed

### Test 5: Branch Switching Prevention
- Login as Director (Branch A)
- Modify JWT token to change branchId to Branch B (attempt)
- Make API call → Should fail (invalid signature)

---

## 🚫 What NOT to Do

1. **Don't Store BranchId in Session:**
   - JWT claims are sufficient
   - Session adds complexity
   - Stateless is better

2. **Don't Allow Users to Change Branch via API:**
   - Branch assignment is permanent
   - Only SuperAdmin can reassign

3. **Don't Filter in Controller:**
   - Always filter in service layer
   - Controller should not know about branch logic

4. **Don't Trust Client-Side Branch Selection:**
   - Always use BranchId from JWT
   - Never accept BranchId from request body/query

5. **Don't Create Multi-Tenancy Infrastructure:**
   - No subdomains needed
   - No URL-based routing
   - Single application deployment

---

## 🎯 Summary

### Branch Isolation Strategy:
✅ **User → BranchId** (stored in database)  
✅ **Login → JWT** (includes branchId claim)  
✅ **Request → Extract BranchId** (from JWT)  
✅ **Query → Filter by BranchId** (in service)  
✅ **SuperAdmin → Bypass Filter** (sees all branches)

### Key Benefits:
- ✅ Simple implementation
- ✅ No multi-tenancy complexity
- ✅ Stateless (JWT-based)
- ✅ Secure by default
- ✅ Easy to test and maintain

### No Need For:
- ❌ Subdomain routing
- ❌ URL-based branch detection
- ❌ Session storage
- ❌ Complex middleware
- ❌ DNS configuration

---

**This approach gives you branch isolation without multi-tenancy overhead. Each user is locked to their branch, and SuperAdmin has full visibility.**
