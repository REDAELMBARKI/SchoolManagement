# User Management Workflows & API Specification

**Date**: August 1, 2026  
**Purpose**: Complete workflow specifications for managing users, roles, claims, and branch assignments

---

## 🎯 Overview

This document defines:
1. **User CRUD Operations** - Create, Read, Update, Delete users
2. **Role Management** - Assign and update user roles
3. **Branch Assignment** - Assign users to branches
4. **Claims Management** - Custom claims for authorization
5. **Password Management** - Reset passwords
6. **User Activation** - Activate/Deactivate users

---

## 📋 User Management Workflows

### Workflow 1: SuperAdmin Creates New User

**Actors**: SuperAdmin  
**Goal**: Create a new staff member and assign them to a branch

**Steps:**
1. SuperAdmin logs into admin panel
2. Navigates to "User Management"
3. Clicks "Create New User"
4. Fills form:
   - First Name, Last Name
   - Email (unique)
   - Phone (optional)
   - Gender
   - Date of Birth (optional)
   - **Role** (dropdown): Director, Administrator, Receptionist, Teacher, CommercialAgent
   - **Branch** (dropdown): Select from available branches
   - Initial Password (temporary)
5. Clicks "Create User"
6. System validates:
   - Email is unique
   - Branch exists (if not SuperAdmin role)
   - Role is valid
7. System creates:
   - ApplicationUser (ASP.NET Identity)
   - DomainUser (linked to ApplicationUser)
   - Assigns role
   - Assigns branch
8. User receives email with credentials
9. User must change password on first login

**API Call:**
```
POST /api/users
Body: CreateUserRequestDto
```

---

### Workflow 2: Director Creates User for Their Branch

**Actors**: Director  
**Goal**: Create staff member for their own branch

**Steps:**
1. Director logs into admin panel
2. Navigates to "User Management"
3. Clicks "Create New User"
4. Fills form:
   - First Name, Last Name
   - Email
   - Phone
   - Gender
   - **Role** (dropdown): Administrator, Receptionist, Teacher, CommercialAgent (NO SuperAdmin or Director)
   - **Branch** (auto-filled, read-only): Director's branch
5. Clicks "Create User"
6. System auto-assigns Director's branch
7. User created and assigned to Director's branch

**API Call:**
```
POST /api/users
Body: CreateUserRequestDto (BranchId auto-filled by backend)
```

**Business Rules:**
- Director cannot create SuperAdmin
- Director cannot create user for other branches
- BranchId is forced to Director's branch (backend)

---

### Workflow 3: SuperAdmin Changes User's Branch

**Actors**: SuperAdmin  
**Goal**: Reassign user to different branch

**Steps:**
1. SuperAdmin views user list
2. Selects user (e.g., Receptionist currently in Branch A)
3. Clicks "Edit" or "Assign Branch"
4. Modal opens with:
   - Current Branch: Branch A
   - New Branch (dropdown): Branch B, Branch C
5. Selects Branch B
6. Clicks "Save"
7. System updates user.BranchId = Branch B
8. User must logout and login to get new JWT token
9. User now sees Branch B data only

**API Call:**
```
POST /api/users/{userId}/assign-branch
Body: { "branchId": "branch-B-guid" }
```

**Business Rules:**
- Only SuperAdmin can reassign branches
- Cannot assign branch to SuperAdmin (SuperAdmin has no branch)
- User must re-login after branch change

---

### Workflow 4: SuperAdmin Changes User's Role

**Actors**: SuperAdmin  
**Goal**: Change user's role (e.g., Receptionist → Administrator)

**Steps:**
1. SuperAdmin views user list
2. Selects user (current role: Receptionist)
3. Clicks "Change Role"
4. Modal opens:
   - Current Role: Receptionist
   - New Role (dropdown): Director, Administrator, Teacher, CommercialAgent
5. Selects "Administrator"
6. Clicks "Save"
7. System:
   - Updates user.Role = Administrator
   - Updates ASP.NET Identity role
   - Removes old role, adds new role
