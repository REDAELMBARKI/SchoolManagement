# 🔐 Remember Me with JWT Tokens - Complete Guide

## ⚠️ **Important: `isPersistent` Does NOT Work with JWT!**

```csharp
// This ONLY affects cookies, NOT JWT tokens!
await _signInManager.PasswordSignInAsync(
    user, 
    password, 
    isPersistent: true,  // ❌ Has NO effect on JWT expiration!
    lockoutOnFailure: true
);
```

**Why?** `isPersistent` controls ASP.NET Identity cookies, but you're using JWT tokens!

---

## ✅ **How to Implement "Remember Me" with JWT**

### **Strategy: Different Token Expiration Based on RememberMe**

```
RememberMe = false → Short-lived token (15 minutes)
RememberMe = true  → Long-lived token (7 days)
```

---

## 📝 **Step-by-Step Implementation**

### **1. Update LoginRequestDto**

```csharp
public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;  // ✅ Added
}
```

---

### **2. Update IAuthService Interface**

```csharp
Task<string> AuthenticateAsync(string email, string password, bool rememberMe = false);
```

---

### **3. Update AuthService Implementation**

```csharp
public async Task<string> AuthenticateAsync(string email, string password, bool rememberMe = false)
{
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
        throw new Exception("Invalid email or password.");
    }

    if (await _userManager.IsLockedOutAsync(user))
    {
        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
        var remainingMinutes = (lockoutEnd - DateTimeOffset.UtcNow)?.TotalMinutes ?? 0;
        throw new Exception($"Account is locked. Try again in {Math.Ceiling(remainingMinutes)} minutes.");
    }

    // isPersistent only affects cookies (not JWT), but we track rememberMe for later use
    var result = await _signInManager.PasswordSignInAsync(
        user, 
        password, 
        isPersistent: rememberMe,  // Tracked for audit/cookie tracking
        lockoutOnFailure: true
    );

    if (result.Succeeded)
    {
        await _userManager.ResetAccessFailedCountAsync(user);
        return user.Id;  // Return userId + rememberMe flag to controller
    }

    if (result.IsLockedOut)
    {
        throw new Exception("Account locked. Try again in 15 minutes.");
    }

    if (result.IsNotAllowed)
    {
        throw new Exception("Email not confirmed.");
    }

    var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
    var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
    var attemptsRemaining = maxAttempts - failedAttempts;

    if (attemptsRemaining > 0)
    {
        throw new Exception($"Invalid credentials. {attemptsRemaining} attempts remaining.");
    }

    throw new Exception("Invalid email or password.");
}
```

---

### **4. Update AccountController Login**

