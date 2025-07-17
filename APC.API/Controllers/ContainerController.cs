using APC.Services;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContainerController : ControllerBase {
  private readonly IArtifactService aps_;

  public ContainerController(IArtifactService aps) {
    aps_ = aps;
  }

  // GET
  [HttpGet("container")]
  public ActionResult GetContainer() {
    aps_.Collect("docker-archive://docker.io/nginx:latest", "docker-archive");
    return Ok();
  }
}