using APC.Infrastructure;
using APC.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers; 

[Route("api/[controller]")]
[ApiController]
public class ArtifactController : ControllerBase {
  private readonly Database db_;

  public ArtifactController(Database db) {
    db_ = db;
  }

  // GET: api/Artifact
  [HttpGet]
  public async Task<IEnumerable<Artifact>> Get([FromQuery] string module) {
    return await db_.GetArtifacts(module);
  }

  // POST: api/Artifact
  [HttpPost]
  public void Post([FromBody] string value) {
  }

  // DELETE: api/Artifact/5
  [HttpDelete("{id}")]
  public void Delete(int id) {
  }
}