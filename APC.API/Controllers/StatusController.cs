using APC.API.Output;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase {
  private readonly KeycloakAuthenticationOptions kc_opt_ = new();

  public StatusController(IConfiguration configuration) {
    KeycloakAuthenticationOptions opts = new();
    configuration
      .GetSection(KeycloakAuthenticationOptions.Section)
      .Bind(kc_opt_, opt => opt.BindNonPublicProperties = true);
  }

  [HttpGet("status")]
  public ActionResult GetStatus() {
    return Ok("APC is OK.");
  }

  [HttpGet("keycloak")]
  public ActionResult GetKeycloak() {
    return Ok(new KeycloakOptions {
      url = kc_opt_.AuthServerUrl,
      realm = kc_opt_.Realm,
      resource = kc_opt_.Resource
    });
  }
}