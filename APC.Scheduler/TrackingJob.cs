using APC.Services;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace APC.Scheduler;

public class TrackingJob : IJob {
  public static readonly JobKey S_KEY = new("track-job", "apc");
  private readonly IArtifactService aps_;
  private readonly ILogger<TrackingJob> logger_;

  public TrackingJob(ILogger<TrackingJob> logger, IArtifactService aps) {
    aps_ = aps;
    logger_ = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    logger_.LogInformation("Tracking artifacts");
    string? processor = context.JobDetail.JobDataMap.GetString("processor");
    if (string.IsNullOrEmpty(processor)) {
      throw new InvalidConfigurationException("Processor not defined");
    }
    await aps_.ReTrack(processor);
  }
}