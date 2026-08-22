using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SchoolManagement.Tests.IntegrationTests;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static string SchemeName = nameof(TestAuthenticationHandler);

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Headers.TryGetValue("X-Test-Unauthorized", out var _))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = Context.Request.Headers["X-Test-UserId"].FirstOrDefault();
        var branchId = Context.Request.Headers["X-Test-BranchId"].FirstOrDefault();
        var role = Context.Request.Headers["X-Test-Role"].FirstOrDefault();
        var userName = Context.Request.Headers["X-Test-UserName"].FirstOrDefault();

        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userName ?? "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role ?? "User")
        };

        if (!string.IsNullOrEmpty(branchId))
        {
            claims.Add(new Claim("BranchId", branchId));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
