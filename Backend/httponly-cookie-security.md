# 🍪 HttpOnly Cookie Implementation - Complete Security Guide

## ✅ What's Been Implemented

### **1. HttpOnly Cookie Configuration**

```csharp
private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
{
    var cookieOptions = new CookieOptions
    {
        HttpOnly = true,        // ✅ Cannot be accessed by JavaScript (XSS protection)
        Expires = expires,      // ✅ Expires with refresh token
        SameSite = SameSiteMode.Strict,  // ✅ CSRF protection
        Secure = true,          // ✅ Only sent over HTTPS
        IsEssential = true,     // ✅ Not affected by GDPR consent
        Path = "/api/account"   // ✅ Only sent to auth endpoints
    };

    Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
}
```

---

## 🔒 **Security Features Explained**

### **1. HttpOnly = true**
**Protection:** XSS (Cross-Site Scripting) attacks

```javascript
// ❌ This will NOT work (JavaScript cannot access it)
console.log(document.cookie);  // refreshToken is hidden!
localStorage.getItem('refreshToken');  // Doesn't exist!

// ✅ Only the browser automatically sends it with requests
fetch('/api/account/refresh-token');  // Cookie sent automatically
```

**Why it matters:**
- If attacker injects malicious JavaScript, they **cannot steal** the refresh token
- Even if your site has XSS vulnerability, refresh token is safe

---

### **2. Secure = true**
**Protection:** Man-in-the-middle (MITM) attacks

```
❌ HTTP request: Cookie NOT sent
✅ HTTPS request: Cookie sent

Attacker on public WiFi → Cannot intercept cookie (encrypted)
```

**Why it matters:**
- Cookie only sent over encrypted HTTPS connections
- Public WiFi attackers cannot sniff the token

---

### **3. SameSite = Strict**
**Protection:** CSRF (Cross-Site Request Forgery) attacks

```
Scenario:
1. User logs into yourbank.com
2. Attacker tricks user to visit evil.com
3. evil.com tries to make request to yourbank.com/transfer

❌ With SameSite=Strict: Cookie NOT sent from evil.com
✅ Cookie only sent from same site (yourbank.com)
```

**Why it matters:**
- Prevents CSRF attacks where attacker makes requests on your behalf
- Cookie only sent if request originates from your domain

---

### **4. Path = "/api/account"**
**Protection:** Limits cookie exposure

```
✅ Cookie sent: POST /api/account/login
✅ Cookie sent: POST /api/account/refresh-token
❌ Cookie NOT sent: GET /api/students
❌ Cookie NOT sent: GET /api/invoices
```

**Why it matters:**
- Reduces attack surface
- Cookie only exposed to authentication endpoints

---

### **5. IsEssential = true**
**Protection:** GDPR compliance

```
✅ Cookie works even if user rejects "analytics cookies"
✅ Marked as "essential for functionality"
```

**Why it matters:**
- Authentication cookies are essential for the app to work
- Not subject to cookie consent requirements

---

## 🌐 **How It Works**

### **Login Flow (Web):**
```
1. User submits login form
   POST /api/account/login
   Body: { email, password, rememberMe }

2. Backend validates credentials
   ✅ Authentication successful

3. Backend generates tokens
   - Access Token (JWT, 15 min)
   - Refresh Token (random, 7-30 days)

4. Backend saves refresh token to database
   - Token value
   - User ID
   - Expiration
   - IP address

5. Backend sets httpOnly cookie
   Set-Cookie: refreshToken=abc123; HttpOnly; Secure; SameSite=Strict; Path=/api/account

6. Backend returns response
   {
     "accessToken": "eyJhbGc...",
     "refreshToken": "abc123",  // Also in response for mobile apps
     "accessTokenExpiresAt": "...",
     "refreshTokenExpiresAt": "..."
   }

7. Frontend stores only access token
   sessionStorage.setItem('accessToken', data.accessToken);
   // NO need to store refreshToken - it's in httpOnly cookie!
```

---

### **Token Refresh Flow (Web):**
```
1. Access token expires (15 min)
   Frontend detects 401 response

2. Frontend calls refresh endpoint
   POST /api/account/refresh-token
   // NO body needed! Cookie sent automatically

3. Browser automatically sends cookie
   Cookie: refreshToken=abc123

4. Backend reads cookie
   var refreshToken = GetRefreshTokenFromCookie();

5. Backend validates refresh token
   - Token exists in database?
   - Token not expired?
   - Token not revoked?

6. Backend generates new tokens
   - New Access Token (15 min)
   - New Refresh Token (replaces old one)

7. Backend revokes old token (token rotation)
   storedToken.Revoke("Replaced by new token");

8. Backend sets new cookie
   Set-Cookie: refreshToken=xyz789; HttpOnly; Secure; ...

9. Backend returns new access token
   {
     "accessToken": "eyJnew...",
     "refreshToken": "xyz789",  // For mobile apps
     ...
   }

10. Frontend uses new access token
    sessionStorage.setItem('accessToken', data.accessToken);
```

---

### **Logout Flow (Web):**
```
1. User clicks logout
   POST /api/account/revoke-token
   // NO body needed! Cookie sent automatically

2. Backend reads cookie
   var refreshToken = GetRefreshTokenFromCookie();

3. Backend revokes token in database
   token.Revoke(ipAddress, "Revoked by user");

4. Backend deletes cookie
   Response.Cookies.Delete("refreshToken");

5. Frontend clears access token
   sessionStorage.clear();

6. User logged out ✅
```