8. User must logout and login to get new JWT with new role

**API Call:**
```
POST /api/users/{userId}/change-role
Body: { "newRole": "Administrator" }
```

**Business Rules:**
- Only SuperAdmin can change roles
- User must re-login after role change
- Audit log tracks who changed role and when

---

### Workflow 5: SuperAdmin or Director Resets User Password

**Actors**: SuperAdmin, Director (for their branch users)  
**Goal**: Reset user's password (user forgot password or needs reset)

**Steps:**
1. Admin views user list
2. Selects user
3. Clicks "Reset Password"
4. Modal opens:
   - Option 1: Generate random password (recommended)
   - Option 2: Enter custom password (min 8 chars)
5. Clicks "Reset"
6. System:
   - Updates password in ASP.NET Identity
   - Marks as "MustChangePassword" on next login
   - Sends email with new password
7. User receives email with temporary password
8. User logs in and forced to change password

**API Call:**
```
POST /api/users/{userId}/reset-password
Body: { "newPassword": "TempPass@123" }
```

**Business Rules:**
- SuperAdmin can reset any user's password
- Director can reset passwords for users in their branch only
- User must change password on next login
- Old password is invalidated immediately

---

### Workflow 6: Deactivate User (Soft Delete)

**Actors**: SuperAdmin, Director  
**Goal**: Deactivate user account (user left company)

**Steps:**
1. Admin views user list
2. Selects user
3. Clicks "Deactivate"
4. Confirmation dialog: "Are you sure? User will not be able to login."
5. Clicks "Yes, Deactivate"
6. System:
   - Sets user.IsActive = false
   - User cannot login anymore
   - User's data remains in system (soft delete)
7. User sees "Account Deactivated" on login attempt

**API Call:**
```
POST /api/users/{userId}/deactivate
```

**Business Rules:**
- SuperAdmin can deactivate any user
- Director can deactivate users in their branch only
- Deactivated user's data is preserved
- Can be reactivated later if needed

---

### Workflow 7: Reactivate User

**Actors**: SuperAdmin, Director  
**Goal**: Reactivate previously deactivated user

**Steps:**
1. Admin views user list (filter: "Inactive Users")
2. Selects deactivated user
3. Clicks "Activate"
4. Confirmation: "Reactivate user account?"
5. Clicks "Yes"
6. System sets user.IsActive = true
7. User can login again
8. Optionally reset password

**API Call:**
```
POST /api/users/{userId}/activate
```

---

### Workflow 8: User Updates Own Profile

**Actors**: Any authenticated user  
**Goal**: Update personal information

**Steps:**
1. User clicks profile icon
2. Clicks "Edit Profile"
3. Can update:
   - First Name, Last Name
   - Phone
   - Date of Birth
   - Profile picture (optional)
4. **Cannot update**:
   - Email (requires admin)
   - Role (requires SuperAdmin)
   - Branch (requires SuperAdmin)
5. Clicks "Save"
6. System validates and updates

**API Call:**
```
PUT /api/users/me
Body: UpdateProfileRequestDto
```

**Business Rules:**
- Users can only update their own profile
- Cannot change critical fields (email, role, branch)
- Changes don't require re-login

---

### Workflow 9: SuperAdmin Views All Users

**Actors**: SuperAdmin  
**Goal**: View all users across all branches

**Steps:**
1. SuperAdmin navigates to "User Management"
2. Sees table with all users:
   - Columns: Name, Email, Role, Branch, Status, Actions
   - Filters: Branch, Role, Status (Active/Inactive)
   - Search: By name or email
3. Can sort by any column
4. Can export to Excel/CSV

**API Call:**
```
GET /api/users
Response: List<UserResponseDto>
```

**Response includes:**
- All users from all branches
- Branch name for each user
- Role, Status, Last Active

---

### Workflow 10: Director Views Branch Users

**Actors**: Director  
**Goal**: View users in their branch only

**Steps:**
1. Director navigates to "User Management"
2. Sees table with users from their branch:
   - Automatically filtered by Director's branch
   - Columns: Name, Email, Role, Status, Actions
   - Filters: Role, Status
   - Search: By name or email
