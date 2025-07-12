using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase {
  [HttpGet("login")]
  public IActionResult Login([FromQuery] string returnUrl = "/") {
    return Challenge(
      new AuthenticationProperties {
        RedirectUri = returnUrl
      },
      OpenIdConnectDefaults.AuthenticationScheme
    );
  }

  [HttpGet("logout")]
  public IActionResult Logout() {
    return SignOut(
      new AuthenticationProperties {
        RedirectUri = "/"
      },
      CookieAuthenticationDefaults.AuthenticationScheme,
      OpenIdConnectDefaults.AuthenticationScheme
    );
  }

  [Authorize]
  [HttpGet("me")]
  public IActionResult Me() {
    return Ok(
      new {
        name = User.Identity?.Name,
        roles = User.Claims
                    .Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)
                    .Select(c => c.Value)
      }
    );
  }
}