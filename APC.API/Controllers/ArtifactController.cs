using APC.Infrastructure;
using APC.Infrastructure.Models;
using APC.Kernel.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers; 

[Route("api/[controller]")]
[ApiController]
public class ArtifactController : ControllerBase {
  private readonly Database db_;
  private readonly ISendEndpointProvider bus_;

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
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(APC.Kernel.Endpoints.APC_INGEST_UNPROCESSED);
    endpoint.Send<ArtifactIngestRequest>(request);
  }

  // DELETE: api/Artifact/5
  [HttpDelete("{id}")]
  public void Delete(int id) {
  }
}