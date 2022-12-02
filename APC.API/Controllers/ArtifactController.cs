using APC.API.Input;
using APC.Infrastructure;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

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
      await db_.AddArtifact(new Artifact {
        module = input.Module,
        id = input.Name,
        filter = input.Filter,
        root = true
      });
    } else if (!artifact.root) {
      artifact.root = true;
      await db_.UpdateArtifact(artifact);
    } else {
      return Ok(new {
        Message = $"{input.Module}/{input.Name} already Exists!"
      });
    }

    ArtifactIngestRequest ingest_request = new();
    ingest_request.Artifacts.Add(input.Name);
    ingest_request.Module = input.Module;
    await SendToIngest(ingest_request);
    return Ok(new {
      Message = $"Added {input.Module}/{input.Name}"
    });
  }

  // POST: api/Artifact/track
  [HttpPost("track")]
  public async Task<ActionResult>
    Track([FromBody] ArtifactTrackerInput request) {
    Artifact artifact =
      await db_.GetArtifactByName(request.Artifact, request.Module);
    if (artifact == null) {
      return NotFound();
    }

    if (!artifact.root) {
      return Problem("Artifact is not a root artifact");
    }

    ArtifactIngestRequest ingest_request = new();
    ingest_request.Module = request.Module;
    ingest_request.Artifacts.Add(request.Artifact);
    await SendToIngest(ingest_request);
    return Ok("Artifact being reprocessed");
  }

  [HttpPost("track/all")]
  public async Task<ActionResult> TrackAll() {
    IEnumerable<string> modules = await db_.GetModules();
    int module_count = 0;
    int artifact_count = 0;
    foreach (string module in modules) {
      IEnumerable<Artifact> artifacts = await db_.GetRoots(module);
      ArtifactIngestRequest ingest_request = new();
      foreach (Artifact artifact in artifacts) {
        ingest_request.Module = module;
        ingest_request.Artifacts.Add(artifact.id);
        artifact_count++;
      }

      await SendToIngest(ingest_request);
      module_count++;
    }

    return Ok(
      $"{artifact_count} artifacts being reprocessed in {module_count} modules!");
  }

  [HttpPost("validate/all")]
  public async Task<ActionResult> ValidateAllArtifacts() {
    IEnumerable<string> modules = await db_.GetModules();
    foreach (string module in modules) {
      try {
        Console.WriteLine($"Trying to validate {module}");
        await ValidateModule(module);
      } catch (Exception e) {
        Console.WriteLine(e);
      }
    }

    return Ok("Validating all artifacts!");
  }

  private async Task ValidateModule(string module) {
    IEnumerable<Artifact>
      artifacts = await db_.GetArtifacts(module);
    Console.WriteLine(artifacts.Count());
    ArtifactRouteRequest route_request = new();
    foreach (Artifact artifact in artifacts) {
      route_request.Artifact = artifact;
      await SendToCollect(route_request);
    }
  }

  // DELETE: api/Artifact/5
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id) {
    if (!await db_.DeleteArtifact(new Artifact {
        })) {
      return Problem();
    }

    return Ok();
  }

  [HttpPost("collect")]
  public async Task<ActionResult> Collect(ArtifactCollectRequest request) {
    Console.WriteLine($"Collecting {request.location}");
    await SendDirectCollect(request);
    return Ok("OK");
  }

  private async Task SendToCollect(ArtifactRouteRequest request) {
    await SendRequest(Endpoints.APC_ACM_ROUTER, request);
  }

  private async Task SendDirectCollect(ArtifactCollectRequest request) {
    await SendRequest(new Uri($"queue:{request.GetCollectorModule()}"),
                      request);
  }

  private async Task SendToIngest(ArtifactIngestRequest request) {
    await SendRequest(Endpoints.APC_INGEST_UNPROCESSED, request);
  }

  private async Task SendRequest<T>(Uri uri, T request) {
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(uri);
    await endpoint.Send(request);
  }
}