3. Cannot see users from other branches

**API Call:**
```
GET /api/users
Response: List<UserResponseDto> (filtered by branch on backend)
```

**Backend Logic:**
- Extract Director's BranchId from JWT
- Filter query: WHERE BranchId = Director's BranchId
- Return only matching users

---

## 🔐 Claims Management Workflows

### Workflow 11: Add Custom Claim to User

**Actors**: SuperAdmin  
**Goal**: Add custom claim for specific permission

**Steps:**
1. SuperAdmin selects user
2. Clicks "Manage Claims"
3. Modal shows:
   - Current Claims: List of claim types and values
   - Add New Claim:
     - Claim Type (dropdown or text): e.g., "CanApproveExpenses", "MaxDiscountPercent"
     - Claim Value: e.g., "true", "20"
4. Clicks "Add Claim"
5. System adds claim to user
6. User must re-login to get updated JWT

**API Call:**
```
POST /api/users/{userId}/claims
Body: { "claimType": "CanApproveExpenses", "claimValue": "true" }
```

**Use Cases:**
- Grant specific permissions beyond role
- Set user-specific limits (e.g., max discount percentage)
- Feature flags per user

---

### Workflow 12: Remove Claim from User

**Actors**: SuperAdmin  
**Goal**: Revoke custom permission

**Steps:**
1. SuperAdmin selects user
2. Clicks "Manage Claims"
3. Views list of current claims
4. Clicks "Remove" next to claim
5. Confirmation dialog
6. System removes claim
7. User must re-login

**API Call:**
```
DELETE /api/users/{userId}/claims/{claimType}
```

---

## 📊 API Endpoints Specification

### UserController Complete Specification

**Base Route**: `/api/users`  
**Authorization**: All endpoints require authentication

---

### 1. GET /api/users
**Purpose**: Get all users (filtered by permissions)

**Authorization**:
- SuperAdmin: All users
- Director: Users in their branch only
- Others: Forbidden

**Query Parameters**:
- `branchId` (optional, GUID): Filter by branch (SuperAdmin only)
- `role` (optional, string): Filter by role
- `status` (optional, string): "active", "inactive", "all"
- `search` (optional, string): Search by name or email
- `page` (optional, int): Page number
- `pageSize` (optional, int): Items per page

**Response**: `200 OK`
```json
{
  "data": [
    {
      "id": "guid",
      "firstName": "Ahmed",
      "lastName": "Ali",
      "email": "ahmed@school.com",
      "phone": "+201234567890",
      "role": "Director",
      "branchId": "branch-guid",
      "branchName": "Main Campus",
      "isActive": true,
      "lastActiveAt": "2026-08-01T10:00:00Z",
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 20
}
```

---

### 2. GET /api/users/{id}
**Purpose**: Get user details by ID

**Authorization**:
- SuperAdmin: Any user
- Director: Users in their branch
- User: Own profile only

