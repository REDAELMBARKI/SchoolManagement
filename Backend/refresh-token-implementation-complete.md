# ✅ Refresh Token Implementation - Complete

## 🎉 What's Been Implemented

### **1. RefreshToken Entity**
**File:** `SchoolManagement.Domain/Common/Entities/RefreshToken.cs`

```csharp
public class RefreshToken : BaseEntity
{
    public string Token { get; private set; }
    public string ApplicationUserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedByIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }  // Token rotation
    public string? ReasonRevoked { get; private set; }
    
    public bool IsActive => !IsRevoked && !IsExpired;
}
```

---

### **2. DTOs**

**AuthResponseDto:**
```csharp
public class AuthResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
```

**RefreshTokenRequestDto:**
```csharp
public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; }
}
```

---

### **3. Repository**

**IRefreshTokenRepository & Implementation:**
- `GetByTokenAsync(string token)`
- `GetActiveTokensByUserIdAsync(string userId)`
- `RevokeAllUserTokensAsync(string userId, string ip, string reason)`

---

### **4. JWT Service**

**IJwtService:**
```csharp
public interface IJwtService
{
    string GenerateAccessToken(DomainUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
```

**Features:**
- ✅ Generates short-lived access tokens (15 minutes)
- ✅ Generates secure random refresh tokens
- ✅ Validates expired tokens for refresh flow

---

### **5. API Endpoints**

#### **POST /api/account/refresh-token**
**Purpose:** Get new access token using refresh token

**Request:**
```json
{
  "refreshToken": "base64-encoded-token"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "new-base64-token",
  "accessTokenExpiresAt": "2024-01-01T12:15:00Z",
  "refreshTokenExpiresAt": "2024-01-08T12:00:00Z"
}
```

**Features:**
- ✅ Token rotation (old token revoked, new one issued)
- ✅ Tracks IP address
- ✅ Validates token is active (not expired/revoked)

---

#### **POST /api/account/revoke-token**
**Purpose:** Manually revoke refresh token (logout)

**Request:**
```json
{
  "refreshToken": "base64-encoded-token"
}
```

**Response:**
```json
{
  "message": "Token revoked successfully"
}
```

**Security:**
- ✅ Requires authentication
- ✅ User can only revoke their own tokens
- ✅ Tracks who revoked and why

---

## 🔄 **How It Works**

### **Login Flow:**
```
1. User logs in with email/password
2. Backend generates:
   - Access Token (15 min)
   - Refresh Token (7 days if RememberMe, else 1 day)
3. Both tokens returned to frontend
4. Frontend stores:
   - Access Token: memory/sessionStorage
   - Refresh Token: httpOnly cookie (most secure) or localStorage
```

### **Token Refresh Flow:**
```
1. Access token expires after 15 minutes
2. Frontend detects 401 error
3. Frontend calls /api/account/refresh-token with refresh token
4. Backend validates refresh token
5. Backend generates new access token + new refresh token
6. Old refresh token revoked (token rotation)
7. Frontend uses new access token
```

### **Logout Flow:**
```
1. User clicks logout
2. Frontend calls /api/account/revoke-token
3. Backend marks refresh token as revoked
4. Frontend clears all tokens
5. User logged out
```

---

## 🔒 **Security Features**

### **1. Token Rotation**
Every time you refresh, the old token is revoked and a new one is issued.
- ✅ Prevents token reuse attacks
- ✅ If attacker steals old token, it's already revoked

### **2. IP Tracking**
Tracks which IP created and revoked each token.
- ✅ Detect suspicious activity
- ✅ Audit trail for security investigations

### **3. Short-Lived Access Tokens**
Access tokens always expire in 15 minutes.
- ✅ Stolen access token expires quickly
- ✅ Limited damage window

### **4. Revokable Refresh Tokens**
Refresh tokens can be revoked anytime.
- ✅ Force logout on role change
- ✅ Revoke all sessions on password reset
- ✅ Ban compromised accounts

---

## 📝 **Next Steps**

### **1. Run Migration**
```bash
dotnet ef migrations add AddRefreshTokenTable --project SchoolManagement.Infrastructure --startup-project SchoolManagement.Api
dotnet ef database update --project SchoolManagement.Infrastructure --startup-project SchoolManagement.Api
```

### **2. Update Login Endpoint**
Currently Login returns `applicationUserId`. Update it to return `AuthResponseDto`:

```csharp
[HttpPost("login")]
[AllowAnonymous]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    try
    {
        var applicationUserId = await _authService.AuthenticateAsync(
            request.Email, 
            request.Password, 
            request.RememberMe
        );

        var domainUser = await _domainUserService.GetByApplicationUserIdAsync(applicationUserId);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(domainUser);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token to database
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var refreshTokenExpiration = request.RememberMe 
            ? DateTime.UtcNow.AddDays(30)  // Remember Me: 30 days
            : DateTime.UtcNow.AddDays(1);   // Default: 1 day

        var refreshTokenEntity = RefreshToken.Create(
            refreshToken,
            applicationUserId,
            refreshTokenExpiration,
            ipAddress
        );

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = refreshTokenExpiration
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
```

### **3. Update Frontend**

```typescript
// Login
const response = await fetch('/api/account/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ 
        email, 
        password, 
        rememberMe 
    })
});

const { accessToken, refreshToken } = await response.json();

// Store tokens
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('refreshToken', refreshToken);

// API request with auto-refresh
async function apiCall(url) {
    let token = localStorage.getItem('accessToken');
    
    let response = await fetch(url, {
        headers: { 'Authorization': `Bearer ${token}` }
    });

    // If 401, refresh token
    if (response.status === 401) {
        const refreshToken = localStorage.getItem('refreshToken');
        
        const refreshResponse = await fetch('/api/account/refresh-token', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ refreshToken })
        });

        if (refreshResponse.ok) {
            const { accessToken, refreshToken: newRefreshToken } = await refreshResponse.json();
            localStorage.setItem('accessToken', accessToken);
            localStorage.setItem('refreshToken', newRefreshToken);
            
            // Retry original request
            response = await fetch(url, {
                headers: { 'Authorization': `Bearer ${accessToken}` }
            });
        } else {
            // Refresh failed, logout
            localStorage.clear();
            window.location.href = '/login';
        }
    }

    return response;
}

// Logout
async function logout() {
    const refreshToken = localStorage.getItem('refreshToken');
    
    await fetch('/api/account/revoke-token', {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        },
        body: JSON.stringify({ refreshToken })
    });

    localStorage.clear();
    window.location.href = '/login';
}
```

---

## ✅ **Benefits**

1. ✅ **Short-lived access tokens** (15 min) - Stolen tokens expire quickly
2. ✅ **Long-lived refresh tokens** (1-30 days) - Good UX without compromising security
3. ✅ **Token rotation** - Old tokens automatically revoked
4. ✅ **Revokable sessions** - Force logout anytime
5. ✅ **IP tracking** - Security audit trail
6. ✅ **Remember Me** - Flexible expiration based on user choice

---

## 🎯 **Summary**

✅ RefreshToken entity created  
✅ Repository & Service implemented  
✅ JWT Service with token generation  
✅ `/refresh-token` endpoint (get new access token)  
✅ `/revoke-token` endpoint (logout)  
✅ Token rotation for security  
✅ IP tracking for audit  
✅ Remember Me support (1 day vs 30 days)  

**Ready to use after running migration!** 🚀
