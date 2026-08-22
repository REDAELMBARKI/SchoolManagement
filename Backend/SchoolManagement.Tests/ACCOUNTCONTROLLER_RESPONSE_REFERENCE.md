# AccountController Test Response Reference

## Security Note
Many endpoints return **generic error messages** to prevent information leakage. Tests MUST assert these exact responses.

---

## Register (`POST /api/account/register`)

### Success (200 OK)
```json
{
  "message": "Registration successful. Please check your email to confirm your account.",
  "applicationUserId": "guid-string"
}
```

### Duplicate Email (400 BadRequest OR 500 InternalServerError)
```json
{
  "error": "Failed to create user: DuplicateUserName: ..."
}
```
**Note**: May return 500 instead of 400 depending on exception type.

### Validation Error (400 BadRequest)
```json
{
  "error": "message from validation"
}
```

### Server Error (500)
```json
{
  "error": "Registration failed: {ex.Message}",
  "details": "{innerException.Message}"
}
```

---

## Login (`POST /api/account/login`)

### Success (200 OK)
```json
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "accessTokenExpiresAt": "2024-...",
  "refreshTokenExpiresAt": "2024-..."
}
```

### Invalid Credentials (401 Unauthorized)
```json
{
  "error": "Invalid email or password." // or other specific auth error
}
```

---

## ChangePassword (`POST /api/account/change-password`)

### Success (200 OK)
```json
{
  "message": "Password changed successfully"
}
```

### Unauthorized (401)
No body - requires authentication

### Forbidden (403)
No body - authorization failed (not your own account)

### ANY Error (500 InternalServerError)
```json
{
  "error": "An error occurred during password change."
}
```
**CRITICAL**: Controller catches ALL exceptions and returns this generic message to prevent information leakage!
- Wrong current password → 500 with generic message
- Weak new password → 500 with generic message  
- User not found → 500 with generic message

---

## ForgotPassword (`POST /api/account/forgot-password`)

### Always Returns Success (200 OK)
```json
{
  "message": "If an account exists with this email, a password reset link has been sent."
}
```
**Security**: Returns same response whether email exists or not (prevents enumeration)

### Server Error (500)
```json
{
  "error": "An error occurred. Please try again later."
}
```

---

## ResetPassword (`POST /api/account/reset-password`)

### Success (200 OK)
```json
{
  "message": "Password reset successfully"
}
```

### Invalid Token (400 BadRequest)
```json
{
  "error": "Invalid token / Token expired / etc."
}
```

---

## ConfirmEmail (`GET /api/account/confirm-email?userId={id}&token={token}`)

### Success
- **Status**: 302 Redirect
- **Location**: `{frontendUrl}/login?emailConfirmed=true`

### Error
- **Status**: 302 Redirect
- **Location**: `{frontendUrl}/login?emailConfirmed=false&error={encoded-message}`

### Missing Parameters (400 BadRequest)
No body - validation error

---

## ResendConfirmationEmail (`POST /api/account/resend-confirmation-email`)

### Success (200 OK)
```json
{
  "message": "If an account exists with this email, a confirmation link has been sent."
}
```
**Security**: Returns same response whether email exists or not

### Already Confirmed (400 BadRequest)
```json
{
  "error": "Email is already confirmed. You can login now."
}
```

### Server Error (500)
```json
{
  "error": "An error occurred. Please try again later."
}
```

---

## RefreshToken (`POST /api/account/refresh-token`)

### Success (200 OK)
```json
{
  "accessToken": "new-jwt-token",
  "refreshToken": "new-refresh-token",
  "accessTokenExpiresAt": "2024-...",
  "refreshTokenExpiresAt": "2024-..."
}
```

### Invalid Token (401 Unauthorized)
```json
{
  "error": "Invalid or expired refresh token"
}
```

### Server Error (500)
```json
{
  "error": "An error occurred while refreshing token."
}
```

---

## RevokeToken (`POST /api/account/revoke-token`)

### Success (200 OK)
```json
{
  "message": "Token revoked successfully"
}
```

### Invalid Token (400 BadRequest)
```json
{
  "error": "Invalid token" // or "Refresh token is required"
}
```

### Unauthorized (401)
No body - requires authentication

### Forbidden (403)
No body - token belongs to different user

### Server Error (500)
```json
{
  "error": "An error occurred while revoking token."
}
```

---

## CreateStaffUser (`POST /api/account/create-staff-user`)

### Success (201 Created)
```json
{
  "message": "Staff user created successfully",
  "user": { ...domainUser object... }
}
```
**Location Header**: `/api/account/user/{userId}`

### Unauthorized (401)
No body - requires authentication

### Forbidden (403)
No body - requires `IsDirectorOrAbove` policy OR failed authorization checks

### Validation Error (400 BadRequest)
```json
{
  "error": "BranchId is required for staff user creation." // or other validation message
}
```

### Server Error (500)
```json
{
  "error": "An error occurred while creating staff user."
}
```

---

## GetUserById (`GET /api/account/user/{id}`)

### Success (200 OK)
```json
{
  ...domainUser object...
}
```

### Not Found (404)
```json
{
  "error": "User not found"
}
```
**Note**: Also returns 404 if user exists but is in different branch

### Unauthorized (401)
No body - requires authentication with `IsAdministratorOrAbove`

### Forbidden (403)
No body - branch authorization failed

---

## Test Assertion Guidelines

1. **Check Status Code First**: Always assert the exact status code
2. **Check Response Shape**: Assert expected properties exist
3. **Check Generic Messages**: When controller returns generic errors, assert the EXACT generic message
4. **Don't Assert Details**: For security endpoints, don't try to assert specific error details that are intentionally hidden
5. **Test Both Paths**: Test success AND expected failure scenarios
6. **Use FluentAssertions**: `.Should().Be()`, `.Should().Contain()`, `.Should().BeOneOf()`

## Example Test Pattern

```csharp
// Act
var response = await Client.PostAsJsonAsync("/api/account/endpoint", request);
var content = await response.Content.ReadAsStringAsync();
_output.WriteLine($"Response: {content}"); // For debugging

// Assert Status
response.StatusCode.Should().Be(HttpStatusCode.Expected);

// Assert Body (if not empty)
var body = await response.Content.ReadFromJsonAsync<JsonElement>();
body.GetProperty("expectedProperty").GetString().Should().Be("expected value");
```