```csharp
[HttpPost("login")]
[AllowAnonymous]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    try
    {
        // Authenticate user
        var applicationUserId = await _authService.AuthenticateAsync(
            request.Email, 
            request.Password, 
            request.RememberMe
        );

        // Get user details for JWT
        var user = await _domainUserService.GetByApplicationUserIdAsync(applicationUserId);
        
        // Generate JWT with different expiration based on RememberMe
        var token = GenerateJwtToken(user, request.RememberMe);
        
        return Ok(new
        {
            message = "Login successful",
            token = token,
            expiresIn = request.RememberMe ? "7 days" : "15 minutes"
        });
    }
    catch (Exception ex)
    {
        await _auditLogService.StoreAsync(
            action: AuditLog.FailedLoginAction(),
            entityName: "Authentication",
            entityId: Guid.Empty,
            branchId: Guid.Empty,
            newValues: new { Email = request.Email },
            message: $"Failed login attempt for {request.Email}",
            severity: AuditLog.SeverityWarning,
            category: AuditLog.CategorySecurity
        );

        return Unauthorized(new { error = ex.Message });
    }
}

// Helper method to generate JWT
private string GenerateJwtToken(DomainUserResponseDto user, bool rememberMe)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.ApplicationUserId),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("BranchId", user.BranchId.ToString()),
        new Claim("UserId", user.Id.ToString())
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
    );
    
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Different expiration based on RememberMe
    var expirationMinutes = rememberMe ? 10080 : 15; // 7 days vs 15 minutes
    
    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expirationMinutes),  // 👈 Key difference!
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

---

## 🎯 **How It Works End-to-End**

### **Scenario 1: Remember Me = FALSE (Default)**

```
1. User logs in (rememberMe: false)
2. Backend generates JWT with 15-minute expiration
3. Frontend stores token in sessionStorage (cleared on browser close)
4. Token expires after 15 minutes
5. User must login again
```

### **Scenario 2: Remember Me = TRUE**

```
1. User logs in with "Remember Me" checked (rememberMe: true)
2. Backend generates JWT with 7-day expiration
3. Frontend stores token in localStorage (persists across browser sessions)
4. Token lasts 7 days
5. User stays logged in for a week
```

---

## 🔐 **Security Considerations**

### **Risk: Long-lived tokens can be stolen**

**Mitigation: Use Refresh Tokens (Better Approach)**

Instead of long-lived access tokens, use:
1. **Short-lived Access Token (15 min)** - Always short
2. **Long-lived Refresh Token (7-30 days)** - Only if RememberMe = true

```
RememberMe = false:
  Access Token: 15 min
  Refresh Token: None (or 1 day)

RememberMe = true:
  Access Token: 15 min
  Refresh Token: 30 days
```

**Benefits:**
- ✅ Stolen access token expires quickly (15 min)
- ✅ Refresh token can be revoked
- ✅ Refresh token stored in httpOnly cookie (more secure)
- ✅ Better security without sacrificing UX

---

## 📊 **Comparison Table**

| Approach | RememberMe = false | RememberMe = true | Security | UX |
|----------|-------------------|-------------------|----------|-----|
| **Cookie (isPersistent)** | Session cookie | 14-day cookie | Good | Good |
| **JWT Long Token** | 15 min token | 7-day token | ⚠️ Medium | Good |
| **JWT + Refresh Token** | 15 min + 1 day refresh | 15 min + 30 day refresh | ✅ Best | ✅ Best |

---

## 🚀 **Recommended Implementation**

### **For Now (Simple):**
✅ Use different JWT expiration based on RememberMe

### **For Production (Secure):**
✅ Implement Refresh Token strategy

---

## 💻 **Frontend Implementation**

```typescript
// Login form (React example)
const [rememberMe, setRememberMe] = useState(false);

const handleLogin = async () => {
    const response = await fetch('/api/account/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            email: email,
            password: password,
            rememberMe: rememberMe  // 👈 Send to backend
        })
    });

    const data = await response.json();
    
    // Store token based on RememberMe
    if (rememberMe) {
        localStorage.setItem('token', data.token);  // Persists across sessions
    } else {
        sessionStorage.setItem('token', data.token);  // Cleared on browser close
    }
};

// JSX
<input 
    type="checkbox" 
    checked={rememberMe}
    onChange={(e) => setRememberMe(e.target.checked)}
/> Remember Me
```

---

## ✅ **Summary**

1. ✅ **`isPersistent` does NOT affect JWT** - Only cookies
2. ✅ **"Remember Me" for JWT = Longer token expiration**
3. ✅ **Simple approach:** 15 min vs 7 days token
4. ✅ **Secure approach:** Refresh tokens (recommended for production)
5. ✅ **Frontend:** Store in localStorage (RememberMe) or sessionStorage (default)

---

## 🎯 **Current Status**

✅ Updated: `LoginRequestDto` with `RememberMe` property  
✅ Updated: `IAuthService` interface with `rememberMe` parameter  
✅ Need to: Implement JWT generation with dynamic expiration in AccountController  
✅ Need to: (Optional) Implement Refresh Token strategy for better security  

---

**Want me to implement the JWT generation logic with dynamic expiration?** 🚀
