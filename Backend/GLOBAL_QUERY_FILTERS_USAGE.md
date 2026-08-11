# Global Query Filters - Usage Guide

## ✅ What Was Implemented

**Global branch isolation filters** applied to ALL queries in EF Core.  
**ALL users** (including SuperAdmin) are filtered by their BranchId by default.

---

## 🎯 How It Works

### For Regular Users (Director, Administrator, etc.):
```csharp
// Normal query - Automatically filtered by user's branch
var students = await _context.Students.ToListAsync();
// SQL: SELECT * FROM Students WHERE BranchId = @UserBranchId
// Result: Only students from their branch
```

✅ **Automatic security** - No need to add `.Where(s => s.BranchId == userBranchId)` everywhere!

---

### For SuperAdmin - When They Want Their Own Branch:
```csharp
// Same as regular users - filtered by SuperAdmin's "assigned" branch
var students = await _context.Students.ToListAsync();
// Result: Only students from SuperAdmin's branch (if they have one)
```

---

### For SuperAdmin - When They Want ALL Branches:
```csharp
// Explicitly bypass the filter using IgnoreQueryFilters()
var students = await _context.Students
    .IgnoreQueryFilters()  // ⚠️ Bypass branch filter
    .ToListAsync();

// SQL: SELECT * FROM Students (NO WHERE clause)
// Result: Students from ALL branches
```

---

## 📋 Usage Patterns

### 1. **Repository Pattern** (Recommended)

Create methods in repositories that handle this:

```csharp
public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;

    // Get students from user's branch (filtered)
    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
        // Automatically filtered by user's BranchId
    }

    // Get students from ALL branches (only for SuperAdmin)
    public async Task<List<Student>> GetAllFromAllBranchesAsync()
    {
        // Check authorization first!
        if (_currentUserContext.Role != "SuperAdmin")
            throw new ForbiddenException("Only SuperAdmin can view all branches");

        return await _context.Students
            .IgnoreQueryFilters()  // Bypass branch filter
            .ToListAsync();
    }

    // Get student by ID (filtered by branch)
    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
        // Automatically filtered: WHERE Id = @id AND BranchId = @UserBranchId
    }

    // Get student by ID from any branch (only for SuperAdmin)
    public async Task<Student?> GetByIdFromAnyBranchAsync(Guid id)
    {
        if (_currentUserContext.Role != "SuperAdmin")
            throw new ForbiddenException("Only SuperAdmin can view students from other branches");

        return await _context.Students
            .IgnoreQueryFilters()
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }
}
```

---

### 2. **Service Pattern**

```csharp
public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;

    // Get students from user's branch
    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await _repository.GetAllAsync();
        // Automatically filtered by branch
        return students.Select(StudentMapper.ToResponse).ToList();
    }

    // Get students from ALL branches (SuperAdmin dashboard)
    public async Task<List<StudentResponseDto>> GetAllBranchesAsync()
    {
        // Only SuperAdmin can call this
        if (_currentUserContext.Role != "SuperAdmin")
            throw new ForbiddenException("Only SuperAdmin can view all branches");

        var students = await _repository.GetAllFromAllBranchesAsync();
        return students.Select(StudentMapper.ToResponse).ToList();
    }

    // Get student statistics across all branches (SuperAdmin only)
    public async Task<BranchStatisticsDto> GetGlobalStatisticsAsync()
    {
        if (_currentUserContext.Role != "SuperAdmin")
            throw new ForbiddenException("Only SuperAdmin can view global statistics");

        // Need to bypass filters to get data from all branches
        var totalStudents = await _context.Students
            .IgnoreQueryFilters()
            .CountAsync();

        var studentsByBranch = await _context.Students
            .IgnoreQueryFilters()
            .GroupBy(s => s.BranchId)
            .Select(g => new { BranchId = g.Key, Count = g.Count() })
            .ToListAsync();

        return new BranchStatisticsDto
        {
            TotalStudents = totalStudents,
            StudentsByBranch = studentsByBranch
        };
    }
}
```

---

### 3. **Controller Pattern**

```csharp
[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _service;

    // Regular endpoint - filtered by user's branch
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _service.GetAllAsync();
        return Ok(students);
    }

    // SuperAdmin endpoint - all branches
    [HttpGet("all-branches")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAllBranches()
    {
        var students = await _service.GetAllBranchesAsync();
        return Ok(students);
    }

    // SuperAdmin dashboard - global statistics
    [HttpGet("statistics/global")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetGlobalStatistics()
    {
        var stats = await _service.GetGlobalStatisticsAsync();
        return Ok(stats);
    }
}
```

