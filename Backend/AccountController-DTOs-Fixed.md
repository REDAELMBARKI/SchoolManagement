# AccountController DTOs - Fixed Naming

## ✅ Problem Solved

### **Before (Confusing Naming):**

| Endpoint | DTO Used | Problem |
|----------|----------|---------|
| `POST /register` | `LoginRequestDto` | ❌ Register using "Login" DTO?! |
| `POST /create-staff-user` | `RegisterUserRequestDto` | ❌ Generic "Register" name for staff creation |
| `POST /login` | `LoginRequestDto` | ✅ Correct |

---

### **After (Explicit Naming):**

| Endpoint | DTO Used | Purpose |
|----------|----------|---------|
| `POST /register` | `RegisterRequestDto` | ✅ Public user registration (students/parents) |
| `POST /create-staff-user` | `CreateStaffUserRequestDto` | ✅ Staff creation (Director/SuperAdmin only) |
| `POST /login` | `LoginRequestDto` | ✅ Authentication |

---

## 📋 DTO Details

### **1. RegisterRequestDto** (NEW)
**Purpose:** Public user registration  
**Fields:**
```csharp
public class RegisterRequestDto
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; }

    [Required, MinLength(8)]
    public string Password { get; set; }

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
```

**Used by:**
- `POST /api/account/register` - Public endpoint

---

### **2. CreateStaffUserRequestDto** (RENAMED from RegisterUserRequestDto)
**Purpose:** Staff user creation with business data  
**Fields:**
```csharp
public class CreateStaffUserRequestDto
{
    // Authentication
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; }

    [Required, MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; } // Director, Administrator, etc.

    // Business data
    [Required, MinLength(2), MaxLength(50)]
    public string FirstName { get; set; }

    [Required, MinLength(2), MaxLength(50)]
    public string LastName { get; set; }

    [Phone, MaxLength(20)]
    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public Guid? GenderId { get; set; }

    public Guid? BranchId { get; set; } // Required for staff
}
```

**Used by:**
- `POST /api/account/create-staff-user` - Director/SuperAdmin only

---

### **3. LoginRequestDto** (UNCHANGED)
**Purpose:** User authentication  
**Fields:**
```csharp
public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
```

**Used by:**
- `POST /api/account/login` - Public endpoint

---

## 🎯 Why This Naming is Better

### **Before:**
```csharp
// Confusing!
POST /register → LoginRequestDto (Wrong!)
POST /create-staff-user → RegisterUserRequestDto (Generic!)
POST /login → LoginRequestDto (Correct)
```

### **After:**
```csharp
// Clear and explicit!
POST /register → RegisterRequestDto (Public user)
POST /create-staff-user → CreateStaffUserRequestDto (Staff creation)
POST /login → LoginRequestDto (Authentication)
```

---

## 📊 Comparison: Register vs CreateStaffUser

| Feature | RegisterRequestDto | CreateStaffUserRequestDto |
|---------|-------------------|---------------------------|
| **Endpoint** | `/register` | `/create-staff-user` |
| **Authorization** | `[AllowAnonymous]` | `[Authorize(Policy = "IsDirectorOrAbove")]` |
| **Default Role** | "User" (hardcoded) | Provided in request |
| **Personal Info** | ❌ Not required | ✅ FirstName, LastName, Phone, etc. |
| **BranchId** | ❌ No branch | ✅ Required |
| **Creates DomainUser** | ❌ No | ✅ Yes |
| **Use Case** | Public registration (students/parents) | Staff onboarding |

---

## 🔧 Files Changed

1. ✅ **Renamed:** `RegisterUserRequestDto.cs` → `CreateStaffUserRequestDto.cs`
2. ✅ **Created:** `RegisterRequestDto.cs` (new DTO for public registration)
3. ✅ **Updated:** `AccountController.cs` (uses correct DTOs)

---

## ✅ Benefits

1. **Clear naming:** DTO name matches the action/endpoint
2. **No confusion:** Register is for public, CreateStaffUser is for staff
3. **Explicit intent:** Anyone reading the code understands immediately
4. **Maintainable:** Easy to find and modify the right DTO

---

**Now the naming is tight with the action/method!** 🎯
