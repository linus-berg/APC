using Core.Services;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace Tracker.Scheduler;

public class TrackingJob : IJob {
  public static readonly JobKey S_KEY = new("track-job", "backpack");
  private readonly IArtifactService aps_;
  private readonly ILogger<TrackingJob> logger_;

  public TrackingJob(ILogger<TrackingJob> logger, IArtifactService aps) {
    aps_ = aps;
    logger_ = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    string? processor = context.MergedJobDataMap.GetString("processor");
    if (string.IsNullOrEmpty(processor)) {
      throw new InvalidConfigurationException("Processor not defined");
    }

    logger_.LogInformation("Tracking {Processor}", processor);
    await aps_.Track(processor);
  }
}