---

## ⚠️ Important Rules

### 1. **Always Check Authorization Before Bypassing**
```csharp
// ❌ DANGEROUS - Anyone can see all branches!
var students = await _context.Students.IgnoreQueryFilters().ToListAsync();

// ✅ SAFE - Check role first
if (_currentUserContext.Role != "SuperAdmin")
    throw new ForbiddenException();

var students = await _context.Students.IgnoreQueryFilters().ToListAsync();
```

---

### 2. **Document When You Bypass Filters**
```csharp
// ✅ GOOD - Clear intent
// SuperAdmin dashboard: Show total students across all branches
var totalStudents = await _context.Students
    .IgnoreQueryFilters()  // Intentional: Need all branches for dashboard
    .CountAsync();
```

---

### 3. **Use Specific Methods in Repositories**
```csharp
// ❌ BAD - Exposing IgnoreQueryFilters() in service layer
var students = await _context.Students.IgnoreQueryFilters().ToListAsync();

// ✅ GOOD - Repository handles it with clear method name
var students = await _repository.GetAllFromAllBranchesAsync();
```

---

## 📊 What Gets Filtered Automatically

All entities with BranchId property:

✅ **People:** DomainUser, Teacher, CommercialAgent, Opc, StudentResponsable  
✅ **Students:** Student, Intake  
✅ **Academic:** Group, Schedule, Room  
✅ **Operations:** Enrollment, Absence, Grade  
✅ **Financial:** Invoice, Payment, Charge, Expense, PayrollPayment, Commission, Refund  
✅ **Communication:** WhatsAppMessage  
✅ **Audit:** AuditLog  

---

## 🔍 Testing Examples

### Test 1: Regular User Cannot See Other Branches
```csharp
// Login as Director of Branch A
var students = await _context.Students.ToListAsync();
// Result: Only Branch A students ✅

// Try to get student from Branch B
var studentFromBranchB = await _context.Students.FindAsync(branchBStudentId);
// Result: NULL (filtered out) ✅
```

---

### Test 2: SuperAdmin Filtered by Default
```csharp
// Login as SuperAdmin (BranchId = Guid.Empty or NULL)
var students = await _context.Students.ToListAsync();
// Result: No students (SuperAdmin has no branch) ✅
```

---

### Test 3: SuperAdmin Can Bypass When Needed
```csharp
// Login as SuperAdmin
var allStudents = await _context.Students
    .IgnoreQueryFilters()
    .ToListAsync();
// Result: Students from ALL branches ✅
```

---

## 🎯 When SuperAdmin Needs ALL Branches

Common scenarios:

1. **Dashboard/Reports** - Global statistics
2. **System Monitoring** - Total counts, health checks
3. **Cross-Branch Operations** - Transferring students between branches
4. **Financial Reports** - Company-wide revenue
5. **Data Export** - Full system backup

For these cases, use `.IgnoreQueryFilters()` in the repository/service layer with proper authorization checks.

---

## 🚀 Benefits of This Approach

✅ **Automatic Security** - Filters applied to ALL queries  
✅ **Can't Forget** - No need to remember `.Where(x => x.BranchId == ...)`  
✅ **Flexible** - SuperAdmin can bypass when needed  
✅ **Explicit** - `.IgnoreQueryFilters()` makes it obvious  
✅ **Clean Code** - Less repetitive filtering logic  
✅ **Defense in Depth** - Even if you forget authorization checks, filter still applies  

---

## 📝 Summary

| User Type | Default Behavior | How to See All Branches |
|-----------|------------------|------------------------|
| **Director** | Filtered by their branch | ❌ Cannot bypass |
| **Administrator** | Filtered by their branch | ❌ Cannot bypass |
| **Receptionist** | Filtered by their branch | ❌ Cannot bypass |
| **SuperAdmin** | Filtered by branch (usually empty) | ✅ Use `.IgnoreQueryFilters()` |

**Key Takeaway:** SuperAdmin is NOT special by default. They must **explicitly choose** to bypass filters using `.IgnoreQueryFilters()`.

This is **SAFER** because:
- Most queries don't need all branches
- Reduces accidental data exposure
- Makes cross-branch queries explicit and auditable
