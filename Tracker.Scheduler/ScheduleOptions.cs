namespace Tracker.Scheduler;

public class ScheduleOptions {
  /* Cron Schedule */
  public required string schedule { get; set; }

  /* Processor */
  public required string processor { get; set; }
}