**Path Parameters**:
- `id` (GUID): User ID

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Ahmed",
  "lastName": "Ali",
  "slug": "ahmed-ali-ahmed-school-com",
  "email": "ahmed@school.com",
  "phone": "+201234567890",
  "dateOfBirth": "1990-05-15",
  "genderId": "gender-guid",
  "genderName": "Male",
  "role": "Director",
  "branchId": "branch-guid",
  "branchName": "Main Campus",
  "isActive": true,
  "lastActiveAt": "2026-08-01T10:00:00Z",
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-07-15T08:30:00Z"
}
```

**Error**: `404 Not Found` if user doesn't exist  
**Error**: `403 Forbidden` if not authorized

---

### 3. POST /api/users
**Purpose**: Create new user

**Authorization**:
- SuperAdmin: Can create any role, any branch
- Director: Can create for their branch only (except SuperAdmin)

**Request Body**:
```json
{
  "firstName": "Sara",
  "lastName": "Mohamed",
  "email": "sara@school.com",
  "phone": "+201234567890",
  "dateOfBirth": "1995-03-20",
  "genderId": "gender-guid",
  "role": "Administrator",
  "branchId": "branch-guid",
  "password": "TempPass@123"
}
```

**Validation**:
- `firstName`: Required, max 50 chars
- `lastName`: Required, max 50 chars
- `email`: Required, unique, valid email format
- `role`: Required, valid role
- `branchId`: Required if role != SuperAdmin
- `password`: Required, min 8 chars, must contain uppercase, lowercase, number, special char

**Response**: `201 Created`
```json
{
  "id": "new-user-guid",
  "firstName": "Sara",
  "lastName": "Mohamed",
  "email": "sara@school.com",
  "role": "Administrator",
  "branchId": "branch-guid",
  "branchName": "Main Campus",
  "isActive": true,
  "createdAt": "2026-08-01T12:00:00Z"
}
```

**Error**: `400 Bad Request` if validation fails  
**Error**: `409 Conflict` if email already exists  
**Error**: `403 Forbidden` if not authorized

---

### 4. PUT /api/users/{id}
**Purpose**: Update user information

**Authorization**:
- SuperAdmin: Any user
- Director: Users in their branch
- User: Own profile (limited fields)

**Path Parameters**:
- `id` (GUID): User ID

**Request Body**:
```json
{
  "firstName": "Sara",
  "lastName": "Mohamed",
  "phone": "+201234567890",
  "dateOfBirth": "1995-03-20",
  "genderId": "gender-guid"
}
```

**Note**: Cannot update Email, Role, Branch via this endpoint (use specific endpoints)

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Sara",
  "lastName": "Mohamed",
  "email": "sara@school.com",
  "phone": "+201234567890",
  "role": "Administrator",
  "branchId": "branch-guid",
  "updatedAt": "2026-08-01T12:30:00Z"
}
```

**Error**: `404 Not Found`  
**Error**: `403 Forbidden`

---

### 5. DELETE /api/users/{id}
**Purpose**: Delete user (soft delete - deactivate)

**Authorization**: SuperAdmin only

**Path Parameters**:
- `id` (GUID): User ID

**Response**: `204 No Content`

**Error**: `404 Not Found`  
**Error**: `403 Forbidden`

**Note**: This is a soft delete. User.IsActive set to false. Data preserved.

---

### 6. POST /api/users/{id}/assign-branch
**Purpose**: Assign or reassign user to a branch

**Authorization**: SuperAdmin only

**Path Parameters**:
- `id` (GUID): User ID

**Request Body**:
```json
{
  "branchId": "branch-guid"
}
```

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Ahmed",
  "lastName": "Ali",
  "role": "Director",
  "branchId": "new-branch-guid",
  "branchName": "East Campus",
  "message": "User assigned to branch. User must re-login."
}
```

**Business Logic**:
1. Validate branch exists
2. Update user.BranchId
3. Audit log: User reassigned from Branch A to Branch B
4. User must logout and login to get new JWT

**Error**: `400 Bad Request` if trying to assign branch to SuperAdmin  
**Error**: `404 Not Found` if user or branch not found  
**Error**: `403 Forbidden` if not SuperAdmin

---

### 7. POST /api/users/{id}/remove-branch
**Purpose**: Remove user from branch (set to null - make SuperAdmin)

**Authorization**: SuperAdmin only

**Path Parameters**:
- `id` (GUID): User ID

**Response**: `200 OK`

**Note**: This effectively makes user a SuperAdmin (no branch restriction)

**Error**: `403 Forbidden`

---

### 8. POST /api/users/{id}/change-role
**Purpose**: Change user's role

**Authorization**: SuperAdmin only

**Path Parameters**:
- `id` (GUID): User ID

**Request Body**:
```json
{
  "newRole": "Administrator"
}
```

**Validation**:
- `newRole`: Must be valid role (SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent)

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Ahmed",
  "lastName": "Ali",
  "email": "ahmed@school.com",
  "oldRole": "Receptionist",
  "newRole": "Administrator",
  "message": "Role changed. User must re-login."
}
```

