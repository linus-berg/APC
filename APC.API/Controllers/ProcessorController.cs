using APC.API.Input;
using APC.API.Output;
using APC.Kernel.Models;
using APC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
    List<ProcessorOutput> proc_out = new();

    foreach (Processor processor in processors) {
      proc_out.Add(new ProcessorOutput {
        Id = processor.id,
        Config = processor.config.ToJson(),
        Description = processor.description
      });
    }

    return proc_out;
  }

  [HttpPost("update")]
  [Authorize(Roles = "Administrator")]
  public async Task<Processor> UpdateProcessor(
    [FromBody] UpdateProcessorInput input) {
    Processor processor = await database_.GetProcessor(input.processor_id);

    processor.description = input.description;
    await database_.UpdateProcessor(processor);
    return processor;
  }

  [HttpPost]
  [Authorize(Roles = "Administrator")]
  public async Task<ActionResult> Post([FromBody] AddProcessorInput input) {
    await database_.AddProcessor(new Processor {
      id = input.processor_id
    });
    return Ok(new {
      Message = $"Added {input.processor_id}!"
    });
  }
}