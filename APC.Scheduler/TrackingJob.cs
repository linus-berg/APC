using APC.Services;
using Quartz;

namespace APC.Scheduler;

public class TrackingJob : IJob {
  public static readonly JobKey Key = new("track-all", "apc");
  private readonly IArtifactService aps_;
  private readonly ILogger<TrackingJob> logger_;

  public TrackingJob(ILogger<TrackingJob> logger, IArtifactService aps) {
    aps_ = aps;
    logger_ = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    logger_.LogInformation("Tracking artifacts.");
    await aps_.ReTrack();
  }
}