**Business Logic**:
1. Get current role
2. Validate new role
3. Update user.Role
4. Remove old ASP.NET Identity role
5. Add new ASP.NET Identity role
6. Audit log
7. User must re-login

**Error**: `400 Bad Request` if invalid role  
**Error**: `404 Not Found`  
**Error**: `403 Forbidden`

---

### 9. POST /api/users/{id}/reset-password
**Purpose**: Reset user password

**Authorization**:
- SuperAdmin: Any user
- Director: Users in their branch
- User: Own password

**Path Parameters**:
- `id` (GUID): User ID

**Request Body**:
```json
{
  "newPassword": "NewSecurePass@123"
}
```

**Validation**:
- Min 8 characters
- Must contain uppercase, lowercase, number, special character

**Response**: `200 OK`
```json
{
  "message": "Password reset successfully. User must change password on next login."
}
```

**Business Logic**:
1. Hash new password
2. Update ApplicationUser password
3. Set flag: MustChangePasswordOnLogin = true
4. Send email notification
5. Audit log

**Error**: `400 Bad Request` if password doesn't meet requirements  
**Error**: `403 Forbidden`

---

### 10. POST /api/users/{id}/activate
**Purpose**: Activate deactivated user

**Authorization**:
- SuperAdmin: Any user
- Director: Users in their branch

**Path Parameters**:
- `id` (GUID): User ID

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Ahmed",
  "lastName": "Ali",
  "isActive": true,
  "message": "User activated successfully"
}
```

**Business Logic**:
1. Set user.IsActive = true
2. Audit log
3. User can login

**Error**: `404 Not Found`  
**Error**: `403 Forbidden`

---

### 11. POST /api/users/{id}/deactivate
**Purpose**: Deactivate user (soft delete)

**Authorization**:
- SuperAdmin: Any user
- Director: Users in their branch

**Path Parameters**:
- `id` (GUID): User ID

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Ahmed",
  "lastName": "Ali",
  "isActive": false,
  "message": "User deactivated successfully"
}
```

**Business Logic**:
1. Set user.IsActive = false
2. Invalidate all active sessions/tokens
3. Audit log
4. User cannot login

**Error**: `404 Not Found`  
**Error**: `403 Forbidden`

---

### 12. GET /api/users/branch/{branchId}
**Purpose**: Get all users in specific branch

**Authorization**:
- SuperAdmin: Any branch
- Director: Own branch only

**Path Parameters**:
- `branchId` (GUID): Branch ID

**Response**: `200 OK`
```json
{
  "branchId": "branch-guid",
  "branchName": "Main Campus",
  "users": [
    {
      "id": "guid",
      "firstName": "Ahmed",
      "lastName": "Ali",
      "email": "ahmed@school.com",
      "role": "Director",
      "isActive": true
    }
  ]
}
```

**Error**: `404 Not Found` if branch doesn't exist  
**Error**: `403 Forbidden` if Director trying to access other branch

---

### 13. GET /api/users/role/{role}
**Purpose**: Get all users with specific role

**Authorization**:
- SuperAdmin: All roles
- Director: Roles in their branch only

**Path Parameters**:
- `role` (string): Role name

**Response**: `200 OK`
```json
{
  "role": "Administrator",
  "users": [...]
}
```

**Error**: `400 Bad Request` if invalid role

---

### 14. POST /api/users/{id}/claims
**Purpose**: Add custom claim to user

**Authorization**: SuperAdmin only

**Path Parameters**:
- `id` (GUID): User ID

**Request Body**:
```json
{
  "claimType": "CanApproveExpenses",
  "claimValue": "true"
}
```

**Response**: `200 OK`
```json
{
  "message": "Claim added successfully",
  "claim": {
    "type": "CanApproveExpenses",
    "value": "true"
  }
}
```

**Business Logic**:
1. Add claim to ApplicationUser (ASP.NET Identity)
2. User must re-login to get claim in JWT
3. Audit log