---

## 📱 **Mobile App Support**

**Problem:** Mobile apps can't use httpOnly cookies!

**Solution:** Support both cookie AND body:

```csharp
[HttpPost("refresh-token")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto? request = null)
{
    // Try cookie first (web), then body (mobile)
    var refreshToken = request?.RefreshToken ?? GetRefreshTokenFromCookie();
    
    if (string.IsNullOrEmpty(refreshToken))
    {
        return Unauthorized(new { error = "Refresh token is required" });
    }
    
    // ... validate and refresh
}
```

**Web apps:** Use httpOnly cookie (most secure)  
**Mobile apps:** Send token in request body  

---

## 💻 **Frontend Implementation**

### **Web (React/Vue/Angular):**

```typescript
// Login
async function login(email: string, password: string, rememberMe: boolean) {
    const response = await fetch('/api/account/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',  // ✅ Important! Send cookies
        body: JSON.stringify({ email, password, rememberMe })
    });

    const data = await response.json();
    
    // Only store access token (refresh token in httpOnly cookie)
    sessionStorage.setItem('accessToken', data.accessToken);
    
    return data;
}

// API call with auto-refresh
async function apiCall(url: string, options: RequestInit = {}) {
    const accessToken = sessionStorage.getItem('accessToken');
    
    let response = await fetch(url, {
        ...options,
        headers: {
            ...options.headers,
            'Authorization': `Bearer ${accessToken}`
        }
    });

    // If 401, try to refresh
    if (response.status === 401) {
        const refreshResponse = await fetch('/api/account/refresh-token', {
            method: 'POST',
            credentials: 'include'  // ✅ Send cookie automatically
        });

        if (refreshResponse.ok) {
            const data = await refreshResponse.json();
            sessionStorage.setItem('accessToken', data.accessToken);
            
            // Retry original request
            response = await fetch(url, {
                ...options,
                headers: {
                    ...options.headers,
                    'Authorization': `Bearer ${data.accessToken}`
                }
            });
        } else {
            // Refresh failed, logout
            sessionStorage.clear();
            window.location.href = '/login';
        }
    }

    return response;
}

// Logout
async function logout() {
    await fetch('/api/account/revoke-token', {
        method: 'POST',
        credentials: 'include',  // ✅ Send cookie to revoke
        headers: {
            'Authorization': `Bearer ${sessionStorage.getItem('accessToken')}`
        }
    });

    sessionStorage.clear();
    window.location.href = '/login';
}
```

---

### **Mobile (React Native/Flutter):**

```typescript
// Mobile apps: Store refresh token in secure storage
import * as SecureStore from 'expo-secure-store';

// Login
async function login(email: string, password: string, rememberMe: boolean) {
    const response = await fetch('https://api.yourdomain.com/api/account/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password, rememberMe })
    });

    const data = await response.json();
    
    // Store both tokens securely
    await SecureStore.setItemAsync('accessToken', data.accessToken);
    await SecureStore.setItemAsync('refreshToken', data.refreshToken);
    
    return data;
}

// Refresh
async function refreshToken() {
    const refreshToken = await SecureStore.getItemAsync('refreshToken');
    
    const response = await fetch('https://api.yourdomain.com/api/account/refresh-token', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken })  // ✅ Send in body for mobile
    });

    const data = await response.json();
    
    await SecureStore.setItemAsync('accessToken', data.accessToken);
    await SecureStore.setItemAsync('refreshToken', data.refreshToken);
    
    return data;
}
```

---

## 🔐 **Security Comparison**

| Storage Method | XSS Protection | CSRF Protection | MITM Protection | Mobile Support |
|----------------|----------------|-----------------|-----------------|----------------|
| **localStorage** | ❌ Vulnerable | ✅ Safe | ⚠️ If HTTPS | ✅ Yes |
| **sessionStorage** | ❌ Vulnerable | ✅ Safe | ⚠️ If HTTPS | ✅ Yes |
| **HttpOnly Cookie** | ✅ **Protected** | ✅ **Protected** | ✅ **Protected** | ❌ No |
| **Secure Storage (Mobile)** | ✅ Protected | ✅ Safe | ✅ If HTTPS | ✅ Yes |

**Recommendation:**
- ✅ **Web apps:** HttpOnly cookie (most secure)
- ✅ **Mobile apps:** Secure storage (platform-specific)

---

## ✅ **Summary**

### **What We Implemented:**

1. ✅ **HttpOnly cookie** for refresh tokens (web)
2. ✅ **Body parameter** support (mobile apps)
3. ✅ **Automatic cookie handling** (browser sends/receives)
4. ✅ **SameSite=Strict** (CSRF protection)
5. ✅ **Secure=true** (HTTPS only)
6. ✅ **Path scoping** (only auth endpoints)
7. ✅ **Token rotation** (new cookie on refresh)
8. ✅ **Cookie deletion** on logout

### **Security Benefits:**

- 🛡️ **XSS protected** - JavaScript cannot access token
- 🛡️ **CSRF protected** - SameSite prevents cross-origin attacks
- 🛡️ **MITM protected** - Secure flag ensures HTTPS only
- 🛡️ **Token rotation** - Old tokens automatically revoked
- 🛡️ **Flexible** - Works for web AND mobile apps

---

**Your refresh tokens are now stored in httpOnly cookies for maximum security!** 🚀🔒
