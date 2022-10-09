using APC.API.Input;
using APC.Infrastructure;
using APC.Infrastructure.Models;
using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtifactController : ControllerBase {
  private readonly ISendEndpointProvider bus_;
  private readonly Database db_;

  public ArtifactController(Database db, ISendEndpointProvider bus) {
    db_ = db;
    bus_ = bus;
  }

  // GET: api/Artifact
  [HttpGet]
  public async Task<IEnumerable<Artifact>> Get([FromQuery] string module) {
    return await db_.GetArtifacts(module);
  }

  // POST: api/Artifact
  [HttpPost]
  public async Task Post([FromBody] ArtifactIngestRequest request) {
    await SendToIngest(request);
  }

  // POST: api/Artifact/track
  [HttpPost("track")]
  public async Task<ActionResult> Track([FromBody] ArtifactTrackerInput request) {
    Artifact artifact = await db_.GetArtifactByName(request.Artifact, request.Module);
    if (artifact == null) return NotFound();

    if (!artifact.root) return Problem("Artifact is not a root artifact");

    ArtifactIngestRequest ingest_request = new();
    ingest_request.Module = request.Module;
    ingest_request.Artifacts.Add(request.Artifact);
    await SendToIngest(ingest_request);
    return Ok("Artifact being reprocessed");
  }

  // DELETE: api/Artifact/5
  [HttpDelete("{id}")]
  public void Delete(int id) {
  }

  private async Task SendToIngest(ArtifactIngestRequest request) {
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(Endpoints.APC_INGEST_UNPROCESSED);
    endpoint.Send(request);
  }
}