**Error**: `400 Bad Request` if claim already exists  
**Error**: `403 Forbidden`

---

### 15. DELETE /api/users/{id}/claims/{claimType}
**Purpose**: Remove custom claim from user

**Authorization**: SuperAdmin only

**Path Parameters**:
- `id` (GUID): User ID
- `claimType` (string): Claim type to remove

**Response**: `204 No Content`

**Business Logic**:
1. Remove claim from ApplicationUser
2. User must re-login
3. Audit log

**Error**: `404 Not Found` if claim doesn't exist  
**Error**: `403 Forbidden`

---

### 16. GET /api/users/{id}/claims
**Purpose**: Get all claims for user

**Authorization**:
- SuperAdmin: Any user
- User: Own claims

**Path Parameters**:
- `id` (GUID): User ID

**Response**: `200 OK`
```json
{
  "userId": "guid",
  "claims": [
    {
      "type": "CanApproveExpenses",
      "value": "true"
    },
    {
      "type": "MaxDiscountPercent",
      "value": "20"
    }
  ]
}
```

---

### 17. GET /api/users/me
**Purpose**: Get current user's profile

**Authorization**: Any authenticated user

**Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Ahmed",
  "lastName": "Ali",
  "email": "ahmed@school.com",
  "phone": "+201234567890",
  "role": "Director",
  "branchId": "branch-guid",
  "branchName": "Main Campus",
  "isActive": true
}
```

**Note**: Returns profile of currently logged-in user

---

### 18. PUT /api/users/me
**Purpose**: Update current user's profile

**Authorization**: Any authenticated user

**Request Body**:
```json
{
  "firstName": "Ahmed",
  "lastName": "Ali",
  "phone": "+201234567890",
  "dateOfBirth": "1990-05-15"
}
```

**Note**: Cannot update email, role, branch, isActive

**Response**: `200 OK`

---

## 🔄 Business Logic Summary

### User Creation Flow:
1. Validate input (email unique, role valid, branch valid)
2. Hash password
3. Create ApplicationUser (ASP.NET Identity)
4. Assign role to ApplicationUser
5. Create DomainUser (linked to ApplicationUser)
6. Set BranchId (if not SuperAdmin)
7. Generate slug (FirstName-LastName-Email)
8. Audit log
9. Send welcome email
10. Return user response

### Branch Assignment Flow:
1. Validate: SuperAdmin only
2. Validate: Branch exists
3. Validate: User is not SuperAdmin
4. Update user.BranchId
5. Audit log (old branch → new branch)
6. User must re-login to get new JWT

### Role Change Flow:
1. Validate: SuperAdmin only
2. Validate: New role is valid
3. Get current role
4. Remove old role from ApplicationUser
5. Add new role to ApplicationUser
6. Update user.Role in DomainUser
7. Audit log (old role → new role)
8. User must re-login

### Password Reset Flow:
1. Validate permissions
2. Validate password strength
3. Hash new password
4. Update ApplicationUser password
5. Set MustChangePassword flag
6. Invalidate existing tokens (optional)
7. Send email notification
8. Audit log

### Deactivation Flow:
1. Validate permissions
2. Set user.IsActive = false
3. Invalidate all user tokens/sessions
4. Audit log
5. User cannot login

---

## ✅ Summary

**Total Endpoints**: 18 endpoints for complete user management

**Key Features**:
- ✅ Complete CRUD for users
- ✅ Role management (assign, change)
- ✅ Branch assignment (assign, reassign)
- ✅ Claims management (add, remove, view)
- ✅ Password management (reset, force change)
- ✅ User activation/deactivation
- ✅ Profile management (self-update)
- ✅ Branch-based filtering
- ✅ Role-based authorization
- ✅ Audit logging for all operations

**Authorization Levels**:
- **SuperAdmin**: Full access to all endpoints
- **Director**: Limited to their branch users
- **User**: Can view/update own profile only

**Re-login Required After**:
- Branch assignment change
- Role change
- Claims added/removed

---

**This specification provides everything needed to implement the UserController with complete user management workflows.**
