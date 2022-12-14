using APC.API.Input;
using APC.API.Output;
using MongoDB.Bson;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using APC.Services;
using Microsoft.AspNetCore.Mvc;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcessorController : ControllerBase {
  private readonly IArtifactService aps_;
  private readonly IApcDatabase database_;

  public ProcessorController(IArtifactService aps, IApcDatabase database) {
    database_ = database;
    aps_ = aps;
  }
  
  [HttpGet("processors")]
  public async Task<IEnumerable<ProcessorOutput>> GetProcessors() {
    IEnumerable<Processor> processors = await database_.GetProcessors();
    List<ProcessorOutput> proc_out = new List<ProcessorOutput>();

    foreach(Processor processor in processors) {
      proc_out.Add(new ProcessorOutput {
          Id = processor.Id,
          Config = processor.Config.ToJson()
      });
    }
    return proc_out;
  }

  // GET: api/Artifact
  [HttpPost]
  public async Task<ActionResult> Post([FromBody] AddProcessorInput input) {
    await database_.AddProcessor(new Processor() {
      Id = input.ProcessorId
    });
    return Ok(new {
      Message = $"Added {input.ProcessorId}!"
    });
  }
}
