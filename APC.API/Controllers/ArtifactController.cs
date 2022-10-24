using APC.API.Input;
using APC.Infrastructure;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtifactController : ControllerBase {
  private readonly ISendEndpointProvider bus_;
  private readonly ApcDatabase db_;

  public ArtifactController(ApcDatabase db, ISendEndpointProvider bus) {
    db_ = db;
    bus_ = bus;
  }

  // GET: api/Artifact
  [HttpGet]
  public async Task<IEnumerable<Artifact>> Get([FromQuery] string module) {
    return await db_.GetArtifacts(module);
  }

  [HttpGet("modules")]
  public async Task<IEnumerable<string>> GetModules() {
    return await db_.GetModules();
  }

  // POST: api/Artifact
  [HttpPost]
  public async Task<ActionResult> Post([FromBody] ArtifactInput input) {
    Artifact artifact = await db_.GetArtifactByName(input.Name, input.Module);

    if (artifact == null) {
      await db_.AddArtifact(new Artifact() {
        module = input.Module,
        name = input.Name,
        filter = input.Filter,
        root = true,
      });
    } else if (!artifact.root) {
      artifact.root = true;
      await db_.UpdateArtifact(artifact);
    }
    else {
      return Ok(new {
        Message = $"{input.Module}/{input.Name} already Exists!"
      });
    }
    await db_.Commit();
    ArtifactIngestRequest ingest_request = new ArtifactIngestRequest();
    ingest_request.Artifacts.Add(input.Name);
    ingest_request.Module = input.Module;
    await SendToIngest(ingest_request);
    return Ok(new {
      Message = $"Added {input.Module}/{input.Name}"
    });
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
  public async Task<ActionResult> Delete(int id) {
    if (!await db_.DeleteArtifact(new Artifact() { id = id })) return Problem();
    await db_.Commit();
    return Ok();
  }

  private async Task SendToIngest(ArtifactIngestRequest request) {
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(Endpoints.APC_INGEST_UNPROCESSED);
    endpoint.Send(request);
  }
}