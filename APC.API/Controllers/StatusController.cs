using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase {
  [HttpGet("status")]
  public ActionResult GetStatus() {
    return Ok("APC is OK.");
  }
}