using APC.API.Input;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using APC.Services;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtifactController : ControllerBase {
  private readonly IArtifactService aps_;
  private readonly IApcDatabase database_;

  public ArtifactController(IArtifactService aps, IApcDatabase database) {
    database_ = database;
    aps_ = aps;
  }

  // GET: api/Artifact
  [HttpGet]
  public async Task<IEnumerable<Artifact>> Get([FromQuery] string processor, [FromQuery] bool only_roots) {
    return await database_.GetArtifacts(processor, only_roots);
  }

  // POST: api/Artifact
  [HttpPost]
  public async Task<ActionResult> Post([FromBody] ArtifactInput input) {
    Artifact artifact =
      await database_.GetArtifact(input.Id, input.Processor);
    if (artifact == null) {
      artifact =
        await aps_.AddArtifact(input.Id, input.Processor, input.Filter, true);
    } else if (!artifact.root) {
      artifact.root = true;
      await database_.UpdateArtifact(artifact);
    } else {
      return Ok(new {
        Message = $"{input.Processor}/{input.Id} already Exists!"
      });
    }

    await aps_.Ingest(artifact);
    return Ok(new {
      Message = $"Added {input.Processor}/{input.Id}"
    });
  }

  // POST: api/Artifact/track
  [HttpPost("track")]
  public async Task<ActionResult>
    Track([FromBody] ArtifactTrackerInput request) {
    if (await aps_.Track(request.Artifact, request.Processor)) {
      return Ok("Artifact being reprocessed");
    }

    return BadRequest("Something went wrong");
  }

  [HttpPost("track/all")]
  public async Task<ActionResult> TrackAll() {
    await aps_.ReTrack();
    return Ok("Triggered re-tracking");
  }

  [HttpPost("validate/all")]
  public async Task<ActionResult> ValidateAllArtifacts() {
    await aps_.Validate();
    return Ok("Validating all artifacts!");
  }

  // DELETE: api/Artifact/npm/react
  [HttpDelete("{processor}/{id}")]
  public async Task<ActionResult> Delete(string id, string processor) {
    Artifact artifact = await database_.GetArtifact(id, processor);
    if (artifact == null) {
      return NotFound();
    }

    if (!await database_.DeleteArtifact(artifact)) {
      return Problem();
    }

    return Ok();
  }

  [HttpPost("collect")]
  public async Task<ActionResult> Collect(ArtifactCollectRequest request) {
    Console.WriteLine($"Collecting {request.location}");
    await aps_.Collect(request.location, request.module);
    return Ok("OK");
  }
}
